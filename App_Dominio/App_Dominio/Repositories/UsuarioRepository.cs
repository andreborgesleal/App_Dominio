using App_Dominio.Component;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Repositories
{
    public class UsuarioRepository : Repository
    {
        [DisplayName("ID")]
        public int usuarioId { get; set; }

        [DisplayName("Empresa ID")]
        public int empresaId { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Informe um e-mail válido")]
        [EmailAddress]
        [Display(Name = "Login")]
        public string login { get; set; }

        [DisplayName("Nome")]
        [Required]
        [StringLength(40, ErrorMessage="O nome do usuário deve ter no máximo 40 caracteres" )]
        public string nome { get; set; }

        public DateTime dt_cadastro { get; set; }

        [Required]
        [DisplayName("Situação")]
        public string situacao { get; set; }

        [Required]
        [DisplayName("Administrador (S/N")]
        public string isAdmin { get; set; }
        
        [DataType(DataType.Password)]
        public string senha { get; set; }

        public string keyword { get; set; }

        public Nullable<DateTime> dt_keyword { get; set; }
    }
}
