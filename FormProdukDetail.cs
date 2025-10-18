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
    public partial class FormProdukDetail : Form
    {
        public FormProdukDetail()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int value;
            if (!string.IsNullOrEmpty(txtStok.Text) && !int.TryParse(txtStok.Text, out value))
            {
                MessageBox.Show("Masukkan angka saja", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStok.Clear();
            }
        }

        private void LoadDataProduk()
        {
            if (ProdukId == null) return;
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT NamaProduk, Harga, Stok, KategoriId FROM Produk WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", ProdukId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtNamaProduk.Text = reader["NamaProduk"].ToString();
                        txtHarga.Text = reader["Harga"].ToString();
                        txtStok.Text = reader["Stok"].ToString();
                        cmbKategori.SelectedValue =
                        Convert.ToInt32(reader["KategoriId"]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data produk: " + ex.Message);
                }
            }
        }



        private void FormProdukDetail_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Id, NamaKategori FROM Kategori";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, string> kategoriDict = new Dictionary<int,
                    string>();
                    while (reader.Read())
                    {
                        kategoriDict.Add((int)reader["Id"],
                        reader["NamaKategori"].ToString());
                    }
                    cmbKategori.DataSource = new BindingSource(kategoriDict, null);
                    cmbKategori.DisplayMember = "Value";
                    cmbKategori.ValueMember = "Key";
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat kategori: " + ex.Message);
                }

                if (ProdukId.HasValue)
                {
                    LoadDataProduk();
                    this.Text = "Edit Produk";
                }
                else
                {
                    this.Text = "Tambah Produk";
                }
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNamaProduk.Text))
            {
                MessageBox.Show("Nama produk tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamaProduk.Focus();
                return;
            }

            using (SqlConnection conn = Koneksi.GetConnection())
            {

                errorProvider1.Clear();
                bool valid = true;
                if (string.IsNullOrWhiteSpace(txtNamaProduk.Text))
                {
                    errorProvider1.SetError(txtNamaProduk, "Nama produk tidak boleh kosong.");
                    valid = false;
                }
                if (!decimal.TryParse(txtHarga.Text, out decimal harga) || harga < 0)
                {
                    errorProvider1.SetError(txtHarga, "Harga harus berupa angka positif.");
                    valid = false;
                }
                if (!int.TryParse(txtStok.Text, out int stok) || stok < 0)
                {
                    errorProvider1.SetError(txtStok, "Stok harus berupa angka ≥ 0.");
                    valid = false;
                }
                if (!valid) return;

                try
                {
                    conn.Open();
                    string query;
                    if (ProdukId.HasValue)
                    {
                        // UPDATE
                        query = @"UPDATE Produk
                          SET NamaProduk = @nama, Harga = @harga, Stok = @stok, KategoriId = @kategori
                          WHERE Id = @id";
                    }
                    else
                    {
                        // INSERT
                        query = @"INSERT INTO Produk (NamaProduk, Harga, Stok, KategoriId)
                          VALUES (@nama, @harga, @stok, @kategori)";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nama", txtNamaProduk.Text);
                    cmd.Parameters.AddWithValue("@harga", Convert.ToDecimal(txtHarga.Text));
                    cmd.Parameters.AddWithValue("@stok", Convert.ToInt32(txtStok.Text));
                    cmd.Parameters.AddWithValue("@kategori", ((KeyValuePair<int, string>)cmbKategori.SelectedItem).Key);

                    if (ProdukId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", ProdukId);
                    }

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Produk berhasil disimpan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menyimpan produk: " + ex.Message);
                }
            }
        }


        private void btnBatal_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtHarga_TextChanged(object sender, EventArgs e)
        {
            int value;
            if (!string.IsNullOrEmpty(txtHarga.Text) && !int.TryParse(txtHarga.Text, out value))
            {
                MessageBox.Show("Kolom Harga hanya boleh angka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHarga.Clear();
            }
        }

        public int? ProdukId { get; set; } = null;


    }
}
