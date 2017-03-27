using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;

using UIAssistant.Utility.Win32;

namespace UIAssistant.Utility
{
    public static class Migemo
    {
        #region Enumerations (from migemo.h)
        #region enum DictionaryIndex
        public enum DictionaryId
        {
            Invalid = 0,
            Migemo = 1,
            RomaToHira = 2,
            HiraToKata = 3,
            HanToZen = 4,
        }
        #endregion

        #region enum OperatorIndex
        public enum OperatorIndex
        {
            Or = 0,
            NestIn = 1,
            NestOut = 2,
            SelectIn = 3,
            SelectOut = 4,
            NewLine = 5,
        }
        #endregion

        #region enum CharSet
        public enum CharSet : int
        {
            None = 0,
            Cp932 = 1,
            Eucjp = 2,
            UTF8 = 3,
        };
        #endregion

        #endregion

        #region Struct
        [StructLayout(LayoutKind.Sequential)]
        public struct MigemoStruct
        {
            public int enable;
            public IntPtr mtree;
            public CharSet charset;
            public IntPtr roma2hira;
            public IntPtr hira2kata;
            public IntPtr han2zen;
            public IntPtr zen2han;
            public IntPtr rx;
            public IntPtr addword;
            public IntPtr char2int;
        };
        #endregion

        #region Link to migemo.dll
        private static UnmanagedDll migemoDll;

        private delegate IntPtr MigemoOpen(string dict);
        private delegate void MigemoClose(IntPtr obj);
        private delegate IntPtr MigemoQuery(IntPtr obj, string query);
        private delegate void MigemoRelease(IntPtr obj, IntPtr result);

        private delegate int MigemoSetOperator(IntPtr obj, OperatorIndex index, string op);
        private delegate IntPtr MigemoGetOperator(IntPtr obj, OperatorIndex index);

