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
        [DisplayName("ID Alerta")]
        public int alertaId { get; set; }

        public int usuarioId { get; set; }

        public int sistemaId { get; set; }

        public DateTime dt_emissao { get; set; }

        public Nullable<DateTime> dt_leitura { get; set; }

        public string linkText { get; set; }

        public string url { get; set; }

        public string mensagem { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual Sistema Sistema { get; set; }

    }
}
