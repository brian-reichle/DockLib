// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace DockLib
{
	[TypeConverter(typeof(DockResourceConverter))]
	[DebuggerDisplay("Dock Resource: {ResourceID}")]
	sealed class DockResourceKey : ResourceKey, IEquatable<DockResourceKey>
	{
		public DockResourceKey(DockResourceKeyID resourceID)
		{
			ResourceID = resourceID;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[ConstructorArgument("resourceID")]
		public DockResourceKeyID ResourceID { get; }
		public override Assembly Assembly => Assembly.GetExecutingAssembly();
		public override bool Equals(object obj) => Equals(obj as DockResourceKey);
		public bool Equals(DockResourceKey key) => key != null && key.ResourceID == ResourceID;
		public override int GetHashCode() => typeof(DockResourceKey).GetHashCode() ^ ResourceID.GetHashCode();
		public override string ToString() => ResourceID.ToString();
	}
}
