// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
namespace DockLib
{
	static partial class NativeMethods
	{
		public enum RegionResult : int
		{
			ERROR = 0,
			NULLREGION = 1,
			SIMPLEREGION = 2,
			COMPLEXREGION = 3,
		}
	}
}
