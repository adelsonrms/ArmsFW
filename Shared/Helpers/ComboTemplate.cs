using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ArmsFW.Services.Shared.Helpers
{
	public class ComboTemplate
	{
		public ComboTemplate Criar() => new ComboTemplate();
		public string idCombo { get; set; }
        public string Rotulo { get; set; }
        public string Seletor { get; set; }
		[JsonIgnore]
		public string Comando { get; set; }
		[JsonIgnore]
		public bool IncluirOpcaoTodos { get; set; }

		public bool MultiSelecao { get; set; }
		public bool HabilitarSelecao { get; set; }

		

		[JsonIgnore]
		public string IdParent { get; set; }

		public string idControl { get; set; }
		[JsonIgnore]
		public bool HabilitaOnChange { get; set; }

        public string ExecutarAoSelecionar { get; set; }

        public List<ComboItem> ComboItems { get; set; }

		public string ItemSelecionado { get; set; }

		[JsonIgnore]
		public string BotaoSubmit_OnClick { get; set; }

		[JsonIgnore]
		public string BotaoSubmit_Label { get; set; }
		[JsonIgnore]
		public string CallBackOnChange { get; set; }

		public ComboTemplate(string comando)
			: this()
		{
			Comando = comando;
			ComboItems = CarregarItems(null);
		}

		public ComboTemplate()
		{
			idCombo = "GUID" + Guid.NewGuid().ToString().Replace("-", "");
			ComboItems = new List<ComboItem>();
		}

		public ComboTemplate(string idItemSelecionado, bool incluirOpcaoTodos)
			: this()
		{
			ComboItems = new List<ComboItem>();
			IncluirOpcaoTodos = incluirOpcaoTodos;
			ItemSelecionado = idItemSelecionado;
		}

		public string GetOnChange()
		{
			return GetOnChange(idCombo, CallBackOnChange);
		}

		public string GetOnChange(string id)
		{
			return GetOnChange(id, CallBackOnChange);
		}

		public string GetOnChange(string id, string callback)
		{
			return "\r\n            <script type='text/javascript'>\r\n                $('#" + id + "').on('change', \r\n                    function (event) {\r\n                        event.preventDefault(); \r\n                        " + (string.IsNullOrEmpty(idControl) ? "" : ("$('" + idControl + "').val($(this).val())")) + "\r\n                        " + (string.IsNullOrEmpty(callback) ? "" : (callback + "($(this).val());")) + "\r\n                     })\r\n            </script>\r\n            ";
		}

		public List<ComboItem> CarregarItems(IEnumerable<ComboItem> items)
		{
			List<ComboItem> list = new List<ComboItem>();
			if (IncluirOpcaoTodos)
			{
				list.Add(new ComboItem("0", "Todos"));
			}
			_ = Comando;
			if (items != null)
			{
				items.ToList().ForEach(delegate(ComboItem cmbItem)
				{
					SelectListGroup val = new SelectListGroup();
					val.Name=(cmbItem.Grupo);
					((SelectListItem)cmbItem).Group=(val);
				});
				list.AddRange(items);
			}
			ComboItems.AddRange(list);
			return list;
		}
	}
}
