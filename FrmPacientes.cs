using SistemaCitasDentales.Data;
using SistemaCitasDentales.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaCitasDentales
{
    public partial class FrmPacientes : Form
    {
        public FrmPacientes()
        {
            InitializeComponent();
        }

        private void CargarPacientes()
        {
            using (var db = new AppDbContext())
            {
                dgvPacientes.DataSource = db.Pacientes.ToList();
            }
        }
        private void btnMostrar_Click(object sender, EventArgs e)
        {
            CargarPacientes();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var p = new Paciente
                {
                    NombreCompleto = txtNombreCompleto.Text,
                    Telefono = txtTelefono.Text,
                    CorreoElectronico = txtCorreoElectronico.Text
                };

                db.Pacientes.Add(p);
                db.SaveChanges();
            }

            CargarPacientes();
        }

        private void dgvPacientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dgvPacientes.CurrentRow.Cells["PacienteId"].Value.ToString();
            txtNombreCompleto.Text = dgvPacientes.CurrentRow.Cells["NombreCompleto"].Value.ToString();
            txtTelefono.Text = dgvPacientes.CurrentRow.Cells["Telefono"].Value.ToString();
            txtCorreoElectronico.Text = dgvPacientes.CurrentRow.Cells["CorreoElectronico"].Value.ToString();
        }
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtID.Text);

            using (var db = new AppDbContext())
            {
                var p = db.Pacientes.Find(id);
               
                    p.NombreCompleto = txtNombreCompleto.Text;
                    p.Telefono = txtTelefono.Text;
                    p.CorreoElectronico = txtCorreoElectronico.Text;

                    db.SaveChanges();    
            }

            CargarPacientes();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtID.Text);

            using (var db = new AppDbContext())
            {
                var p = db.Pacientes.Find(id);
                db.Pacientes.Remove(p);
                db.SaveChanges();
            }

            CargarPacientes();
        }

        private void Limpiar()
        {
            txtID.Clear();
            txtNombreCompleto.Clear();
            txtTelefono.Clear();
            txtCorreoElectronico.Clear();
        }

        private List<Paciente> ObtenerPacientes()
        {
            using (var db = new AppDbContext())
            {
                return db.Pacientes.ToList();
            }
        }
    }
}
