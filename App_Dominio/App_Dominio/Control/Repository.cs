using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Web;

namespace App_Dominio.Control
{
    public abstract class Repository
    {
        [DisplayName("Session ID")]
        public string sessionId { get; set; }

        [DisplayName("Empresa")]
        public int empresaId { get; set; }

        [DisplayName("Mensagem")]
        public Validate mensagem { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}