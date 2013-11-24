using App_Dominio.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc
{
    public interface IPagedList
    {
        int TotalCount
        {
            get;
            set;
        }

        int PageIndex
        {
            get;
            set;
        }

        int PageSize
        {
            get;
            set;
        }

        int LastPage 
        { 
            get; 
        }

        bool IsPreviousPage
        {
            get;
        }

        bool IsNextPage
        {
            get;
        }

        string action { get; set; }

        string DivId { get; set; }

        IEnumerable<FiltroRepository> Filtros { get; set; }
    }

    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.Skip(PageIndex * pageSize).Take(pageSize).ToList());
            this.action = "Browse";
            this.DivId = "div-list";
        }

        public PagedList(List<T> source, int index, int pageSize)
        {
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.Skip(PageIndex * pageSize).Take(pageSize).ToList());
            this.action = "Browse";
            this.DivId = "div-list";
        }

        public PagedList(List<T> source, int index, int pageSize, string action, IEnumerable<FiltroRepository> Filtros, string DivId = "div-list")
        {
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.Skip(PageIndex * pageSize).Take(pageSize).ToList());
            this.action = action;
            this.Filtros = Filtros;
            this.DivId = DivId;
        }

        public PagedList(List<T> source, int index, int pageSize, int totalCount, string action, IEnumerable<FiltroRepository> Filtros, string DivId = "div-list")
        {
            this.TotalCount = totalCount;
            this.PageSize = pageSize;
            this.PageIndex = index > LastPage ? LastPage : index;
            this.AddRange(source.ToList());
            this.action = action;
            this.Filtros = Filtros;
            this.DivId = DivId;
        }

        public string action { get; set; }

        public int TotalCount
        {
            get;
            set;
        }

        public int LastPage
        {
            get {
                    int resto = TotalCount % PageSize;
                    int quociente = (TotalCount / PageSize) - 1; 
                    return quociente + (resto > 0 ? 1 : 0);
                }
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public bool IsPreviousPage
        {
            get
            {
                return ((PageIndex - 1) >= 0);
            }
        }

        public bool IsNextPage
        {
            get
            {
                return ((PageIndex + 1) * PageSize) <= TotalCount;
            }
        }

        public string DivId { get; set; }

        public IEnumerable<FiltroRepository> Filtros { get; set; }
    }

    public static class Pagination
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize)
        {
            return new PagedList<T>(source, index, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index)
        {
            return new PagedList<T>(source, index, 50);
        }
    }
}