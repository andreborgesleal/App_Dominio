using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("Sistema")]
    public class Sistema
    {
        [Key]
        [DisplayName("ID Sistema")]
        public int sistemaId { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }

    }
}
