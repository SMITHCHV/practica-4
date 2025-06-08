using Microsoft.AspNetCore.Mvc;
using AnalizadorOpiniones.Services;

namespace AnalizadorOpiniones.Controllers
{
    public class RecomendacionController : Controller
    {
        private readonly ProductRecommendationService _service;

        public RecomendacionController()
        {
            _service = new ProductRecommendationService();
        }

        public IActionResult Productos()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Productos(string userId)
        {
            var recomendaciones = _service.Recommend(userId);
            ViewBag.UserId = userId;
            ViewBag.Recomendaciones = recomendaciones;
            return View();
        }
    }
}
