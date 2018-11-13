// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace DockLib
{
	static partial class NativeMethods
	{
		[Flags]
		public enum WindowStylesEx
		{
			WS_EX_TOOLWINDOW = 0x00000080,
		}
	}
}
