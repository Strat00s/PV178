namespace PatternMatching;

/// <summary>
/// Uloha:
/// Implementujte metodu AnalyzeCarPerformance(...), 
/// ktera bere jako parametr instanci tridy Car a pomoci Pattern 
/// Matchingu pak vypise zpravu na zaklade nasledujicich podminek:
///
/// I.   Pokud se jedna o PersonalCar, s hodnotou zrychleni (0 -> 100) 
///      mensi nez 7 vterin vypiste zpravu: "Given personal car can accelerate quickly"
///
/// II.  Pokud se jedna o SportsCar, s hodnotou zrychleni (0 -> 100) 
///      mensi nez 4 vteriny vypiste zpravu: "Given sports car can accelerate very fast"
///
/// III. Pokud je splnena podminka (II.) a vozidlo je navic vybaveno systemem Launch Control
///      (tedy property HasLaunchControl je nastavena na true), pripojte k vypisu zpravu:
///      " and is equiped with launch control system"
///
/// IV.  Ve vsech ostatnich pripadech vypiste: "Given car has rather average acceleration",
///      pokud instance tridy Car bude nastavena na null, vypiste: "N/A"
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        AnalyzeCarPerformance(null);
        AnalyzeCarPerformance(new PersonalCar(8));
        AnalyzeCarPerformance(new PersonalCar(6));
        AnalyzeCarPerformance(new SportsCar(3, true));
        AnalyzeCarPerformance(new SportsCar(4, false));
    }

    private static void AnalyzeCarPerformance(Car car, bool hasLaunchControl = false)
    {
        // TODO
    }
}