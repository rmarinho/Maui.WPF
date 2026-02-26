using System;
using Microsoft.Maui.Controls;

namespace Maui.Controls.Sample.WPF;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	void OnButtonClicked(object sender, EventArgs e)
	{
		if (sender is Button btn)
			btn.Text = "Clicked!";
	}
}
