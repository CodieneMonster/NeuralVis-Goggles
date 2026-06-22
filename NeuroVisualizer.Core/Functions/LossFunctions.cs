public class LossFunctions
{
    public static double MeanSquaredError(Matrix predictions, Matrix targets)
    {
        if (predictions.Rows != targets.Rows || predictions.Cols != targets.Cols)
        {
            throw new ArgumentException("Matrices must have the same dimensions");
        }

        double sumOfSqaureDifferences = 0;
        for (int i = 0; i < predictions.Rows; i++)
        {
            for (int j = 0; j < predictions.Cols; j++)
            {
                double error = targets.Data[i, j] - predictions.Data[i, j];
                sumOfSqaureDifferences += error * error;
            }
        }
        return sumOfSqaureDifferences / (predictions.Rows * predictions.Cols);
    }

    public static Matrix mseDerivative(Matrix predictions, Matrix targets) 
    {
        if (predictions.Rows != targets.Rows || predictions.Cols != targets.Cols)
        {
            throw new ArgumentException("Matrices must have the same dimensions");
        }

        Matrix result = new Matrix(predictions.Rows, predictions.Cols);
        for (int i = 0; i < predictions.Rows; i++)
        {
            for (int j = 0;j < predictions.Cols; j++)
            {
                result.Data[i,j] =  2 * (predictions.Data[i, j] - targets.Data[i, j]);
            }
        }
        return result;
    }
}