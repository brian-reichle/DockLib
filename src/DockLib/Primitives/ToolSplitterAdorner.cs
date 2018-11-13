// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DockLib.Primitives
{
	public sealed class ToolSplitterAdorner : Adorner
	{
		public static readonly DependencyProperty RectangleProperty = DependencyProperty.Register(
			"Rectangle",
			typeof(Rect),
			typeof(ToolSplitterAdorner),
			new FrameworkPropertyMetadata(default(Rect), FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
			"Background",
			typeof(Brush),
			typeof(ToolSplitterAdorner),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
			"BorderBrush",
			typeof(Brush),
			typeof(ToolSplitterAdorner),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
			"BorderThickness",
			typeof(double),
			typeof(ToolSplitterAdorner),
			new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

		static ToolSplitterAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolSplitterAdorner), new FrameworkPropertyMetadata(typeof(ToolSplitterAdorner)));
		}

		public ToolSplitterAdorner(FrameworkElement adornedElement)
			: base(adornedElement)
		{
		}

		public Rect Rectangle
		{
			get => (Rect)GetValue(RectangleProperty);
			set => SetValue(RectangleProperty, value);
		}

		public Brush Background
		{
			get => (Brush)GetValue(BackgroundProperty);
			set => SetValue(BackgroundProperty, value);
		}

		public Brush BorderBrush
		{
			get => (Brush)GetValue(BorderBrushProperty);
			set => SetValue(BorderBrushProperty, value);
		}

		public double BorderThickness
		{
			get => (double)GetValue(BorderThicknessProperty);
			set => SetValue(BorderThicknessProperty, value);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var element = (FrameworkElement)AdornedElement;

			return new Size(element.ActualWidth, element.ActualHeight);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var element = (FrameworkElement)AdornedElement;

			return new Size(element.ActualWidth, element.ActualHeight);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawRectangle(Background, new Pen(BorderBrush, BorderThickness), Rectangle);
		}
	}
}
