using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("EmpresaSistema")]
    public class EmpresaSistema
    {
        [Key, Column(Order = 0)]
        [DisplayName("Empresa ID")]
        public int empresaId { get; set; }

        [Key, Column(Order = 1)]
        [DisplayName("Sistema ID")]
        public int sistemaId { get; set; }

        [DisplayName("Situação")]
        public string situacao { get; set; }

    }
}
