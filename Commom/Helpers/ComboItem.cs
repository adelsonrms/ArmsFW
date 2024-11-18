using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArmsFW.Services.Shared.Helpers
{
	public class ComboItem : SelectListItem
	{
		public int RowIndex { get; set; }
		public string Grupo { get; set; }

		public string Hierarquia { get; set; }

		public int Nivel { get; set; }

		public ComboItem()
		{

		}

		public ComboItem(string value, string text)
			: base(text, value)
		{
		}

		public ComboItem(string value, string text, bool value_selecionado)
		 : base(text, value, value_selecionado)
		{
		}


		public SelectListItem GetSelectListItem => new SelectListItem { Value = this.Value, Text = this.Text, Selected = this.Selected };
		public override string ToString() => base.Value + " - " + base.Text;
	}
}
