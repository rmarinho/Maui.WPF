using System;
using Microsoft.Maui.Controls;
using Maui.Controls.Sample.WPF.Views;

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

	void NavigateTo(Page page)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = page;
	}

	void OnLabelDemoClicked(object sender, EventArgs e) => NavigateTo(new LabelDemoPage());
	void OnButtonDemoClicked(object sender, EventArgs e) => NavigateTo(new ButtonDemoPage());
	void OnEntryDemoClicked(object sender, EventArgs e) => NavigateTo(new EntryDemoPage());
	void OnEditorDemoClicked(object sender, EventArgs e) => NavigateTo(new EditorDemoPage());
	void OnCheckBoxDemoClicked(object sender, EventArgs e) => NavigateTo(new CheckBoxDemoPage());
	void OnSwitchDemoClicked(object sender, EventArgs e) => NavigateTo(new SwitchDemoPage());
	void OnSliderDemoClicked(object sender, EventArgs e) => NavigateTo(new SliderDemoPage());
	void OnProgressBarDemoClicked(object sender, EventArgs e) => NavigateTo(new ProgressBarDemoPage());
	void OnActivityIndicatorDemoClicked(object sender, EventArgs e) => NavigateTo(new ActivityIndicatorDemoPage());
	void OnDatePickerDemoClicked(object sender, EventArgs e) => NavigateTo(new DatePickerDemoPage());
	void OnTimePickerDemoClicked(object sender, EventArgs e) => NavigateTo(new TimePickerDemoPage());
	void OnPickerDemoClicked(object sender, EventArgs e) => NavigateTo(new PickerDemoPage());
	void OnStepperDemoClicked(object sender, EventArgs e) => NavigateTo(new StepperDemoPage());
	void OnSearchBarDemoClicked(object sender, EventArgs e) => NavigateTo(new SearchBarDemoPage());
	void OnBorderDemoClicked(object sender, EventArgs e) => NavigateTo(new BorderDemoPage());
	void OnBoxViewDemoClicked(object sender, EventArgs e) => NavigateTo(new BoxViewDemoPage());
	void OnScrollViewDemoClicked(object sender, EventArgs e) => NavigateTo(new ScrollViewDemoPage());
	void OnShapesDemoClicked(object sender, EventArgs e) => NavigateTo(new ShapesDemoPage());
}
