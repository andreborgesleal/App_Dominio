using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Alerta")]
    public class Alerta
    {
        [Key]
        [DisplayName("ID")]
        public int alertaId { get; set; }

        [DisplayName("ID_Usuário")]
        public int usuarioId { get; set; }

        [DisplayName("ID_Sistema")]
        public int sistemaId { get; set; }

        [DisplayName("Dt_Emissão")]
        public DateTime dt_emissao { get; set; }

        [DisplayName("Dt_Leitura")]
        public Nullable<DateTime> dt_leitura { get; set; }

        [DisplayName("LinkText")]
        public string linkText { get; set; }

        [DisplayName("xml")]
        public string url { get; set; }

        [DisplayName("Mensagem")]
        public string mensagem { get; set; }

        [DisplayName("Usuário")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Sistema")]
        public virtual Sistema Sistema { get; set; }

    }
}
