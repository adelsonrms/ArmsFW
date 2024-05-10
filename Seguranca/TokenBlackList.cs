using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    [Table("TokenBlackList", Schema = "Auth")]
    public class TokenBlackList
    {
        /// <summary>
        /// Id unico 
        /// </summary>
        [Key]
        [Column("id_token")]
        public int Id { get; set; }

        [Column("cd_token")]
        public string Token { get; set; }

        [Column("dt_criacao")]
        public DateTime? DataCriacao { get; set; } = DateTime.Now;
        public string Motivo { get; internal set; }

        public override string ToString()
        {
            return $"Token ID {Id} - Adicionado em {DataCriacao}";
        }
    }
}


