// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using System.Security;

namespace DockLib
{
	static partial class NativeMethods
	{
		// DWORD WINAPI GetCurrentProcessId(void)
		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll")]
		public static extern int GetCurrentProcessId();
	}
}
