using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DockLib.Demo
{
	sealed class BigRedX : FrameworkElement
	{
		public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(
			typeof(BigRedX),
			new FrameworkPropertyMetadata()
			{
				AffectsRender = true,
			});

		public static readonly DependencyProperty BackgroundProperty = Border.BackgroundProperty.AddOwner(
			typeof(BigRedX),
			new FrameworkPropertyMetadata()
			{
				AffectsRender = true,
			});

		static BigRedX()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BigRedX), new FrameworkPropertyMetadata(typeof(BigRedX)));
			FocusableProperty.OverrideMetadata(typeof(BigRedX), new FrameworkPropertyMetadata(true));
		}

		public Brush Foreground
		{
			get => (Brush)GetValue(ForegroundProperty);
			set => SetValue(ForegroundProperty, value);
		}

		public Brush Background
		{
			get => (Brush)GetValue(BackgroundProperty);
			set => SetValue(BackgroundProperty, value);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				Focus();
			}

			base.OnMouseDown(e);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(
				double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width,
				double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return finalSize;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var rect = new Rect(0, 0, ActualWidth, ActualHeight);
			var pen = new Pen(Foreground, 2);
			drawingContext.DrawRectangle(Background, pen, rect);
			drawingContext.DrawLine(pen, rect.TopLeft, rect.BottomRight);
			drawingContext.DrawLine(pen, rect.TopRight, rect.BottomLeft);
		}
	}
}
