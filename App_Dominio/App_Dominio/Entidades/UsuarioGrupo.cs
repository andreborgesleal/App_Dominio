﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_Dominio.Entidades
{
    [Table("UsuarioGrupo")]
    public class UsuarioGrupo
    {
        [Key, Column(Order = 0)]
        [DisplayName("ID Usuário")]
        public decimal usuarioId { get; set; }
        [Key, Column(Order = 1)]
        [DisplayName("ID Grupo")]
        public int grupoId { get; set; }
        public string  situacao { get; set; }
        public virtual Usuario usuario { get; set; }
        public virtual Grupo grupo { get; set; }
    }
}