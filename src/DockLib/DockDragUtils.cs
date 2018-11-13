// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using DockLib.Primitives;

namespace DockLib
{
	static class DockDragUtils
	{
		static readonly DependencyPropertyKey ShowOverlayPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
			"ShowOverlay",
			typeof(bool),
			typeof(DockDragUtils),
			new PropertyMetadata(false, OnShowOverlayChanged));

		static readonly DependencyPropertyKey DropClothPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
			"DropCloth",
			typeof(ToolDropCloth),
			typeof(DockDragUtils),
			new PropertyMetadata(OnDropClothChanged));

		public static void DoDragOver(ContentControl root, ToolPanel over, Point mousePoint)
		{
			Rect? rect;

			if (over != null)
			{
				rect = over.TransformToAncestor(root).TransformBounds(new Rect(0, 0, over.ActualWidth, over.ActualHeight));
			}
			else
			{
				rect = null;
			}

			SetShowOverlay(root, true);

			var adorner = GetDropCloth(root);
			adorner.Overlay.PanelRect = rect;
			adorner.Overlay.TargetType = adorner.Overlay.CalculateTargetType(mousePoint);
		}

		public static void DoDragLeave(ContentControl root)
		{
			SetShowOverlay(root, false);
		}

		public static void DoDragDrop(ContentControl root, ToolPanel over, ToolPanel draggedPanel, Point mousePoint)
		{
			if (!GetShowOverlay(root))
			{
				return;
			}

			var adorner = GetDropCloth(root);
			var targetType = adorner.Overlay.CalculateTargetType(mousePoint);

			switch (targetType)
			{
				case ToolDropTargetType.OuterLeft:
					DoDropOuter(root, draggedPanel, Orientation.Horizontal, true);
					break;

				case ToolDropTargetType.OuterRight:
					DoDropOuter(root, draggedPanel, Orientation.Horizontal, false);
					break;

				case ToolDropTargetType.OuterTop:
					DoDropOuter(root, draggedPanel, Orientation.Vertical, true);
					break;

				case ToolDropTargetType.OuterBottom:
					DoDropOuter(root, draggedPanel, Orientation.Vertical, false);
					break;

				case ToolDropTargetType.InnerLeft:
					DoDropInner(over, draggedPanel, Orientation.Horizontal, true);
					break;

				case ToolDropTargetType.InnerRight:
					DoDropInner(over, draggedPanel, Orientation.Horizontal, false);
					break;

				case ToolDropTargetType.InnerTop:
					DoDropInner(over, draggedPanel, Orientation.Vertical, true);
					break;

				case ToolDropTargetType.InnerBottom:
					DoDropInner(over, draggedPanel, Orientation.Vertical, false);
					break;

				default:
					break;
			}

			SetShowOverlay(root, false);
		}

		public static double CalculateTotalFactor(this UIElementCollection children)
		{
			var totalFactor = 0d;

			foreach (UIElement element in children)
			{
				var factor = ToolSplitPanel.GetSplitFactor(element);
				totalFactor += factor;
			}

			return totalFactor;
		}

		static bool GetShowOverlay(DependencyObject dobj) => (bool)dobj.GetValue(ShowOverlayPropertyKey.DependencyProperty);
		static void SetShowOverlay(DependencyObject dobj, bool value) => dobj.SetValue(ShowOverlayPropertyKey, value);
		static ToolDropCloth GetDropCloth(DependencyObject dobj) => (ToolDropCloth)dobj.GetValue(DropClothPropertyKey.DependencyProperty);
		static void SetDropCloth(DependencyObject dobj, ToolDropCloth value) => dobj.SetValue(DropClothPropertyKey, value);

		static void DoDropInner(ToolPanel over, ToolPanel draggedPanel, Orientation orientation, bool isLow)
		{
			var panel = over.Parent as ToolSplitPanel;
			int index;

			if (panel == null)
			{
				var root = (ContentControl)over.Parent;

				panel = new ToolSplitPanel()
				{
					Orientation = orientation,
				};

				root.Content = panel;
				panel.Children.Add(over);
				index = isLow ? 0 : 1;
			}
			else if (panel.Orientation != orientation)
			{
				var oldParentPanel = panel;

				panel = new ToolSplitPanel()
				{
					Orientation = orientation,
				};

				index = oldParentPanel.Children.IndexOf(over);
				oldParentPanel.Children.RemoveAt(index);
				oldParentPanel.Children.Insert(index, panel);
				ToolSplitPanel.SetSplitFactor(panel, ToolSplitPanel.GetSplitFactor(over));

				index = isLow ? 0 : 1;

				panel.Children.Add(over);
				over.ClearValue(ToolSplitPanel.SplitFactorProperty);
			}
			else
			{
				index = panel.Children.IndexOf(over);

				if (!isLow)
				{
					index++;
				}
			}

			var overFactor = ToolSplitPanel.GetSplitFactor(over);
			var factor = overFactor * 2d / 5d;

			ToolSplitPanel.SetSplitFactor(draggedPanel, factor);
			ToolSplitPanel.SetSplitFactor(over, overFactor - factor);

			PanelEvents.RaiseRemove(draggedPanel);
			panel.Children.Insert(index, draggedPanel);
		}

		static void DoDropOuter(ContentControl root, ToolPanel draggedPanel, Orientation orientation, bool isLow)
		{
			var panel = root.Content as ToolSplitPanel;

			if (panel == null || panel.Orientation != orientation)
			{
				var old = root.Content as FrameworkElement;

				panel = new ToolSplitPanel()
				{
					Orientation = orientation,
				};

				root.Content = panel;

				if (old != null)
				{
					panel.Children.Add(old);
				}
			}

			var index = isLow ? 0 : panel.Children.Count;

			double factor;

			if (panel.Children.Count == 0)
			{
				factor = 1;
			}
			else
			{
				var axisRange = orientation == Orientation.Horizontal ? root.ActualWidth : root.ActualHeight;
				var axisSection = Math.Min(100, axisRange / 5d * 2d);
				var existingFactor = panel.Children.CalculateTotalFactor();
				factor = existingFactor * axisSection / (axisRange - axisSection);
			}

			ToolSplitPanel.SetSplitFactor(draggedPanel, factor);

			PanelEvents.RaiseRemove(draggedPanel);
			panel.Children.Insert(index, draggedPanel);
		}

		static void OnShowOverlayChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			var root = (FrameworkElement)sender;

			if (true.Equals(args.NewValue))
			{
				SetDropCloth(root, new ToolDropCloth(root));
			}
			else
			{
				SetDropCloth(root, null);
			}
		}

		static void OnPanelRectChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			var root = (FrameworkElement)sender;

			var adorner = GetDropCloth(root);

			if (adorner != null)
			{
				var rect = (Rect?)args.NewValue;
				adorner.Overlay.PanelRect = rect;
			}
		}

		static void OnDropClothChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			if (args.OldValue != null)
			{
				var window = (ToolDropCloth)args.OldValue;
				window.Close();
			}

			if (args.NewValue != null)
			{
				var window = (ToolDropCloth)args.NewValue;
				window.Show();
			}
		}
	}
}
