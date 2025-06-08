using Microsoft.ML.Data;

namespace AnalizadorOpiniones.MLModels
{
    public class ProductRating
    {
        [LoadColumn(0)]
        public string UserId { get; set; } = string.Empty;

        [LoadColumn(1)]
        public string ProductId { get; set; } = string.Empty;

        [LoadColumn(2)]
        public float Label { get; set; }
    }
}
