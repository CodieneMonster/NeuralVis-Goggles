using System.Text;
using System.IO;
using System.Linq;
public class NeuralNetwork
{
    Layer hiddenLayer1;
    Layer hiddenLayer2;
    Layer outputLayer;
    private int inputSize;
    private int hidden1Size;
    private int hidden2Size;
    private int outputSize;


    public NeuralNetwork(int inputSize, int hidden1Size, int hidden2Size, int outputSize, Random rng)
    {
        hiddenLayer1 = new Layer(inputSize, hidden1Size, rng);
        hiddenLayer2 = new Layer(hidden1Size, hidden2Size, rng);
        outputLayer = new Layer(hidden2Size, outputSize, rng);
        this.inputSize = inputSize;
        this.hidden1Size = hidden1Size;
        this.hidden2Size = hidden2Size;
        this.outputSize = outputSize;
    }

    // TODO: Change architecture later 
    public NeuralNetwork(Layer l1, Layer l2, Layer output)
    {
        hiddenLayer1 = l1;
        hiddenLayer2 = l2;
        outputLayer = output;
    }

    public Matrix forwardPass(Matrix input)
    {
        Matrix a1 = hiddenLayer1.forwardPass(input);
        Matrix a2 = hiddenLayer2.forwardPass(a1);
        Matrix a3 = outputLayer.forwardPass(a2);

        return a3;
    }

    public void backwardPass(Matrix target, double learningRate)
    {
        //// forward pass to get activations and z values
        //Matrix a1 = hiddenLayer1.forwardPass(input);
        //Matrix a2 = hiddenLayer2.forwardPass(a1);
        //Matrix a3 = outputLayer.forwardPass(a2);

        // get prediction
        Matrix prediction = outputLayer.a;


        // compute loss derivative
        Matrix lossDerivative = LossFunctions.mseDerivative(prediction, target);
        Matrix sigmoidDerivative = Activation.sigmoidDerivativeFromOutputMatrix(prediction);

        // compute output layer delta
        Matrix outputDelta = Matrix.elementWiseMulMatrix(lossDerivative, sigmoidDerivative);

        // backpropagate through output layer
        Matrix errorForHidden2 = outputLayer.backProp(outputDelta, learningRate);

        // compute hidden layer 2 sigmoid
        Matrix hidden2SigmoidDerivative = Activation.sigmoidDerivativeFromOutputMatrix(hiddenLayer2.a);

        // compute hidden2 delta
        Matrix hidden2Delta = Matrix.elementWiseMulMatrix(errorForHidden2, hidden2SigmoidDerivative);

        // backpropagate through hidden layer 2
        Matrix errorForHidden1 = hiddenLayer2.backProp(hidden2Delta, learningRate);

        // compute hidden layer 1 delta
        Matrix hidden1SigmoidDerivative = Activation.sigmoidDerivativeFromOutputMatrix(hiddenLayer1.a);

        // compute hidden1 delta
        Matrix hidden1Delta = Matrix.elementWiseMulMatrix(errorForHidden1, hidden1SigmoidDerivative);

        // backpropagate through hidden layer 1
        hiddenLayer1.backProp(hidden1Delta, learningRate);

        return;
    }


