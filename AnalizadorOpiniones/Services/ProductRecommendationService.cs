using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using AnalizadorOpiniones.MLModels;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AnalizadorOpiniones.Services
{
    public class ProductRecommendationService
    {
        private readonly string _dataPath = Path.Combine("Data", "ratings-data.csv");
        private readonly MLContext _mlContext;
        private ITransformer _model = default!;
        private PredictionEngine<ProductRatingEncoded, ProductPrediction> _predictionEngine = default!;
        private List<string> _products = new();

        public ProductRecommendationService()
        {
            _mlContext = new MLContext();
            TrainModel();
        }

        private void TrainModel()
        {
            if (!File.Exists(_dataPath)) return;

            var dataView = _mlContext.Data.LoadFromTextFile<ProductRating>(_dataPath, hasHeader: true, separatorChar: ',');

            _products = dataView.GetColumn<string>("ProductId").Distinct().ToList();

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("UserIdEncoded", "UserId")
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("ProductIdEncoded", "ProductId"))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "UserIdEncoded",
                    MatrixRowIndexColumnName = "ProductIdEncoded",
                    LabelColumnName = "Label",
                    LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                    Alpha = 0.01,
                    Lambda = 0.025,
                    NumberOfIterations = 20,
                    ApproximationRank = 100
                }));

            _model = pipeline.Fit(dataView);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductRatingEncoded, ProductPrediction>(_model);
        }

        public List<(string ProductId, float Score)> Recommend(string userId, int topN = 5)
        {
            return _products
                .Select(p => new ProductRating { UserId = userId, ProductId = p })
                .Select(input => (
                    ProductId: input.ProductId,
                    Score: _predictionEngine.Predict(new ProductRatingEncoded
                    {
                        UserId = input.UserId,
                        ProductId = input.ProductId
                    }).Score))
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .ToList();
        }
    }

    public class ProductRating
    {
        [LoadColumn(0)]
        public string UserId { get; set; } = string.Empty;

        [LoadColumn(1)]
        public string ProductId { get; set; } = string.Empty;

        [LoadColumn(2)]
        public float Label { get; set; }
    }

    public class ProductRatingEncoded
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public float Label { get; set; }
    }

    public class ProductPrediction
    {
        public float Score { get; set; }
    }
}
