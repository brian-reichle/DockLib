// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace DockLib
{
	static partial class NativeMethods
	{
		[Flags]
		public enum SetWindowPosFlags
		{
			SWP_NOACTIVATE = 0x0010,
			SWP_NOMOVE = 0x0002,
			SWP_NOOWNERZORDER = 0x0200,
			SWP_NOSIZE = 0x0001,
		}
	}
}
