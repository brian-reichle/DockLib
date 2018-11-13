// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DockLib.Primitives;

namespace DockLib
{
	[TemplatePart(Name = "PART_Header", Type = typeof(ToolHeader))]
	public class ToolPanel : ContentControl
	{
		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
			"Header",
			typeof(string),
			typeof(ToolPanel),
			new FrameworkPropertyMetadata(string.Empty, null, CoerceHeader));

		static ToolPanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolPanel), new FrameworkPropertyMetadata(typeof(ToolPanel)));
			FocusableProperty.OverrideMetadata(typeof(ToolPanel), new FrameworkPropertyMetadata(false));
			IsTabStopProperty.OverrideMetadata(typeof(ToolPanel), new FrameworkPropertyMetadata(false));

			CommandManager.RegisterClassCommandBinding(typeof(ToolPanel), new CommandBinding(ToolHeader.Close, OnCloseExecuted));
		}

		public string Header
		{
			get => (string)GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public override void OnApplyTemplate()
		{
			var header = (ToolHeader)GetTemplateChild("PART_Header");

			if (header != null)
			{
				header.BeginDrag -= HeaderBeginDrag;
				header.Drag -= HeaderDrag;
				header.EndDrag -= HeaderEndDrag;
			}

			base.OnApplyTemplate();

			header = (ToolHeader)GetTemplateChild("PART_Header");

			if (header != null)
			{
				header.BeginDrag += HeaderBeginDrag;
				header.Drag += HeaderDrag;
				header.EndDrag += HeaderEndDrag;
			}
		}

		void HeaderBeginDrag(object sender, ToolDragEventArgs e)
		{
			var header = (ToolHeader)sender;
			PanelEvents.RaiseDetach(this);
			header.SetupDrag(e);
			var window = Window.GetWindow(this) as ToolDragWindow;

			if (window != null)
			{
				window.IsDragging = true;
			}
		}

		void HeaderDrag(object sender, ToolDragEventArgs e)
		{
			var header = (ToolHeader)sender;
			var window = Window.GetWindow(this) as ToolDragWindow;

			if (window != null)
			{
				var fromPoint = header.TranslatePoint(e.Anchor, window);
				var toPoint = e.GetPosition(window);
				var oldCorner = new Point(window.Left, window.Top);
				var target = oldCorner - fromPoint + toPoint;

				window.Top = target.Y;
				window.Left = target.X;

				var point = WindowUtils.GetScreenPosition();
				var dragTarget = WindowUtils.FindRootVisualIgnoringWindow(point, window) as UIElement;

				if (dragTarget != null)
				{
					var cloth = dragTarget as ToolDropCloth;

					if (cloth != null)
					{
						dragTarget = cloth.Root;
					}

					var tester = new HitTester();
					tester.Find(dragTarget, point);

					if (tester.Root != null)
					{
						if (_root != null && _root != tester.Root)
						{
							DockDragUtils.DoDragLeave(_root);
							_root = null;
						}

						var newRootControl = (ContentControl)tester.Root;
						DockDragUtils.DoDragOver(newRootControl, tester.Panel, newRootControl.PointFromScreen(point));
						_root = newRootControl;
						return;
					}
				}

				if (_root != null)
				{
					DockDragUtils.DoDragLeave(_root);
					_root = null;
				}
			}
		}

		void HeaderEndDrag(object sender, ToolDragEndedEventArgs e)
		{
			var window = Window.GetWindow(this) as ToolDragWindow;

			if (window != null && !e.Cancelled)
			{
				var point = WindowUtils.GetScreenPosition();
				var dragTarget = WindowUtils.FindRootVisualIgnoringWindow(point, window) as UIElement;

				if (dragTarget != null)
				{
					var cloth = dragTarget as ToolDropCloth;

					if (cloth != null)
					{
						dragTarget = cloth.Root;
					}

					var tester = new HitTester();
					tester.Find(dragTarget, point);

					var newRootControl = (ContentControl)tester.Root;

					if (newRootControl != null)
					{
						DockDragUtils.DoDragDrop(newRootControl, tester.Panel, this, newRootControl.PointFromScreen(point));

						if (_root == newRootControl)
						{
							DockDragUtils.DoDragLeave(_root);
							_root = null;
						}
					}
				}
			}

			if (_root != null)
			{
				DockDragUtils.DoDragLeave(_root);

				_root = null;
			}

			if (window != null)
			{
				window.IsDragging = false;
			}
		}

		static void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var control = (ToolPanel)sender;
				PanelEvents.RaiseRemove(control);
			}
		}

		static object CoerceHeader(DependencyObject d, object baseValue)
		{
			return baseValue ?? string.Empty;
		}

		ContentControl _root;
	}
}
