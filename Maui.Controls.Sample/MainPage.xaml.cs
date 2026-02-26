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

	void NavigateTo(Page page)
	{
		if (Application.Current?.Windows.Count > 0)
			Application.Current.Windows[0].Page = page;
	}

	void OnLabelClicked(object sender, EventArgs e) => NavigateTo(new LabelDemoPage());
	void OnButtonClicked(object sender, EventArgs e) => NavigateTo(new ButtonDemoPage());
	void OnEntryClicked(object sender, EventArgs e) => NavigateTo(new EntryDemoPage());
	void OnEditorClicked(object sender, EventArgs e) => NavigateTo(new EditorDemoPage());
	void OnCheckBoxClicked(object sender, EventArgs e) => NavigateTo(new CheckBoxDemoPage());
	void OnSwitchClicked(object sender, EventArgs e) => NavigateTo(new SwitchDemoPage());
	void OnSliderClicked(object sender, EventArgs e) => NavigateTo(new SliderDemoPage());
	void OnProgressBarClicked(object sender, EventArgs e) => NavigateTo(new ProgressBarDemoPage());
	void OnActivityIndicatorClicked(object sender, EventArgs e) => NavigateTo(new ActivityIndicatorDemoPage());
	void OnDatePickerClicked(object sender, EventArgs e) => NavigateTo(new DatePickerDemoPage());
	void OnTimePickerClicked(object sender, EventArgs e) => NavigateTo(new TimePickerDemoPage());
	void OnPickerClicked(object sender, EventArgs e) => NavigateTo(new PickerDemoPage());
	void OnStepperClicked(object sender, EventArgs e) => NavigateTo(new StepperDemoPage());
	void OnSearchBarClicked(object sender, EventArgs e) => NavigateTo(new SearchBarDemoPage());
	void OnBorderClicked(object sender, EventArgs e) => NavigateTo(new BorderDemoPage());
	void OnBoxViewClicked(object sender, EventArgs e) => NavigateTo(new BoxViewDemoPage());
	void OnScrollViewClicked(object sender, EventArgs e) => NavigateTo(new ScrollViewDemoPage());
	void OnShapesClicked(object sender, EventArgs e) => NavigateTo(new ShapesDemoPage());
}
