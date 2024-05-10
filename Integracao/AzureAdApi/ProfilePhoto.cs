namespace ArmsFW.Services.Azure
{
	public class ProfilePhoto : GraphEntity
	{
		public int? Height { get; set; }

		public int? Width { get; set; }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(base.Response?.Conteudo))
            {
                return base.Response?.Conteudo;
            }
            return string.Empty;
        }
    }
}
