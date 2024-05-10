#region "Namepaces do Sistema"
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Expoe entidades genericas

#endregion


namespace ArmsFW.Services.UI
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Rotulo { get; set; }
        public int ParentMenuId { get; set; }
        public string tipo { get; set; }
        public string Icone { get; set; }
        public string Acao { get; set; }
        public bool Ativo { get; set; }
        public bool Grupo { get; set; }
        public int Nivel { get; set; }
        public int Ordem { get; set; }
        public bool Habilitado { get; set; }
        public string CustumHTML { get; set; }
        public string partialPath { get; set; }
        [NotMapped]
        public List<string> Roles { get; set; } = new List<string>();
        [NotMapped]
        public List<Menu> SubMenus { get; set; } = new List<Menu>();
        [NotMapped]
        public bool CriarMenu { get; set; }
        [NotMapped]
        public Menu Parent { get; set; }
        public string PerfilSelecionado { get; set; }
        public string Perfil { get; set; }
        public bool EntraNoMenu { get; set; }

        public override string ToString() => $"Menu ID : {this.Id} - Rotulo {this.Rotulo} / Action : {Acao}";
    }
}