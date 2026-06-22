public class Activation
{
    public static double sigmoid(double x)
    {

        if (x >= 0)
        {
            double z = Math.Exp(-x);
            return 1.0 / (1.0 + z);
        }
        else
        {
            double z = Math.Exp(x);
            return z / (1.0 + z);
        }

        // return 1.0 / (1.0 + Math.Exp(-x));
    }

    public static double sigmoidDerivative(double x)
    {
        double s = sigmoid(x);
        return s * (1.0 - s);
    }

    public static double sigmoidDerivativeFromOutput(double y)
    {
        return y * (1.0 - y);
    }

    public static Matrix sigmoidDerivativeMatrix(Matrix a)
    {
        return Matrix.mapMatrix(a, sigmoidDerivative);
    }

    public static Matrix sigmoidDerivativeFromOutputMatrix(Matrix a)
    {
        return Matrix.mapMatrix(a, sigmoidDerivativeFromOutput);
    }
}

