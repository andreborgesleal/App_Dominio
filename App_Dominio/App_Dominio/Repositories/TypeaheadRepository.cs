using App_Dominio.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Dominio.Repositories
{
    public class TypeaheadRepository : Repository
    {
        public string label { get; set; }
        /// <summary>
        /// Hidden Field. Ex: historicoId
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Nome do campo da entidade (tabela) que será atribuído para o do parâmetro "id" após a busca ser encontrada. Se nao informado o sistema entenderá que o atributo da tabela a ser atribuído tem o mesmo nome do parâmetro "id"
        /// </summary>
        public string fieldId { get; set; }
        
        /// <summary>
        /// Campo que recebe o texto que será digitado ou pesquisado. Ex: descricao_historico
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Nome do formulário modal que será chamado quando pressionadr o botão de busca. Ex: LovCentroCustoModal
        /// </summary>
        public string lovModal { get; set; }

        /// <summary>
        /// Função Javascript que será chamada quando clicar no botão de pesquisa (lupa) para a exibição do formulário modal. Deve ser passada a função com seus parâmetros. Se não informada vai ser chamada a função padrão 'showLookup'
        /// </summary>
        public string javaScriptFunction { get; set; }

        /// <summary>
        /// Nome do método que irá incluir o item digitado, caso o botão "+" seja acionado. Ex: CrudHistoricoModal
        /// </summary>
        public string crudModal { get; set; }
        public string controller { get; set; }
        /// <summary>
        /// Nome do atributo do objeto repository que será recuperado para mostrar na listagem do typeahead
        /// </summary>
        public string descricao { get; set; }
        /// <summary>
        /// Código e descrição que serão preenchidos como valores iniciais para o ID e para o Text. Ex: new SelectListItem() { Value = Model.EnquadramentoItem.centroCustoId.ToString(), Text = Model.EnquadramentoItem.descricao_centroCusto }
        /// </summary>
        public System.Web.Mvc.SelectListItem values { get; set; }
        /// <summary>
        /// Informa se os botões de busca e limpar ficarão desabilitados. Normalmente usados na exclusão
        /// </summary>
        public bool disableButtons { get; set; }

        /// <summary>
        /// Lista de items selecionados no mini-crud
        /// </summary>
        public IEnumerable<System.Web.Mvc.SelectListItem> MiniCrud { get; set; }
        
        /// <summary>
        /// Action que será chamada pelo botão Add para adicionar o item selecionado (textbox) ao minicrud (repository)
        /// </summary>
        public string MiniCrudAdd { get; set; }

        /// <summary>
        /// Action que será chamada pelo botão Del para excluir o item selecionado (textbox) do minicrud (repository)
        /// </summary>
        public string MiniCrudDel { get; set; }

        /// <summary>
        /// Action que será chamada pelo botão Clear para excluir trodos os items do mini crud
        /// </summary>
        public string MiniCrudClearAll { get; set; }

        public string nextField { get; set; }

        /// <summary>
        /// Determina se o campo texto está disponível para editar um valor que não esteja na lista de itens encontrados
        /// </summary>
        public bool readOnly { get; set; }
    }
}