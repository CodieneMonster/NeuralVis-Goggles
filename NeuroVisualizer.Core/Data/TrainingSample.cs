public class TrainingSample
{
    public Matrix Input { get; }
    public Matrix Target { get; }

    public TrainingSample(Matrix input, Matrix target)
    {
        if (input == null || target == null)
        {
            throw new ArgumentException("Input and Target cannot be null");
        }

        Input = input;
        Target = target;
    }
}