using Microsoft.AspNetCore.Mvc;
using TokoBusanaAlex.Models;

namespace TokoBusanaAlex.Controllers
{
    public class ProdukController : Controller
    {
        private readonly ProdukRepository _repo;

        public ProdukController(ProdukRepository repo)
        {
            _repo = repo;
        }

        // Halaman Utama Produk: Menampilkan semua data (Read)
        public IActionResult Index()
        {
            var data = _repo.GetAll();
            return View(data);
        }

        // Halaman Form Tambah Produk (Tampilan Form)
        public IActionResult Create()
        {
            return View();
        }

        // Proses Simpan Data dari Form ke Database (Create)
        [HttpPost]
        public IActionResult Create(Produk produk)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(produk);
                return RedirectToAction("Index");
            }
            return View(produk);
        }

        // Halaman Form Edit Data
        public IActionResult Edit(int id)
        {
            var produk = _repo.GetById(id);
            if (produk == null) return NotFound();
            return View(produk);
        }

        // Proses Simpan Perubahan Data (Update)
        [HttpPost]
        public IActionResult Edit(Produk produk)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(produk);
                return RedirectToAction("Index");
            }
            return View(produk);
        }

        // Proses Hapus Data (Delete)
        public IActionResult Delete(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}