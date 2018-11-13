// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;

namespace DockLib
{
	[StyleTypedProperty(Property = nameof(ToolWindowStyle), StyleTargetType = typeof(ToolDragWindow))]
	public class ToolHost : ContentControl, IDockRoot
	{
		public static readonly DependencyProperty ToolWindowStyleProperty = DependencyProperty.RegisterAttached(
			"ToolWindowStyle",
			typeof(Style),
			typeof(ToolHost),
			new FrameworkPropertyMetadata(null, OnToolWindowStyleChanged));

		static ToolHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolHost), new FrameworkPropertyMetadata(typeof(ToolHost)));
			FocusableProperty.OverrideMetadata(typeof(ToolHost), new FrameworkPropertyMetadata(false));
			IsTabStopProperty.OverrideMetadata(typeof(ToolHost), new FrameworkPropertyMetadata(false));

			EventManager.RegisterClassHandler(typeof(ToolHost), PanelEvents.RemoveEvent, (RoutedEventHandler)OnRemove);
			EventManager.RegisterClassHandler(typeof(ToolHost), PanelEvents.NotifyUnwrapEvent, (RoutedEventHandler)OnNotifyUnwrap);
			EventManager.RegisterClassHandler(typeof(ToolHost), PanelEvents.DetachEvent, (RoutedEventHandler)OnDetach);
		}

		public ToolHost()
		{
			ToolWindows = new ToolDragWindowCollection();
			Unloaded += OnUnloaded;
		}

		public ToolDragWindowCollection ToolWindows { get; }

		public Style ToolWindowStyle
		{
			get => (Style)GetValue(ToolWindowStyleProperty);
			set => SetValue(ToolWindowStyleProperty, value);
		}

		public void SummonToolWindows()
		{
			if (ToolWindows.Count > 0)
			{
				var arr = new Window[ToolWindows.Count + 1];

				for (var i = 0; i < ToolWindows.Count; i++)
				{
					arr[ToolWindows.Count - i - 1] = ToolWindows[i];
				}

				arr[arr.Length - 1] = Window.GetWindow(this);

				WindowUtils.SetTopWindowSequence(arr);
			}
		}

		static void OnRemove(object sender, RoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var host = (ToolHost)sender;
				var child = (FrameworkElement)e.OriginalSource;

				if (host.Content == child)
				{
					host.Content = null;
				}
			}
		}

		static void OnNotifyUnwrap(object sender, RoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var host = (ToolHost)sender;
				var child = (ToolSplitPanel)e.OriginalSource;

				if (host.Content == child)
				{
					var grandChild = child.Children[0];

					child.Children.Remove(grandChild);
					host.Content = grandChild;
					host.ClearValue(ToolSplitPanel.SplitFactorProperty);
				}
			}
		}

		static void OnDetach(object sender, RoutedEventArgs e)
		{
			var host = (ToolHost)sender;
			var panel = (ToolPanel)e.OriginalSource;
			var sourceWindow = Window.GetWindow(host);

			var width = panel.ActualWidth;
			var height = panel.ActualHeight;
			var point = panel.TranslatePoint(new Point(0, 0), sourceWindow);

			if (PanelEvents.RaiseRemove(panel))
			{
				var window = new ToolDragWindow(host)
				{
					Height = height,
					Width = width,
					Content = panel,
					Left = sourceWindow.Left + point.X,
					Top = sourceWindow.Top + point.Y,
					Style = host.ToolWindowStyle,
				};

				window.Show();
			}
		}

		static void OnUnloaded(object sender, RoutedEventArgs e)
		{
			var host = (ToolHost)sender;
			var windows = host.ToolWindows;

			while (windows.Count > 0)
			{
				windows[windows.Count - 1].Close();
			}
		}

		static void OnToolWindowStyleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var host = (ToolHost)sender;
			var newStyle = (Style)e.NewValue;

			foreach (var window in host.ToolWindows)
			{
				window.Style = newStyle;
			}
		}
	}
}
