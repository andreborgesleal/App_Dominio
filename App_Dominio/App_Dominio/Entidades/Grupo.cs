using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Grupo")]
    public class Grupo
    {
        [Key]
        [DisplayName("ID Grupo")]
        public int grupoId { get; set; }
        public int sistemaId { get; set; }
        public int empresaId { get; set; }
        public string descricao { get; set; }
        public string situacao { get; set; }
        public virtual Sistema Sistema { get; set; }
        public virtual Empresa Empresa{ get; set; }
    }
}
