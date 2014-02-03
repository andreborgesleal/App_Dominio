using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace App_Dominio.Entidades
{
    public sealed class SecurityContext : DbContext
    {
        static SecurityContext()
        {
            Database.SetInitializer<SecurityContext>(null);
        }

        public SecurityContext()
            : base("Name=SecurityContext")
        {
        }
        public DbSet<Sessao> Sessaos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Sistema> Sistemas { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Transacao> Transacaos { get; set; }
        public DbSet<UsuarioGrupo> UsuarioGrupos { get; set; }
        public DbSet<GrupoTransacao> GrupoTransacaos { get; set; }
        public DbSet<LogAuditoria> LogAuditorias { get; set; }
    }
}
