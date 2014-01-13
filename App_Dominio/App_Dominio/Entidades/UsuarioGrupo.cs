using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("UsuarioGrupo")]
    public class UsuarioGrupo
    {
        [Key, Column(Order = 0)]
        [DisplayName("ID_Usuário")]
        public int usuarioId { get; set; }
        [Key, Column(Order = 1)]
        [DisplayName("ID_Grupo")]
        public int grupoId { get; set; }
        [DisplayName("Situação")]
        public string situacao { get; set; }
        [DisplayName("Usuário")]
        public virtual Usuario Usuario { get; set; }
        [DisplayName("Grupo")]
        public virtual Grupo Grupo { get; set; }
    }
}
