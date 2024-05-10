#region "Namepaces do Sistema"
using System.ComponentModel.DataAnnotations.Schema;

//Expoe entidades genericas

#endregion


namespace ArmsFW.Services.UI
{
    [Table("TPA.MenuPerfil")]
    public class MenuPerfil
    {
        public int MenuId { get; set; }
        public int PerfilId { get; set; }
        public char PerfilNome { get; set; }

        public override string ToString() => $"Menu ID : {this.MenuId} - Perfil ID {this.PerfilId}";
    }
}