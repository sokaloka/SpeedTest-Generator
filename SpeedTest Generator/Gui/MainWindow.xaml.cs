using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NotMissing.Logging;
using SpeedTest;

namespace SpeedTest_Generator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ThreadPool.QueueUserWorkItem(GrabServers);
		}

		void GrabServers(object o)
		{
			try
			{
				DisplayServers(STApi.GetServers());
			}
			catch (Exception ex)
			{
				DisplayServers(null);
				StaticLogger.Error(ex);
			}
		}

		public void DisplayServers(STServers servers)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.BeginInvoke(new Action<STServers>(DisplayServers), servers);
				return;
			}

			ServersTree.Items.Clear();

			if (servers == null)
			{
				ServersTree.Items.Add(new TreeViewItem { Header = "Errored" });
				return;
			}

			var countries = servers.Select(s => s.Country).Distinct().OrderBy(s => s);
			foreach (var country in countries)
			{
				var countryitem = new TreeViewItem { Header = country };
				var names = servers.Where(s => s.Country == country).Select(s => s.Name).Distinct().OrderBy(s => s);
				foreach (var name in names)
				{
					var nameitem = new TreeViewItem { Header = name };
					var sponsors = servers.Where(s => s.Country == country && s.Name == name).OrderBy(s => s.Sponsor);
					foreach (var sp in sponsors)
					{
						var sponsoritem = new TreeViewItem { Header = sp.Sponsor, Tag = sp };
						nameitem.Items.Add(sponsoritem);
					}
					countryitem.Items.Add(nameitem);
				}
				ServersTree.Items.Add(countryitem);
			}
		}

		public void DisplayResults(Int64 id)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.BeginInvoke(new Action<Int64>(DisplayResults), id);
				return;
			}

			var resultwindow = new ResultInfo();
			if (id != 0)
			{
				var url = string.Format("http://www.speedtest.net/result/{0}.png", id);
				resultwindow.ResultImage.Source = new BitmapImage(new Uri(url));
				resultwindow.ResultText.Text = url;
			}
			else
			{
				resultwindow.ResultText.Text = "Error";
			}
			resultwindow.ShowDialog();
		}

		void GrabResults(STServer server, int download, int upload, int ping)
		{
			try
			{
				DisplayResults(STApi.Generate(server, download, upload, ping));
			}
			catch (Exception ex)
			{
				DisplayResults(0);
				StaticLogger.Error(ex);
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var item = ServersTree.SelectedItem as TreeViewItem;
			if (item == null || !(item.Tag is STServer))
				return;

			var server = (STServer)item.Tag;
			var down = ParseOrZero(DownloadText.Text);
			var up = ParseOrZero(UploadText.Text);
			var ping = ParseOrZero(PingText.Text);
			ThreadPool.QueueUserWorkItem(o => GrabResults(server, down, up, ping));
		}

		static int ParseOrZero(string str)
		{
			int ret;
			return int.TryParse(str, out ret) ? ret : 0;
		}

		private void ServersTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			var item = GetTreeItem(e.OriginalSource);
			if (item != null)
			{
				item.IsSelected = true;
				e.Handled = true;
			}
		}

		static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
		{
			while (source != null && source.GetType() != typeof(T))
				source = VisualTreeHelper.GetParent(source);

			return source;
		}

		static TreeViewItem GetTreeItem(object source)
		{
			return VisualUpwardSearch<TreeViewItem>(source as DependencyObject) as TreeViewItem;
		}
					
		private void ServersTree_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			var item = GetTreeItem(e.OriginalSource);
			if (item == null || !(item.Tag is STServer))
			{
				e.Handled = true;
			}
		}

	}
}
