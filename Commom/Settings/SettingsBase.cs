namespace ArmsFW.Services.Shared.Settings
{
	public class ConfiguracaoBase<T> : ISettings<T> where T : class
	{
		public string FilePathSettings { get; set; }

		public T Settings { get; set; }

		public TaskResult Result { get; set; }

		public T Load()
		{
			return AppSettings.GetSection<T>();
		}

		public T LoadStore()
		{
			return Config.Load<T>().Settings;
		}

		public override string ToString()
		{
			return ((Settings != null) ? $"Configurações carregadas Objeto : {Settings.GetType().Name} - {Settings}" : "") ?? "";
		}
	}
}
