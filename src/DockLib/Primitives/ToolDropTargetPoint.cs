// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;

namespace DockLib.Primitives
{
	public class ToolDropTargetPoint : Control
	{
		public static readonly DependencyProperty TargetTypeProperty = DependencyProperty.Register(
			"TargetType",
			typeof(ToolDropTargetType),
			typeof(ToolDropTargetPoint),
			new FrameworkPropertyMetadata());

		static ToolDropTargetPoint()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolDropTargetPoint), new FrameworkPropertyMetadata(typeof(ToolDropTargetPoint)));
			FocusableProperty.OverrideMetadata(typeof(ToolDropTargetPoint), new FrameworkPropertyMetadata(false));
			IsTabStopProperty.OverrideMetadata(typeof(ToolDropTargetPoint), new FrameworkPropertyMetadata(false));
		}

		public ToolDropTargetType TargetType
		{
			get => (ToolDropTargetType)GetValue(TargetTypeProperty);
			set => SetValue(TargetTypeProperty, value);
		}
	}
}
