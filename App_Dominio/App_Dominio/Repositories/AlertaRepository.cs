using App_Dominio.Component;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Repositories
{
    public class AlertaRepository : Repository
    {
        [DisplayName("ID")]
        public int alertaId { get; set; }

        [DisplayName("Usuário")]
        [Required]
        public int usuarioId { get; set; }

        [DisplayName("Emissão")]
        [Required]
        public DateTime dt_emissao { get; set; }

        [DisplayName("Dt Leitura")]
        public Nullable<DateTime> dt_leitura { get; set; }

        [DisplayName("Mensagem")]
        [Required]
        public string mensagemAlerta { get; set; }
    }
}
