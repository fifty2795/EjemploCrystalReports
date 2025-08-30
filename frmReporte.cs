using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReportsExample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CrystalReportsExample
{
    public partial class frmRptResumenVenta : Form
    {
        public frmRptResumenVenta()
        {
            InitializeComponent();
        }

        private void frmRptResumenVenta_Load(object sender, EventArgs e)
        {
            var clientes = ObtenerClientes();

            clientes.Insert(0, new Cliente { IdCliente = 0, Nombre = "Todos" });

            cmbClientes.DataSource = clientes;
            cmbClientes.DisplayMember = "Nombre";   
            cmbClientes.ValueMember = "IdCliente"; 
            cmbClientes.SelectedIndex = 0;

            dtpInicio.ShowCheckBox = true;
            dtpInicio.Checked = false;  

            dtpFin.ShowCheckBox = true;
            dtpFin.Checked = false;            
        }

        private void btnGenerarRpt_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime? fechaInicio = dtpInicio.Checked ? dtpInicio.Value.Date : (DateTime?)null;
                DateTime? fechaFin = dtpFin.Checked ? dtpFin.Value.Date : (DateTime?)null;
                int? clienteId = null;

                var report = new ReportDocument();

                report.Load(@"D:\repos\CrystalReportsExample\CrystalReport1.rpt");

                if (cmbClientes.SelectedValue.ToString() != "0")
                {
                    if (int.TryParse(cmbClientes.SelectedValue.ToString(), out var cid)) clienteId = cid;
                }

                var connInfo = new ConnectionInfo
                {
                    ServerName = @"ALEJANDRO\SQLEXPRESS",
                    DatabaseName = "MVC",
                    //UserID = "usuario_sql",
                    //Password = "password_sql",
                    IntegratedSecurity = true
                };

                foreach (Table table in report.Database.Tables)
                {
                    var logonInfo = table.LogOnInfo;
                    logonInfo.ConnectionInfo = connInfo;
                    table.ApplyLogOnInfo(logonInfo);

                    table.Location = table.Location;
                }

                report.SetParameterValue("@fechaInicio", (object)fechaInicio ?? DBNull.Value);
                report.SetParameterValue("@fechaFin", (object)fechaFin ?? DBNull.Value);
                report.SetParameterValue("@idCliente", (object)clienteId ?? DBNull.Value);

                crystalReportViewer1.ReportSource = report;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al generar el reporte." + ex.Message);
            }
        }

        private List<Cliente> ObtenerClientes()
        {
            var lista = new List<Cliente>();

            using (var cn = new SqlConnection(@"Server=ALEJANDRO\SQLEXPRESS;Database=MVC;Integrated Security=True;"))
            using (var cmd = new SqlCommand("select Id_Cliente, Nombre from [dbo].[tbl_Cliente]", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Cliente
                        {
                            IdCliente = dr.GetInt32(0),
                            Nombre = dr.GetString(1)
                        });
                    }
                }
            }

            return lista;
        }
    }
}
