// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Media;

namespace DockLib
{
	sealed class HitTester
	{
		public IDockRoot Root { get; private set; }
		public ToolPanel Panel { get; private set; }

		public void Find(Visual rootVisual, Point pt)
		{
			VisualTreeHelper.HitTest(
				rootVisual,
				HitTestFilter,
				HitTestResult,
				new PointHitTestParameters(rootVisual.PointFromScreen(pt)));
		}

		HitTestFilterBehavior HitTestFilter(DependencyObject potentialHitTestTarget)
		{
			var root = potentialHitTestTarget as IDockRoot;

			if (root != null)
			{
				Root = root;
				return HitTestFilterBehavior.ContinueSkipSelf;
			}

			var panel = potentialHitTestTarget as ToolPanel;

			if (panel != null)
			{
				Panel = panel;
				return HitTestFilterBehavior.Stop;
			}

			return HitTestFilterBehavior.ContinueSkipSelf;
		}

		HitTestResultBehavior HitTestResult(HitTestResult result)
		{
			return HitTestResultBehavior.Stop;
		}
	}
}
