using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Sessao")]
    public class Sessao
    {
        [Key]
        [DisplayName("ID Sessão")]
        [Required(ErrorMessage = "ID da sessão deve ser informado")]
        public string sessaoId { get; set; }

        public int sistemaId { get; set; }

        public int usuarioId { get; set; }

        public int empresaId { get; set; }

        [DisplayName("Dt.Ativação")]
        [Required(ErrorMessage = "Data de ativação da sessão deve ser informada")]
        public DateTime dt_ativacao { get; set; }

        [DisplayName("Dt.Atualização")]
        [Required(ErrorMessage = "Data da atualização da sessão deve ser informada")]
        public DateTime dt_atualizacao { get; set; }

        [DisplayName("Dt.Desativação")]
        public DateTime? dt_desativacao { get; set; }

        public string isOnline { get; set; }

        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }
        public string value4 { get; set; }

    }

}