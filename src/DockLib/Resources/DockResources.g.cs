// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;

namespace DockLib
{
	internal enum DockResourceKeyID
	{
		ClosePanelButtonStyleKey,
	}

	public static partial class DockResources
	{
		public static ResourceKey ClosePanelButtonStyleKey { get; } = new DockResourceKey(DockResourceKeyID.ClosePanelButtonStyleKey);
	}
}
