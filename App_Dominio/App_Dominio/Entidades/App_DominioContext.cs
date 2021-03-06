﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace App_Dominio.Entidades
{
    public class App_DominioContext : DbContext
    {
        static App_DominioContext()
        {
            Database.SetInitializer<App_DominioContext>(null);
        }

        public App_DominioContext()
            : base("Name=App_DominioContext")
		{
		}
        public DbSet<Filtro> Filtros { get; set; }
    }
}
