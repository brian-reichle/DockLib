// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace DockLib
{
	static partial class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			// LONG left;
			public int left;

			// LONG top;
			public int top;

			// LONG right;
			public int right;

			// LONG bottom;
			public int bottom;
		}
	}
}
