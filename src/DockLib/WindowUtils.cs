// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DockLib
{
	static class WindowUtils
	{
		public static Visual FindRootVisualIgnoringWindow(Point pt, Window windowToIgnore)
		{
			if (windowToIgnore == null) throw new ArgumentNullException(nameof(windowToIgnore));

			var sourceToIgnore = (HwndSource)PresentationSource.FromVisual(windowToIgnore);
			HwndSourceHook hook = TransparentWndProc;

			sourceToIgnore.AddHook(hook);
			try
			{
				var resultHWnd = NativeMethods.WindowFromPoint(
					new NativeMethods.POINT()
					{
						x = (int)pt.X,
						y = (int)pt.Y,
					});

				if (resultHWnd == IntPtr.Zero)
				{
					return null;
				}

				return HwndSource.FromHwnd(resultHWnd)?.RootVisual;
			}
			finally
			{
				sourceToIgnore.RemoveHook(hook);
			}
		}

		public static Point GetScreenPosition()
		{
			if (!NativeMethods.GetCursorPos(out var pt))
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return new Point(pt.x, pt.y);
		}

		public static void SetTopWindowSequence(Window[] windows)
		{
			if (windows == null) throw new ArgumentNullException(nameof(windows));

			var h = NativeMethods.BeginDeferWindowPos(windows.Length);

			if (h == IntPtr.Zero)
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			var hTarget = IntPtr.Zero;

			foreach (var window in windows)
			{
				if (window == null) continue;

				var windowSource = (HwndSource)PresentationSource.FromVisual(window);

				if (windowSource == null)
				{
					// if the window has no source, then it doesn't exist yet/anymore and there is nothing to reposition.
					continue;
				}

				h = NativeMethods.DeferWindowPos(
					h,
					windowSource.Handle,
					hTarget,
					0,
					0,
					0,
					0,
					NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE
					| NativeMethods.SetWindowPosFlags.SWP_NOMOVE
					| NativeMethods.SetWindowPosFlags.SWP_NOOWNERZORDER
					| NativeMethods.SetWindowPosFlags.SWP_NOSIZE);

				if (h == IntPtr.Zero)
				{
					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}

				hTarget = windowSource.Handle;
			}

			if (!NativeMethods.EndDeferWindowPos(h))
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		public static void MakeToolWindow(Window window)
		{
			if (window == null) throw new ArgumentNullException(nameof(window));

			var source = (HwndSource)PresentationSource.FromVisual(window);
			var value = SafeWin32.GetWindowLong(source, NativeMethods.GetWindowsLongIndex.GWL_EXSTYLE);
			value = value | (int)NativeMethods.WindowStylesEx.WS_EX_TOOLWINDOW;
			SafeWin32.SetWindowLong(source, NativeMethods.GetWindowsLongIndex.GWL_EXSTYLE, value);
		}

		static IntPtr TransparentWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case NativeMethods.WM_NCHITTEST:
					handled = true;
					return (IntPtr)NativeMethods.HTTRANSPARENT;
			}

			return IntPtr.Zero;
		}
	}
}
