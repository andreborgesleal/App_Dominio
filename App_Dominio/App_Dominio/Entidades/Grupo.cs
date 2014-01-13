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
        [DisplayName("ID_Grupo")]        
        public int grupoId { get; set; }
        [DisplayName("ID_Sistema")]
        public int sistemaId { get; set; }
        [DisplayName("ID_Empresa")]
        public int empresaId { get; set; }
        [DisplayName("Descrição")]
        public string descricao { get; set; }
        [DisplayName("Situação")]
        public string situacao { get; set; }
        [DisplayName("Sistema")]
        public virtual Sistema Sistema { get; set; }
        [DisplayName("Empresa")]
        public virtual Empresa Empresa{ get; set; }
    }
}
