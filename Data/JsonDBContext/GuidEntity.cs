using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArmsFW.Data.JsonStore.Old
{
    public class GuidEntity : IJsonEntity
	{
		public DateTime dt_criacao { get;  set; }

        [Key]
		public string Id { get;  set; }
		public void NovoId() => Id = Guid.NewGuid().ToString();

		public GuidEntity() => dt_criacao = DateTime.Now;

		[JsonIgnore]
        public string fl_status { get; set; }

        public override string ToString()
		{
			return Id;
		}

		public bool IsPersist()
		{
			throw new NotImplementedException();
		}
	}
}
