// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;

namespace DockLib
{
	sealed class DockResourceConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(MarkupExtension) && context is IValueSerializerContext)
			{
				return true;
			}

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(MarkupExtension) &&
				value is IValueSerializerContext vsc &&
				value is DockResourceKey key &&
				vsc.GetValueSerializerFor(typeof(Type)) is var ser &&
				ser != null)
			{
				return new StaticExtension(ser.ConvertToString(typeof(DockResources), vsc) + "." + key.ResourceID.ToString());
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
