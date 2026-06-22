using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;

public class MainViewModel : INotifyPropertyChanged
{
    public string Title { get; set; } = "NeuroVisualizer";
    public List<TrainingSample> Samples { get; set; }
    public ObservableCollection<PredictionRow> PredictionRows { get; set; }
    public ObservableCollection<TrainingHistoryPoint> LossHistory { get; set; }



    private ActivationSnapshot? currentActivationSnapshot;

    public ActivationSnapshot? CurrentActivationSnapshot
    {
        get { return currentActivationSnapshot; }
        set
        {
            currentActivationSnapshot = value;
            OnPropertyChanged(nameof(CurrentActivationSnapshot));
        }
    }

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

    private int selectedSampleIndex;

    public int SelectedSampleIndex
    {
        get { return selectedSampleIndex; }
        set
        {
            selectedSampleIndex = value;
            OnPropertyChanged(nameof(SelectedSampleIndex));
            RefreshActivationSnapshot();
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
    public event PropertyChangedEventHandler? PropertyChanged;
    private Random rng;


    // viewmodel needs to hold current model
    // to be able to refresh predictions when model is updated
    private NeuralNetwork network;

    public MainViewModel()
    {
        TrainingStatus = "Ready";
        EpochsToTrain = 10000;
        LearningRate = 0.5;
        LossHistory = new ObservableCollection<TrainingHistoryPoint>();

        rng = new Random();
        Samples = new List<TrainingSample>();
        PredictionRows = new ObservableCollection<PredictionRow>();
        CreateXORSamples();
        network = new NeuralNetwork(2, 4, 4, 1, rng);
        RefreshPredictions();
        RefreshActivationSnapshot();
}

    public void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
    }    

public void CreateXORSamples() 
    {
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
        Samples.Add(new TrainingSample(input00, target00));
        Samples.Add(new TrainingSample(input01, target01));
        Samples.Add(new TrainingSample(input02, target02));
        Samples.Add(new TrainingSample(input03, target03));
    }

    //public void TrainXor()
    //{
    //    CurrentEpoch = 0;
    //    TrainingStatus = "Training...";

    //    if (Samples == null)
    //    {
    //        Samples = new List<TrainingSample>();
    //        CreateXORSamples();
    //    }

    //    if (network == null)
    //    {
    //        network = new NeuralNetwork(2, 4, 4, 1, rng);
    //    }

    //    // LossHistory.Clear();

    //    int recordEvery = 100;
    //    int startingEpoch = TotalEpochsTrained;
    //    for (int epoch = 1; epoch <= EpochsToTrain; epoch++)
    //    {
    //        double averageLoss = network.trainOneEpoch(Samples, LearningRate);

    //        CurrentEpoch = epoch;
    //        TotalEpochsTrained += 1;
    //        int globalEpoch = startingEpoch + epoch;
    //        if (epoch % recordEvery == 0 || epoch == EpochsToTrain)
    //        {
    //            double currentAccuracy = network.accuracyTest(Samples);

    //            LossHistory.Add(
    //                new TrainingHistoryPoint(
    //                    globalEpoch,
    //                    averageLoss,
    //                    currentAccuracy
    //                )
    //            );
    //        }
    //    }

    //    RefreshPredictions();

    //    TrainingStatus = $"Finished | Epochs: {EpochsToTrain} | LR: {LearningRate}";
    //}

    public async Task TrainXor()
    {
        CurrentEpoch = 0;
        TrainingStatus = "Training...";

        if (Samples == null)
        {
            Samples = new List<TrainingSample>();
            CreateXORSamples();
        }

        if (network == null)
        {
            network = new NeuralNetwork(2, 4, 4, 1, rng);
        }

        int recordEvery = 1;
        int startingEpoch = TotalEpochsTrained;

        for (int epoch = 1; epoch <= EpochsToTrain; epoch++)
        {
            double averageLoss = network.trainOneEpoch(Samples, LearningRate);

            CurrentEpoch = epoch;
            TotalEpochsTrained += 1;

            int globalEpoch = startingEpoch + epoch;

            if (epoch % recordEvery == 0 || epoch == EpochsToTrain)
            {
                double currentAccuracy = network.accuracyTest(Samples);

                Loss = averageLoss;
                Accuracy = currentAccuracy;

                LossHistory.Add(
                    new TrainingHistoryPoint(
                        globalEpoch,
                        averageLoss,
                        currentAccuracy
                    )
                );

                RefreshPredictions();
                RefreshActivationSnapshot();
                await Task.Yield();
                await Task.Delay(150);
            }
        }

        RefreshPredictions();
        RefreshActivationSnapshot();

    TrainingStatus = $"Finished | Epochs: {EpochsToTrain} | LR: {LearningRate}";
    }

    public void LoadXorModel()
    {
        CurrentEpoch = 0;
        TotalEpochsTrained = 0;
        LossHistory.Clear();

        string filePath = "xor_weights.txt";
        if (Samples == null)
        {
            CreateXORSamples();
        }

        network = NeuralNetwork.loadFromFile(filePath, rng);
        RefreshPredictions();
        RefreshActivationSnapshot();
        TrainingStatus = "Loaded model";
    }

    public void RefreshPredictions() 
    {
        double totalLoss = 0;
        if (network == null || Samples == null)
        {
            return;
        }
        PredictionRows.Clear();

        foreach (PredictionRow row in network.GetPredictionRows(Samples))
        {
            PredictionRows.Add(row);
        }

        foreach (TrainingSample sample in Samples)
        {
            Matrix prediction = network.forwardPass(sample.Input);
            totalLoss += LossFunctions.MeanSquaredError(prediction, sample.Target);
        }
        Loss = totalLoss / Samples.Count;

        Accuracy = network.accuracyTest(Samples);
    }

    public void NewModel()
    {
        currentEpoch = 0;
        TotalEpochsTrained = 0;
        LossHistory.Clear();

        network = new NeuralNetwork(2, 4, 4, 1, rng);
        RefreshPredictions();
        RefreshActivationSnapshot();
        TrainingStatus = "New Random Model";
    }


    public void RefreshActivationSnapshot()
    {
        if (network == null || Samples == null || Samples.Count == 0)
        {
            return;
        }

        if (SelectedSampleIndex < 0)
        {
            SelectedSampleIndex = 0;
        }

        if (SelectedSampleIndex >= Samples.Count)
        {
            SelectedSampleIndex = Samples.Count - 1;
        }

        TrainingSample selectedSample = Samples[SelectedSampleIndex];

        CurrentActivationSnapshot = network.GetActivationSnapshot(selectedSample.Input);
    }
}