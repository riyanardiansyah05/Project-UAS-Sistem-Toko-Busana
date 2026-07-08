using System;

namespace TokoBusanaAlex.Models
{
    public class Transaksi
    {
        public int IdTransaksi { get; set; }
        public int IdProduk { get; set; }
        public string NamaProduk { get; set; } = string.Empty;
        public int Harga { get; set; }
        public int JumlahBeli { get; set; }
        public int TotalBayar { get; set; }
        public DateTime TanggalTransaksi { get; set; }
    }
}