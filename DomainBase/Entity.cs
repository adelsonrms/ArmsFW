using Portal.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArmsFW.Domain
{

    public class Entity : Entity<int>
	{
	}
	public class Entity<TKeyDataType> : IEFEntity where TKeyDataType : IEquatable<TKeyDataType>
	{
		private eLastCommand _lasCommad;

		[Key]
		public virtual TKeyDataType Id { get; set; }

		[Ignore]
		[NotMapped]
		public string EFGuid { get; set; }
		

		public void setLastCommand(eLastCommand cmd)
		{
			_lasCommad = cmd;
		}

		public eLastCommand getLastCommand()
		{
			return _lasCommad;
		}

		public override string ToString()
		{
			return "ID : " + Id.ToString();
		}

		public bool IsPersist()
		{
			throw new NotImplementedException();
		}
	}
}

namespace Portal.Domain.Enums
{
    public enum eLastCommand
	{
		Inserted,
		Updated,
		Deleted
	}
}