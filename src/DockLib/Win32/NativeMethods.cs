// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Security;

namespace DockLib
{
	[SecurityCritical]
	static partial class NativeMethods
	{
		public const int ERROR_SUCCESS = 0;
		public const int ERROR_INVALID_HANDLE = 6;
	}
}
