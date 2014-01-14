using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace App_Dominio.Models
{
    public class Funcoes
    {
        #region Funções de Formatação
        /// <summary>
        /// Recebe uma string de CEP e retorna no formato 99999-999
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormataCep(string value)
        {
            string cep = value.Replace("-", "");
            return cep.Substring(0, 5) + "-" + cep.Replace("-", "").Substring(5, 3);
        }

        public static string tipoPessoaExtenso(string tipo)
        {
            if (tipo == "PF")
            { return "Pessoa Física"; }
            else { return "Pessoa Jurídica"; }
        }

        /// <summary>
        /// Recebe uma string moeda no formato 2.000,00 e devolve 2000.00
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        public static decimal MoneyToDecimal(string value)
        {
            string remov = value.Replace(".", "");
            decimal dec = Convert.ToDecimal(remov);
            return dec;
        }

        public static decimal DecimalToMoney(string value)
        {

            string remov = value.Replace(".", "");
            decimal dec = Convert.ToDecimal(remov);
            return dec;
        }

        public static string formataData(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy") : String.Empty;
        }

        public static string formataData_notnull(DateTime value)
        {
            return value.ToString("dd/MM/yyyy");
        }

        public static string FormataTelefone(string value)
        {
            string fone = "";
            if (!string.IsNullOrEmpty(value))
            {
                fone = value.Replace("(", "").Replace(")", "").Replace("-", "").Replace("/", "");
                if (fone.Trim().Length.Equals(8))
                    fone = fone.Substring(0, 4) + "-" + fone.Substring(4, 4);
                else if (fone.Trim().Length.Equals(10))
                    fone = "(" + fone.Substring(0, 2) + ")" + fone.Substring(2, 4) + "-" + fone.Substring(6, 4);
                else if (fone.Trim().Length.Equals(11))
                    fone = "(" + fone.Substring(0, 2) + ")" + fone.Substring(2, 5) + "-" + fone.Substring(7, 4);
            }

            return fone;
        }

        public static string RemoveCaracterEspecial(string value)
        {
            value = value ?? "";
            value = value.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
            return value;
        }

        public static string FormataCPFCNPJ(string value)
        {
            if (value != null)
            {

                if (value.Length == 11)
                {
                    string value2 = FormataCPF(value);
                    return value2;
                }
                else if (value.Length == 14)
                {
                    string value2 = FormataCNPJ(value);
                    return value2;
                }

                else return value;
            }

            else return value;
        }

        public static string FormataCPF(string value)
        {
            if (value != null)
            {

                if (value.Length == 11)
                {
                    string value2 = value.Insert(3, ".").Insert(7, ".").Insert(11, "-");
                    return value2;
                }
                else return value;
            }

            else return value;

        }

        public static string FormataCEP(string value)
        {
            if (value.Length == 8)
            {
                string value2 = value.Insert(5, "-");
                return value2;
            }
            else return value;

        }

        public static string FormataPercentual(string value)
        {
            string value2 = value.Insert((value.Length), "%");
            return value2;
        }

        public static string FormataCNPJ(string value)
        {
            if (value != null)
            {

                if (value.Length == 14)
                {
                    string value2 = value.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
                    return value2;
                }
                else return value;
            }

            else return value;

        }


        #endregion

        #region ValidaCPF e CNPJ
        public static bool ValidaCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        public static bool ValidaCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            tempCnpj = cnpj.Substring(0, 12);

            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
        #endregion

        #region DropDownList

        public static IEnumerable<SelectListItem> SelectListEnum(IList<SelectListItem> values, string selectedValue = "", string header = "Selecione...")
        {
            selectedValue = selectedValue ?? "";

            if (!string.IsNullOrEmpty(header))
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Text = header;
                listItem.Value = "";
                listItem.Selected = (listItem.Value.Equals(selectedValue) ? true : false);

                values.Insert(0, listItem);
            }

            if (values.Where(m => m.Value.Equals(selectedValue)).Count() > 0)
                (values.Where(m => m.Value.Equals(selectedValue))).First().Selected = true;

            return values;

        }

        public static IEnumerable<SelectListItem> getDropDownList(IDictionary<string, string> values, string selectedValue = "", string header = "Selecione...")
        {
            selectedValue = selectedValue ?? "";
            SelectListItem listItem = null;
            IList<SelectListItem> l = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(header))
            {
                listItem = new SelectListItem();
                listItem.Text = header;
                listItem.Value = "";
                listItem.Selected = (listItem.Value.Equals(selectedValue) ? true : false);

                l.Add(listItem);
            }

            for (int i = 0; i <= values.Count - 1; i++)
            {
                listItem = new SelectListItem();
                listItem.Value = (from x in values select new { x.Key }).ElementAt(i).Key;
                listItem.Text = (from x in values select new { x.Value }).ElementAt(i).Value;
                listItem.Selected = listItem.Value.Trim().Equals(selectedValue.ToString());
                //listItem.Selected = (listItem.Value.Equals(selectedValue) ? true : false);

                l.Add(listItem);
            }

            return l;

        }
        #endregion

        public static bool validaEmail(string value)
        {
            bool flag = true;
            if (!string.IsNullOrEmpty(value) &&
               (!value.Contains("@") ||
                !value.Contains(".") ||
                value.FirstOrDefault().ToString().Equals("@") ||
                value.FirstOrDefault().ToString().Equals(".") ||
                value.LastOrDefault().ToString().Equals("@") ||
                value.LastOrDefault().ToString().Equals(".")))
                flag = false;
            return flag;
        }

        public static bool ValidaHora(string hora)
        {
            Regex r = new Regex(@"([0-1][0-9]|2[0-3]):[0-5][0-9]");
            Match m = r.Match(hora);
            return m.Success;
        }

        public static string soma_data(DateTime data, int meses, int anos)
        {
            string data_exit = "";

            if (data.ToString("dd/MM/yyyy") != "01/01/1900")
            {
                DateTime data_soma = data.AddYears(anos).AddMonths(meses);
                data_exit = formataData_notnull(data_soma);
            }
            else
            {
                data_exit = "";
            }

            return data_exit;
        }

        public static bool valida_hora(string hora)
        {
            string h = hora.Substring(0, 2);
            string min = hora.Substring(3, 2);

            int horas = Convert.ToInt32(h);
            int minutos = Convert.ToInt32(min);

            if (horas > 24 || horas <= 0 || minutos < 0 || minutos > 60)
            {
                return false;
            }
            else return true;

        }

        public static DateTime Converte_Hora(DateTime value)
        {
            return new System.DateTime(1900, 1, 1, value.Hour, value.Minute, value.Second);
        }

        public static String Trata_Hora(string value)
        {
            value = value.Replace(";", ":");

            return value;
        }

        public static String data_extenso(DateTime value)
        {

            CultureInfo culture = new CultureInfo("pt-BR");
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;
            return dtfi.GetDayName(value.DayOfWeek) + ", " + (value.Day) + " " + dtfi.GetMonthName(value.Month) + " de " + (value.Year);

        }

        public static bool ValidaData(string value)
        {
            try
            {
                DateTime data = Convert.ToDateTime(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime[] CalculaPeriodo(string tipo, string data1, string data2)
        {
            switch (tipo)
            {
                case "Mês Atual":
                    //data1 = "01/" + DateTime.Today.ToString("MM/yyyy");
                    //data2 = ("01/" + DateTime.Today.AddMonths(1).ToString("MM/yyyy")).AddDays(-1).ToString("dd/MM/yyyy");
                    data1 = DateTime.Today.ToString("yyyy-MM-") + "01"  ;
                    data2 = DateTime.Parse(DateTime.Today.AddMonths(1).ToString("yyyy-MM-") + "01").AddDays(-1).ToString("yyyy-MM-dd");                    
                    break;
                case "Mês Anterior":
                    data1 = "01/" + DateTime.Today.AddMonths(-1).ToString("MM/yyyy");
                    data2 = DateTime.Parse("01/" + DateTime.Today.AddMonths(1).ToString("MM/yyyy")).AddDays(-1).ToString("dd/MM/yyyy");
                    break;
                case "Próximo mês":
                    data1 = DateTime.Today.ToString("dd/MM/yyyy");
                    data2 = DateTime.Parse("01/" + DateTime.Today.AddMonths(2).ToString("MM/yyyy")).AddDays(-1).ToString("dd/MM/yyyy");
                    break;
                case "Últimos 3 meses":
                    data1 = "01/" + DateTime.Today.AddMonths(-3).ToString("MM/yyyy");
                    data2 = DateTime.Parse("01/" + DateTime.Today.AddMonths(1).ToString("MM/yyyy")).AddDays(-1).ToString("dd/MM/yyyy");
                    break;
                case "Ano atual":
                    data1 = "01/01/" + DateTime.Today.ToString("yyyy");
                    data2 = "31/12/" + DateTime.Today.ToString("yyyy");
                    break;
            }

            DateTime[] datas = { DateTime.Parse(data1), DateTime.Parse(data2) };
            
            return datas;
        }


    }

}