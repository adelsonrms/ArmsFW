namespace ArmsFW.Services.Shared.Settings
{
	public interface ISettings<T>
	{
		string FilePathSettings { get; set; }

		T Settings { get; set; }

		T Load();
	}
}
