using App_Dominio.Contratos;
using App_Dominio.Component;
using App_Dominio.Entidades;
using App_Dominio.Security;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Web.Mvc;

namespace App_Dominio.Controllers
{
    public abstract class ReportController<R> : SuperController where R : Repository
    {
        #region Exportar para PDF (Report Server)
        public FileResult _PDF(string export, string fileName, IListReportRepository<R> report, ReportParameter[] p,
                                string PageWidth = "21cm", string PageHeight = "29,7cm", params object[] param)
        {
            p[0] = new ReportParameter("empresa", new EmpresaSecurity<App_DominioContext>().getEmpresa().nome, false);

            LocalReport relatorio = new LocalReport();
            relatorio.ReportPath = Server.MapPath("~/App_Data/rdlc/" + fileName + ".rdlc");
            IEnumerable<IReportRepository<R>> r = (IEnumerable<IReportRepository<R>>)report.ListReportRepository(param);
            relatorio.DataSources.Add(new ReportDataSource("DataSet1", r));

            relatorio.SetParameters(p);
            relatorio.Refresh();

            string reportType = "PDF";
            string reportFile = fileName + ".pdf";

            if (export == "png")
            {
                reportType = "Image";
                reportFile = fileName + ".png";
            }
            else if (export == "excel")
            {
                reportType = "Excel";
                reportFile = fileName + ".xls";
            }
            else if (export == "word")
            {
                reportType = "Word";
                reportFile = fileName + ".doc";
            }

            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
             "<DeviceInfo>" +
             " <OutputFormat>PDF</OutputFormat>" +
             " <PageWidth>" + PageWidth + "</PageWidth>" +
             " <PageHeight>" + PageHeight + "</PageHeight>" +
             " <MarginTop>0.5cm</MarginTop>" +
             " <MarginLeft>0.5cm</MarginLeft>" +
             " <MarginRight>0.5cm</MarginRight>" +
             " <MarginBottom>0.5cm</MarginBottom>" +
             "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] bytes;

            //Renderiza o relatório em bytes
            bytes = relatorio.Render(
            reportType,
            deviceInfo,
            out mimeType,
            out encoding,
            out fileNameExtension,
            out streams,
            out warnings);

            if (export != "view")
                return File(bytes, mimeType, reportFile);
            else
                return File(bytes, mimeType);
        }
        #endregion
    }
}
