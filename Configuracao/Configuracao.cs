namespace ArmsFW.Services.Shared.Settings
{
    public class Configuracao
    {
        public Configuracao()
        {
        }
        public Configuracao(string configName)
        {
            this.Name = configName;
        }

        public Configuracao(string configName, string configValue):this(configName)
        {
            this.Value = configValue;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
