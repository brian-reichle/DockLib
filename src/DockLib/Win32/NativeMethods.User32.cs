// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DockLib
{
	static partial class NativeMethods
	{
		// BOOL CALLBACK EnumWindowsProc(
		//     _In_ HWND   hwnd,
		//     _In_ LPARAM lParam
		// );
		[SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		public delegate bool EnumWindowsProc(
			IntPtr hwnd,
			IntPtr lParam);

		// BOOL WINAPI GetCursorPos(
		//     _Out_ LPPOINT lpPoint
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(
			out POINT lpPoint);

		// BOOL WINAPI EnumWindows(
		//     _In_ WNDENUMPROC lpEnumFunc,
		//     _In_ LPARAM      lParam
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumWindows(
			EnumWindowsProc lpEnumFunc,
			IntPtr lParam);

		// DWORD WINAPI GetWindowThreadProcessId(
		//     _In_      HWND    hWnd,
		//     _Out_opt_ LPDWORD lpdwProcessId
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowThreadProcessId(
			IntPtr hWnd,
			out int lpdwProcessId);

		// BOOL WINAPI IsWindowVisible(
		//     _In_ HWND hWnd
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(
			IntPtr hWnd);

		// BOOL WINAPI GetWindowRect(
		//     _In_  HWND   hWnd,
		//     _Out_ LPRECT lpRect
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(
			IntPtr hWnd,
			out RECT lpRect);

		// int GetWindowRgn(
		//     _In_  HWND hWnd,
		//     _In_  HRGN hRgn
		// );
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern RegionResult GetWindowRgn(
			IntPtr hWnd,
			IntPtr hRgn);

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
