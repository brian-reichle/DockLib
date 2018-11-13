// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DockLib
{
	static partial class NativeMethods
	{
		// HRGN CreateRectRgn(
		//     _In_  int nLeftRect,
		//     _In_  int nTopRect,
		//     _In_  int nRightRect,
		//     _In_  int nBottomRect
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRectRgn(
			int nLeftRect,
			int nTopRect,
			int nRightRect,
			int nBottomRect);

		// BOOL DeleteObject(
		//     _In_  HGDIOBJ hObject
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(
			IntPtr hObject);

		// BOOL PtInRegion(
		//     _In_  HRGN hrgn,
		//     _In_  int  X,
		//     _In_  int  Y
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PtInRegion(
			IntPtr hRgn,
			int X,
			int Y);
	}
}
