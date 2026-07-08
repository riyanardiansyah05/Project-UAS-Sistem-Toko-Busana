namespace TokoBusanaAlex.Models
{
    public class Produk
    {
        public int IdProduk { get; set; }
        public string NamaProduk { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public int Harga { get; set; }
        public int Stok { get; set; }
    }
}