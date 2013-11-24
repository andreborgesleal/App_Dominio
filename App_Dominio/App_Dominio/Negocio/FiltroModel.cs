using App_Dominio.Contratos;
using App_Dominio.Component;
using App_Dominio.Entidades;
using App_Dominio.Enumeracoes;
using App_Dominio.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App_Dominio.Negocio
{
    public class FiltroModel : CrudContext<Filtro, FiltroRepository>
    {
        #region Métodos da classe CrudContext
        public override Filtro MapToEntity(FiltroRepository value)
        {
            return new Filtro()
            {
                report = value.report,
                controller = value.controller,
                action = value.action,
                atributo = value.atributo,
                empresaId = value.empresaId,
                valor = value.valor
            };
        }

        public override FiltroRepository MapToRepository(Filtro entity)
        {
            return new FiltroRepository()
            {
                report = entity.report,
                controller = entity.controller,
                action = entity.action,
                atributo = entity.atributo,
                empresaId = entity.empresaId,
                valor = entity.valor,
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };

        }

        public override Filtro Find(FiltroRepository key)
        {
            return db.Filtros.Find(key.report, key.controller, key.action, key.atributo, key.empresaId);
        }

        public override Validate Validate(FiltroRepository value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString() };

            if (value.empresaId == 0)
            {
                value.mensagem.Code = 35;
                value.mensagem.Message = MensagemPadrao.Message(35).ToString();
                value.mensagem.MessageBase = "Sua sessão expirou. Faça um novo login no sistema";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            if (value.report.Trim().Length == 0 || value.controller.Trim().Length == 0 || value.action.Trim().Length == 0 || value.atributo.Trim().Length == 0)
            {
                value.mensagem.Code = 999;
                value.mensagem.Message = MensagemPadrao.Message(999).ToString();
                value.mensagem.MessageBase = "Não foi possível salvar os atributos de busca.";
                value.mensagem.MessageType = MsgType.WARNING;
                return value.mensagem;
            }

            return value.mensagem;
        }
        #endregion

        #region Métodos customizados
        public string getReport(string report, string controller, string action)
        {
            using (db = base.Create())
            {
                Filtro f = db.Filtros.Where(info => info.report == report && info.controller == controller && info.action == action && info.empresaId == sessaoCorrente.empresaId).FirstOrDefault();
                if (f == null)
                    report = "_default";
            }
            return report;
        }

        #endregion
    }

    public class ListViewFiltro : ListViewRepository<FiltroRepository>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<FiltroRepository> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _report = param[0].ToString();
            string _controller = param[1].ToString() ;
            string _action = param[2].ToString();

            return (from f in db.Filtros
                    where f.report == _report
                          && f.controller == _controller
                          && f.action == _action
                          && f.empresaId.Equals(sessaoCorrente.empresaId)                           
                    orderby f.atributo
                    select new FiltroRepository
                    {
                        report = f.report,
                        controller = f.controller,
                        action = f.action,
                        atributo = f.atributo,
                        empresaId = sessaoCorrente.empresaId,
                        valor = f.valor,
                        PageSize = pageSize,
                        TotalCount = (from f1 in db.Filtros
                                      where f1.report == _report
                                            && f1.controller == _controller
                                            && f1.action == _action
                                            && f1.empresaId.Equals(sessaoCorrente.empresaId)                           
                                      select f1).Count()
                    }).Skip((index ?? 0) * pageSize).Take(pageSize).ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new FiltroModel().getObject((FiltroRepository)id);
        }
        #endregion
    }

    public class ListViewFiltroByDescricao : ListViewRepository<FiltroRepository>
    {
        #region Métodos da classe ListViewRepository
        public override IEnumerable<FiltroRepository> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _report = null;
            string _controller = null; 
            //string _action = param[2].ToString();            

            _report = param[0] != null && param[0].ToString() != "" ? param[0].ToString() : _report;
            _controller = param[1] != null && param[1].ToString() != "" ? param[1].ToString() : _controller;

            return (from f in db.Filtros
                    where f.report.StartsWith(_report)
                          && f.controller == _controller
                          //&& f.action == _action
                          && f.empresaId.Equals(sessaoCorrente.empresaId)
                    orderby f.report
                    select new FiltroRepository
                    {
                        report = f.report,
                        controller = f.controller,
                        action = f.action,
                        empresaId = sessaoCorrente.empresaId,
                        PageSize = pageSize,
                        TotalCount = 1
                    }).Distinct().ToList();
        }

        public override Repository getRepository(Object id)
        {
            return new FiltroModel().getObject((FiltroRepository)id);
        }
        #endregion
    }

    public class LookupFiltroModel : ListViewFiltro
    {
        public override IEnumerable<FiltroRepository> Bind(int? index, int pageSize = 50, params object[] param)
        {
            string _report = null;
            string _controller = param[1].ToString();
            string _action = param[2].ToString();            
            
            
            _report = param[0] != null && param[0].ToString() != "" ? param[0].ToString() : _report;

            return (from f in db.Filtros
                    where (_report == null || f.report == _report)
                          && f.controller == _controller
                          && f.action == _action
                          && f.empresaId.Equals(sessaoCorrente.empresaId)
                    orderby f.report
                    select new FiltroRepository
                    {
                        report = f.report,
                        controller = f.controller,
                        action = f.action,
                        empresaId = sessaoCorrente.empresaId,
                        PageSize = pageSize,
                        TotalCount = 1
                    }).Distinct().ToList();
        }

        public override string action()
        {
            return "../Filtro/ListFiltroModal"; // completo (inclusive com os filtros)
        }
    }

    public class LookupFiltroFiltroModel : ListViewFiltro
    {
        public override string action()
        {
            return "../Filtro/_ListFiltroModal";
        }
    }

}