using Microsoft.AspNetCore.Mvc;
using AnalizadorOpiniones.Services;

namespace AnalizadorOpiniones.Controllers
{
    public class AnalisisController : Controller
    {
        private readonly SentimentService _sentimentService;

        public AnalisisController(SentimentService sentimentService)
        {
            _sentimentService = sentimentService;
        }

        public IActionResult Sentimiento()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Sentimiento(string opinion)
        {
            var resultado = _sentimentService.Predict(opinion);
            ViewBag.Opinion = opinion;
            ViewBag.Resultado = resultado;
            return View();
        }
    }
}
