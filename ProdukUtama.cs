using ManajemenToko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tugas2
{
    public partial class ProdukUtama : Form
    {
        public ProdukUtama()
        {
            InitializeComponent();
        }

        private void LogError(Exception ex)
        {
            try
            {
                string logPath = "error_log.txt";
                string message = $"[{DateTime.Now}] {ex.Message}\n{ex.StackTrace}\n\n";
                System.IO.File.AppendAllText(logPath, message);
            }
            catch { }
        }



        private void ProdukUtama_Load(object sender, EventArgs e)
        {
            LoadDataProduk();
            dgvProduk.SelectionChanged += dgvProduk_SelectionChanged;
            UpdateButtonState();
        }

        private void dgvProduk_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            bool rowSelected = dgvProduk.SelectedRows.Count > 0;
            btnEdit.Enabled = rowSelected;
            btnHapus.Enabled = rowSelected;
        }



        private void btnTambah_Click(object sender, EventArgs e)
        {
            FormProdukDetail frm = new FormProdukDetail();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadDataProduk(); // refresh data setelah tambah
            }
        }

        private int? GetSelectedProductId()
        {
            if (dgvProduk.SelectedRows.Count > 0)
            {
                return Convert.ToInt32(dgvProduk.SelectedRows[0].Cells[0].Value);
            }
            return null;
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedProductId();
            if (id == null)
            {
                MessageBox.Show("Pilih produk yang ingin diedit.");
                return;
            }
            FormProdukDetail form = new FormProdukDetail();
            form.ProdukId = id;
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDataProduk();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoadDataProduk()
        {
            dgvProduk.Rows.Clear();
            dgvProduk.Columns.Clear();

            // Pastikan mode seleksi per baris (penting!)
            dgvProduk.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProduk.MultiSelect = false;

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT p.Id, p.NamaProduk, p.Harga, p.Stok, k.NamaKategori
                             FROM Produk p
                             LEFT JOIN Kategori k ON p.KategoriId = k.Id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    dgvProduk.Columns.Add("Id", "ID");
                    dgvProduk.Columns.Add("NamaProduk", "Nama Produk");
                    dgvProduk.Columns.Add("Harga", "Harga");
                    dgvProduk.Columns.Add("Stok", "Stok");
                    dgvProduk.Columns.Add("Kategori", "Kategori");

                    while (reader.Read())
                    {
                        decimal harga = Convert.ToDecimal(reader["Harga"]);
                        string hargaFormatted = string.Format("Rp. {0:N0}", harga);

                        dgvProduk.Rows.Add(
                            reader["Id"],
                            reader["NamaProduk"],
                            hargaFormatted, // tampilkan format Rp.
                            reader["Stok"],
                            reader["NamaKategori"]
                        );
                    }
                    reader.Close();

                    // Setelah load data, nonaktifkan tombol edit/hapus
                    btnEdit.Enabled = false;
                    btnHapus.Enabled = false;

                    // Event saat user klik baris
                    dgvProduk.SelectionChanged += (s, e) =>
                    {
                        bool rowSelected = dgvProduk.SelectedRows.Count > 0;
                        btnEdit.Enabled = rowSelected;
                        btnHapus.Enabled = rowSelected;
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menampilkan data: " + ex.Message);
                    LogError(ex);
                }
            }
        }

        private void btnHapus_Click_1(object sender, EventArgs e)
        {
            int? id = GetSelectedProductId();
            if (id == null)
            {
                MessageBox.Show("Pilih produk yang ingin dihapus.");
                return;
            }
            DialogResult result = MessageBox.Show(
            "Yakin ingin menghapus produk ini?",
            "Konfirmasi Hapus",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = Koneksi.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM Produk WHERE Id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Produk berhasil dihapus!");
                        LoadDataProduk();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menghapus produk: " + ex.Message);
                    }
                }
            }
        }

        private void btnEdit_Click_1(object sender, EventArgs e)
        {

        }

        private void btnEdit_ClientSizeChanged(object sender, EventArgs e)
        {

        }

        private void btnEdit_ChangeUICues(object sender, UICuesEventArgs e)
        {

        }

        private void btnKategori_Click(object sender, EventArgs e)
        {
        
                FormKategori frmKategori = new FormKategori();
                frmKategori.ShowDialog();
            
        }
    }
}
