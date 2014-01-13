using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [DisplayName("ID")]
        public int usuarioId { get; set; }

        [DisplayName("ID_Empresa")]
        public int empresaId { get; set; }

        [DisplayName("Login")]
        public string login { get; set; }

        [DisplayName("Nome")]
        public string nome { get; set; }

        [DisplayName("Dt_Cadastro")]
        public DateTime dt_cadastro { get; set; }

        [DisplayName("Situação")]
        public string situacao { get; set; }

        [DisplayName("Administrador")]
        public string isAdmin { get; set; }

        [DisplayName("Senha")]
        public string senha { get; set; }

        [DisplayName("Keyword")]
        public string keyword { get; set; }

        [DisplayName("Dt_Keyword")]
        public Nullable<DateTime> dt_keyword { get; set; }

        [DisplayName("Empresa")]
        public virtual Empresa empresa { get; set;}

    }
}
