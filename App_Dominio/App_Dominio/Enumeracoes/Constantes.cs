using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App_Dominio.App_Start;

namespace App_Dominio.Enumeracoes
{
    public enum Crud
    {
        INCLUIR = 1,
        ALTERAR = 2,
        EXCLUIR = 3,
        CONSULTAR = 4
    }

    public enum MsgType
    {
        SUCCESS = 0,
        WARNING = 1,
        ERROR = 2
    }


    public class Enumeradores
    {
        /// <summary>
        /// Situação Padrão
        /// </summary>
        public enum Situacao
        {
            [StringDescription("Ativado")]
            [StringValue("A")]
            ATIVADO,
            [StringDescription("Desativado")]
            [StringValue("D")]
            DESATIVADO
        }

        /// <summary>
        /// Natureza da Operação
        /// </summary>
        public enum NaturezaOperacao
        {
            [StringDescription("Crédito")]
            [StringValue("C")]
            CREDITO,
            [StringDescription("Débito")]
            [StringValue("D")]
            DEBITO
        }

        /// <summary>
        /// Sim ou Não
        /// </summary>
        public enum SimNao
        {
            [StringDescription("Sim")]
            [StringValue("S")]
            SIM,
            [StringDescription("Não")]
            [StringValue("N")]
            NAO
        }

        /// <summary>
        /// Aberto ou Fechado
        /// </summary>
        public enum AbertoFechado
        {
            [StringDescription("Aberto")]
            [StringValue("A")]
            ABERTO,
            [StringDescription("Fechado")]
            [StringValue("F")]
            FECHADO
        }

        /// <summary>
        /// Unidades Federativas
        /// </summary>
        public enum UnidadeFederativa
        {
            [StringDescription("Acre")]
            [StringValue("AC")]
            AC,
            [StringDescription("Alagoas")]
            [StringValue("AL")]
            AL,
            [StringDescription("Amapá")]
            [StringValue("AP")]
            AP,
            [StringDescription("Amazonas")]
            [StringValue("AM")]
            AM,
            [StringDescription("Bahia")]
            [StringValue("BA")]
            BA,
            [StringDescription("Ceará")]
            [StringValue("CE")]
            CE,
            [StringDescription("Distrito Federal")]
            [StringValue("DF")]
            DF,
            [StringDescription("Espírito Santo")]
            [StringValue("ES")]
            ES,
            [StringDescription("Goiás")]
            [StringValue("GO")]
            GO,
            [StringDescription("Maranhão")]
            [StringValue("MA")]
            MA,
            [StringDescription("Mato Grosso")]
            [StringValue("MT")]
            MT,
            [StringDescription("Mato Grosso do Sul")]
            [StringValue("MS")]
            MS,
            [StringDescription("Minas Gerais")]
            [StringValue("MG")]
            MG,
            [StringDescription("Pará")]
            [StringValue("PA")]
            PA,
            [StringDescription("Paraíba")]
            [StringValue("PB")]
            PB,
            [StringDescription("Paraná")]
            [StringValue("PR")]
            PR,
            [StringDescription("Pernambuco")]
            [StringValue("PE")]
            PE,
            [StringDescription("Piauí")]
            [StringValue("PI")]
            PI,
            [StringDescription("Rio de Janeiro")]
            [StringValue("RJ")]
            RJ,
            [StringDescription("Rio Grande do Norte")]
            [StringValue("RN")]
            RN,
            [StringDescription("Rio Grande do Sul")]
            [StringValue("RS")]
            RS,
            [StringDescription("Rondônia")]
            [StringValue("RO")]
            RO,
            [StringDescription("Roraima")]
            [StringValue("RR")]
            RR,
            [StringDescription("Santa Catarina")]
            [StringValue("SC")]
            SC,
            [StringDescription("São Paulo")]
            [StringValue("SP")]
            SP,
            [StringDescription("Sergipe")]
            [StringValue("SE")]
            SE,
            [StringDescription("Tocantins")]
            [StringValue("TO")]
            TO
        }

        /// <summary>
        /// Tipo de Pessoa
        /// </summary>
        public enum TipoPessoa
        {
            [StringDescription("Pessoa Física")]
            [StringValue("PF")]
            FISICA,
            [StringDescription("Pessoa Jurídica")]
            [StringValue("PJ")]
            JURIDICA
        }

        /// <summary>
        /// Sexo
        /// </summary>
        public enum Sexo
        {
            [StringDescription("Masculino")]
            [StringValue("M")]
            MASCULINO,
            [StringDescription("Feminino")]
            [StringValue("F")]
            FEMININO
        }

        /// <summary>
        /// Estado Civil
        /// </summary>
        public enum EstadoCivil
        {
            [StringDescription("Solteiro")]
            [StringValue("S")]
            SOLTEIRO,
            [StringDescription("Casado")]
            [StringValue("C")]
            CASADO,
            [StringDescription("Divorciado")]
            [StringValue("D")]
            DIVORCIADO,
            [StringDescription("Viúvo")]
            [StringValue("V")]
            VIUVO,
            [StringDescription("União Estável")]
            [StringValue("U")]
            UNIAO_ESTAVEL,
            [StringDescription("Outros")]
            [StringValue("O")]
            OUTROS
        }


        /// <summary>
        /// Tipo de Telefone
        /// </summary>
        public enum TipoTelefone
        {
            [StringDescription("Telefone Comercial")]
            COMERCIAL = 1,
            [StringDescription("Telefone Celular")]
            CELULAR = 2,
            [StringDescription("Fax")]
            FAX = 3,
            [StringDescription("Telefone Residencial")]
            RESIDENCIAL = 4
        }

        /// <summary>
        /// Tipo de Conta Contábil
        /// </summary>
        public enum TipoContaContabil
        {
            [StringDescription("Sintética")]
            [StringValue("S")]
            SINTETICA,
            [StringDescription("Analítica")]
            [StringValue("A")]
            ANALITICA
        }

        /// <summary>
        /// Classificação Conta Bancária
        /// </summary>
        public enum ClassificacaoContaBancaria
        {
            [StringDescription("Conta Corrente")]
            [StringValue("C/C")]
            CONTA_CORRENTE,
            [StringDescription("Conta de Investimento")]
            [StringValue("INV")]
            CONTA_INVESTIMENTO,
            [StringDescription("Cartão de Crédito")]
            [StringValue("CAR")]
            CARTAO_CREDITO
        }

        /// <summary>
        /// Classificação Conta Bancária
        /// </summary>
        public enum Periodo
        {
            [StringDescription("Mês atual")]
            [StringValue("Mês Atual")]
            MES_ATUAL,
            [StringDescription("Mês anterior")]
            [StringValue("Mês Anterior")]
            MES_ANTERIOR,
            [StringDescription("Próximo mês")]
            [StringValue("Próximo mês")]
            PROXIMO_MES,
            [StringDescription("Últimos 3 meses")]
            [StringValue("Últimos 3 meses")]
            ULTIMOS_3MESES,
            [StringDescription("Ano atual")]
            [StringValue("Ano atual")]
            ANO_ATUAL,
            [StringDescription("Personalizado")]
            [StringValue("Personalizado")]
            PERSONALIZADO
        }

        public enum MiniCrudOperacao
        {
            [StringDescription("Add")]
            [StringValue("Add")]
            ADD,
            [StringDescription("Del")]
            [StringValue("Del")]
            DEL,
            [StringDescription("Clear")]
            [StringValue("Clear")]
            CLEAR
        }
    }



}