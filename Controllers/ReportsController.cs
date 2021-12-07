using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using Telerik.Reporting;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace webfmis.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ReportsControllerBase
    {
        public ReportsController(IReportServiceConfiguration reportServiceConfiguration)
            : base(reportServiceConfiguration)
        {
        }

        protected override HttpStatusCode SendMailMessage(MailMessage mailMessage)
        {
            //throw new System.NotImplementedException("This method should be implemented in order to send mail messages");

            using (var smtpClient = new SmtpClient("smtp01.mycompany.com", 25))
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = false;

                smtpClient.Send(mailMessage);
            }
            return HttpStatusCode.OK;
        }

        [Authorize]
        public override IActionResult CreateDocument(string clientID, string instanceID, CreateDocumentArgs args)
        {
            return base.CreateDocument(clientID, instanceID, args);
        }
    }

    class CustomReportSourceResolver : IReportSourceResolver
    {
        public ReportSource Resolve(string reportId, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
        {
            var cmdText = "SELECT Definition FROM Reports WHERE ID = @ReportId";

            var reportXml = string.Empty;
            using (var conn = new System.Data.SqlClient.SqlConnection(@"server=(local)\sqlexpress;database=REPORTS;integrated security=true;"))
            {
                var command = new System.Data.SqlClient.SqlCommand(cmdText, conn);
                command.Parameters.Add("@ReportId", System.Data.SqlDbType.Int);
                command.Parameters["@ReportId"].Value = reportId;

                try
                {
                    conn.Open();
                    reportXml = (string)command.ExecuteScalar();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
            }

            if (string.IsNullOrEmpty(reportXml))
            {
                throw new System.Exception("Unable to load a report with the specified ID: " + reportId);
            }

            return new Telerik.Reporting.XmlReportSource { Xml = reportXml };
        }
    }

    class ReportConnectionStringManager
    {
        readonly string connectionString;

        public ReportConnectionStringManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public ReportSource UpdateReportSource(ReportSource sourceReportSource)
        {
            if (sourceReportSource is UriReportSource)
            {
                var uriReportSource = (UriReportSource)sourceReportSource;
                // unpackage TRDP report
                // http://docs.telerik.com/reporting/report-packaging-trdp#unpackaging
                var reportInstance = UnpackageReport(uriReportSource);
                // or deserialize TRDX report(legacy format)
                // http://docs.telerik.com/reporting/programmatic-xml-serialization#deserialize-report-definition-from-xml-file
                // var reportInstance = DeserializeReport(uriReportSource);
                ValidateReportSource(uriReportSource.Uri);
                this.SetConnectionString(reportInstance);
                return CreateInstanceReportSource(reportInstance, uriReportSource);
            }

            if (sourceReportSource is XmlReportSource)
            {
                var xml = (XmlReportSource)sourceReportSource;
                ValidateReportSource(xml.Xml);
                var reportInstance = this.DeserializeReport(xml);
                this.SetConnectionString(reportInstance);
                return CreateInstanceReportSource(reportInstance, xml);
            }

            if (sourceReportSource is InstanceReportSource)
            {
                var instanceReportSource = (InstanceReportSource)sourceReportSource;
                this.SetConnectionString((ReportItemBase)instanceReportSource.ReportDocument);
                return instanceReportSource;
            }

            if (sourceReportSource is TypeReportSource)
            {
                var typeReportSource = (TypeReportSource)sourceReportSource;
                var typeName = typeReportSource.TypeName;
                ValidateReportSource(typeName);
                var reportType = Type.GetType(typeName);
                var reportInstance = (Report)Activator.CreateInstance(reportType);
                this.SetConnectionString((ReportItemBase)reportInstance);
                return CreateInstanceReportSource(reportInstance, typeReportSource);
            }

            throw new NotImplementedException("Handler for the used ReportSource type is not implemented.");
        }

        ReportSource CreateInstanceReportSource(IReportDocument report, ReportSource originalReportSource)
        {
            var instanceReportSource = new InstanceReportSource { ReportDocument = report };
            instanceReportSource.Parameters.AddRange(originalReportSource.Parameters);
            return instanceReportSource;
        }

        void ValidateReportSource(string value)
        {
            if (value.Trim().StartsWith("="))
            {
                throw new InvalidOperationException("Expressions for ReportSource are not supported when changing the connection string dynamically");
            }
        }

        Report UnpackageReport(UriReportSource uriReportSource)
        {
            var reportPackager = new ReportPackager();
            using (var sourceStream = System.IO.File.OpenRead(uriReportSource.Uri))
            {
                var report = (Report)reportPackager.UnpackageDocument(sourceStream);
                return report;
            }
        }

        Report DeserializeReport(UriReportSource uriReportSource)
        {
            var settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (var xmlReader = System.Xml.XmlReader.Create(uriReportSource.Uri, settings))
            {
                var xmlSerializer = new Telerik.Reporting.XmlSerialization.ReportXmlSerializer();
                var report = (Telerik.Reporting.Report)xmlSerializer.Deserialize(xmlReader);
                return report;
            }
        }

        Report DeserializeReport(XmlReportSource xmlReportSource)
        {
            var settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            var textReader = new System.IO.StringReader(xmlReportSource.Xml);
            using (var xmlReader = System.Xml.XmlReader.Create(textReader, settings))
            {
                var xmlSerializer = new Telerik.Reporting.XmlSerialization.ReportXmlSerializer();
                var report = (Telerik.Reporting.Report)xmlSerializer.Deserialize(xmlReader);
                return report;
            }
        }

        void SetConnectionString(ReportItemBase reportItemBase)
        {
            if (reportItemBase.Items.Count < 1)
                return;

            if (reportItemBase is Report)
            {
                var report = (Report)reportItemBase;

                if (report.DataSource is SqlDataSource)
                {
                    var sqlDataSource = (SqlDataSource)report.DataSource;
                    sqlDataSource.ConnectionString = connectionString;
                }
                foreach (var parameter in report.ReportParameters)
                {
                    if (parameter.AvailableValues.DataSource is SqlDataSource)
                    {
                        var sqlDataSource = (SqlDataSource)parameter.AvailableValues.DataSource;
                        sqlDataSource.ConnectionString = connectionString;
                    }
                }
            }

            foreach (var item in reportItemBase.Items)
            {
                //recursively set the connection string to the items from the Items collection
                SetConnectionString(item);

                //set the drillthrough report connection strings
                var drillThroughAction = item.Action as NavigateToReportAction;
                if (null != drillThroughAction)
                {
                    var updatedReportInstance = this.UpdateReportSource(drillThroughAction.ReportSource);
                    drillThroughAction.ReportSource = updatedReportInstance;
                }

                if (item is SubReport)
                {
                    var subReport = (SubReport)item;
                    subReport.ReportSource = this.UpdateReportSource(subReport.ReportSource);
                    continue;
                }

                //Covers all data items(Crosstab, Table, List, Graph, Map and Chart)
                if (item is DataItem)
                {
                    var dataItem = (DataItem)item;
                    if (dataItem.DataSource is SqlDataSource)
                    {
                        var sqlDataSource = (SqlDataSource)dataItem.DataSource;
                        sqlDataSource.ConnectionString = connectionString;
                        continue;
                    }
                }

            }
        }
    }
}
