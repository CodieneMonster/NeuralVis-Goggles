public class PredictionRow
{
    public double Input1 { get; set; }
    public double Input2 { get; set; }
    public double Prediction { get; set; }
    public int PredictedLabel { get; set; }
    public int TargetLabel { get; set; }
    public bool Correct { get; set; }

    public PredictionRow(double input1, double input2, double prediction, int predictedLabel, int targetLabel)
    {
        Input1 = input1;
        Input2 = input2;
        Prediction = prediction;
        PredictedLabel = predictedLabel;
        TargetLabel = targetLabel;
        Correct = predictedLabel == targetLabel;
    }
}