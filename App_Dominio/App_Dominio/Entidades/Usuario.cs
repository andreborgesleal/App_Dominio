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
        [DisplayName("ID Usuário")]
        public int usuarioId { get; set; }

        public int empresaId { get; set; }

        public string login { get; set; }

        public string nome { get; set; }

        public DateTime dt_cadastro { get; set; }

        public string situacao { get; set; }

        public string isAdmin { get; set; }

        public string senha { get; set; }

        public string keyword { get; set; }

        public Nullable<DateTime> dt_keyword { get; set; }

        public virtual Empresa empresa { get; set;}

    }
}
