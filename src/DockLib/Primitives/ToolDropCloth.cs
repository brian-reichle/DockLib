// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Media;

namespace DockLib.Primitives
{
	[TemplatePart(Name = "PART_Overlay", Type = typeof(ToolDropOverlay))]
	sealed class ToolDropCloth : Window
	{
		static ToolDropCloth()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolDropCloth), new FrameworkPropertyMetadata(typeof(ToolDropCloth)));
		}

		public ToolDropCloth(FrameworkElement root)
		{
			if (root == null) throw new ArgumentNullException(nameof(root));

			Root = root;
			Content = new ToolDropOverlay();
		}

		public FrameworkElement Root { get; }
		public ToolDropOverlay Overlay { get; private set; }

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Overlay = (ToolDropOverlay)GetTemplateChild("PART_Overlay");
		}

		protected override DependencyObject GetUIParentCore() => Root;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			PlaceWindowOver(this, Root);
		}

		static void PlaceWindowOver(Window window, FrameworkElement target)
		{
			var transform = new TransformGroup();
			var source = PresentationSource.FromVisual(target);

			transform.Children.Add((Transform)target.TransformToAncestor(source.RootVisual));
			transform.Children.Add(new MatrixTransform(source.CompositionTarget.TransformToDevice));

			var point = source.RootVisual.PointToScreen(new Point(0, 0));
			transform.Children.Add(new TranslateTransform(point.X, point.Y));

			source = PresentationSource.FromVisual(window);
			transform.Children.Add(new MatrixTransform(source.CompositionTarget.TransformFromDevice));

			var bounds = transform.TransformBounds(new Rect(0, 0, target.ActualWidth, target.ActualHeight));

			window.Width = bounds.Width;
			window.Height = bounds.Height;
			window.Left = bounds.Left;
			window.Top = bounds.Top;
		}
	}
}
