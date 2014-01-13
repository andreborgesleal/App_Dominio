using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("LogAuditoria")]
    public class LogAuditoria
    {
        [Key]
        [DisplayName("ID Log")]
        public int logId { get; set; }
        public int transacaoId { get; set; }
        public int empresaId { get; set; }
        public int usuarioId { get; set; }
        public DateTime dt_log { get; set; }
        public string ip { get; set; }
        public string notacao { get; set; }
    }
}
