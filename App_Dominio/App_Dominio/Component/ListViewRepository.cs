using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Negocio;
using App_Dominio.Repositories;
using App_Dominio.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App_Dominio.Component
{
    public abstract class ListViewRepository<R> : Context, IListRepository where R : Repository
    {
        private IEnumerable<FiltroRepository> Filtros;

        public IEnumerable<FiltroRepository> getFiltros() { return Filtros; }

        #region Métodos da Interface IListRepository
        public IPagedList getPagedList(int? index, int pageSize = 50, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;

                IEnumerable<R> list = (IEnumerable<R>)ListRepository(index, pageSize, param);

                return new PagedList<R>(list.ToList(), pageIndex, pageSize, list.Count() > 0 ? list.First().TotalCount : 0, action(), null, DivId());
            }
            catch (Exception ex)
            {
                throw new Exception(App_DominioException.Exception(ex, GetType().FullName, App_DominioException.ErrorType.PaginationError));
            }
        }

        public IPagedList getPagedList(int? index, string report, string controllerName, string actionName, int pageSize = 50, params object[] param)
        {
            try
            {
                int pageIndex = index ?? 0;
                Filtros = getFiltros(report, controllerName, actionName, pageSize);
                IEnumerable<R> list = (IEnumerable<R>)ListRepository(index, pageSize, param);
                return new PagedList<R>(list.ToList(), pageIndex, pageSize, list.Count() > 0 ? list.First().TotalCount : 0, action(), Filtros, DivId());
            }
            catch (Exception ex)
            {
                throw new Exception(App_DominioException.Exception(ex, GetType().FullName, App_DominioException.ErrorType.PaginationError));
            }
        }

        public virtual IEnumerable<Repository> ListRepository(int? index, int pageSize = 50, params object[] param)
        {
            using (default_db = this.CreateContext())
            {
                return Bind(index, pageSize, param);
            }
        }

        public IEnumerable<FiltroRepository> getFiltros(string report, string controllerName, string actionName, int pageSize = 50)
        {
            ListViewFiltro listViewFiltro = new ListViewFiltro();
            Filtros = (IEnumerable<FiltroRepository>)listViewFiltro.ListRepository(0, 50, report, controllerName, actionName);
            return Filtros;
        }

        public virtual string action()
        {
            return "List";
        }

        public virtual string DivId()
        {
            return "div-list";
        }

        public abstract IEnumerable<R> Bind(int? index, int pageSize = 50, params object[] param);

        public abstract Repository getRepository(Object id);
        #endregion
    }

    public abstract class ReportRepository<R> : ListViewRepository<R>, IListReportRepository<R> where R : Repository
    {
        public string totalizaColuna1 { get; set; }
        public string totalizaColuna2 { get; set; }

        public abstract IEnumerable<R> BindReport(params object[] param);

        public override IEnumerable<Repository> ListRepository(int? index, int pageSize = 50, params object[] param)
        {
            using (default_db = this.CreateContext())
            {
                IEnumerable<R> r = Bind(index, pageSize, param);
                if (r.Count() > 0)
                    return LineBreak(r);
                else
                    return r;
            }
        }

        public IEnumerable<R> ListReportRepository(params object[] param)
        {
            using (default_db = this.CreateContext())
            {
                IEnumerable<R> r = BindReport(param);
                return r;
            }
        }

        public IEnumerable<R> LineBreak(IEnumerable<R> repository)
        {
            int idx = 0;
            object value1 = "@";
            object value2 = "@";
            bool flag = true;

            IList<R> repo = new List<R>();
            repo = repository.ToList();

            foreach (IReportRepository<R> r in repository.ToList())
            {
                #region group
                if (value1.Equals(r.getValueColumn1()))
                    ((IEnumerable<IReportRepository<R>>)repo).ElementAt(idx).ClearColumn1();
                else if (!value1.Equals("@"))
                {
                    #region totaliza sub grupo
                    if (totalizaColuna2 == "S")
                    {
                        R subGroupKey = r.getKey(value1, value2);
                        R subGroup = r.Create(subGroupKey, repository);
                        repo.Insert(idx++, subGroup);
                    }

                    value2 = r.getValueColumn2();
                    #endregion

                    #region totaliza grupo
                    if (totalizaColuna1 == "S")
                    {
                        R groupKey = r.getKey(value1);
                        R group = r.Create(groupKey, repository);
                        repo.Insert(idx++, group);
                    }

                    value1 = r.getValueColumn1();
                    #endregion
                    flag = true;
                }
                else
                {
                    value1 = r.getValueColumn1();
                    value2 = r.getValueColumn2();
                }
                #endregion

                #region sub-group
                if (!flag)
                    if (value2.Equals(r.getValueColumn2()))
                        ((IEnumerable<IReportRepository<R>>)repo).ElementAt(idx).ClearColumn2();
                    else
                    {
                        if (totalizaColuna2 == "S")
                        {
                            R key = r.getKey(value1, value2);
                            R item = r.Create(key, repository);
                            repo.Insert(idx++, item);
                        }

                        value2 = r.getValueColumn2();
                    }

                flag = false;
                #endregion

                idx++;
            }

            #region totaliza sub grupo
            if (totalizaColuna2 == "S")
            {
                R geralSubGroupKey = ((IReportRepository<R>)repo.Last()).getKey(value1, value2);
                R geralSubGroup = ((IReportRepository<R>)repo.Last()).Create(geralSubGroupKey, repository);
                repo.Insert(idx++, geralSubGroup);
            }
            #endregion

            #region totaliza grupo
            if (totalizaColuna1 == "S")
            {
                R geralGroupKey = ((IReportRepository<R>)repo.Last()).getKey(value1);
                R geralGroup = ((IReportRepository<R>)repo.Last()).Create(geralGroupKey, repository);
                repo.Insert(idx++, geralGroup);
            }
            #endregion

            #region total geral
            R geralKey = ((IReportRepository<R>)repo.Last()).getKey();
            R geral = ((IReportRepository<R>)repo.Last()).Create(geralKey, repository);
            repo.Insert(idx++, geral);
            #endregion

            return repo;
        }

    }

}