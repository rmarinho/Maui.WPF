namespace Microsoft.Maui.Handlers.WPF
{
	public partial class SearchBarHandler
	{
		public static IPropertyMapper<ISearchBar, SearchBarHandler> Mapper = new PropertyMapper<ISearchBar, SearchBarHandler>(ViewHandler.ViewMapper)
		{
			[nameof(ISearchBar.Text)] = MapText,
			[nameof(ISearchBar.Placeholder)] = MapPlaceholder,
			[nameof(ITextStyle.TextColor)] = MapTextColor,
			[nameof(ISearchBar.CancelButtonColor)] = MapCancelButtonColor,
		};

		public static CommandMapper<ISearchBar, SearchBarHandler> CommandMapper = new(ViewCommandMapper)
		{
		};

		public SearchBarHandler() : base(Mapper, CommandMapper)
		{
		}

		public SearchBarHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
		{
		}
	}
}
