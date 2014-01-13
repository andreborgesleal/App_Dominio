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
        [DisplayName("ID")]
        [Required(ErrorMessage = "ID da sessão deve ser informado")]
        public string sessaoId { get; set; }

        [DisplayName("ID_Sistema")]
        public int sistemaId { get; set; }

        [DisplayName("ID_Usuário")]
        public int usuarioId { get; set; }

        [DisplayName("ID_Empresa")]
        public int empresaId { get; set; }

        [DisplayName("Login")]
        public string login { get; set; }

        [Required(ErrorMessage = "Data de ativação da sessão deve ser informada")]
        [DisplayName("Dt_Ativação")]
        public DateTime dt_criacao { get; set; }

        [Required(ErrorMessage = "Data da atualização da sessão deve ser informada")]
        [DisplayName("Dt_Atualização")]
        public DateTime dt_atualizacao { get; set; }

        [DisplayName("Dt_Desativação")]
        public DateTime? dt_desativacao { get; set; }

        [DisplayName("Online")]
        public string isOnline { get; set; }

        [DisplayName("IP")]
        public string ip { get; set; }

        [DisplayName("Valor_1")]
        public string value1 { get; set; }
        [DisplayName("Valor_2")]
        public string value2 { get; set; }
        [DisplayName("Valor_3")]
        public string value3 { get; set; }
        [DisplayName("Valor_4")]
        public string value4 { get; set; }

    }

}