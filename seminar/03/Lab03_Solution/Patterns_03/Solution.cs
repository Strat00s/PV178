namespace Patterns_03;

internal class Solution
{
    internal static void TestSolution()
    {
        AnalyzeCarPerformance(null);
        AnalyzeCarPerformance(new PersonalCar(8));
        AnalyzeCarPerformance(new PersonalCar(6));
        AnalyzeCarPerformance(new SportsCar(3, true));
        AnalyzeCarPerformance(new SportsCar(4, false));
    }

    internal static void AnalyzeCarPerformance(Car car)
    {
        switch (car)
        {
            case Car car1 when car1 is PersonalCar && car1.AccelerationTime < 7.0:
                Console.WriteLine("Given personal car can accelerate quickly");
                break;
            case Car car1 when car1 is SportsCar sportsCar && car1.AccelerationTime < 4.0:
                Console.WriteLine($"Given sports car can accelerate very fast {(sportsCar.HasLaunchControl ? "and is equiped with launch control system" : string.Empty)}");
                break;
            default:
                Console.WriteLine("Given car has rather average acceleration");
                break;
            case null:
                Console.WriteLine("N/A");
                break;
        }
    }
}
