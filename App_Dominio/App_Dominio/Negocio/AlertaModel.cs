using System;
using System.Collections.Generic;
using System.Linq;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Component;
using App_Dominio.Enumeracoes;
using App_Dominio.Repositories;
using App_Dominio.Security;


namespace App_Dominio.Negocio
{
    public class AlertaModel : CrudContext<Alerta, AlertaRepository, SecurityContext>
    {
        #region Métodos da classe CrudContext
        public override Alerta MapToEntity(AlertaRepository value)
        {
            return new Alerta()
            {
                alertaId = value.alertaId,
                usuarioId = value.usuarioId,
                sistemaId = value.sistemaId,
                dt_emissao = value.dt_emissao,
                dt_leitura = value.dt_leitura,
                linkText = value.linkText,
                url = value.url,
                mensagem = value.mensagemAlerta
            };
        }

        public override AlertaRepository MapToRepository(Alerta entity)
        {
            return new AlertaRepository()
            {
                alertaId = entity.alertaId,
                usuarioId = entity.usuarioId,
                sistemaId = entity.sistemaId,
                dt_emissao = entity.dt_emissao,
                dt_leitura = entity.dt_leitura,
                linkText = entity.linkText,
                url = entity.url,
                mensagemAlerta = entity.mensagem,                
                mensagem = new Validate() { Code = 0, Message = "Registro incluído com sucesso", MessageBase = "Registro incluído com sucesso", MessageType = MsgType.SUCCESS }
            };
        }

        public override Alerta Find(AlertaRepository key)
        {
            return db.Alertas.Find(key.alertaId);
        }

        public override Validate Validate(AlertaRepository value, Crud operation)
        {
            value.mensagem = new Validate() { Code = 0, Message = MensagemPadrao.Message(0).ToString(), MessageType = MsgType.SUCCESS };

            return value.mensagem;
        }

        public override AlertaRepository CreateRepository(System.Web.HttpRequestBase Request)
        {
            return new AlertaRepository();
        }
        #endregion
    }
}
