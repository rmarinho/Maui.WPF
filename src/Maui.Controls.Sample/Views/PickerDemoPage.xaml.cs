using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class PickerDemoPage : ContentPage
{
	public PickerDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnFruitSelected(object sender, EventArgs e)
	{
		if (FruitPicker.SelectedIndex >= 0)
			FruitSelectionLabel.Text = $"Selected: {FruitPicker.Items[FruitPicker.SelectedIndex]}";
	}
}
