using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Media.Animation;

public class MainViewModel : INotifyPropertyChanged
{
    public string Title { get; set; } = "NeuroVisualizer";
    public ObservableCollection<TrainingHistoryPoint> LossHistory { get; set; }


    private double learningRate;
    public double LearningRate
    {
        get { return learningRate; }
        set
        {
            learningRate = value;
            OnPropertyChanged(nameof(LearningRate));
        }
    }

private int epochsToTrain;
    public int EpochsToTrain
    {
        get { return epochsToTrain; }
        set
        {
            epochsToTrain = value;
            OnPropertyChanged(nameof(EpochsToTrain));
        }
    }

    private int totalEpochsTrained;

    public int TotalEpochsTrained
    {
        get { return totalEpochsTrained; }
        set
        {
            totalEpochsTrained = value;
            OnPropertyChanged(nameof(TotalEpochsTrained));
        }
    }

    private int currentEpoch;

    public int CurrentEpoch
    {
        get
        {
            return currentEpoch;
        }

        set
        {
            currentEpoch = value;
            OnPropertyChanged(nameof(CurrentEpoch));
        }
    }

    private string trainingStatus;
    public string TrainingStatus
    {
        get
        {
            return trainingStatus;
        }
        set
        {
            trainingStatus = value;
            OnPropertyChanged(nameof(TrainingStatus));
        }
    }

    private double loss;
    public double Loss
    {
        get
        {
            return loss;
        }
        set
        {
            loss = value;
            OnPropertyChanged(nameof(Loss));
        }
    }

    private double accuracy;
    public double Accuracy
    {
        get
        {
            return accuracy;
        }

        set
        {
            accuracy = value;
            OnPropertyChanged(nameof(Accuracy));
        }
    }

    // MNist project additions
    private int currentMnistTargetDigit;
    public int CurrentMnistTargetDigit
    {
        get { return currentMnistTargetDigit; }
        set
        {
            currentMnistTargetDigit = value;
            OnPropertyChanged(nameof(CurrentMnistTargetDigit));
        }
    }

    private int currentMnistPredictedDigit;
    public int CurrentMnistPredictedDigit
    {
        get { return currentMnistPredictedDigit; }
        set
        {
            currentMnistPredictedDigit = value;
            OnPropertyChanged(nameof(CurrentMnistPredictedDigit));
        }
    }

    private Matrix? currentMnistInput;

    public Matrix? CurrentMnistInput
    {
        get { return currentMnistInput;  }
        set
        {
            currentMnistInput = value;
            OnPropertyChanged(nameof(CurrentMnistInput));
        }
    }

    private int currentMnistSampleIndex;

    public int CurrentMnistSampleIndex
    {
        get { return currentMnistSampleIndex; }
        set
        {
            currentMnistSampleIndex = value;
            OnPropertyChanged(nameof(CurrentMnistSampleIndex));
        }
    }

    private List<TrainingSample> mnistSamples;
    private NeuralNetwork mnistNetwork;


    public event PropertyChangedEventHandler? PropertyChanged;
    private Random rng;


    public MainViewModel()
    {
        TrainingStatus = "Ready";
        EpochsToTrain = 10000;
        LearningRate = 0.5;
        LossHistory = new ObservableCollection<TrainingHistoryPoint>();

        rng = new Random();
        mnistSamples = new List<TrainingSample>();
        LoadMnistPreview();

        // TestMnistLoader();
    }


    private void LoadMnistPreview()
    {
        string filePath = @"C:\Users\ekin\Documents\GitHub\NeuralVis-Goggles\Datasets\MNIST_CSV\mnist_train.csv";

        mnistSamples = MnistCsvLoader.LoadSamples(filePath, 100);

        mnistNetwork = new NeuralNetwork(784, 64, 32, 10, rng);

        CurrentMnistInput = mnistSamples[0].Input;

        int targetDigit = ArgMax(mnistSamples[0].Target);

        Matrix output = mnistNetwork.forwardPass(mnistSamples[0].Input);
        int predictedDigit = ArgMax(output);

        CurrentMnistTargetDigit = targetDigit;
        CurrentMnistPredictedDigit = predictedDigit;

        TrainingStatus = "MNIST samples loaded";
    }

    //private void TestMnistLoader()
    //{
    //    string filePath = @"C:\Users\ekin\Documents\GitHub\NeuralVis-Goggles\Datasets\MNIST_CSV\mnist_train.csv";

    //    mnistSamples = MnistCsvLoader.LoadSamples(filePath, 100);

    //    CurrentMnistInput = LoadMnistPreview();

    //    Debug.WriteLine(mnistSamples.Count);
    //    Debug.WriteLine(mnistSamples[0].Input.Rows);
    //    Debug.WriteLine(mnistSamples[0].Input.Cols);
    //    Debug.WriteLine(mnistSamples[0].Target.Rows);
    //    Debug.WriteLine(mnistSamples[0].Target.Cols);

    //    int targetDigit = ArgMax(mnistSamples[0].Target);
    //    Debug.WriteLine($"Target Digit: {targetDigit}");

    //    // TEMP MNIST NETWORK TEST
    //    mnistNetwork = new NeuralNetwork(784, 64, 32, 10, rng);

