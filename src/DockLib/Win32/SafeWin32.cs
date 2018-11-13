// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace DockLib
{
	static class SafeWin32
	{
		public static int GetWindowLong(HwndSource source, NativeMethods.GetWindowsLongIndex index)
		{
			var value = NativeMethods.GetWindowLong(source.Handle, index);

			if (value == 0 && Marshal.GetLastWin32Error() != 0)
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return value;
		}

		public static int SetWindowLong(HwndSource source, NativeMethods.GetWindowsLongIndex index, int value)
		{
			value = NativeMethods.SetWindowLong(source.Handle, NativeMethods.GetWindowsLongIndex.GWL_EXSTYLE, value);

			if (value == 0 && Marshal.GetLastWin32Error() != 0)
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return value;
		}
	}
}
