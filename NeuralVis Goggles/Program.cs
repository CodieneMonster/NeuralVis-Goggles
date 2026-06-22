
public class Program
{



 public static void main() 
 {
    double[,] data = new double[,] { { 1, 2 }, { 3, 4 } };
    Random rng = new Random();
    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("XOR test for trainingsamples");
    List<TrainingSample> xorSamples = new List<TrainingSample>();
    NeuralNetwork networkXOR = new NeuralNetwork(2, 4, 4, 1, rng);
    // Matrix inputA = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });

    Matrix input00 = new Matrix(2, 1);
    input00.Data[0, 0] = 0;
    input00.Data[1, 0] = 0;
    Matrix target00 = new Matrix(1, 1);
    target00.Data[0, 0] = 0;

    Matrix input01 = new Matrix(2, 1);
    input01.Data[0, 0] = 0;
    input01.Data[1, 0] = 1;
    Matrix target01 = new Matrix(1, 1);
    target01.Data[0, 0] = 1;
    Matrix input02 = new Matrix(2, 1);
    input02.Data[0, 0] = 1;
    input02.Data[1, 0] = 0;
    Matrix target02 = new Matrix(1, 1);
    target02.Data[0, 0] = 1;
    Matrix input03 = new Matrix(2, 1);
    input03.Data[0, 0] = 1;
    input03.Data[1, 0] = 1;
    Matrix target03 = new Matrix(1, 1);
    target03.Data[0, 0] = 0;

    xorSamples.Add(new TrainingSample(input00, target00));
    xorSamples.Add(new TrainingSample(input01, target01));
    xorSamples.Add(new TrainingSample(input02, target02));
    xorSamples.Add(new TrainingSample(input03, target03));
    int epochs = 50000;
    double learningRate = 0.5;
    networkXOR.train(xorSamples, epochs, learningRate);
    networkXOR.test(xorSamples);
    networkXOR.saveWeights("xor_weights.txt");

    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("loading network xor_weights.txt");
    NeuralNetwork loadedNetwork = NeuralNetwork.loadFromFile("xor_weights.txt", rng);
    loadedNetwork.test(xorSamples);

    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("accuracy test");
    loadedNetwork.accuracyTest(xorSamples);
    Console.WriteLine(loadedNetwork.accuracyTest(xorSamples));

    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("prediction rows test");
    List<PredictionRow> rows = loadedNetwork.GetPredictionRows(xorSamples);
    Console.WriteLine(" " + NeuralNetwork.GetPredictionRowsAsString(rows));



    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("8x16 matrix training test");
    List<TrainingSample> bigSamples = new List<TrainingSample>();

    // SAMPLE 1
    Matrix input1 = new Matrix(8, 1);

    input1.Data[0, 0] = 1;
    input1.Data[1, 0] = 0;
    input1.Data[2, 0] = 0;
    input1.Data[3, 0] = 0;
    input1.Data[4, 0] = 1;
    input1.Data[5, 0] = 0;
    input1.Data[6, 0] = 1;
    input1.Data[7, 0] = 0;

    Matrix target1 = new Matrix(16, 1);

    target1.Data[0, 0] = 1;
    target1.Data[1, 0] = 0;
    target1.Data[2, 0] = 0;
    target1.Data[3, 0] = 0;
    target1.Data[4, 0] = 1;
    target1.Data[5, 0] = 0;
    target1.Data[6, 0] = 0;
    target1.Data[7, 0] = 1;
    target1.Data[8, 0] = 0;
    target1.Data[9, 0] = 0;
    target1.Data[10, 0] = 1;
    target1.Data[11, 0] = 0;
    target1.Data[12, 0] = 0;
    target1.Data[13, 0] = 1;
    target1.Data[14, 0] = 0;
    target1.Data[15, 0] = 1;

    Matrix input2 = new Matrix(8, 1);
    input2.Data[0, 0] = 0;
    input2.Data[1, 0] = 1;
    input2.Data[2, 0] = 1;
    input2.Data[3, 0] = 1;
    input2.Data[4, 0] = 0;
    input2.Data[5, 0] = 1;
    input2.Data[6, 0] = 0;
    input2.Data[7, 0] = 1;

    Matrix target2 = new Matrix(16, 1);
    target2.Data[0, 0] = 0;
    target2.Data[1, 0] = 1;
    target2.Data[2, 0] = 1;
    target2.Data[3, 0] = 1;
    target2.Data[4, 0] = 0;
    target2.Data[5, 0] = 1;
    target2.Data[6, 0] = 0;
    target2.Data[7, 0] = 1;
    target2.Data[8, 0] = 1;
    target2.Data[9, 0] = 1;
    target2.Data[10, 0] = 0;
    target2.Data[11, 0] = 1;
    target2.Data[12, 0] = 1;
    target2.Data[13, 0] = 1;
    target2.Data[14, 0] = 0;
    target2.Data[15, 0] = 1;

    bigSamples.Add(new TrainingSample(input1, target1));
    bigSamples.Add(new TrainingSample(input2, target2));

    NeuralNetwork bigNet = new NeuralNetwork(8, 16, 16, 16, rng);
    bigNet.train(bigSamples, 10000, 0.1);
    bigNet.saveWeights("big_weights.txt");

    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("loading network big_weights.txt");
    NeuralNetwork loadedBig = NeuralNetwork.loadFromFile("big_weights.txt", rng);
    loadedBig.test(bigSamples);


    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("accuracy test");
    loadedBig.accuracyTest(bigSamples);
    Console.WriteLine(loadedBig.accuracyTest(bigSamples));

    Console.WriteLine("---------------------------------------------");
    Console.WriteLine("prediction rows test");
    List<PredictionRow> rows1 = loadedBig.GetPredictionRows(bigSamples);
    Console.WriteLine(" " + NeuralNetwork.GetPredictionRowsAsString(rows1));
 }
}