    //    Matrix output = mnistNetwork.forwardPass(mnistSamples[0].Input);

    //    Debug.WriteLine($"Output Shape: {output.Rows} x {output.Cols}");

    //    int predictedDigit = ArgMax(output);

    //    CurrentMnistTargetDigit = targetDigit;
    //    CurrentMnistPredictedDigit = predictedDigit;

    //    Debug.WriteLine($"Predicted Digit: {predictedDigit}");
    //    Debug.WriteLine($"Target Digit: {targetDigit}");
    //}


    // returns the best value  that is the closest match to target 
    public static int ArgMax(Matrix matrix)
    {
        if (matrix == null)
        {
            throw new ArgumentNullException(nameof(matrix));
        }

        if (matrix.Rows == 0 || matrix.Cols == 0)
        {
            throw new ArgumentException("Matrix cannot be empty.");
        }

        int bestIndex = 0;
        double bestValue = matrix.Data[0, 0];

        for (int i = 1; i < matrix.Rows; i++)
        {
            double currentValue = matrix.Data[i, 0];

            if (currentValue > bestValue)
            {
                bestValue = currentValue;
                bestIndex = i;
            }
        }

        return bestIndex;
    }


   private double CalculateMnistAccuracy(List<TrainingSample> samples)
    {
        if (mnistNetwork == null || samples == null || samples.Count == 0)
        {
            return 0;
        }

        int correct = 0;


        foreach(TrainingSample sample in samples)
        {
            Matrix output = mnistNetwork.forwardPass(sample.Input);

            int predictedDigit = ArgMax(output);
            int targetDigit = ArgMax(sample.Target);

            if (predictedDigit == targetDigit) 
            {
                correct++;
            }
        }

        return (double)correct / samples.Count;
    }

    public void TrainMnistOneEpoch()
    {
        if (mnistSamples == null || mnistSamples.Count == 0)
        {
            TrainingStatus = "MNIST samples not loaded";
            return;
        }

        if (mnistNetwork == null)
        {
            mnistNetwork = new NeuralNetwork(784, 64, 32, 10, rng);
        }

        TrainingStatus = "Training MNIST...";

        double averageLoss = mnistNetwork.trainOneEpoch(mnistSamples, LearningRate);

        Loss = averageLoss;
        Accuracy = CalculateMnistAccuracy(mnistSamples);

        TotalEpochsTrained += 1;
        CurrentEpoch = TotalEpochsTrained;

        ShowMnistSample(0);

        TrainingStatus = $"MNIST trained 1 epoch | Loss: {Loss:F6} | Accuracy: {Accuracy:P0}";
    }


    public async Task TrainMnist()
    {
        if (mnistSamples == null || mnistSamples.Count == 0)
        {
            LoadMnistPreview();
        }

        if (mnistSamples == null || mnistSamples.Count == 0)
        {
            TrainingStatus = "MNIST samples not loaded";
            return;
        }

        if (mnistNetwork == null)
        {
            mnistNetwork = new NeuralNetwork(784, 64, 32, 10, rng);
        }

        TrainingStatus = "Training MNIST...";

        int epochs = EpochsToTrain;

        if (epochs <= 0)
        {
            epochs = 1;
        }

        for (int epoch = 1; epoch <= epochs; epoch++)
        {
            var result = await Task.Run(() =>
            {
                double averageLoss = mnistNetwork.trainOneEpoch(mnistSamples, LearningRate);
                double currentAccuracy = CalculateMnistAccuracy(mnistSamples);

                return new
                {
                    AverageLoss = averageLoss,
                    Accuracy = currentAccuracy
                };
            });

            Loss = result.AverageLoss;
            Accuracy = result.Accuracy;

            TotalEpochsTrained += 1;
            CurrentEpoch = epoch;

            LossHistory.Add(
                new TrainingHistoryPoint(
                    TotalEpochsTrained,
                    Loss,
                    Accuracy
                )
            );

            if (CurrentMnistInput != null)
            {
                Matrix output = mnistNetwork.forwardPass(CurrentMnistInput);
                CurrentMnistPredictedDigit = ArgMax(output);
            }

            TrainingStatus = $"MNIST training... Epoch {epoch}/{epochs} | Loss: {Loss:F6} | Accuracy: {Accuracy:P0}";

            await Task.Delay(5);
        }

        TrainingStatus = $"MNIST finished | Epochs: {epochs} | Loss: {Loss:F6} | Accuracy: {Accuracy:P0}";
    }

    public void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
    }

    // Mnist

    public void ShowMnistSample(int index)
    {
        if (mnistSamples == null || mnistSamples.Count == 0)
        {
            return;
        }

        if (index < 0)
        {
            index = 0;
        }

        if (index >= mnistSamples.Count)
        {
            index = 0;
        }

        CurrentMnistSampleIndex = index;

        TrainingSample sample = mnistSamples[index];

        CurrentMnistInput = sample.Input;

        int targetDigit = ArgMax(sample.Target);

        int predictedDigit = -1;

        if (mnistNetwork != null)
        {
            Matrix output = mnistNetwork.forwardPass(sample.Input);
            predictedDigit = ArgMax(output);
        }

        CurrentMnistTargetDigit = targetDigit;
        CurrentMnistPredictedDigit = predictedDigit;
    }

}