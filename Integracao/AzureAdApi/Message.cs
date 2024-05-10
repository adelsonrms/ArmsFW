using System;

namespace ArmsFW.Services.Azure
{
	public class Message : GraphEntity
	{
		public DateTimeOffset? CreatedDateTime { get; set; }

		public DateTimeOffset? LastModifiedDateTime { get; set; }

		public DateTimeOffset? ReceivedDateTime { get; set; }

		public string DataCriacao => CreatedDateTime?.ToString("dd/MM/yyyy hh:mm:ss");

		public string DataModificacao => LastModifiedDateTime?.ToString("dd/MM/yyyy hh:mm:ss");

		public string DataRecebimento => ReceivedDateTime?.ToString("dd/MM/yyyy hh:mm:ss");
	}
}
