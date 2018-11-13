// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows.Controls;

namespace DockLib
{
	static class CommonValidation
	{
		public static bool ValidateOrientation(object value)
		{
			switch ((Orientation)value)
			{
				case Orientation.Horizontal:
				case Orientation.Vertical:
					return true;

				default:
					return false;
			}
		}

		public static bool ValidateFiniteAndGreaterThanZero(object value)
		{
			var d = (double)value;
			return !double.IsNaN(d) && !double.IsInfinity(d) && d > 0;
		}

		public static bool ValidateFiniteAndGreaterThanOrEqualToZero(object value)
		{
			var d = (double)value;
			return !double.IsNaN(d) && !double.IsInfinity(d) && d >= 0;
		}
	}
}
