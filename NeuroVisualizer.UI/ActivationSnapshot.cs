public class ActivationSnapshot
{
    public double[] InputValues { get; set; }
    public double[] Hidden1Values { get; set; }
    public double[] Hidden2Values { get; set; }
    public double[] OutputValues { get; set; }

    public ActivationSnapshot(
        double[] inputValues,
        double[] hidden1Values,
        double[] hidden2Values,
        double[] outputValues)
    {
        InputValues = inputValues;
        Hidden1Values = hidden1Values;
        Hidden2Values = hidden2Values;
        OutputValues = outputValues;
    }
}