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
    public partial class FrmDentista : Form
    {
        public FrmDentista()
        {
            InitializeComponent();
        }

        private void CargarDentistas()
        {
            using (var db = new AppDbContext())
            {
                dgvDentistas.DataSource = db.Dentistas.ToList();
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            CargarDentistas();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var d = new Dentista
                    {
                        NombreCompleto = txtNombreCompleto.Text,
                        Especialidad = txtEspecialidad.Text,
                    };

                    db.Dentistas.Add(d);
                    db.SaveChanges();
                }

                CargarDentistas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar dentista: " + ex.Message);
            }
        }

        private void dgvDentistas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtDentistaID.Text = dgvDentistas.CurrentRow.Cells["DentistaID"].Value.ToString();
            txtNombreCompleto.Text = dgvDentistas.CurrentRow.Cells["NombreCompleto"].Value.ToString();
            txtEspecialidad.Text = dgvDentistas.CurrentRow.Cells["Especialidad"].Value.ToString();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(txtDentistaID.Text);

                using (var db = new AppDbContext())
                {
                    var d = db.Dentistas.Find(id);

                    d.NombreCompleto = txtNombreCompleto.Text;
                    d.Especialidad = txtEspecialidad.Text;

                    db.SaveChanges();
                }

                CargarDentistas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar dentista: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(txtDentistaID.Text);

                using (var db = new AppDbContext())
                {
                    var d = db.Dentistas.Find(id);
                    db.Dentistas.Remove(d);
                    db.SaveChanges();
                }

                CargarDentistas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar dentista: " + ex.Message);
            }
        }

        private void Limpira()
        {
            txtDentistaID.Clear();
            txtNombreCompleto.Clear();
            txtEspecialidad.Clear();
        }

        private List<Dentista> ObtenerDentistas()
        {
            using (var db = new AppDbContext())
            {
                return db.Dentistas.ToList();
            }
        }
    }
}
