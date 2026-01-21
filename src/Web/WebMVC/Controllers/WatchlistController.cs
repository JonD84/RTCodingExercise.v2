using RTCodingExercise.Microservices.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace RTCodingExercise.Microservices.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly ILogger<WatchlistController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public WatchlistController(ILogger<WatchlistController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("CatalogApi");


            var viewModel = await client.GetFromJsonAsync<List<WatchlistItem>>("/api/Watchlist");
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            var client = _httpClientFactory.CreateClient("CatalogApi");
            var url = $"/api/Watchlist/{id}/remove";

            var response = await client.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to remove item from watchlist: {error}";
            }
            else
            {
                TempData["Success"] = "Item removed successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWatchPrice(Guid id, decimal price)
        {
            var client = _httpClientFactory.CreateClient("CatalogApi");
            var url = $"/api/Watchlist/{id}/update-price?price={price}";

            var response = await client.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to update item watch price: {error}";
            }
            else
            {
                TempData["Success"] = "Watch price updated successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}