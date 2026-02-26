using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class ButtonDemoPage : ContentPage
{
	int _clickCount;

	public ButtonDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnCounterClicked(object sender, EventArgs e)
	{
		_clickCount++;
		CounterLabel.Text = $"Click count: {_clickCount}";
	}
}
