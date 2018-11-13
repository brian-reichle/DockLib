// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DockLib
{
	[DebuggerDisplay("Count = {Count}")]
	public sealed class ToolDragWindowCollection : IList, IList<ToolDragWindow>
	{
		public ToolDragWindowCollection()
		{
			_windows = new List<ToolDragWindow>();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Count => _windows.Count;
		public ToolDragWindow this[int index] => _windows[index];
		public int IndexOf(ToolDragWindow item) => item == null ? -1 : _windows.IndexOf(item);
		public bool Contains(ToolDragWindow item) => IndexOf(item) >= 0;
		public void CopyTo(ToolDragWindow[] array, int arrayIndex) => _windows.CopyTo(array, arrayIndex);
		public IEnumerator<ToolDragWindow> GetEnumerator() => _windows.GetEnumerator();

		public void MoveToTop(ToolDragWindow window)
		{
			if (window == null) throw new ArgumentNullException(nameof(window));

			var index = _windows.IndexOf(window);
			if (index < 0) throw new ArgumentException("window not in collection.", nameof(window));
			if (index + 1 == _windows.Count) return;

			while (true)
			{
				var nextIndex = index + 1;

				if (nextIndex >= _windows.Count)
				{
					break;
				}

				_windows[index] = _windows[nextIndex];
				index = nextIndex;
			}

			_windows[index] = window;
		}

		internal void AddInternal(ToolDragWindow window) => _windows.Add(window);
		internal void RemoveInternal(ToolDragWindow window) => _windows.Remove(window);

		#region IList<ToolDragWindow> Members

		void IList<ToolDragWindow>.Insert(int index, ToolDragWindow item)
		{
			throw new NotSupportedException();
		}

		void IList<ToolDragWindow>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		ToolDragWindow IList<ToolDragWindow>.this[int index]
		{
			[DebuggerStepThrough]
			get { return this[index]; }
			set { throw new NotSupportedException(); }
		}

		#endregion

		#region ICollection<ToolDragWindow> Members

		void ICollection<ToolDragWindow>.Add(ToolDragWindow item)
		{
			throw new NotSupportedException();
		}

		void ICollection<ToolDragWindow>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<ToolDragWindow>.Remove(ToolDragWindow item)
		{
			throw new NotSupportedException();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<ToolDragWindow>.IsReadOnly => true;

		#endregion

		#region IList Members

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object value) => Contains(value as ToolDragWindow);
		int IList.IndexOf(object value) => IndexOf(value as ToolDragWindow);

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsFixedSize => true;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsReadOnly => true;

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		object IList.this[int index]
		{
			[DebuggerStepThrough]
			get { return this[index]; }
			set { throw new NotSupportedException(); }
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index) => ((ICollection)_windows).CopyTo(array, index);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized => false;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot => ((ICollection)_windows).SyncRoot;

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<ToolDragWindow> _windows;
	}
}
