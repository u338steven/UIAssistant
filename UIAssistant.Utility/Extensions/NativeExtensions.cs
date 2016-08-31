﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UIAssistant.Utility.Extensions
{
    public static class NativeExtensions
    {
        public static ushort ToLoWord(this IntPtr dword)
        {
            return (ushort)((uint)dword & 0xffff);
        }

        public static ushort ToHiWord(this IntPtr dword)
        {
            return (ushort)((uint)dword >> 16);
        }

        public static Point ToPoint(this IntPtr dword)
        {
            return new Point((short)((uint)dword & 0xffff), (short)((uint)dword >> 16));
        }

        public static IntPtr ToDword(this Point pt)
        {
            return new IntPtr(((uint)pt.X & 0xffff) + (((uint)pt.Y) << 16));
        }
    }
}
