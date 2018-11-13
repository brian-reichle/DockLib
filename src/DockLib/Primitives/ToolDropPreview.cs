// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;

namespace DockLib.Primitives
{
	public class ToolDropPreview : Control
	{
		static ToolDropPreview()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolDropPreview), new FrameworkPropertyMetadata(typeof(ToolDropPreview)));
		}
	}
}
