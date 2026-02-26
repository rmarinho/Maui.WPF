using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class SearchBarDemoPage : ContentPage
{
	Label[] _items;

	public SearchBarDemoPage()
	{
		InitializeComponent();
		_items = new[] { Item1, Item2, Item3, Item4, Item5, Item6, Item7 };
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnSearchButtonPressed(object sender, EventArgs e)
	{
		SearchResultLabel.Text = $"Searched for: \"{DemoSearchBar.Text}\"";
	}

	void OnSearchTextChanged(object sender, TextChangedEventArgs e)
	{
		var filter = e.NewTextValue?.ToLowerInvariant() ?? string.Empty;
		foreach (var item in _items)
		{
			item.IsVisible = string.IsNullOrEmpty(filter) || item.Text.ToLowerInvariant().Contains(filter);
		}
	}
}
