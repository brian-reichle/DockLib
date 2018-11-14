// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;

namespace DockLib
{
	static class PanelEvents
	{
		public static readonly RoutedEvent RemoveEvent = EventManager.RegisterRoutedEvent("Remove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanelEvents));
		public static readonly RoutedEvent NotifyUnwrapEvent = EventManager.RegisterRoutedEvent("NotifyUnwrap", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanelEvents));
		public static readonly RoutedEvent DetachEvent = EventManager.RegisterRoutedEvent("Detach", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PanelEvents));

		public static bool RaiseRemove(FrameworkElement element)
		{
			var args = new RoutedEventArgs(RemoveEvent, element);
			element.RaiseEvent(args);
			return args.Handled;
		}

		public static void RaiseNotifyUnwrap(ToolSplitPanel element)
			=> element.RaiseEvent(new RoutedEventArgs(NotifyUnwrapEvent, element));

		public static void RaiseDetach(ToolPanel panel)
			=> panel.RaiseEvent(new RoutedEventArgs(DetachEvent, panel));
	}
}
