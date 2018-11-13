// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Input;

namespace DockLib
{
	public class ToolDragEndedEventArgs : ToolDragEventArgs
	{
		public ToolDragEndedEventArgs(MouseDevice device, int timestamp, Point anchor, bool cancelled)
			: base(device, timestamp, anchor)
		{
			Cancelled = cancelled;
		}

		public bool Cancelled { get; }
	}
}
