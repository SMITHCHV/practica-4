using Microsoft.ML;
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
        private ITransformer _model;
        private PredictionEngine<ProductRating, ProductPrediction> _predictionEngine;

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

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "UserId",
                MatrixRowIndexColumnName = "ProductId",
                LabelColumnName = "Label",
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                Alpha = 0.01,
                Lambda = 0.025,
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var estimator = _mlContext.Recommendation().Trainers.MatrixFactorization(options);
            _model = estimator.Fit(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductRating, ProductPrediction>(_model);
        }

        public List<(string ProductId, float Score)> Recommend(string userId, int topN = 5)
        {
            return _products
                .Select(p => (ProductId: p, Score: _predictionEngine.Predict(new ProductRating { UserId = userId, ProductId = p }).Score))
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .ToList();
        }
    }
}
