namespace Microsoft.Maui.Handlers.WPF
{
	public partial class EditorHandler
	{
		public static IPropertyMapper<IEditor, EditorHandler> Mapper = new PropertyMapper<IEditor, EditorHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IEditor.Text)] = MapText,
			[nameof(IEditor.Placeholder)] = MapPlaceholder,
			[nameof(ITextStyle.TextColor)] = MapTextColor,
			[nameof(ITextStyle.Font)] = MapFont,
			[nameof(IEditor.IsReadOnly)] = MapIsReadOnly,
		};

		public static CommandMapper<IEditor, EditorHandler> CommandMapper = new(ViewCommandMapper)
		{
		};

		public EditorHandler() : base(Mapper, CommandMapper)
		{
		}

		public EditorHandler(IPropertyMapper? mapper, CommandMapper? commandMapper = null)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
		{
		}
	}
}
