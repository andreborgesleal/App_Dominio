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
        [DisplayName("ID Grupo")]
        public int grupoId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("ID Transacao")]
        public int transacaoId { get; set; }
        
        public string situacao { get; set; }
    }
}
