using Microsoft.ML.Data;

namespace AnalizadorOpiniones.MLModels
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public bool Label { get; set; }

        [LoadColumn(1)]
        public string Text { get; set; } = string.Empty;
    }
}