    public void train(List<TrainingSample> samples, int epochs, double learningRate)
    {
        //for (int i = 0; i < epochs; i++)
        //{
        //    double totalLoss = 0;
        //    foreach (TrainingSample sample in samples)
        //    {
        //        Matrix prediction = forwardPass(sample.Input);
        //        totalLoss += LossFunctions.MeanSquaredError(prediction, sample.Target);

        //        backwardPass(sample.Target, learningRate);
        //    }
        //    if ((i + 1) % 1000 == 0)
        //    {
        //        Console.WriteLine($"Epoch {i + 1}/{epochs}, Loss: {totalLoss / samples.Count}");
        //    }
        //}
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            double averageLoss = trainOneEpoch(samples, learningRate);

            if (epoch % 1000 == 0)
            {
                Console.WriteLine($"Epoch {epoch}, Loss: {averageLoss}");
            }
        }
    }



    public void test(List<TrainingSample> samples)
    {
        foreach (TrainingSample sample in samples)
        {
            Matrix prediction = forwardPass(sample.Input);

            Console.WriteLine("Input:");
            sample.Input.Print();

            Console.WriteLine("Prediction:");
            prediction.Print();

            Console.WriteLine("Target:");
            sample.Target.Print();

            Console.WriteLine("---------------------------------------------");
        }
    }

    public void saveWeights(string filePath)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Architecture:");
        sb.AppendLine($"InputSize: {inputSize}");
        sb.AppendLine($"Hidden1Size: {hidden1Size}");
        sb.AppendLine($"Hidden2Size: {hidden2Size}");
        sb.AppendLine($"OutputSize: {outputSize}");
        sb.AppendLine();

        sb.AppendLine("Hidden Layer 1 Weights:");
        sb.AppendLine(Matrix.matrixToString(hiddenLayer1.weights));

        sb.AppendLine("Hidden Layer 1 Biases:");
        sb.AppendLine(Matrix.matrixToString(hiddenLayer1.bias));

        sb.AppendLine("Hidden Layer 2 Weights:");
        sb.AppendLine(Matrix.matrixToString(hiddenLayer2.weights));

        sb.AppendLine("Hidden Layer 2 Biases:");
        sb.AppendLine(Matrix.matrixToString(hiddenLayer2.bias));

        sb.AppendLine("Output Layer Weights:");
        sb.AppendLine(Matrix.matrixToString(outputLayer.weights));

        sb.AppendLine("Output Layer Biases:");
        sb.AppendLine(Matrix.matrixToString(outputLayer.bias));

        File.WriteAllText(filePath, sb.ToString());

    }

    // Method loadweights gets called in loadFromFile after reading architecture values to populate the weights and biases of the network
    public void loadWeights(string filePath)
    {
        string currentsection = "";
        string[] lines = File.ReadAllLines(filePath);
        Dictionary<string, StringBuilder> sections = new Dictionary<string, StringBuilder>();
        foreach (string line in lines)
        {
            if (line.StartsWith("Hidden"))
            {
                currentsection = line;
                sections[currentsection] = new StringBuilder();
                continue;
            }
            if (line.StartsWith("output") || line.StartsWith("Output"))
            {
                currentsection = line;
                sections[currentsection] = new StringBuilder();
                continue;
            }
            if (line.Trim() == "" )
            {
                continue;
            }

            if (currentsection != "")
            {
                sections[currentsection].AppendLine(line);
            }
        }

        hiddenLayer1.weights = Matrix.stringToMatrix(sections["Hidden Layer 1 Weights:"].ToString());
        hiddenLayer1.bias = Matrix.stringToMatrix(sections["Hidden Layer 1 Biases:"].ToString());

        hiddenLayer2.weights = Matrix.stringToMatrix(sections["Hidden Layer 2 Weights:"].ToString());
        hiddenLayer2.bias = Matrix.stringToMatrix(sections["Hidden Layer 2 Biases:"].ToString());

        outputLayer.weights = Matrix.stringToMatrix(sections["Output Layer Weights:"].ToString());
        outputLayer.bias = Matrix.stringToMatrix(sections["Output Layer Biases:"].ToString());
    }


    public static int readArchitectureValue(string[] lines, string key)
    {
        foreach (string line in lines)
        {
            if (line.StartsWith(key))
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int value))
                {
                    return value;
                }
            }
        }
        throw new ArgumentException($"Key '{key}' not found in architecture section.");

    }

    public static NeuralNetwork loadFromFile(string filePath, Random rng)
    {
        string[] line = File.ReadAllLines(filePath);
        int inputSize = readArchitectureValue(line, "InputSize:");
        int hidden1Size = readArchitectureValue(line, "Hidden1Size:");
        int hidden2Size =readArchitectureValue(line, "Hidden2Size:");
        int outputSize = readArchitectureValue(line, "OutputSize:");
        NeuralNetwork net = new NeuralNetwork(inputSize, hidden1Size, hidden2Size, outputSize, rng);
        net.loadWeights(filePath);

        return net;
    }

    // To test Accuracy of all correct answers vs the amount of questions
    public double accuracyTest(List<TrainingSample> samples)
    {
        double correct = 0;
        foreach (TrainingSample sample in samples)
        {
            Matrix prediction = forwardPass(sample.Input);
            // convert prediction and target to binary labels using 0.5 as threshold
            int predictedLabel = prediction.Data[0, 0] >= 0.5 ? 1 : 0;
            // compare against target
            int actualLabel = sample.Target.Data[0, 0] >= 0.5 ? 1 : 0;
            // if they match incrrease accuracy 
            if (predictedLabel == actualLabel)
            {
                correct += 1;
            }
        }
        double accuracy = correct / samples.Count;
        return accuracy;
    }

    public List<PredictionRow> GetPredictionRows(List<TrainingSample> samples) 
    {
        List<PredictionRow> predictionRows = new List<PredictionRow>();
        foreach (TrainingSample sample in samples) 
        {
            Matrix prediction = forwardPass(sample.Input);
            double input1 = sample.Input.Data[0, 0];
            double input2 = sample.Input.Data[1, 0];
            double rawPrediction = prediction.Data[0, 0];
            
            int predictedLabel = rawPrediction >= 0.5 ? 1 : 0;
            int targetLabel = sample.Target.Data[0, 0] >= 0.5 ? 1 : 0;
            predictionRows.Add(new PredictionRow(input1, input2, rawPrediction, predictedLabel, targetLabel));
        }
        return predictionRows;
    }

    // helper method to print each rows predictiosn
    public static String GetPredictionRowsAsString(List<PredictionRow> rows)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Input1\tInput2\tRaw Prediction\tPredicted Label\tTarget Label");
        foreach (PredictionRow row in rows)
        {
            sb.AppendLine($"{row.Input1}\t{row.Input2}\t{row.Prediction}\t{row.PredictedLabel}\t{row.TargetLabel}");
        }
        return sb.ToString();
    }

    public double trainOneEpoch(List<TrainingSample> samples, double learningRate)
    {
        double totalLoss = 0;

        foreach (TrainingSample sample in samples)
        {
            Matrix prediction = forwardPass(sample.Input);

            double sampleLoss = LossFunctions.MeanSquaredError(prediction, sample.Target);
            totalLoss += sampleLoss;

            backwardPass(sample.Target, learningRate);
        }

        double averageLoss = totalLoss / samples.Count;
        return averageLoss;
    }

    public ActivationSnapshot GetActivationSnapshot(Matrix input)
    {
        Matrix a1 = hiddenLayer1.forwardPass(input);
        Matrix a2 = hiddenLayer2.forwardPass(a1);
        Matrix output = outputLayer.forwardPass(a2);
        return new ActivationSnapshot(
            input.Data.Cast<double>().ToArray(),
            a1.Data.Cast<double>().ToArray(),
            a2.Data.Cast<double>().ToArray(),
            output.Data.Cast<double>().ToArray()
        );
    }
}