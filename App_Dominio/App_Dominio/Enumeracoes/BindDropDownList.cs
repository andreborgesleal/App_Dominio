using App_Dominio.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App_Dominio.App_Start;
using App_Dominio.Models;

namespace App_Dominio.Enumeracoes
{
    public static class DropDownListEnum
    {
        #region DropDownList Situação (Ativo ou Desativado)
        /// <summary>
        /// Retorna a Situação (Ativado ou Desativado)
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> Situacao(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.Situacao.ATIVADO.GetStringValue(), Text = Enumeradores.Situacao.ATIVADO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Situacao.DESATIVADO.GetStringValue(), Text = Enumeradores.Situacao.DESATIVADO.GetStringDescription() } 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList TipoConta (Sintética ou Analítica)
        /// <summary>
        /// Retorna o tipo da Conta (Sintética ou Analítica)
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> TipoConta(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.TipoContaContabil.SINTETICA.GetStringValue(), Text = Enumeradores.TipoContaContabil.SINTETICA.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.TipoContaContabil.ANALITICA.GetStringValue(), Text = Enumeradores.TipoContaContabil.ANALITICA.GetStringDescription()  } 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Exercicio 
        /// <summary>
        /// Retorna o tipo da Conta (Sintética ou Analítica)
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> Exercicio(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = "2012", Text = "2012" }, 
                new SelectListItem() { Value = "2013", Text = "2013" }, 
                new SelectListItem() { Value = "2014", Text = "2014" }, 
                new SelectListItem() { Value = "2015", Text = "2015" }, 
                new SelectListItem() { Value = "2016", Text = "2016" }, 
                new SelectListItem() { Value = "2017", Text = "2017" }, 
                new SelectListItem() { Value = "2018", Text = "2018" }, 
                new SelectListItem() { Value = "2019", Text = "2019" }, 
                new SelectListItem() { Value = "2020", Text = "2020" }, 
                new SelectListItem() { Value = "2021", Text = "2021" }, 
                new SelectListItem() { Value = "2022", Text = "2022" }, 
                new SelectListItem() { Value = "2023", Text = "2023" }, 
                new SelectListItem() { Value = "2024", Text = "2024" }, 
                new SelectListItem() { Value = "2025", Text = "2025" }, 
                new SelectListItem() { Value = "2026", Text = "2026" }, 
                new SelectListItem() { Value = "2027", Text = "2027" }, 
                new SelectListItem() { Value = "2028", Text = "2028" } 
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList ContaBancaria.Classificacao
        /// <summary>
        /// Retorna a classificação da conta bancária
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ClassificacaoContaBancaria(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.ClassificacaoContaBancaria.CONTA_CORRENTE.GetStringValue(), Text = Enumeradores.ClassificacaoContaBancaria.CONTA_CORRENTE.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.ClassificacaoContaBancaria.CONTA_INVESTIMENTO.GetStringValue(), Text = Enumeradores.ClassificacaoContaBancaria.CONTA_INVESTIMENTO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.ClassificacaoContaBancaria.CARTAO_CREDITO.GetStringValue(), Text = Enumeradores.ClassificacaoContaBancaria.CARTAO_CREDITO.GetStringDescription()  }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Exercicio.Encerrado
        /// <summary>
        /// Retorna se o Exercício contábil está encerrado ou não
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> Encerrado(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.SimNao.SIM.GetStringValue(), Text = Enumeradores.SimNao.SIM.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.SimNao.NAO.GetStringValue(), Text = Enumeradores.SimNao.NAO.GetStringDescription() }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList NaturezaOperacao
        /// <summary>
        /// Retorna a natureza da operação: Débito ou Crédito
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> NaturezaOperacao(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.NaturezaOperacao.DEBITO.GetStringValue(), Text = Enumeradores.NaturezaOperacao.DEBITO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.NaturezaOperacao.CREDITO.GetStringValue(), Text = Enumeradores.NaturezaOperacao.CREDITO.GetStringDescription() }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Periodo
        /// <summary>
        /// Retorna os períodos possível para consulta dos lançamentos
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> Periodo(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.Periodo.MES_ATUAL.GetStringValue(), Text = Enumeradores.Periodo.MES_ATUAL.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Periodo.MES_ANTERIOR.GetStringValue(), Text = Enumeradores.Periodo.MES_ANTERIOR.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Periodo.PROXIMO_MES.GetStringValue(), Text = Enumeradores.Periodo.PROXIMO_MES.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Periodo.ULTIMOS_3MESES.GetStringValue(), Text = Enumeradores.Periodo.ULTIMOS_3MESES.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Periodo.ANO_ATUAL.GetStringValue(), Text = Enumeradores.Periodo.ANO_ATUAL.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Periodo.PERSONALIZADO.GetStringValue(), Text = Enumeradores.Periodo.PERSONALIZADO.GetStringDescription() }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList SimNao
        /// <summary>
        /// Retorna os valores constantes Sim e Não para uma seleção 
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> SimNao(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.SimNao.SIM.GetStringValue(), Text = Enumeradores.SimNao.SIM.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.SimNao.NAO.GetStringValue(), Text = Enumeradores.SimNao.NAO.GetStringDescription() }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

        #region DropDownList Sexo
        /// <summary>
        /// Retorna os valores constantes M ou F para uma seleção 
        /// </summary>
        /// <param name="selectedValue">Item da lista que receberá o foco inicial</param>
        /// <param name="header">Informar o cabeçalho do dropdownlist. Exemplo: "Selecione...". Observação: Se não informado o dropdownlist não terá cabeçalho.</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> Sexo(string selectedValue = "", string header = "")
        {
            List<SelectListItem> drp = new List<SelectListItem>() { 
                new SelectListItem() { Value = Enumeradores.Sexo.MASCULINO.GetStringValue(), Text = Enumeradores.Sexo.MASCULINO.GetStringDescription() }, 
                new SelectListItem() { Value = Enumeradores.Sexo.FEMININO.GetStringValue(), Text = Enumeradores.Sexo.FEMININO.GetStringDescription() }
            };

            return Funcoes.SelectListEnum(drp, selectedValue, header);
        }
        #endregion

    }
}