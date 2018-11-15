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

						RaiseBeginDrag(e, _state.MouseDownPoint);
					}
				}

				if (_state.IsDragging)
				{
					RaiseDrag(e, _state.MouseDownPoint);
				}
			}

			base.OnMouseMove(e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			var state = _state;

			if (state != null && state.IsDragging)
			{
				_state = null;
				e.Handled = true;
				RaiseEndDrag(e, state.MouseDownPoint, false);
			}

			ReleaseMouseCapture();

			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnGotMouseCapture(MouseEventArgs e)
		{
			InputManager.Current.PreProcessInput += OnPreProcessInput;
			base.OnGotMouseCapture(e);
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			InputManager.Current.PreProcessInput -= OnPreProcessInput;
			base.OnLostMouseCapture(e);
			e.MouseDevice.UpdateCursor();

			var state = _state;

			if (state != null)
			{
				_state = null;

				RaiseEndDrag(e, state.MouseDownPoint, true);
			}
		}

		void OnPreProcessInput(object sender, PreProcessInputEventArgs e)
		{
			var args = e.StagingItem.Input;

			if (args.RoutedEvent == PreviewKeyDownEvent)
			{
				var keyArgs = (KeyEventArgs)args;

				args.Handled = true;
				e.Cancel();

				if (keyArgs.Key == Key.Escape)
				{
					ReleaseMouseCapture();
				}
			}
			else if (args.RoutedEvent == PreviewKeyUpEvent ||
				args.RoutedEvent == PreviewTextInputEvent)
			{
				args.Handled = true;
				e.Cancel();
			}
		}

		void RaiseBeginDrag(MouseEventArgs e, Point mouseDownPoint)
			=> BeginDrag?.Invoke(this, new ToolDragEventArgs(e.MouseDevice, e.Timestamp, mouseDownPoint));

		void RaiseDrag(MouseEventArgs e, Point mouseDownPoint)
			=> Drag?.Invoke(this, new ToolDragEventArgs(e.MouseDevice, e.Timestamp, mouseDownPoint));

		void RaiseEndDrag(MouseEventArgs e, Point mouseDownPoint, bool cancelled)
			=> EndDrag?.Invoke(this, new ToolDragEndedEventArgs(e.MouseDevice, e.Timestamp, mouseDownPoint, cancelled));

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
