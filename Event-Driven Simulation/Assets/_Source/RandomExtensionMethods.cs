namespace Assets._Source
{
public static class RandomExtensionMethods
{
    public static double NextDoubleRange(this System.Random random, double minNumber, double maxNumber)
    {
        return random.NextDouble() * (maxNumber - minNumber) + minNumber;
    }
}
}
