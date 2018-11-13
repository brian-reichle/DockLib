// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DockLib.Primitives;

namespace DockLib
{
	public class ToolSplitPanel : Panel
	{
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			nameof(Orientation),
			typeof(Orientation),
			typeof(ToolSplitPanel),
			new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
			CommonValidation.ValidateOrientation);

		public static readonly DependencyProperty SplitterThicknessProperty = DependencyProperty.Register(
			nameof(SplitterThickness),
			typeof(double),
			typeof(ToolSplitPanel),
			new FrameworkPropertyMetadata(4d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
			CommonValidation.ValidateFiniteAndGreaterThanZero);

		static readonly DependencyPropertyKey TotalSplitFactorPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(TotalSplitFactor),
			typeof(double),
			typeof(ToolSplitPanel),
			new FrameworkPropertyMetadata(0d),
			CommonValidation.ValidateFiniteAndGreaterThanOrEqualToZero);

		public static readonly DependencyProperty TotalSplitFactorProperty = TotalSplitFactorPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SplitFactorProperty = DependencyProperty.RegisterAttached(
			"SplitFactor",
			typeof(double),
			typeof(ToolSplitPanel),
			new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
			CommonValidation.ValidateFiniteAndGreaterThanZero);

		static ToolSplitPanel()
		{
			EventManager.RegisterClassHandler(typeof(ToolSplitPanel), PanelEvents.RemoveEvent, (RoutedEventHandler)OnRemove);
			EventManager.RegisterClassHandler(typeof(ToolSplitPanel), PanelEvents.NotifyUnwrapEvent, (RoutedEventHandler)OnNotifyUnwrap);
		}

		public ToolSplitPanel()
		{
			_splitters = new List<ToolSplitter>();
		}

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public double SplitterThickness
		{
			get => (double)GetValue(SplitterThicknessProperty);
			set => SetValue(SplitterThicknessProperty, value);
		}

		public double TotalSplitFactor
		{
			get => (double)GetValue(TotalSplitFactorProperty);
			private set => SetValue(TotalSplitFactorPropertyKey, value);
		}

		[AttachedPropertyBrowsableForChildren]
		public static double GetSplitFactor(DependencyObject dobj) => (double)dobj.GetValue(SplitFactorProperty);
		public static void SetSplitFactor(DependencyObject dobj, double value) => dobj.SetValue(SplitFactorProperty, value);

		protected override bool HasLogicalOrientation => true;
		protected override Orientation LogicalOrientation => Orientation;

		protected override IEnumerator LogicalChildren
		{
			get
			{
				var enumerator = base.LogicalChildren;
				{
					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
				}

				foreach (var child in _splitters)
				{
					yield return child;
				}
			}
		}

		protected override int VisualChildrenCount
		{
			get { return base.VisualChildrenCount + _splitters.Count; }
		}

		protected override Visual GetVisualChild(int index)
		{
			var baseCount = base.VisualChildrenCount;

			if (index < baseCount)
			{
				return base.GetVisualChild(index);
			}
			else
			{
				return _splitters[index - baseCount];
			}
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var children = InternalChildren;

			var totalFactor = children.CalculateTotalFactor();
			TotalSplitFactor = totalFactor;

			var count = children.Count;
			var totalSplitterSize = count > 1 ? (count - 1) * SplitterThickness : 0;
			var isHorizontal = Orientation == Orientation.Horizontal;
			var axisLength = isHorizontal ? availableSize.Width : availableSize.Height;
			var availableAxisUnit = double.IsInfinity(axisLength) ? axisLength : Math.Max(0, axisLength - totalSplitterSize) / totalFactor;
			var requiredUnitHeight = 0d;
			var requiredUnitWidth = 0d;

			SyncSplitters(count);

			foreach (UIElement element in Children)
			{
				var factor = GetSplitFactor(element);

				var size = isHorizontal
					? new Size(availableAxisUnit * factor, availableSize.Height)
					: new Size(availableSize.Width, availableAxisUnit * factor);

				element.Measure(size);

				var x = element.DesiredSize.Width;
				var y = element.DesiredSize.Height;

				if (isHorizontal)
				{
					x /= factor;
				}
				else
				{
					y /= factor;
				}

				if (x > requiredUnitWidth)
				{
					requiredUnitWidth = x;
				}

				if (y > requiredUnitHeight)
				{
					requiredUnitHeight = y;
				}
			}

			Size splitterSize;

			if (isHorizontal)
			{
				requiredUnitWidth = (requiredUnitWidth * totalFactor) + totalSplitterSize;
				splitterSize = new Size(SplitterThickness, requiredUnitHeight);
			}
			else
			{
				requiredUnitHeight = (requiredUnitHeight * totalFactor) + totalSplitterSize;
				splitterSize = new Size(requiredUnitWidth, SplitterThickness);
			}

			foreach (var splitter in _splitters)
			{
				splitter.Measure(splitterSize);
			}

			return new Size(requiredUnitWidth, requiredUnitHeight);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var children = InternalChildren;
			var totalFactor = TotalSplitFactor;

			if (totalFactor == 0)
			{
				return finalSize;
			}

			var count = children.Count;
			var totalSplitterSize = count > 1 ? (count - 1) * SplitterThickness : 0;
			var isHorizontal = Orientation == Orientation.Horizontal;
			var axisTotalLength = isHorizontal ? finalSize.Width : finalSize.Height;
			var axisUnit = Math.Max(0, axisTotalLength - totalSplitterSize) / totalFactor;
			var offset = 0d;

			for (var i = 0; i < count; i++)
			{
				var element = children[i];
				var axisLength = GetSplitFactor(element) * axisUnit;

				var rect = isHorizontal
					? new Rect(offset, 0, axisLength, finalSize.Height)
					: new Rect(0, offset, finalSize.Width, axisLength);

				element.Arrange(rect);

				offset += axisLength;

				if (i < _splitters.Count)
				{
					rect = isHorizontal
						? new Rect(offset, 0, SplitterThickness, finalSize.Height)
						: new Rect(0, offset, finalSize.Width, SplitterThickness);

					_splitters[i].Arrange(rect);

					offset += SplitterThickness;
				}
			}

			return finalSize;
		}

