namespace Microsoft.Maui.Handlers.WPF
{
	public partial class ButtonHandler
	{
		public static IPropertyMapper<IButton, ButtonHandler> Mapper = new PropertyMapper<IButton, ButtonHandler>(ViewHandler.ViewMapper)
		{
			[nameof(ITextStyle.Font)] = MapFont,
			[nameof(IText.Text)] = MapText,
			[nameof(ITextStyle.TextColor)] = MapTextColor,
			[nameof(IButton.Padding)] = MapPadding,
		};

		public static CommandMapper<IButton, ButtonHandler> CommandMapper = new(ViewCommandMapper)
		{
		};

		public ButtonHandler() : base(Mapper, CommandMapper)
		{
		}

		public ButtonHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
		{
		}
	}
}
