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
        public decimal alertaId { get; set; }

        public int usuarioId { get; set; }

        public DateTime dt_emissao { get; set; }

        public DateTime dt_leitura { get; set; }

        public string mensagem { get; set; }

        public virtual Usuario usuario { get; set; }

    }
}