		static void OnRemove(object sender, RoutedEventArgs e)
		{
			if (!e.Handled)
			{
				var panel = (ToolSplitPanel)sender;
				var child = (FrameworkElement)e.OriginalSource;

				if (panel != child)
				{
					e.Handled = true;

					panel.Children.Remove(child);

					switch (panel.Children.Count)
					{
						case 0:
							PanelEvents.RaiseRemove(panel);
							break;

						case 1:
							PanelEvents.RaiseNotifyUnwrap(panel);
							break;
					}
				}
			}
		}

		static void OnNotifyUnwrap(object sender, RoutedEventArgs e)
		{
			if (!e.Handled)
			{
				var panel = (ToolSplitPanel)sender;
				var child = (ToolSplitPanel)e.OriginalSource;

				if (panel != child)
				{
					e.Handled = true;

					var index = panel.Children.IndexOf(child);

					if (index >= 0)
					{
						var grandChild = child.Children[0];
						var factor = GetSplitFactor(child);

						child.Children.Remove(grandChild);
						panel.Children.RemoveAt(index);

						var gcSplitPanel = grandChild as ToolSplitPanel;

						if (gcSplitPanel != null && gcSplitPanel.Orientation == panel.Orientation)
						{
							var mul = factor / gcSplitPanel.TotalSplitFactor;

							while (gcSplitPanel.Children.Count > 0)
							{
								var tmp = gcSplitPanel.Children[0];
								gcSplitPanel.Children.RemoveAt(0);

								SetSplitFactor(tmp, mul * GetSplitFactor(tmp));
								panel.Children.Insert(index, tmp);
							}
						}
						else
						{
							panel.Children.Insert(index, grandChild);
							SetSplitFactor(grandChild, factor);
						}
					}
				}
			}
		}

		void SyncSplitters(int count)
		{
			if (count > 0)
			{
				count--;
			}

			if (count < _splitters.Count)
			{
				for (var i = count; i < _splitters.Count; i++)
				{
					RemoveLogicalChild(_splitters[i]);
					RemoveVisualChild(_splitters[i]);
				}

				_splitters.RemoveRange(count, _splitters.Count - count);
			}
			else
			{
				while (count > _splitters.Count)
				{
					var splitter = CreateSplitter();
					_splitters.Add(splitter);
					AddLogicalChild(splitter);
					AddVisualChild(splitter);
				}
			}
		}

		ToolSplitter CreateSplitter()
		{
			var splitter = new ToolSplitter();
			splitter.Orientation = Orientation;
			splitter.BeginDrag += SplitterBeginDrag;
			splitter.Drag += SplitterDrag;
			splitter.EndDrag += SplitterEndDrag;
			return splitter;
		}

