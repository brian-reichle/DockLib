// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace DockLib
{
	static partial class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			// LONG x
			public int x;

			// LONG x
			public int y;
		}
	}
}
