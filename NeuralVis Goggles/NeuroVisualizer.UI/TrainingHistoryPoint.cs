public class TrainingHistoryPoint
{
    public int Epoch { get; set; }
    public double Loss { get; set; }
    public double Accuracy { get; set; }

    public TrainingHistoryPoint(int epoch, double loss, double accuracy)
    {
        Epoch = epoch;
        Loss = loss;
        Accuracy = accuracy;
    }
}