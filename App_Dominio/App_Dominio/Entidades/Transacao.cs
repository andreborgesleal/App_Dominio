using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Transacao")]
    public class Transacao
    {
        [Key]
        [DisplayName("ID Transação")]
        public int transacaoId { get; set; }
        public int sistemaId { get; set; }
        public int transacaoId_pai { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public string referencia { get; set; }
        public string exibir { get; set; }
        public int posicao { get; set; }
        public string url { get; set; }
        public virtual Sistema sistema { get; set; }
        public virtual Transacao transacao { get; set; }
    }
}