        private delegate DictionaryId MigemoLoad(IntPtr obj, DictionaryId id, string file);
        private delegate int MigemoIsEnable(IntPtr obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe private delegate int MigemoProcInt2Char(uint input, byte* output);
        private delegate void MigemoSetProcInt2Char(IntPtr obj, MigemoProcInt2Char proc);

        private static MigemoOpen migemo_open;
        private static MigemoClose migemo_close;
        private static MigemoQuery migemo_query;
        private static MigemoRelease migemo_release;

        private static MigemoSetOperator migemo_set_operator;
        private static MigemoGetOperator migemo_get_operator;

        private static MigemoLoad migemo_load;
        private static MigemoIsEnable migemo_is_enable;

        private static MigemoSetProcInt2Char migemo_setproc_int2char;
        private static MigemoProcInt2Char int2char_delegate;
        #endregion

        public static IntPtr MigemoObject { get; private set; }
        private static Object lockObj = new Object();

        public static bool SetOperator(OperatorIndex index, string op)
        {
            return migemo_set_operator(MigemoObject, index, op) != 0;
        }

        public static string GetOperator(OperatorIndex index)
        {
            IntPtr result = migemo_get_operator(MigemoObject, index);
            if (result != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(result);
            else
                return "";
        }

        #region Operator properties
        public static string OperatorOr
        {
            get { return GetOperator(OperatorIndex.Or); }
            set { SetOperator(OperatorIndex.Or, value); }
        }
        public static string OperatorNestIn
        {
            get { return GetOperator(OperatorIndex.NestIn); }
            set { SetOperator(OperatorIndex.NestIn, value); }
        }
        public static string OperatorNestOut
        {
            get { return GetOperator(OperatorIndex.NestOut); }
            set { SetOperator(OperatorIndex.NestOut, value); }
        }
        public static string OperatorSelectIn
        {
            get { return GetOperator(OperatorIndex.SelectIn); }
            set { SetOperator(OperatorIndex.SelectIn, value); }
        }
        public static string OperatorSelectOut
        {
            get { return GetOperator(OperatorIndex.SelectOut); }
            set { SetOperator(OperatorIndex.SelectOut, value); }
        }
        public static string OperatorNewLine
        {
            get { return GetOperator(OperatorIndex.NewLine); }
            set { SetOperator(OperatorIndex.NewLine, value); }
        }
        #endregion

        public static bool LoadDictionary(DictionaryId id, string file)
        {
            DictionaryId result = migemo_load(MigemoObject, id, file);
            return result == id;
        }

        public static bool IsEnable()
        {
            if (MigemoObject == IntPtr.Zero)
            {
                return false;
            }
            return migemo_is_enable(MigemoObject) != 0;
        }

        public static Regex GetRegex(string query)
        {
            try
            {
                string ret = Query(query);
                return new Regex(ret, RegexOptions.IgnoreCase);
            }
            catch
            {
                return new Regex(query, RegexOptions.IgnoreCase);
            }
        }

        public static string Query(string query)
        {
            lock (lockObj)
            {
                IntPtr result = migemo_query(MigemoObject, query);
                if (result != IntPtr.Zero)
                {
                    string retval = StringFromNativeUtf8(result);
                    //System.Diagnostics.Debug.Print($"utf-8:{retval}");
                    migemo_release(MigemoObject, result);
                    return retval;
                }
                else
                    return "";
            }
        }

        private static UTF8Encoding _encoder = new UTF8Encoding(false);
        public static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            var len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
            var buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return _encoder.GetString(buffer);
        }

        public static void Dispose()
        {
            if (MigemoObject != IntPtr.Zero)
            {
                migemo_close(MigemoObject);
                MigemoObject = IntPtr.Zero;
            }
            migemoDll?.Dispose();
        }

        static Migemo()
        {
            MigemoObject = IntPtr.Zero;
        }

        unsafe public static void Initialize(string migemoPath = null, string migemoDictionaryPath = null)
        {
            try
            {
                if (IsEnable())
                {
                    Dispose();
                }

                string migemoFileName;
                if (Environment.Is64BitProcess)
                {
                    migemoFileName = "migemo.dll";
                }
                else
                {
                    migemoFileName = "migemo32.dll";
                }

                string migemoRootDir;
                string dllPath;
                if (string.IsNullOrEmpty(migemoPath))
                {
                    migemoRootDir = System.IO.Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).ToString();
                    dllPath = System.IO.Path.Combine(migemoRootDir, migemoFileName);
                }
                else
                {
                    migemoRootDir = migemoPath;
                    dllPath = System.IO.Path.Combine(migemoPath, migemoFileName);
                }

                if (!System.IO.File.Exists(dllPath))
                {
                    throw new DllNotFoundException("migemo.dll not found");
                }

                migemoDll = new UnmanagedDll(dllPath);

                migemo_open = migemoDll.GetProcDelegate<MigemoOpen>("migemo_open");
                migemo_close = migemoDll.GetProcDelegate<MigemoClose>("migemo_close");
                migemo_query = migemoDll.GetProcDelegate<MigemoQuery>("migemo_query");
                migemo_release = migemoDll.GetProcDelegate<MigemoRelease>("migemo_release");
                migemo_set_operator = migemoDll.GetProcDelegate<MigemoSetOperator>("migemo_set_operator");
                migemo_get_operator = migemoDll.GetProcDelegate<MigemoGetOperator>("migemo_get_operator");
                migemo_load = migemoDll.GetProcDelegate<MigemoLoad>("migemo_load");
                migemo_is_enable = migemoDll.GetProcDelegate<MigemoIsEnable>("migemo_is_enable");
                migemo_setproc_int2char = migemoDll.GetProcDelegate<MigemoSetProcInt2Char>("migemo_setproc_int2char");

                string dictpath = migemoDictionaryPath;
                if (string.IsNullOrEmpty(dictpath))
                {
                    dictpath = System.IO.Path.Combine(@migemoRootDir, "dict/migemo-dict");
                    if (!System.IO.File.Exists(dictpath))
                    {
                        dictpath = System.IO.Path.Combine(@migemoRootDir, "dict/utf-8/migemo-dict");
                    }
                }
                MigemoObject = migemo_open(dictpath);
                MigemoStruct migemoStruct = new MigemoStruct();
                migemoStruct = Marshal.PtrToStructure<MigemoStruct>(MigemoObject);

                if (!IsEnable())
                {
                    throw new DllNotFoundException("cannot read dictionary");
                }

                OperatorNestIn = "(?:";
                if (migemoStruct.charset == CharSet.Cp932)
                {
                    int2char_delegate = ProcAnsiInt2Char;
                    migemo_setproc_int2char(MigemoObject, int2char_delegate);
                }
                else if (migemoStruct.charset == CharSet.UTF8)
                {
                    int2char_delegate = ProcUtf8Int2Char;
                    migemo_setproc_int2char(MigemoObject, int2char_delegate);
                }

                AppDomain.CurrentDomain.ProcessExit += (o, e) =>
                {
                    Dispose();
                };
            }
            catch (Exception ex)
            {
                throw new Exception("cannot load migemo.dll or dictionary", ex);
            }
        }

