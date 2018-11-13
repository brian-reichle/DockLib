// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Input;

namespace DockLib
{
	public class ToolDragEventArgs : MouseEventArgs
	{
		public ToolDragEventArgs(MouseDevice device, int timestamp, Point anchor)
			: base(device, timestamp)
		{
			Anchor = anchor;
		}

		public Point Anchor { get; }
	}
}
