using Microsoft.AspNetCore.Mvc;
using AnalizadorOpiniones.Services;

namespace AnalizadorOpiniones.Controllers
{
    public class RecomendacionController : Controller
    {
        private readonly ProductRecommendationService _service;

        public RecomendacionController(ProductRecommendationService service)
        {
            _service = service;
        }

        public IActionResult Productos()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Productos(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                ViewBag.Error = "Por favor, ingresa un UserId válido.";
                return View();
            }

            var recomendaciones = _service.Recommend(userId);
            ViewBag.UserId = userId;
            ViewBag.Recomendaciones = recomendaciones;
            return View();
        }
    }
}
