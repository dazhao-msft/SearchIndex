//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Microsoft.BizQA.NLU.QueryRewrite.Speller
{
    /// <summary>
    /// Secure library loading for managed code.
    /// </summary>
    /// <remarks>
    /// The intention here is to meet the needs of the basic LoadLibrary case,
    /// although it can be expanded similarly to the C++ version as needed.
    /// </remarks>
    public static class SecureLibrary
    {
        /// <summary>
        /// Safely loads a library from the built-in paths.
        /// This API will not search CWD.
        /// </summary>
        /// <param name="fileName">fully-qualified library file path</param>
        /// <returns>Handle of library instance on success; IntPTr.Zero on failure</returns>
        public static IntPtr Load(string fileName)
        {
            IntPtr module = NativeMethods.LoadLibraryEx(fileName, IntPtr.Zero, 0x00001000 /*LOAD_LIBRARY_SEARCH_DEFAULT_DIRS*/);
            if (module == IntPtr.Zero && Marshal.GetLastWin32Error() == 87 /*ERROR_INVALID_PARAMETER*/)
            {
                // Could have failed with actual bad parameters or an unpatched OS
                if (NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("kernel32.dll"), "AddDllDirectory") == IntPtr.Zero)
                {
                    // Unpatched OS [no KB 2533623], remove all new flags
                    module = NativeMethods.LoadLibraryEx(fileName, IntPtr.Zero, 0);
                }
            }

            return module;
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr LoadLibraryEx(string dllFilePath, System.IntPtr file, uint flags);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr GetProcAddress(IntPtr module, string procedureName);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr GetModuleHandle(string moduleName);
        }
    }
}
