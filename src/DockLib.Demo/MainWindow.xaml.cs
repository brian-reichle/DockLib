using System;
using System.Windows;

namespace DockLib.Demo
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			Host.SummonToolWindows();
		}
	}
}
