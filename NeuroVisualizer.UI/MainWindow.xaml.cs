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
        DrawLossGraph();
    }


    private void LossHistory_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        DrawLossGraph();
    }

    private void LossGraphCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawLossGraph();
    }

    private void DrawMnistImage_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;

        if (vm.CurrentMnistInput == null)
        {
            return;
        }

        DrawMnistImage(vm.CurrentMnistInput);
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




    private void DrawMnistImage(Matrix input)
    {
        MnistImageCanvas.Children.Clear();

        int imageSize = 28;
        double pixelSize = 10; // 28 * 10 = 280 canvas size

        for (int index = 0; index < 784; index++)
        {
            int row = index / imageSize;
            int col = index % imageSize;

            double value = input.Data[index, 0]; // value between 0.0 and 1.0

            byte brightness = (byte)(value * 255);

            Rectangle pixel = new Rectangle
            {
                Width = pixelSize,
                Height = pixelSize,
                Fill = new SolidColorBrush(Color.FromRgb(brightness, brightness, brightness))
            };

            Canvas.SetLeft(pixel, col * pixelSize);
            Canvas.SetTop(pixel, row * pixelSize);

            MnistImageCanvas.Children.Add(pixel);
        }
    }


    private async void TrainMnist_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;

        await vm.TrainMnist();

        if (vm.CurrentMnistInput != null)
        {
            DrawMnistImage(vm.CurrentMnistInput);
        }
    }

    private void TrainMnistOneEpoch_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel vm = (MainViewModel)DataContext;

        vm.TrainMnistOneEpoch();

        if (vm.CurrentMnistInput != null)
        {
            DrawMnistImage(vm.CurrentMnistInput);
        }
    }

}