        // For escape utf8
        unsafe private static int ProcUtf8Int2Char(uint input, byte* output)
        {
            if (input < 0x80)
            {
                int len = 0;
                switch (input)
                {
                    case '\\':
                    case '.':
                    case '*':
                    case '^':
                    case '$':
                    case '/':
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '{':
                    case '}':
                    case '|':
                    case '+':
                    case '?':
                        if (output != null)
                            output[len] = (byte)'\\';
                        ++len;
                        if (output != null)
                            output[len] = (byte)(input & 0xFF);
                        ++len;
                        break;
                    default:
                        if (output != null)
                            output[len] = (byte)(input & 0xFF);
                        ++len;
                        break;
                }
                return len;
            }
            if (input < 0x800)
            {
                if (output != null)
                {
                    output[0] = (byte)(0xc0 + (input >> 6));
                    output[1] = (byte)(0x80 + ((input >> 0) & 0x3f));
                }
                return 2;
            }
            if (input < 0x10000)
            {
                if (output != null)
                {
                    output[0] = (byte)(0xe0 + (input >> 12));
                    output[1] = (byte)(0x80 + ((input >> 6) & 0x3f));
                    output[2] = (byte)(0x80 + ((input >> 0) & 0x3f));
                }
                return 3;
            }
            if (input < 0x200000)
            {
                if (output != null)
                {
                    output[0] = (byte)(0xf0 + (input >> 18));
                    output[1] = (byte)(0x80 + ((input >> 12) & 0x3f));
                    output[2] = (byte)(0x80 + ((input >> 6) & 0x3f));
                    output[3] = (byte)(0x80 + ((input >> 0) & 0x3f));
                }
                return 4;
            }
            if (input < 0x4000000)
            {
                if (output != null)
                {
                    output[0] = (byte)(0xf8 + (input >> 24));
                    output[1] = (byte)(0x80 + ((input >> 18) & 0x3f));
                    output[2] = (byte)(0x80 + ((input >> 12) & 0x3f));
                    output[3] = (byte)(0x80 + ((input >> 6) & 0x3f));
                    output[4] = (byte)(0x80 + ((input >> 0) & 0x3f));
                }
                return 5;
            }
            else
            {
                if (output != null)
                {
                    output[0] = (byte)(0xf8 + (input >> 30));
                    output[1] = (byte)(0x80 + ((input >> 24) & 0x3f));
                    output[2] = (byte)(0x80 + ((input >> 18) & 0x3f));
                    output[3] = (byte)(0x80 + ((input >> 12) & 0x3f));
                    output[4] = (byte)(0x80 + ((input >> 6) & 0x3f));
                    output[5] = (byte)(0x80 + ((input >> 0) & 0x3f));
                }
                return 6;
            }
        }

        // For escape ansi
        unsafe private static int ProcAnsiInt2Char(uint input, byte* output)
        {
            if (input >= 0x100)
            {
                if (output != null)
                {
                    output[0] = (byte)((input >> 8) & 0xFF);
                    output[1] = (byte)(input & 0xFF);
                }
                return 2;
            }
            else
            {
                int len = 0;
                switch (input)
                {
                    case '\\':
                    case '.':
                    case '*':
                    case '^':
                    case '$':
                    case '/':
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '{':
                    case '}':
                    case '|':
                    case '+':
                    case '?':
                        if (output != null)
                            output[len] = (byte)'\\';
                        ++len;
                        if (output != null)
                            output[len] = (byte)(input & 0xFF);
                        ++len;
                        break;
                    default:
                        if (output != null)
                            output[len] = (byte)(input & 0xFF);
                        ++len;
                        break;
                }
                return len;
            }
        }
    }
}
