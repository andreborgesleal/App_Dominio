using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Filtro")]
    public class Filtro
    {
        [Key, Column(Order = 0)]
        [Required(ErrorMessage = "Por favor, informe o nome do relatório")]
        [StringLength(60, ErrorMessage = "Nome do relatório deve ter no máximo 150 caracteres")]
        public string report { get; set; }

        [Key, Column(Order = 1)]
        [Required(ErrorMessage = "Por favor, informe o Controller Name")]
        [StringLength(60, ErrorMessage = "Controller deve ter no máximo 60 caracteres")]
        public string controller { get; set; }

        [Key, Column(Order = 2)]
        [Required(ErrorMessage = "Por favor, informe a Action")]
        [StringLength(30, ErrorMessage = "Action deve ter no máximo 30 caracteres")]
        public string action { get; set; }

        [Key, Column(Order = 3)]
        [Required(ErrorMessage = "Por favor, informe o Nome do Atributo")]
        [StringLength(30, ErrorMessage = "Atributo deve ter no máximo 30 caracteres")]
        public string atributo { get; set; }

        [Key, Column(Order = 4)]
        [Required(ErrorMessage = "Por favor, informe o ID da Empresa")]
        public int empresaId { get; set; }

        [StringLength(60, ErrorMessage = "Valor do Atributo deve ter no máximo 60 caracteres")]
        public string valor { get; set; }
       
        public virtual Empresa Empresa { get; set; }
    }
}