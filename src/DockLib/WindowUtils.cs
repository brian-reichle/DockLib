// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

			var hwnd = FindWindowIgnoringWindow(
				new NativeMethods.POINT()
				{
					x = (int)pt.X,
					y = (int)pt.Y,
				},
				sourceToIgnore.Handle);

			if (hwnd == IntPtr.Zero)
			{
				return null;
			}

			return HwndSource.FromHwnd(hwnd)?.RootVisual;
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

		static IntPtr FindWindowIgnoringWindow(NativeMethods.POINT pt, IntPtr hWndToIgnore)
		{
			var state = new State(
				hWndToIgnore,
				pt,
				NativeMethods.GetCurrentProcessId());

			var stateHandle = new GCHandle();

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				stateHandle = GCHandle.Alloc(state);
				NativeMethods.EnumWindows(Callback, (IntPtr)stateHandle);
			}
			finally
			{
				if (stateHandle.IsAllocated)
				{
					stateHandle.Free();
				}
			}

			return state.HWndResult;
		}

		static bool Callback(IntPtr hWnd, IntPtr lParam)
		{
			var dataHandle = (GCHandle)lParam;
			if (!dataHandle.IsAllocated) return false;

			var state = (State)dataHandle.Target;

			if (hWnd != state.HWndToIgnore && NativeMethods.IsWindowVisible(hWnd) && IsPointInWindow(hWnd, state.PT))
			{
				if (GetPID(hWnd) == state.PID)
				{
					state.HWndResult = hWnd;
				}

				return false;
			}

			return true;
		}

		static bool IsPointInWindow(IntPtr hWnd, NativeMethods.POINT pt)
		{
			if (!NativeMethods.GetWindowRect(hWnd, out var rect))
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			if (pt.x < rect.left || pt.x >= rect.right ||
				pt.y < rect.top || pt.y >= rect.bottom)
			{
				return false;
			}

			var hRgn = IntPtr.Zero;

			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				hRgn = NativeMethods.CreateRectRgn(0, 0, 0, 0);
				var errorCode = NativeMethods.GetWindowRgn(hWnd, hRgn);

				switch (errorCode)
				{
					case NativeMethods.RegionResult.NULLREGION: return false;
					case NativeMethods.RegionResult.SIMPLEREGION: return true;

					case NativeMethods.RegionResult.COMPLEXREGION:
						return NativeMethods.PtInRegion(hRgn, pt.x, pt.y);

					case NativeMethods.RegionResult.ERROR:
						var code = Marshal.GetLastWin32Error();

						switch (code)
						{
							case NativeMethods.ERROR_SUCCESS:
								return true;

							case NativeMethods.ERROR_INVALID_HANDLE:
								// Apparently the window closed.
								return false;
						}

						throw new Win32Exception(code);

					default:
						throw new InvalidOperationException();
				}
			}
			finally
			{
				if (hRgn != IntPtr.Zero)
				{
					NativeMethods.DeleteObject(hRgn);
				}
			}
		}

		static int GetPID(IntPtr hWnd)
		{
			if (NativeMethods.GetWindowThreadProcessId(hWnd, out var processId) == 0)
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return processId;
		}

		sealed class State
		{
			public State(IntPtr hwndToIgnore, NativeMethods.POINT pt, int pid)
			{
				HWndToIgnore = hwndToIgnore;
				PT = pt;
				PID = pid;
			}

			public IntPtr HWndToIgnore { get; }
			public IntPtr HWndResult { get; set; }
			public NativeMethods.POINT PT { get; }
			public int PID { get; }
		}
	}
}
