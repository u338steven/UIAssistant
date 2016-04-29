using System;
using System.Runtime.InteropServices;

namespace UIAssistant.Utility.Win32
{
    public class UnmanagedDll : IDisposable
    {
        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32")]
        private static extern bool FreeLibrary(IntPtr hModule);

        public IntPtr ModuleHandle { get; private set; }

        public UnmanagedDll(string lpFileName)
        {
            ModuleHandle = LoadLibrary(lpFileName);
        }

        public T GetProcDelegate<T>(string method) where T : class
        {
            if (ModuleHandle == IntPtr.Zero)
            {
                return null;
            }
            IntPtr methodHandle = GetProcAddress(ModuleHandle, method);
            T ptr = Marshal.GetDelegateForFunctionPointer(methodHandle, typeof(T)) as T;
            return ptr;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                FreeLibrary(ModuleHandle);

                disposedValue = true;
            }
        }

        ~UnmanagedDll()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
