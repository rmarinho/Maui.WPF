using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class CheckBoxDemoPage : ContentPage
{
	public CheckBoxDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		CheckStatusLabel.Text = $"IsChecked: {e.Value}";
	}
}
