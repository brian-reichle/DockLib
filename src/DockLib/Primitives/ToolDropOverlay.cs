// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DockLib.Primitives
{
	public class ToolDropOverlay : FrameworkElement
	{
		public static readonly DependencyProperty PanelRectProperty = DependencyProperty.Register(
			nameof(PanelRect),
			typeof(Rect?),
			typeof(ToolDropOverlay),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public static readonly DependencyProperty TargetTypeProperty = DependencyProperty.Register(
			nameof(TargetType),
			typeof(ToolDropTargetType),
			typeof(ToolDropOverlay),
			new FrameworkPropertyMetadata(ToolDropTargetType.None, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public static readonly DependencyProperty PaddingProperty = Control.PaddingProperty.AddOwner(typeof(ToolDropOverlay));

		static ToolDropOverlay()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolDropOverlay), new FrameworkPropertyMetadata(typeof(ToolDropOverlay)));
		}

		public ToolDropOverlay()
		{
			_outerChildren = new[]
			{
				new ToolDropTargetPoint() { TargetType = ToolDropTargetType.OuterLeft },
				new ToolDropTargetPoint() { TargetType = ToolDropTargetType.OuterRight },
				new ToolDropTargetPoint() { TargetType = ToolDropTargetType.OuterTop },
				new ToolDropTargetPoint() { TargetType = ToolDropTargetType.OuterBottom },
			};

			for (var i = 0; i < _outerChildren.Length; i++)
			{
				AddLogicalChild(_outerChildren[i]);
				AddVisualChild(_outerChildren[i]);
			}
		}

		public Rect? PanelRect
		{
			get => (Rect?)GetValue(PanelRectProperty);
			set => SetValue(PanelRectProperty, value);
		}

		public ToolDropTargetType TargetType
		{
			get => (ToolDropTargetType)GetValue(TargetTypeProperty);
			set => SetValue(TargetTypeProperty, value);
		}

		public Thickness Padding
		{
			get => (Thickness)GetValue(PaddingProperty);
			set => SetValue(PaddingProperty, value);
		}

		public ToolDropTargetType CalculateTargetType(Point pt)
		{
			ToolDropTargetPoint point = null;

			VisualTreeHelper.HitTest(
				this,
				x =>
				{
					if (x == this)
					{
						return HitTestFilterBehavior.ContinueSkipSelf;
					}
					else if ((point = x as ToolDropTargetPoint) != null)
					{
						return HitTestFilterBehavior.ContinueSkipChildren;
					}
					else
					{
						return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
					}
				},
				x => HitTestResultBehavior.Stop,
				new PointHitTestParameters(pt));

			if (point != null)
			{
				return point.TargetType;
			}

			return ToolDropTargetType.None;
		}

		protected override IEnumerator LogicalChildren
		{
			get
			{
				foreach (var child in _outerChildren)
				{
					yield return child;
				}

				if (_innerChildren != null)
				{
					foreach (var child in _innerChildren)
					{
						yield return child;
					}
				}

				if (_preview != null)
				{
					yield return _preview;
				}
			}
		}

		protected override int VisualChildrenCount
		{
			get
			{
				var count = _outerChildren.Length;

				if (_innerChildren != null)
				{
					count += _innerChildren.Length;
				}

				if (_preview != null)
				{
					count++;
				}

				return count;
			}
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index < _outerChildren.Length)
			{
				return _outerChildren[index];
			}

			index -= _outerChildren.Length;

			if (_innerChildren != null)
			{
				if (index < _innerChildren.Length)
				{
					return _innerChildren[index];
				}

				index -= _innerChildren.Length;
			}

			if (index == 0)
			{
				return _preview;
			}

			throw new ArgumentOutOfRangeException(nameof(index));
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var width = availableSize.Width;
			var height = availableSize.Height;
			var padding = Padding;

			if (!double.IsInfinity(width))
			{
				width = (width - padding.Left - padding.Right) / 2;
			}

			if (!double.IsInfinity(height))
			{
				height = (height - padding.Top - padding.Bottom) / 2;
			}

			MeasureOuter(width, height);
			MeasureInner();

			height = _outerChildren[2].DesiredSize.Height + _outerChildren[3].DesiredSize.Height;
			width = _outerChildren[0].DesiredSize.Width + _outerChildren[1].DesiredSize.Width;

			var resultSize = new Size(width + padding.Left + padding.Right, height + padding.Top + padding.Bottom);

			MeasurePreview(resultSize);

			return resultSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			ArrangeTargets(finalSize);
			ArrangePreview(finalSize);
			return finalSize;
		}

		void MeasureOuter(double width, double height)
		{
			var size = new Size(width, height);

			for (var i = 0; i < _outerChildren.Length; i++)
			{
				_outerChildren[i].Measure(size);
			}
		}

		void MeasureInner()
		{
			var panelRect = PanelRect;

			if (panelRect.HasValue)
			{
				if (_innerChildren == null)
				{
					_innerChildren = new FrameworkElement[]
					{
						new ToolDropTargetPoint() { TargetType = ToolDropTargetType.InnerLeft },
						new ToolDropTargetPoint() { TargetType = ToolDropTargetType.InnerRight },
						new ToolDropTargetPoint() { TargetType = ToolDropTargetType.InnerTop },
						new ToolDropTargetPoint() { TargetType = ToolDropTargetType.InnerBottom },
					};

					for (var i = 0; i < _innerChildren.Length; i++)
					{
						var child = _innerChildren[i];
						AddLogicalChild(child);
						AddVisualChild(child);
					}
				}

				for (var i = 0; i < _innerChildren.Length; i++)
				{
					_innerChildren[i].Measure(panelRect.Value.Size);
				}
			}
			else if (_innerChildren != null)
			{
				foreach (var child in _innerChildren)
				{
					RemoveLogicalChild(child);
					RemoveVisualChild(child);
				}

				_innerChildren = null;
			}
		}

		void MeasurePreview(Size outer)
		{
			var previewRect = CalculatePreviewRect(TargetType, outer, PanelRect);

			if (previewRect.HasValue)
			{
				if (_preview == null)
				{
					_preview = new ToolDropPreview();
					AddLogicalChild(_preview);
					AddVisualChild(_preview);
				}

				_preview.Measure(previewRect.Value.Size);
			}
			else if (_preview != null)
			{
				RemoveLogicalChild(_preview);
				RemoveVisualChild(_preview);
				_preview = null;
			}
		}

		void ArrangeTargets(Size finalSize)
		{
			var padding = Padding;

			var rect = new Rect(
				padding.Left,
				padding.Top,
				finalSize.Width - padding.Left - padding.Right,
				finalSize.Height - padding.Top - padding.Bottom);

			var vmid = (rect.Top + rect.Bottom) * 0.5;
			var hmid = (rect.Left + rect.Right) * 0.5;

			var lRect = Place(_outerChildren[0], rect.Left, vmid, hAlignment: HorizontalAlignment.Left);
			var rRect = Place(_outerChildren[1], rect.Right, vmid, hAlignment: HorizontalAlignment.Right);
			var tRect = Place(_outerChildren[2], hmid, rect.Top, vAlignment: VerticalAlignment.Top);
			var bRect = Place(_outerChildren[3], hmid, rect.Bottom, vAlignment: VerticalAlignment.Bottom);

			var panelRect = PanelRect;

			if (panelRect.HasValue && _innerChildren != null)
			{
				var centerHeight = Math.Max(_innerChildren[0].DesiredSize.Height, _innerChildren[1].DesiredSize.Height);
				var centerWidth = Math.Max(_innerChildren[2].DesiredSize.Width, _innerChildren[3].DesiredSize.Width);
				var halfCenterHeight = centerHeight / 2;
				var halfCenterWidth = centerWidth / 2;

				var center = Position(
					new Size(
						centerWidth + _innerChildren[1].DesiredSize.Width + _innerChildren[0].DesiredSize.Width,
						centerHeight + _innerChildren[2].DesiredSize.Height + _innerChildren[3].DesiredSize.Height),
					panelRect.Value,
					new Rect(
						lRect.Right + padding.Left,
						tRect.Bottom + padding.Top,
						rRect.Left - lRect.Right - padding.Left - padding.Right,
						bRect.Top - tRect.Bottom - padding.Top - padding.Bottom));

				Place(_innerChildren[0], center.X - halfCenterWidth, center.Y, hAlignment: HorizontalAlignment.Right);
				Place(_innerChildren[1], center.X - halfCenterWidth + centerWidth, center.Y, hAlignment: HorizontalAlignment.Left);
				Place(_innerChildren[2], center.X, center.Y - halfCenterHeight, vAlignment: VerticalAlignment.Bottom);
				Place(_innerChildren[3], center.X, center.Y - halfCenterHeight + centerHeight, vAlignment: VerticalAlignment.Top);
			}
		}

		void ArrangePreview(Size outer)
		{
			var previewRect = CalculatePreviewRect(TargetType, outer, PanelRect);

			if (previewRect.HasValue && _preview != null)
			{
				_preview.Arrange(previewRect.Value);
			}
		}

		static Point Position(Size size, Rect region, Rect bounds)
		{
			var x = region.Left + (region.Width / 2);
			var y = region.Top + (region.Height / 2);

			if (size.Width < bounds.Width)
			{
				var halfTotalWidth = size.Width / 2;

				var tmp = x - halfTotalWidth - bounds.Left;

				if (tmp < 0)
				{
					x -= tmp;
				}

				tmp = x - halfTotalWidth + size.Width - bounds.Right;

				if (tmp > 0)
				{
					x -= tmp;
				}
			}

			if (size.Height < bounds.Height)
			{
				var halfTotalHeight = size.Height / 2;
				var tmp = y - halfTotalHeight - bounds.Top;

				if (tmp < 0)
				{
					x -= tmp;
				}

				tmp = y - halfTotalHeight + size.Height - bounds.Bottom;

				if (tmp > 0)
				{
					y -= tmp;
				}
			}

			return new Point(x, y);
		}

		static Rect Place(FrameworkElement element, double x, double y, HorizontalAlignment hAlignment = HorizontalAlignment.Center, VerticalAlignment vAlignment = VerticalAlignment.Center)
		{
			var size = element.DesiredSize;

			switch (hAlignment)
			{
				case HorizontalAlignment.Left:
					break;

				case HorizontalAlignment.Center:
					x -= size.Width / 2;
					break;

				case HorizontalAlignment.Right:
					x -= size.Width;
					break;
			}

			switch (vAlignment)
			{
				case VerticalAlignment.Top:
					break;

				case VerticalAlignment.Center:
					y -= size.Height / 2;
					break;

				case VerticalAlignment.Bottom:
					y -= size.Height;
					break;
			}

			var rect = new Rect(new Point(x, y), size);
			element.Arrange(rect);
			return rect;
		}

		static Rect? CalculatePreviewRect(ToolDropTargetType target, Size outer, Rect? panel)
		{
			switch (target)
			{
				case ToolDropTargetType.OuterLeft:
					return new Rect(0, 0, Math.Min(100, outer.Width * Factor), outer.Height);

				case ToolDropTargetType.OuterRight:
					{
						var tmp = Math.Min(100, outer.Width * Factor);
						return new Rect(outer.Width - tmp, 0, outer.Width - tmp, outer.Height);
					}

				case ToolDropTargetType.OuterTop:
					return new Rect(0, 0, outer.Width, Math.Min(100, outer.Height * Factor));

				case ToolDropTargetType.OuterBottom:
					{
						var tmp = Math.Min(100, outer.Width * Factor);
						return new Rect(0, outer.Height - tmp, outer.Width, outer.Height - tmp);
					}
			}

			if (!panel.HasValue)
			{
				return null;
			}

			var p = panel.Value;

			switch (target)
			{
				case ToolDropTargetType.InnerLeft:
					return new Rect(p.Left, p.Top, p.Width * Factor, p.Height);

				case ToolDropTargetType.InnerRight:
					{
						var tmp = p.Width * Factor;
						return new Rect(p.Right - tmp, p.Top, tmp, p.Height);
					}

				case ToolDropTargetType.InnerTop:
					return new Rect(p.Left, p.Top, p.Width, p.Height * Factor);

				case ToolDropTargetType.InnerBottom:
					{
						var tmp = p.Height * Factor;
						return new Rect(p.Left, p.Bottom - tmp, p.Width, tmp);
					}
			}

			return null;
		}

		const double Factor = 2 / 5d;

		readonly FrameworkElement[] _outerChildren;
		FrameworkElement[] _innerChildren;
		ToolDropPreview _preview;
	}
}
