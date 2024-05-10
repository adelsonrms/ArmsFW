namespace ArmsFW.Domain
{
    public interface IEFEntity
	{
		string EFGuid { get; set; }

		bool IsPersist();
	}
}
