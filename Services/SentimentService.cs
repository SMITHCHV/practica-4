using Microsoft.ML;
using AnalizadorOpiniones.MLModels;
using System.IO;

namespace AnalizadorOpiniones.Services
{
    public class SentimentService
    {
        private readonly string _modelPath = Path.Combine("MLModels", "sentiment_model.zip");
        private readonly string _dataPath = Path.Combine("Data", "sentiment-data.tsv");

        private readonly MLContext _mlContext = new();
        private readonly PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;

        public SentimentService()
        {
            if (File.Exists(_modelPath))
            {
                var loadedModel = _mlContext.Model.Load(_modelPath, out _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(loadedModel);
            }
            else
            {
                _predictionEngine = TrainModel();
            }
        }

        private PredictionEngine<SentimentData, SentimentPrediction> TrainModel()
        {
            if (!File.Exists(_dataPath))
                throw new FileNotFoundException($"No se encontró el archivo de datos en: {_dataPath}");

            var dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: true);
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(dataView);
            _mlContext.Model.Save(model, dataView.Schema, _modelPath);

            return _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        }

        public SentimentPrediction Predict(string inputText)
        {
            return _predictionEngine.Predict(new SentimentData { Text = inputText });
        }
    }
}
