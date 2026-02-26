using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF;

namespace Maui.Controls.Sample.WPF.Views;

public partial class DatePickerDemoPage : ContentPage
{
	public DatePickerDemoPage()
	{
		InitializeComponent();
	}

	void OnBackClicked(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = new MainPage();
	}

	void OnDateSelected(object sender, DateChangedEventArgs e)
	{
		SelectedDateLabel.Text = $"Selected: {e.NewDate:D}";
	}

	void OnRangeDateSelected(object sender, DateChangedEventArgs e)
	{
		RangeDateLabel.Text = $"Selected: {e.NewDate:yyyy-MM-dd}";
	}
}
