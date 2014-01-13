using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("GrupoTransacao")]
    public class GrupoTransacao
    {
        [Key, Column(Order = 0)]
        [DisplayName("ID_Grupo")]
        public int grupoId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("ID_Transacao")]
        public int transacaoId { get; set; }

        [DisplayName("Situação")]
        public string situacao { get; set; }

        [DisplayName("Grupo")]
        public virtual Grupo Grupo { get; set; }
        [DisplayName("Transação")]
        public virtual Transacao Transacao { get; set; }
    }
}
