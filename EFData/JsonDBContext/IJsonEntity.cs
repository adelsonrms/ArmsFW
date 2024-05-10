using System;

namespace ArmsFW.Infra.Data.JsonStore.Old
{
    public interface IJsonEntity
	{
		string Id { get; set; }
		DateTime dt_criacao { get; set; }
		string fl_status { get; set; }

		bool IsPersist();
	}
}
