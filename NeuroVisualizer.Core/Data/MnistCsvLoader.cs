using System;
using System.Collections.Generic;
using System.IO;
public class MnistCsvLoader
{
    public static List<TrainingSample> LoadSamples(string filePath, int maxSamples)
    {
        // read CSV file
        // convert each row into trainingsample
        // return list
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        if (maxSamples <= 0)
        {
            throw new ArgumentException("maxSamples must be greater than 0");
        }

        List<TrainingSample> samples = new List<TrainingSample>();

        foreach (string line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue; // skip empty lines
            }

            string[] values = line.Split(',');
            if (values.Length != 785)
            {
                throw new FormatException($"Invalid MNIST row. Expected 785 values but got : {values.Length}");
            }

            int label = int.Parse(values[0]);

            if (label < 0 || label > 9)
            {
                throw new FormatException($"Invalid MNIST label value: {label}");
            }
            Matrix input = BuildInputMatrix(values);
            Matrix target = BuildTargetMatrix(label);

            TrainingSample sample = new TrainingSample(input, target);
            samples.Add(sample);
            if (samples.Count >= maxSamples)
            {
                break; // stop if we've reached the maximum number of samples
            }
        }
        return samples;
    }
        

    private static Matrix BuildInputMatrix(string[] values)
    {
        // convert 784 pixel values into a 784 x 1 matrix
        Matrix input = new Matrix(784, 1);
        for (int i = 1; i < values.Length - 1; i++)
        {
            string pixelString = values[i];
            double pixelValue = double.Parse(pixelString); // convert pixel value to double
            double normalizedPixel = pixelValue / 255.0; // normalize pixel value to [0, 1]
            int matrixRow = i - 1;
            input.Data[matrixRow, 0] = normalizedPixel;

        }
        return input;
    }

    private static Matrix BuildTargetMatrix(int label)
    {
        // create a 10 x 1 matrix with all values set to 0
        // set the value at index 'label' to 1
        Matrix target = new Matrix(10, 1);
        if (label < 0 || label > 9) {
            throw new ArgumentException($"Label must be between 0 and 9. Got: {label}");
        }
        for (int i = 0; i < target.Rows; i++)
        {
            target.Data[i, 0] = (i == label) ? 1.0 : 0.0;
        }
        return target;
    }
}