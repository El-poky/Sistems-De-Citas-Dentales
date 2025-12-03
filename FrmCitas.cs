using SistemaCitasDentales.Data;
using SistemaCitasDentales.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaCitasDentales
{
    public partial class FrmCitas : Form
    {
        private List<object> datos = new List<object>();

        public FrmCitas()
        {
            InitializeComponent();
        }

        private void FrmCitas_Load(object sender, EventArgs e)
        {
            dgvCitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCitas.ReadOnly = true;
            dgvCitas.MultiSelect = false;
            dgvCitas.AllowUserToAddRows = false;

            CargarCombos();
            CargarCitasEngrid();
        }

        private void CargarCombos()
        {
            using (var db = new AppDbContext())
            {
                var pacientes = db.Pacientes.Select(p => new { p.PacienteId, p.NombreCompleto }).ToList();
                cmbPaciente.DisplayMember = "NombreCompleto";
                cmbPaciente.ValueMember = "PacienteId";
                cmbPaciente.DataSource = pacientes;

                var dentistas = db.Dentistas.Select(d => new { d.DentistaID, d.NombreCompleto }).ToList();
                cmbdentista.DisplayMember = "NombreCompleto";
                cmbdentista.ValueMember = "DentistaID";
                cmbdentista.DataSource = dentistas;

                var motivos = db.Motivos.Select(m => new { m.MotivoID, m.Descripcion }).ToList();
                cmbMotivo.DisplayMember = "Descripcion";
                cmbMotivo.ValueMember = "MotivoID";
                cmbMotivo.DataSource = motivos;
            }
        }

        private void CargarCitasEngrid()
        {
            using (var db = new AppDbContext())
            {
                var hoy = DateTime.Now.Date;

                var lista = db.Citas
                    .Select(c => new
                    {
                        c.CitaID,
                        Paciente = db.Pacientes.Where(p => p.PacienteId == c.PacienteID)
                                               .Select(p => p.NombreCompleto)
                                               .FirstOrDefault(),
                        Dentista = db.Dentistas.Where(d => d.DentistaID == c.DentistaID)
                                               .Select(d => d.NombreCompleto)
                                               .FirstOrDefault(),
                        Motivo = db.Motivos.Where(m => m.MotivoID == c.MotivoID)
                                           .Select(m => m.Descripcion)
                                           .FirstOrDefault(),
                        c.Fecha,
                        c.Hora,
                        c.Duracion,

                        DiasRestantes = System.Data.Entity.DbFunctions.DiffDays(hoy, c.Fecha),

                        Estado = c.Fecha > hoy ? "Vigente" :
                                 c.Fecha == hoy ? "En proceso" :
                                 "Finalizado"
                    })
                    .ToList();

                datos = lista.Cast<object>().ToList();
                dgvCitas.DataSource = lista;
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            CargarCitasEngrid();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPaciente.SelectedValue == null || cmbdentista.SelectedValue == null || cmbMotivo.SelectedValue == null)
                {
                    MessageBox.Show("Seleccionear paciente, dentista y motivo.");
                    return;
                }
                if (!int.TryParse(txtDuracion.Text, out int duracion) || duracion <= 0)
                {
                    MessageBox.Show("Duracion invalida (minutos).");
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var cita = new Cita
                    {
                        PacienteID = (int)cmbPaciente.SelectedValue,
                        DentistaID = (int)cmbdentista.SelectedValue,
                        MotivoID = (int)cmbMotivo.SelectedValue,
                        Fecha = dtpFecha.Value.Date,
                        Hora = TimeSpan.Parse(txtHora.Text.Trim()),
                        Duracion = duracion
                    };

                    db.Citas.Add(cita);
                    db.SaveChanges();
                }

                CargarCitasEngrid();
                MessageBox.Show("Cita agregada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar cita: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtCitaID.Text, out int citaID))
                {
                    MessageBox.Show("Seleccione una cita valida.");
                    return;
                }
                if (!int.TryParse(txtDuracion.Text, out int duracion) || duracion <= 0)
                {
                    MessageBox.Show("Duracion invalida (minutos).");
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var cita = db.Citas.Find(citaID);
                    if (cita == null)
                    {
                        MessageBox.Show("La cita no existe.");
                        return;
                    }

                    cita.PacienteID = (int)cmbPaciente.SelectedValue;
                    cita.DentistaID = (int)cmbdentista.SelectedValue;
                    cita.MotivoID = (int)cmbMotivo.SelectedValue;
                    cita.Fecha = dtpFecha.Value.Date;
                    cita.Hora = TimeSpan.Parse(txtHora.Text.Trim());
                    cita.Duracion = duracion;

                    db.SaveChanges();
                }

                CargarCitasEngrid();
                MessageBox.Show("Cita actualizada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar cita: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtCitaID.Text, out int citaID))
                {
                    MessageBox.Show("Seleccione una cita valida.");
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var cita = db.Citas.Find(citaID);
                    if (cita == null)
                    {
                        MessageBox.Show("La cita no existe.");
                        return;
                    }
                    if (MessageBox.Show("¿Está seguro de eliminar la cita seleccionada?", "Confirmar eliminación", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }

                    db.Citas.Remove(cita);
                    db.SaveChanges();

                    CargarCitasEngrid();
                    MessageBox.Show("Cita eliminada correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar cita: " + ex.Message);
            }
        }

        private void dgvCitas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvCitas.Rows[e.RowIndex];

            txtCitaID.Text = row.Cells["CitaID"].Value.ToString();

            if (int.TryParse(txtCitaID.Text, out int CitaID))
            {
                using (var db = new AppDbContext())
                {
                    var cita = db.Citas.Find(CitaID);
                    if (cita != null)
                    {
                        cmbPaciente.SelectedValue = cita.PacienteID;
                        cmbdentista.SelectedValue = cita.DentistaID;
                        cmbMotivo.SelectedValue = cita.MotivoID;
                        dtpFecha.Value = cita.Fecha;
                        txtHora.Text = cita.Hora.ToString();
                        txtDuracion.Text = cita.Duracion.ToString();
                    }
                }
            }
        }

        private void LimpiarCampos()
        {
            txtCitaID.Clear();
            cmbPaciente.SelectedIndex = -1;
            cmbdentista.SelectedIndex = -1;
            cmbMotivo.SelectedIndex = -1;
            dtpFecha.Value = DateTime.Now;
            txtHora.Clear();
            txtDuracion.Clear();
        }

        private List<object> ObtenerCitas()
        {
            using (var db = new AppDbContext())
            {
                var citas = db.Citas
                    .Join(db.Pacientes, c => c.PacienteID, p => p.PacienteId, (c, p) => new { c, p })
                    .Join(db.Dentistas, cp => cp.c.DentistaID, d => d.DentistaID, (cp, d) => new { cp.c, cp.p, d })
                    .Join(db.Motivos, cpd => cpd.c.MotivoID, m => m.MotivoID, (cpd, m) => new { cpd.c, cpd.p, cpd.d, m })
                    .ToList()
                    .Select(x => new
                    {
                        x.c.CitaID,
                        Paciente = x.p.NombreCompleto ?? "N/D",
                        Dentista = x.d.NombreCompleto ?? "N/D",
                        Motivo = x.m.Descripcion ?? "N/D",
                        Fecha = x.c.Fecha.ToString("yyyy-MM-dd"),
                        x.c.Hora,
                        x.c.Duracion,
                        DiasRestantes = (x.c.Fecha.Date - DateTime.Now.Date).Days,
                        Estado =
                            (x.c.Fecha.Date > DateTime.Now.Date)
                                ? "Vigente"
                                : (x.c.Fecha.Date == DateTime.Now.Date ? "En proceso" : "Finalizado")
                    })
                    .ToList<object>();

                return citas;
            }
        }

        private void FrmCitas_Load_1(object sender, EventArgs e)
        {
            CargarCombos();
        }

        public void Exportar()
        {
            ExportarGridACSV(dgvCitas);
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
                var headers = grid.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText);
                sw.WriteLine(string.Join(",", headers));

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var cells = row.Cells.Cast<DataGridViewCell>()
                            .Select(cell => cell.Value != null ? cell.Value.ToString().Replace(",", ";") : "");

                        sw.WriteLine(string.Join(",", cells));
                    }
                }
            }

            MessageBox.Show("Archivo exportado correctamente.");
        }
    }
    }


