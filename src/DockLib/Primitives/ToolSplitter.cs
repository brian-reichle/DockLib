// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DockLib.Primitives
{
	[StyleTypedProperty(Property = nameof(PreviewStyle), StyleTargetType = typeof(ToolSplitterAdorner))]
	public class ToolSplitter : Control
	{
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			nameof(Orientation),
			typeof(Orientation),
			typeof(ToolSplitter));

		public static readonly DependencyProperty PreviewStyleProperty = DependencyProperty.Register(
			nameof(PreviewStyle),
			typeof(Style),
			typeof(ToolSplitter));

		static ToolSplitter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolSplitter), new FrameworkPropertyMetadata(typeof(ToolSplitter)));
			IsTabStopProperty.OverrideMetadata(typeof(ToolSplitter), new FrameworkPropertyMetadata(false));
		}

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public Style PreviewStyle
		{
			get => (Style)GetValue(PreviewStyleProperty);
			set => SetValue(PreviewStyleProperty, value);
		}

		public event EventHandler<ToolDragEventArgs> BeginDrag;
		public event EventHandler<ToolDragEventArgs> Drag;
		public event EventHandler<ToolDragEndedEventArgs> EndDrag;

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			e.Handled = true;

			if (e.MouseDevice.Capture(this))
			{
				_state = new State(e.GetPosition(this));
				Focus();
			}

			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_state != null)
			{
				e.Handled = true;

				if (!_state.IsDragging)
				{
					var movement = e.GetPosition(this) - _state.MouseDownPoint;

					if (Math.Abs(movement.X) > SystemParameters.MinimumHorizontalDragDistance ||
						Math.Abs(movement.Y) > SystemParameters.MinimumVerticalDragDistance)
					{
						_state.IsDragging = true;

						BeginDrag?.Invoke(this, new ToolDragEventArgs(e.MouseDevice, e.Timestamp, _state.MouseDownPoint));
					}
				}

				if (_state.IsDragging)
				{
					Drag?.Invoke(this, new ToolDragEventArgs(e.MouseDevice, e.Timestamp, _state.MouseDownPoint));
				}
			}

			base.OnMouseMove(e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (_state != null)
			{
				e.Handled = true;
				DoEndDrag(e.MouseDevice, e, false);
			}

			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e != null && e.Key == Key.Escape && e.IsDown)
			{
				e.Handled = true;
				DoEndDrag(InputManager.Current.PrimaryMouseDevice, e, true);
			}

			base.OnKeyDown(e);
		}

		void DoEndDrag(MouseDevice device, InputEventArgs e, bool cancelled)
		{
			if (device.Captured == this)
			{
				ReleaseMouseCapture();
				device.UpdateCursor();
			}

			var state = _state;

			if (state != null)
			{
				_state = null;

				EndDrag?.Invoke(this, new ToolDragEndedEventArgs(device, e.Timestamp, state.MouseDownPoint, cancelled));
			}
		}

		State _state;

		sealed class State
		{
			public State(Point mouseDown)
			{
				MouseDownPoint = mouseDown;
			}

			public Point MouseDownPoint { get; }
			public bool IsDragging { get; set; }
		}
	}
}
