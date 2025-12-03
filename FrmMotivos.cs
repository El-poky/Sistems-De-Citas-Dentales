using SistemaCitasDentales.Data;
using SistemaCitasDentales.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaCitasDentales
{
    public partial class FrmMotivos : Form
    {
        public FrmMotivos()
        {
            InitializeComponent();
        }

        private void cargarMotivos()
        {
            using (var db = new Data.AppDbContext())
            {
                dgvMotivos.DataSource = db.Motivos.ToList();
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            cargarMotivos();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var m = new Motivo
                    {
                        Descripcion = txtDescripcion.Text,
                    };
                    db.Motivos.Add(m);
                    db.SaveChanges();
                }

                cargarMotivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar motivo: " + ex.Message);
            }

        }

        private void dgvMotivos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMotivoID.Text = dgvMotivos.CurrentRow.Cells["MotivoID"].Value.ToString();
            txtDescripcion.Text = dgvMotivos.CurrentRow.Cells["Descripcion"].Value.ToString();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(txtMotivoID.Text);

                using (var db = new AppDbContext())
                {
                    var m = db.Motivos.Find(id);
                    m.Descripcion = txtDescripcion.Text;
                    db.SaveChanges();
                }

                cargarMotivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar motivo: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(txtMotivoID.Text);

                using (var db = new AppDbContext())
                {
                    var m = db.Motivos.Find(id);
                    db.Motivos.Remove(m);
                    db.SaveChanges();
                }

                cargarMotivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar motivo: " + ex.Message);
            }
        }

        private void Limpira()
        {
            txtMotivoID.Clear();
            txtDescripcion.Clear();
        }

        private List<Motivo> ObtenerMotivos()
        {
            using (var db = new AppDbContext())
            {
                return db.Motivos.ToList();
            }
        }
    }
}