		Rect CalculateNewSplitterRect(ToolSplitter splitter, ToolDragEventArgs e)
		{
			var transform = splitter.TransformToAncestor(this);
			var splitterRect = transform.TransformBounds(new Rect(0, 0, splitter.ActualWidth, splitter.ActualHeight));
			var dragFrom = transform.Transform(new Point(e.Anchor.X, e.Anchor.Y));
			var dragTo = e.MouseDevice.GetPosition(this);
			var bounds = CalculateAcceptableBounds(splitter);

			double x;
			double y;

			if (Orientation == Orientation.Horizontal)
			{
				x = ClipSlider(bounds.X, bounds.Width, splitterRect.Width, splitterRect.X - dragFrom.X + dragTo.X);
				y = 0;
			}
			else
			{
				x = 0;
				y = ClipSlider(bounds.Y, bounds.Height, splitterRect.Height, splitterRect.Y - dragFrom.Y + dragTo.Y);
			}

			return new Rect(new Point(x, y), splitterRect.Size);
		}

		Rect CalculateAcceptableBounds(ToolSplitter splitter)
		{
			var index = _splitters.IndexOf(splitter);

			Point topLeft, bottomRight;

			if (index > 0)
			{
				var previous = _splitters[index - 1];

				topLeft = previous
					.TransformToAncestor(this)
					.Transform(new Point(previous.ActualWidth, previous.ActualHeight));
			}
			else
			{
				topLeft = new Point();
			}

			if (index + 1 < _splitters.Count)
			{
				var next = _splitters[index + 1];

				bottomRight = next
					.TransformToAncestor(this)
					.Transform(new Point());
			}
			else
			{
				bottomRight = new Point(ActualWidth, ActualHeight);
			}

			return new Rect(topLeft, bottomRight);
		}

		static double ClipSlider(double offset, double bredth, double thickness, double pos)
		{
			const double Gap = 20d;

			var lowerBound = offset + Gap;
			var upperBound = offset + bredth - thickness - Gap;

			if (lowerBound > upperBound)
			{
				return (lowerBound + upperBound) / 2;
			}

			if (pos < lowerBound)
			{
				return lowerBound;
			}

			if (pos > upperBound)
			{
				return upperBound;
			}

			return pos;
		}

		void SplitterBeginDrag(object sender, ToolDragEventArgs e)
		{
			var splitter = (ToolSplitter)sender;

			var adorner = new ToolSplitterAdorner(this);
			adorner.Style = splitter.PreviewStyle;
			adorner.Rectangle = CalculateNewSplitterRect(splitter, e);

			var layer = AdornerLayer.GetAdornerLayer(this);
			layer.Add(adorner);
			_adorner = adorner;
		}

		void SplitterDrag(object sender, ToolDragEventArgs e)
		{
			var adorner = _adorner;

			if (adorner != null)
			{
				adorner.Rectangle = CalculateNewSplitterRect((ToolSplitter)sender, e);
			}
		}

		void SplitterEndDrag(object sender, ToolDragEndedEventArgs e)
		{
			var adorner = _adorner;

			if (adorner != null)
			{
				var layer = AdornerLayer.GetAdornerLayer(this);
				layer.Remove(adorner);
				_adorner = null;
			}

			if (!e.Cancelled)
			{
				var splitter = (ToolSplitter)sender;
				var index = _splitters.IndexOf(splitter);

				if (index >= 0)
				{
					SetSplitterPosition(index, CalculateNewSplitterRect(splitter, e));
				}
			}
		}

		void SetSplitterPosition(int index, Rect newPosition)
		{
			if (Children.Count > index + 1)
			{
				var splitter = _splitters[index];
				var prev = (FrameworkElement)Children[index];
				var next = (FrameworkElement)Children[index + 1];
				var bounds = CalculateAcceptableBounds(splitter);

				var prevFactor = GetSplitFactor(prev);
				var nextfactor = GetSplitFactor(next);

				double offset, boundsSize, pos;

				if (Orientation == Orientation.Horizontal)
				{
					offset = bounds.X;
					boundsSize = bounds.Width - splitter.ActualWidth;
					pos = newPosition.X;
				}
				else
				{
					offset = bounds.Y;
					boundsSize = bounds.Height - splitter.ActualHeight;
					pos = newPosition.Y;
				}

				var combinedFactor = prevFactor + nextfactor;
				var unitSize = boundsSize / combinedFactor;

				prevFactor = (pos - offset) / unitSize;
				nextfactor = combinedFactor - prevFactor;

				SetSplitFactor(prev, prevFactor);
				SetSplitFactor(next, nextfactor);
			}
		}

		readonly List<ToolSplitter> _splitters;
		ToolSplitterAdorner _adorner;
	}
}
