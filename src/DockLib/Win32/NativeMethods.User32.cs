// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DockLib
{
	static partial class NativeMethods
	{
		// HWND WindowFromPoint(
		//     POINT Point
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(
			POINT Point);

		// BOOL WINAPI GetCursorPos(
		//     _Out_ LPPOINT lpPoint
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(
			out POINT lpPoint);

		// HDWP WINAPI BeginDeferWindowPos(
		//     _In_ int nNumWindows
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr BeginDeferWindowPos(
			int nNumWindows);

		// HDWP WINAPI DeferWindowPos(
		//     _In_     HDWP hWinPosInfo,
		//     _In_     HWND hWnd,
		//     _In_opt_ HWND hWndInsertAfter,
		//     _In_     int  x,
		//     _In_     int  y,
		//     _In_     int  cx,
		//     _In_     int  cy,
		//     _In_     UINT uFlags
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr DeferWindowPos(
			IntPtr hWinPosInfo,
			IntPtr hWnd,
			IntPtr hWndInsertAfter,
			int x,
			int y,
			int cx,
			int cy,
			SetWindowPosFlags uFlags);

		// BOOL WINAPI EndDeferWindowPos(
		//     _In_ HDWP hWinPosInfo
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EndDeferWindowPos(
			IntPtr hWinPosInfo);

		// LONG WINAPI GetWindowLong(
		//     _In_ HWND hWnd,
		//     _In_ int nIndex
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(
			IntPtr hWnd,
			GetWindowsLongIndex nIndex);

		// LONG WINAPI SetWindowLong(
		//     _In_ HWND hWnd,
		//     _In_ int nIndex,
		//     _In_ LONG dwNewLong
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SetWindowLong(
			IntPtr hWnd,
			GetWindowsLongIndex nIndex,
			int dwNewLong);
	}
}
