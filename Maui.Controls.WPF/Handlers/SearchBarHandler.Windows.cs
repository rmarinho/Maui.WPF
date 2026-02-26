using System.Windows;
using System.Windows.Input;
using WGrid = System.Windows.Controls.Grid;
using WButton = System.Windows.Controls.Button;
using WTextBox = System.Windows.Controls.TextBox;
using WColumnDefinition = System.Windows.Controls.ColumnDefinition;
using WTextChangedEventArgs = System.Windows.Controls.TextChangedEventArgs;

namespace Microsoft.Maui.Handlers.WPF
{
	public partial class SearchBarHandler : WPFViewHandler<ISearchBar, WGrid>
	{
		WTextBox _textBox = null!;
		WButton _searchButton = null!;

		protected override WGrid CreatePlatformView()
		{
			var grid = new WGrid();
			grid.ColumnDefinitions.Add(new WColumnDefinition { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new WColumnDefinition { Width = System.Windows.GridLength.Auto });

			_textBox = new WTextBox { MinWidth = 100 };
			_searchButton = new WButton { Content = "🔍", Padding = new System.Windows.Thickness(8, 2, 8, 2) };

			WGrid.SetColumn(_textBox, 0);
			WGrid.SetColumn(_searchButton, 1);

			grid.Children.Add(_textBox);
			grid.Children.Add(_searchButton);

			return grid;
		}

		protected override void ConnectHandler(WGrid platformView)
		{
			base.ConnectHandler(platformView);
			_textBox.TextChanged += OnTextChanged;
			_textBox.KeyDown += OnKeyDown;
			_searchButton.Click += OnSearchClicked;
		}

		protected override void DisconnectHandler(WGrid platformView)
		{
			_textBox.TextChanged -= OnTextChanged;
			_textBox.KeyDown -= OnKeyDown;
			_searchButton.Click -= OnSearchClicked;
			base.DisconnectHandler(platformView);
		}

		void OnTextChanged(object sender, WTextChangedEventArgs e)
		{
			if (VirtualView == null) return;
			VirtualView.Text = _textBox.Text;
		}

		void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				VirtualView?.SearchButtonPressed();
		}

		void OnSearchClicked(object sender, RoutedEventArgs e)
		{
			VirtualView?.SearchButtonPressed();
		}

		public static void MapText(SearchBarHandler handler, ISearchBar searchBar)
		{
			if (handler._textBox.Text != searchBar.Text)
				handler._textBox.Text = searchBar.Text;
		}

		public static void MapPlaceholder(SearchBarHandler handler, ISearchBar searchBar)
		{
		}

		public static void MapTextColor(SearchBarHandler handler, ISearchBar searchBar)
		{
			if (searchBar.TextColor != null)
				handler._textBox.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb((byte)(searchBar.TextColor.Alpha * 255),
						(byte)(searchBar.TextColor.Red * 255),
						(byte)(searchBar.TextColor.Green * 255),
						(byte)(searchBar.TextColor.Blue * 255)));
		}

		public static void MapCancelButtonColor(SearchBarHandler handler, ISearchBar searchBar)
		{
		}
	}
}
