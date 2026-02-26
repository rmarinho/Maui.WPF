using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class ActivityIndicatorDemoPage : ContentPage
{
	public ActivityIndicatorDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnSwitchToggled(object sender, ToggledEventArgs e)
	{
		ToggleableIndicator.IsRunning = e.Value;
		ToggleStatusLabel.Text = $"Running: {e.Value}";
	}
}
