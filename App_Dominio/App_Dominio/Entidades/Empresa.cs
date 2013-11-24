using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Empresa")]
    public class Empresa
    {
        [Key]
        [DisplayName("Empresa ID")]
        public int empresaId { get; set; }

        [DisplayName("Nome da Empresa")]
        [Required(ErrorMessage = "Por favor, informe o nome da empresa")]
        [StringLength(60, ErrorMessage = "O nome da empresa deve ter no máximo 60 caracteres")]
        public string nome { get; set; }

        [DisplayName("E-Mail")]
        [StringLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres")]
        [Required(ErrorMessage = "Por favor, informe o e-mail")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "E-mail inválido.")]
        public string email { get; set; }
    }
}