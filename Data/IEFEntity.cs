using System;

namespace app.core.Domain
{
	public interface IEFEntity
	{
		string EFGuid { get; set; }

		bool IsPersist();
	}

	public interface IJsonEntity
	{
		string Id { get; set; }
		DateTime dt_criacao { get; set; }
		string fl_status { get; set; }

		bool IsPersist();
	}
}
