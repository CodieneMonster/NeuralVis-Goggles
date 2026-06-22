public class Layer
{
    public Matrix? input;
    public Matrix weights;
    public Matrix bias;
    public Matrix? z;
    public Matrix? a;

    public Layer(Matrix w, Matrix b)
    {
        weights = w;
        bias = b;
    }

    public Layer(int inputSize, int outputSize, Random rng)
    {
        weights = Matrix.Random(outputSize,inputSize, rng);
        bias = Matrix.Random(outputSize,1 , rng);
    }

    public Matrix forwardPass(Matrix input)
    {
        this.input = input;
        z = Matrix.mulMatrix(weights, input);
        z = Matrix.addMatrix(z, bias);
        a = Matrix.mapMatrix(z, Activation.sigmoid);
        return a;
    }

    public Matrix backProp(Matrix delta, double learningRate) 
    {
        if (input == null)
        {
            throw new InvalidOperationException("Must call forwardPass before backProp.");
        }
        Matrix weightGradient = Matrix.mulMatrix(delta, Matrix.transposeMatrix(input));
        Matrix biasGradient = delta;

        // compute returnedDelta using old weights before updating
        Matrix returnedDelta = Matrix.mulMatrix(Matrix.transposeMatrix(weights), delta);

        // Update weights and bias
        // learningRate X weightGradient
        Matrix scaledWeightGradient = Matrix.scalarMatrix(weightGradient, learningRate);
        // upadating weight = weight - learningRate X weightGradient
        weights = Matrix.subMatrix(weights, scaledWeightGradient);
        // learningRate X biasGradient
        Matrix scaledBiasGradient = Matrix.scalarMatrix(biasGradient, learningRate);
        // upadating bias = bias - learningRate X biasGradient
        bias = Matrix.subMatrix(bias, scaledBiasGradient);

        return returnedDelta;
    }

}