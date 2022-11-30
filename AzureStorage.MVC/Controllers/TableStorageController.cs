using AzureStorage.Library.Models;
using AzureStorage.Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorage.MVC.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly ITableStorageService<Product> _tableStorageService;
        public TableStorageController(ITableStorageService<Product> tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        public async Task<IActionResult> Index()
        {
			ViewBag.IsUpdate = false;

			ViewBag.products = await _tableStorageService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
			// Bu iki değeri biz veriyoruz
			product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Category1";

            await _tableStorageService.AddAsync(product);
			
            // eğer kayıt yoksa ve update işlemi isteniyorsa rowkey ve partitionkey'lerinin aynı olması lazım.
            //await _tableStorageService.UpsertAsync(product);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.FindAsync(partitionKey, rowKey);

            ViewBag.products = await _tableStorageService.GetAllAsync();
			ViewBag.IsUpdate = true;

            return View("Index", product);
		}

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
			await _tableStorageService.UpdateAsync(product, product.ETag);
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tableStorageService.DeleteAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Query(int price)
        {
            ViewBag.IsUpdate = false;

            ViewBag.products = await _tableStorageService.GetAsync(x => x.Price == price);
            return View("Index");
        }
    }
}
