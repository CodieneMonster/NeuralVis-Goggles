using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Linq;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
namespace NeuralVisualizer1;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        MainViewModel vm = (MainViewModel)DataContext;

        vm.LossHistory.CollectionChanged += LossHistory_CollectionChanged;
        vm.PropertyChanged += Vm_PropertyChanged;

        UpdateActivationTextValues();
        DrawNetworkGraph();
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrentActivationSnapshot))
        {
            UpdateActivationTextValues();
            DrawNetworkGraph();
        }
    }

    private void LossHistory_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        DrawLossGraph();
    }

    private void LossGraphCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawLossGraph();
    }

    public void LoadModel_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;

        vm.LoadXorModel();
    }

    public async void Train_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        await vm.TrainXor();
        await Task.Delay(1);
    }

    public void NewModel_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;
        vm.NewModel();
    }
    // Loss  Graph
    private void DrawLossGraph()
    {
        MainViewModel vm = (MainViewModel)DataContext;

        LossGraphCanvas.Children.Clear();

        if (vm.LossHistory == null || vm.LossHistory.Count < 2)
        {
            return;
        }

        double width = LossGraphCanvas.ActualWidth;
        double height = LossGraphCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        double leftPadding = 70;
        double rightPadding = 25;
        double topPadding = 30;
        double bottomPadding = 45;

        double chartLeft = leftPadding;
        double chartRight = width - rightPadding;
        double chartTop = topPadding;
        double chartBottom = height - bottomPadding;

        double chartWidth = chartRight - chartLeft;
        double chartHeight = chartBottom - chartTop;

        int minEpoch = 0;
        int maxEpoch = vm.LossHistory.Max(p => p.Epoch);

        double minLoss = 0;
        double maxLoss = vm.LossHistory.Max(p => p.Loss);

        if (maxEpoch <= minEpoch)
        {
            return;
        }

        if (maxLoss <= 0)
        {
            maxLoss = 0.000001;
        }

        DrawGridLines(chartLeft, chartRight, chartTop, chartBottom, minEpoch, maxEpoch, minLoss, maxLoss);

        DrawLossLine(vm, chartLeft, chartBottom, chartWidth, chartHeight, minEpoch, maxEpoch, minLoss, maxLoss);

        DrawLegend(width);

        DrawLatestTag(vm);
    }

    private void DrawGridLines(double chartLeft, double chartRight,
                               double chartTop, double chartBottom,
                               int minEpoch, int maxEpoch,
                               double minLoss, double maxLoss)
    {
        int gridCount = 5;

        for (int i = 0; i <= gridCount; i++)
        {
            double percent = i / (double)gridCount;

            double y = chartTop + percent * (chartBottom - chartTop);

            Line horizontalLine = new Line
            {
                X1 = chartLeft,
                Y1 = y,
                X2 = chartRight,
                Y2 = y,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1
            };

            LossGraphCanvas.Children.Add(horizontalLine);

            double lossValue = maxLoss - percent * (maxLoss - minLoss);

            TextBlock yLabel = new TextBlock
            {
                Text = lossValue.ToString("F6"),
                FontSize = 11,
                Foreground = Brushes.Gray
            };

            Canvas.SetLeft(yLabel, 5);
            Canvas.SetTop(yLabel, y - 8);
            LossGraphCanvas.Children.Add(yLabel);
        }

        for (int i = 0; i <= gridCount; i++)
        {
            double percent = i / (double)gridCount;

            double x = chartLeft + percent * (chartRight - chartLeft);

            Line verticalLine = new Line
            {
                X1 = x,
                Y1 = chartTop,
                X2 = x,
                Y2 = chartBottom,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1
            };

            LossGraphCanvas.Children.Add(verticalLine);

            int epochValue = (int)(minEpoch + percent * (maxEpoch - minEpoch));

            TextBlock xLabel = new TextBlock
            {
                Text = epochValue.ToString(),
                FontSize = 11,
                Foreground = Brushes.Gray
            };

            Canvas.SetLeft(xLabel, x - 15);
            Canvas.SetTop(xLabel, chartBottom + 8);
            LossGraphCanvas.Children.Add(xLabel);
        }

        Line yAxis = new Line
        {
            X1 = chartLeft,
            Y1 = chartTop,
            X2 = chartLeft,
            Y2 = chartBottom,
            Stroke = Brushes.BlueViolet,
            StrokeThickness = 1.5
        };

        Line xAxis = new Line
        {
            X1 = chartLeft,
            Y1 = chartBottom,
            X2 = chartRight,
            Y2 = chartBottom,
            Stroke = Brushes.BlueViolet,
            StrokeThickness = 1.5
        };

        LossGraphCanvas.Children.Add(yAxis);
        LossGraphCanvas.Children.Add(xAxis);

        TextBlock yAxisTitle = new TextBlock
        {
            Text = "Loss",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White
        };

        Canvas.SetLeft(yAxisTitle, 5);
        Canvas.SetTop(yAxisTitle, chartTop - 25);
        LossGraphCanvas.Children.Add(yAxisTitle);

        TextBlock xAxisTitle = new TextBlock
        {
            Text = "Epoch",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White
        };

        Canvas.SetLeft(xAxisTitle, chartRight - 45);
        Canvas.SetTop(xAxisTitle, chartBottom + 25);
        LossGraphCanvas.Children.Add(xAxisTitle);
    }

    private void DrawLossLine(MainViewModel vm, double chartLeft, double chartBottom,
                              double chartWidth, double chartHeight, int minEpoch,
                              int maxEpoch, double minLoss, double maxLoss)
    {
        Polyline lossLine = new Polyline
        {
            Stroke = Brushes.LightSeaGreen,
            StrokeThickness = 2.5
        };

        foreach (TrainingHistoryPoint point in vm.LossHistory)
        {
            double xPercent = (point.Epoch - minEpoch) / (double)(maxEpoch - minEpoch);
            double yPercent = (point.Loss - minLoss) / (maxLoss - minLoss);

            double x = chartLeft + xPercent * chartWidth;
            double y = chartBottom - yPercent * chartHeight;

            lossLine.Points.Add(new Point(x, y));
        }

        LossGraphCanvas.Children.Add(lossLine);

        TrainingHistoryPoint latest = vm.LossHistory[vm.LossHistory.Count - 1];

        double latestXPercent = (latest.Epoch - minEpoch) / (double)(maxEpoch - minEpoch);
        double latestYPercent = (latest.Loss - minLoss) / (maxLoss - minLoss);

        double latestX = chartLeft + latestXPercent * chartWidth;
        double latestY = chartBottom - latestYPercent * chartHeight;

        Ellipse latestPoint = new Ellipse
        {
            Width = 9,
            Height = 9,
            Fill = Brushes.OrangeRed,
            Stroke = Brushes.White,
            StrokeThickness = 1
        };

        Canvas.SetLeft(latestPoint, latestX - 4.5);
        Canvas.SetTop(latestPoint, latestY - 4.5);

        LossGraphCanvas.Children.Add(latestPoint);
    }


    private void DrawLegend(double width)
    {
        double legendX = width - 130;
        double legendY = 10;

        Line legendLine = new Line
        {
            X1 = legendX,
            Y1 = legendY + 8,
            X2 = legendX + 25,
            Y2 = legendY + 8,
            Stroke = Brushes.GhostWhite,
            StrokeThickness = 3
        };

        TextBlock legendText = new TextBlock
        {
            Text = "Loss",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.GhostWhite
        };

        Canvas.SetLeft(legendText, legendX + 35);
        Canvas.SetTop(legendText, legendY);

        LossGraphCanvas.Children.Add(legendLine);
        LossGraphCanvas.Children.Add(legendText);
    }

    private void DrawLatestTag(MainViewModel vm)
    {
        TrainingHistoryPoint latest = vm.LossHistory[vm.LossHistory.Count - 1];

        TextBlock latestText = new TextBlock
        {
            Text = $"Latest: Epoch {latest.Epoch} | Loss {latest.Loss:F6}",
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.AntiqueWhite
        };

        Canvas.SetLeft(latestText, 80);
        Canvas.SetTop(latestText, 8);

        LossGraphCanvas.Children.Add(latestText);
    }


    // Live Network Graph 
    private void DrawNetworkGraph()
    {
        MainViewModel vm = (MainViewModel)DataContext;

        NetworkGraphCanvas.Children.Clear();

        ActivationSnapshot? snapshot = vm.CurrentActivationSnapshot;

        if (snapshot == null)
        {
            return;
        }

        double width = NetworkGraphCanvas.ActualWidth;
        double height = NetworkGraphCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        double leftPadding = 90;
        double rightPadding = 90;
        double topPadding = 60;
        double bottomPadding = 40;

        double graphTop = topPadding;
        double graphBottom = height - bottomPadding;

        double usableWidth = width - leftPadding - rightPadding;

        double inputX = leftPadding;
        double hidden1X = leftPadding + usableWidth / 3.0;
        double hidden2X = leftPadding + usableWidth * 2.0 / 3.0;
        double outputX = width - rightPadding;

        DrawConnections(snapshot.InputValues, snapshot.Hidden1Values, inputX, hidden1X, graphTop, graphBottom);
        DrawConnections(snapshot.Hidden1Values, snapshot.Hidden2Values, hidden1X, hidden2X, graphTop, graphBottom);
        DrawConnections(snapshot.Hidden2Values, snapshot.OutputValues, hidden2X, outputX, graphTop, graphBottom);

        DrawLayer("Input", snapshot.InputValues, inputX, graphTop, graphBottom);
        DrawLayer("Hidden 1", snapshot.Hidden1Values, hidden1X, graphTop, graphBottom);
        DrawLayer("Hidden 2", snapshot.Hidden2Values, hidden2X, graphTop, graphBottom);
        DrawLayer("Output", snapshot.OutputValues, outputX, graphTop, graphBottom);
    }

    private void UpdateActivationTextValues()
    {
        MainViewModel vm = (MainViewModel)DataContext;

        ActivationSnapshot? snapshot = vm.CurrentActivationSnapshot;

        if (snapshot == null)
        {
            return;
        }

        Input1Value.Text = snapshot.InputValues[0].ToString("F4");
        Input2Value.Text = snapshot.InputValues[1].ToString("F4");

        Hidden1_1Value.Text = snapshot.Hidden1Values[0].ToString("F4");
        Hidden1_2Value.Text = snapshot.Hidden1Values[1].ToString("F4");
        Hidden1_3Value.Text = snapshot.Hidden1Values[2].ToString("F4");
        Hidden1_4Value.Text = snapshot.Hidden1Values[3].ToString("F4");

        Hidden2_1Value.Text = snapshot.Hidden2Values[0].ToString("F4");
        Hidden2_2Value.Text = snapshot.Hidden2Values[1].ToString("F4");
        Hidden2_3Value.Text = snapshot.Hidden2Values[2].ToString("F4");
        Hidden2_4Value.Text = snapshot.Hidden2Values[3].ToString("F4");

        OutputValue.Text = snapshot.OutputValues[0].ToString("F4");
    }

    private void DrawConnections(double[] fromLayer, double[] toLayer, double fromX,
                                     double toX, double graphTop, double graphBottom)
    {
        for (int i = 0; i < fromLayer.Length; i++)
        {
            double fromY = GetNeuronY(i, fromLayer.Length, graphTop, graphBottom);

            for (int j = 0; j < toLayer.Length; j++)
            {
                double toY = GetNeuronY(j, toLayer.Length, graphTop, graphBottom);
                // strength controls the amount of line shading based on the activation of the Neuron and the connection weight 
                double strength = (fromLayer[i] + toLayer[j]) / 2.0;
                strength = Math.Clamp(strength, 0.0, 1.0);

                Line connection = new Line
                {
                    X1 = fromX,
                    Y1 = fromY,
                    X2 = toX,
                    Y2 = toY,
                    Stroke = Brushes.White,
                    StrokeThickness = 0.5 + strength * 2.0,
                    Opacity = 0.15 + strength * 0.65
                };

                NetworkGraphCanvas.Children.Add(connection);

                if (strength > 0.35)
                {
                    AnimateConnectionFlash(connection, strength);
                    AnimateSignalPulse(fromX, fromY, toX, toY, strength);
                }
            }
        }
    }

    private void DrawLayer(string layerName, double[] values, double x, double graphTop, double graphBottom)
    {
        TextBlock layerLabel = new TextBlock
        {
            Text = layerName,
            FontSize = 13,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White
        };

        Canvas.SetLeft(layerLabel, x - 30);
        Canvas.SetTop(layerLabel, 20);
        NetworkGraphCanvas.Children.Add(layerLabel);

        for (int i = 0; i < values.Length; i++)
        {
            double y = GetNeuronY(i, values.Length, graphTop, graphBottom);
            DrawNeuron(x, y, values[i]);
        }
    }

    private void NetworkGraphCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawNetworkGraph();
    }

    private double GetNeuronY(int index, int count, double graphTop, double graphBottom)
    {
        if (count == 1)
        {
            return (graphTop + graphBottom) / 2.0;
        }

        double spacing = (graphBottom - graphTop) / (count - 1);
        return graphTop + index * spacing;
    }

    private void DrawNeuron(double x, double y, double activation)
    {
        activation = Math.Clamp(activation, 0.0, 1.0);

        double size = 28 + activation * 18;

        byte greenBlue = (byte)(60 + activation * 195);
        double glowSize = size + 20 + activation * 20;

        if (activation > 0.30) 
        {
            Ellipse glow = new Ellipse
            {
                Width = glowSize,
                Height = glowSize,
                Fill = Brushes.Cyan,
                Opacity = 0.25
            };

            Canvas.SetLeft(glow, x - glowSize / 2.0);
            Canvas.SetTop(glow, y - glowSize / 2.0);

            NetworkGraphCanvas.Children.Add(glow);

            AnimateNeuronGlow(glow);
        }
        Ellipse neuron = new Ellipse
        {
            Width = size,
            Height = size,
            Fill = new SolidColorBrush(Color.FromRgb(20, greenBlue, greenBlue)),
            Stroke = Brushes.White,
            StrokeThickness = 1.5
        };

        Canvas.SetLeft(neuron, x - size / 2.0);
        Canvas.SetTop(neuron, y - size / 2.0);

        NetworkGraphCanvas.Children.Add(neuron);

        if (activation > 0.5)
        {
            AnimateNeuronFlash(neuron);
        }

        TextBlock valueText = new TextBlock
        {
            Text = activation.ToString("F2"),
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.Black
        };

        Canvas.SetLeft(valueText, x - 13);
        Canvas.SetTop(valueText, y - 8);

        NetworkGraphCanvas.Children.Add(valueText);
    }

    // network Graph Animation
    private void AnimateConnectionFlash(Line line, double strength)
    {
        double baseOpacity = line.Opacity;
        double flashOpacity = Math.Min(1.0, baseOpacity + 0.35 + strength * 0.25);

        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = flashOpacity,
            To = baseOpacity,
            Duration = TimeSpan.FromMilliseconds(500),
            AutoReverse = false
        };

        DoubleAnimation thicknessAnimation = new DoubleAnimation
        {
            From = line.StrokeThickness + 2.0,
            To = line.StrokeThickness,
            Duration = TimeSpan.FromMilliseconds(500),
            AutoReverse = false
        };

        line.BeginAnimation(Line.OpacityProperty, opacityAnimation);
        line.BeginAnimation(Line.StrokeThicknessProperty, thicknessAnimation);
    }



    private void AnimateSignalPulse(double fromX, double fromY, double toX, double toY, double strength)
    {
        double size = 6 + strength * 8;

        Ellipse pulse = new Ellipse
        {
            Width = size,
            Height = size,
            Fill = Brushes.Cyan,
            Stroke = Brushes.White,
            StrokeThickness = 1,
            Opacity = 0.9
        };

        Canvas.SetLeft(pulse, fromX - size / 2.0);
        Canvas.SetTop(pulse, fromY - size / 2.0);

        NetworkGraphCanvas.Children.Add(pulse);

        DoubleAnimation moveX = new DoubleAnimation
        {
            From = fromX - size / 2.0,
            To = toX - size / 2.0,
            Duration = TimeSpan.FromMilliseconds(550)
        };

        DoubleAnimation moveY = new DoubleAnimation
        {
            From = fromY - size / 2.0,
            To = toY - size / 2.0,
            Duration = TimeSpan.FromMilliseconds(550)
        };

        DoubleAnimation fadeOut = new DoubleAnimation
        {
            From = 0.9,
            To = 0.0,
            BeginTime = TimeSpan.FromMilliseconds(250),
            Duration = TimeSpan.FromMilliseconds(300)
        };

        fadeOut.Completed += (sender, e) =>
        {
            NetworkGraphCanvas.Children.Remove(pulse);
        };

        pulse.BeginAnimation(Canvas.LeftProperty, moveX);
        pulse.BeginAnimation(Canvas.TopProperty, moveY);
        pulse.BeginAnimation(Ellipse.OpacityProperty, fadeOut);
    }


    private void AnimateNeuronGlow(Ellipse glow)
    {
        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = 0.45,
            To = 0.05,
            Duration = TimeSpan.FromMilliseconds(650)
        };

        glow.BeginAnimation(Ellipse.OpacityProperty, opacityAnimation);
    }


    private void AnimateNeuronFlash(Ellipse neuron)
    {
        DoubleAnimation strokeAnimation = new DoubleAnimation
        {
            From = 4.0,
            To = 1.5,
            Duration = TimeSpan.FromMilliseconds(450)
        };

        neuron.BeginAnimation(Ellipse.StrokeThicknessProperty, strokeAnimation);
    }
}