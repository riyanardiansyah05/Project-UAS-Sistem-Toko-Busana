using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace TokoBusanaAlex.Models
{
    public class ProdukRepository
    {
        private readonly string _connectionString;

        public ProdukRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        // ==========================================
        // 1. FITUR CRUD PRODUK
        // ==========================================

        // Ambil semua data produk (Read)
        public List<Produk> GetAll()
        {
            var list = new List<Produk>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM produk", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Produk
                        {
                            IdProduk = Convert.ToInt32(reader["id_produk"]),
                            NamaProduk = reader["nama_produk"].ToString() ?? "",
                            Kategori = reader["kategori"].ToString() ?? "",
                            Harga = Convert.ToInt32(reader["harga"]),
                            Stok = Convert.ToInt32(reader["stok"])
                        });
                    }
                }
            }
            return list;
        }

        // Menambah data produk baru (Create)
        public void Add(Produk produk)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO produk (nama_produk, kategori, harga, stok) VALUES (@nama, @kategori, @harga, @stok)", conn);
                cmd.Parameters.AddWithValue("@nama", produk.NamaProduk);
                cmd.Parameters.AddWithValue("@kategori", produk.Kategori);
                cmd.Parameters.AddWithValue("@harga", produk.Harga);
                cmd.Parameters.AddWithValue("@stok", produk.Stok);
                cmd.ExecuteNonQuery();
            }
        }

        // Ambil 1 data produk berdasarkan ID (untuk Edit)
        public Produk? GetById(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM produk WHERE id_produk = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Produk
                        {
                            IdProduk = Convert.ToInt32(reader["id_produk"]),
                            NamaProduk = reader["nama_produk"].ToString() ?? "",
                            Kategori = reader["kategori"].ToString() ?? "",
                            Harga = Convert.ToInt32(reader["harga"]),
                            Stok = Convert.ToInt32(reader["stok"])
                        };
                    }
                }
            }
            return null;
        }

        // Memperbarui data produk (Update)
        public void Update(Produk produk)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("UPDATE produk SET nama_produk=@nama, kategori=@kategori, harga=@harga, stok=@stok WHERE id_produk=@id", conn);
                cmd.Parameters.AddWithValue("@nama", produk.NamaProduk);
                cmd.Parameters.AddWithValue("@kategori", produk.Kategori);
                cmd.Parameters.AddWithValue("@harga", produk.Harga);
                cmd.Parameters.AddWithValue("@stok", produk.Stok);
                cmd.Parameters.AddWithValue("@id", produk.IdProduk);
                cmd.ExecuteNonQuery();
            }
        }

        // Menghapus data produk (Delete)
        public void Delete(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM produk WHERE id_produk = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ==========================================
        // 2. FITUR TRANSAKSI & PERHITUNGAN
        // ==========================================

        // Menyimpan transaksi kasir dan mengurangi stok
        public void AddTransaksi(Transaksi t)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var produk = GetById(t.IdProduk);
                if (produk != null && produk.Stok >= t.JumlahBeli)
                {
                    // Hitung otomatis total bayar
                    t.TotalBayar = produk.Harga * t.JumlahBeli;
                    t.NamaProduk = produk.NamaProduk;
                    t.Harga = produk.Harga;

                    // Masukkan ke tabel transaksi
                    var cmdTrans = new MySqlCommand("INSERT INTO transaksi (id_produk, nama_produk, harga, jumlah_beli, total_bayar) VALUES (@idP, @nama, @harga, @jumlah, @total)", conn);
                    cmdTrans.Parameters.AddWithValue("@idP", t.IdProduk);
                    cmdTrans.Parameters.AddWithValue("@nama", t.NamaProduk);
                    cmdTrans.Parameters.AddWithValue("@harga", t.Harga);
                    cmdTrans.Parameters.AddWithValue("@jumlah", t.JumlahBeli);
                    cmdTrans.Parameters.AddWithValue("@total", t.TotalBayar);
                    cmdTrans.ExecuteNonQuery();

                    // Kurangi stok produk secara otomatis
                    int stokBaru = produk.Stok - t.JumlahBeli;
                    var cmdStok = new MySqlCommand("UPDATE produk SET stok = @stok WHERE id_produk = @id", conn);
                    cmdStok.Parameters.AddWithValue("@stok", stokBaru);
                    cmdStok.Parameters.AddWithValue("@id", t.IdProduk);
                    cmdStok.ExecuteNonQuery();
                }
            }
        }

        // Ambil semua riwayat transaksi (Cetak Laporan)
        public List<Transaksi> GetAllTransaksi()
        {
            var list = new List<Transaksi>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM transaksi ORDER BY tanggal_transaksi DESC", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Transaksi
                        {
                            IdTransaksi = Convert.ToInt32(reader["id_transaksi"]),
                            IdProduk = Convert.ToInt32(reader["id_produk"]),
                            NamaProduk = reader["nama_produk"].ToString() ?? "",
                            Harga = Convert.ToInt32(reader["harga"]),
                            JumlahBeli = Convert.ToInt32(reader["jumlah_beli"]),
                            TotalBayar = Convert.ToInt32(reader["total_bayar"]),
                            TanggalTransaksi = Convert.ToDateTime(reader["tanggal_transaksi"])
                        });
                    }
                }
            }
            return list;
        }
    }
}