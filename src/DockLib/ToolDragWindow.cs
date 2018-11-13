// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Input;
using DockLib.Primitives;

namespace DockLib
{
	[TemplatePart(Name = "PART_Header", Type = typeof(ToolHeader))]
	public sealed class ToolDragWindow : Window, IDockRoot
	{
		static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly(
			"IsDragging",
			typeof(bool),
			typeof(ToolDragWindow),
			new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;

		static readonly DependencyPropertyKey HasSinglePanelPropertyKey = DependencyProperty.RegisterReadOnly(
			"HasSinglePanel",
			typeof(bool),
			typeof(ToolDragWindow),
			new FrameworkPropertyMetadata(true));

		public static readonly DependencyProperty HasSinglePanelProperty = HasSinglePanelPropertyKey.DependencyProperty;

		static ToolDragWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolDragWindow), new FrameworkPropertyMetadata(typeof(ToolDragWindow)));

			EventManager.RegisterClassHandler(typeof(ToolDragWindow), PanelEvents.RemoveEvent, (RoutedEventHandler)OnRemove);
			EventManager.RegisterClassHandler(typeof(ToolDragWindow), PanelEvents.NotifyUnwrapEvent, (RoutedEventHandler)OnNotifyUnwrap);
			EventManager.RegisterClassHandler(typeof(ToolDragWindow), PanelEvents.DetachEvent, (RoutedEventHandler)OnDetach);

			CommandManager.RegisterClassCommandBinding(typeof(ToolDragWindow), new CommandBinding(ToolHeader.Close, OnCloseExecuted));
		}

		public ToolDragWindow(ToolHost host)
		{
			Host = host;
		}

		public ToolHost Host { get; }

		public bool IsDragging
		{
			get => (bool)GetValue(IsDraggingProperty);
			internal set => SetValue(IsDraggingPropertyKey, value);
		}

		public bool HasSinglePanel
		{
			get => (bool)GetValue(HasSinglePanelPropertyKey.DependencyProperty);
			private set => SetValue(HasSinglePanelPropertyKey, value);
		}

		protected override DependencyObject GetUIParentCore()
		{
			return Host;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			Host.ToolWindows.AddInternal(this);
		}

		protected override void OnClosed(EventArgs e)
		{
			Host.ToolWindows.RemoveInternal(this);
			base.OnClosed(e);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowUtils.MakeToolWindow(this);
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			HasSinglePanel = !(newContent is ToolSplitPanel);
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			Host.ToolWindows.MoveToTop(this);
		}

		public override void OnApplyTemplate()
		{
			if (_header != null)
			{
				_header.BeginDrag -= HeaderBeginDrag;
				_header.Drag -= HeaderDrag;
			}

			base.OnApplyTemplate();

			_header = (ToolHeader)GetTemplateChild("PART_Header");

			if (_header != null)
			{
				_header.BeginDrag += HeaderBeginDrag;
				_header.Drag += HeaderDrag;
			}
		}

		void HeaderBeginDrag(object sender, ToolDragEventArgs e)
		{
			var header = (ToolHeader)sender;
			header.SetupDrag(e);
			IsDragging = true;
		}

		void HeaderDrag(object sender, ToolDragEventArgs e)
		{
			var header = (ToolHeader)sender;
			var fromPoint = header.TranslatePoint(e.Anchor, this);
			var toPoint = e.GetPosition(this);
			var oldCorner = new Point(Left, Top);
			var target = oldCorner - fromPoint + toPoint;

			Top = target.Y;
			Left = target.X;
		}

		static void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var control = (ToolDragWindow)sender;
				control.Close();
			}
		}

		static void OnRemove(object sender, RoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var window = (ToolDragWindow)sender;
				var child = (FrameworkElement)e.OriginalSource;

				if (window.Content == child)
				{
					window.Content = null;
					window.Close();
				}
			}
		}

		static void OnNotifyUnwrap(object sender, RoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var window = (ToolDragWindow)sender;
				var child = (ToolSplitPanel)e.OriginalSource;

				if (window.Content == child)
				{
					var grandChild = child.Children[0];

					child.Children.Remove(grandChild);
					window.Content = grandChild;
					window.ClearValue(ToolSplitPanel.SplitFactorProperty);
				}
			}
		}

		static void OnDetach(object sender, RoutedEventArgs e)
		{
			if (!e.Handled)
			{
				var window = (ToolDragWindow)sender;
				var child = (FrameworkElement)e.OriginalSource;

				if (window.Content == child)
				{
					e.Handled = true;
				}
			}
		}

		ToolHeader _header;
	}
}
