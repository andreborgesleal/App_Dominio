﻿using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using System;
using System.Collections.Generic;

namespace App_Dominio.Contratos
{
    public interface ICrud
    {
        Repository getObject(Object id);
        Validate Validate(Repository value, App_Dominio.Enumeracoes.Crud operation);
        IEnumerable<Repository> ListAll();
        Repository Insert(Repository value);
        Repository Update(Repository value);
        Repository Delete(Repository value);
    }

    public interface ICrudContext<R> where R : Repository
    {
        R CreateRepository(System.Web.HttpRequestBase Request = null);
        R getObject(R id);
        Validate Validate(R value, App_Dominio.Enumeracoes.Crud operation);
        IEnumerable<R> ListAll();
        R Insert(R value);
        R Update(R value);
        R Delete(R value);
    }

    public interface ICrudItemContext<R> : ICrudContext<R> where R : Repository
    {
        void SetListItem(IList<R> list);
        R SetKey(R r);
    }

    public interface IProcessContext<R> : ICrudContext<R> where R : Repository
    {
        R SaveAll(R value, Crud operation);
    }

    public interface IExecContext<R> where R : Repository
    {
        R Run(R value, Crud operation);
        Validate AfterRun(R value, Crud operation);
        R CreateRepository(System.Web.HttpRequestBase Request = null);
        Validate Validate(R value, App_Dominio.Enumeracoes.Crud operation);
    }


}

