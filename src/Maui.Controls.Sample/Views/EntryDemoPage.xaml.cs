using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class EntryDemoPage : ContentPage
{
	public EntryDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		CurrentTextLabel.Text = $"Current text: {e.NewTextValue}";
	}
}
