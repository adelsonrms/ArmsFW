using System.Net;
using ArmsFW.Lib.Web.Json;

namespace ArmsFW.Lib.Web.HttpRest
{
	

	public class ResponseOld
	{
		public HttpStatusCode Status { get; set; }

		public string Description { get; set; }

		//public JsonData Data { get; private set; } = new JsonData();


		public dynamic HttpResponse { get; set; }

		public string Conteudo { get; set; }

		public bool Success { get; set; }

		public string ContentAsString()
		{
			return HttpResponse?.Content;
		}
		
		public void SetDataObject(dynamic responseObject)
		{
		}

        public override string ToString()
        {
            return (this.Success? $"Sucesso ! {Status} | {Conteudo?.Substring(0,30)}": $"Sucesso ! {Status}");
        }
    }
}
