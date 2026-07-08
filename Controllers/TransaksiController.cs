using Microsoft.AspNetCore.Mvc;
using TokoBusanaAlex.Models;

namespace TokoBusanaAlex.Controllers
{
    public class TransaksiController : Controller
    {
        private readonly ProdukRepository _repo;

        public TransaksiController(ProdukRepository repo)
        {
            _repo = repo;
        }

        // Tampilan Utama Transaksi (Input Penjualan)
        public IActionResult Index()
        {
            ViewBag.ProdukList = _repo.GetAll(); // Kirim daftar produk ke dropdown select
            return View();
        }

        // Proses checkout transaksi
        [HttpPost]
        public IActionResult Bayar(Transaksi t)
        {
            var produk = _repo.GetById(t.IdProduk);
            if (produk != null && produk.Stok >= t.JumlahBeli)
            {
                _repo.AddTransaksi(t);
                return RedirectToAction("Laporan");
            }
            
            // Jika stok tidak cukup
            ModelState.AddModelError("", "Stok baju tidak mencukupi untuk transaksi ini!");
            ViewBag.ProdukList = _repo.GetAll();
            return View("Index", t);
        }

        // Halaman Riwayat Transaksi & Cetak Laporan
        public IActionResult Laporan()
        {
            var data = _repo.GetAllTransaksi();
            return View(data);
        }
    }
}