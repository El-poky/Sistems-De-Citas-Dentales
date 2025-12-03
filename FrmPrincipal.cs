using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Drawing.Text;


namespace SistemaCitasDentales
{
    public partial class FrmPricinpal : Form
    {
        private List<object> datos = new List<object>();

        public FrmPricinpal()
        {
            InitializeComponent();
        }

        private void ExportarListaCSV<T>(List<T> lista, string nombreArchivo)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = nombreArchivo + ".csv";
            save.Filter = "Archivo CSV (*.csv)|*.csv";

            if (save.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();

                var propiedades = typeof(T).GetProperties();

                sb.AppendLine(string.Join(",", propiedades.Select(p => p.Name)));

                foreach (var item in lista)
                {
                    var valores = propiedades.Select(p => p.GetValue(item)?.ToString()?.Replace(",", " ") ?? "");
                    sb.AppendLine(string.Join(",", valores));
                }
                File.WriteAllText(save.FileName, sb.ToString(), Encoding.UTF8);

                MessageBox.Show("Datos exportados exitosamente.", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void pacientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPacientes frm = new FrmPacientes();
            frm.ShowDialog();
        }

        private void dentistasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDentista frm = new FrmDentista();
            frm.ShowDialog();
        }

        private void motivosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMotivos frm = new FrmMotivos();
            frm.ShowDialog();
        }

        private void citasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FrmCitas == null || FrmCitas.IsDisposed)
            {
                FrmCitas = new FrmCitas();
            }

            FrmCitas.ShowDialog();
            FrmCitas.BringToFront();

        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sistema de Citas Dentales\nVersión 1.0\nDesarrollado por Edgar Fortuna",
                           "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private FrmCitas FrmCitas;

        private void exportarAToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if(FrmCitas == null || FrmCitas.IsDisposed)
    {
                MessageBox.Show("Primero abre el formulario de Citas.");
                return;
            }

            FrmCitas.Exportar();

        }

        private void ExportarGridACSV(DataGridView grid)
        {
            if (grid.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "CitasExportadas.csv";
            save.Filter = "CSV|*.csv";

            if (save.ShowDialog() != DialogResult.OK)
                return;

            using (StreamWriter sw = new StreamWriter(save.FileName))
            {

                var headers = grid.Columns
                    .Cast<DataGridViewColumn>()
                    .Select(c => c.HeaderText);

                sw.WriteLine(string.Join(",", headers));

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var cells = row.Cells
                            .Cast<DataGridViewCell>()
                            .Select(cell => cell.Value != null
                                ? cell.Value.ToString().Replace(",", ";")
                                : "");

                        sw.WriteLine(string.Join(",", cells));
                    }
                }
            }

            MessageBox.Show("Archivo exportado correctamente.");
        }
    }
}
