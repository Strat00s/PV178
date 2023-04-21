using hw04;

Console.WriteLine("Start");
var simulationSilverstone = new Simulation(CurrentF1.Tracks.Silverstone);

// Testování nejlepší strategie
//CurrentF1.Cars.All.First().SetMediumHardStrategy();
//var lapsMediumHard = await simulationSilverstone.SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);

//CurrentF1.Cars.All.First().SetMediumHardSoftStrategy();
//var lapsMediumHardSoft = await simulationSilverstone.SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);

//CurrentF1.Cars.All.First().SetMediumMediumSoftStrategy();
//var lapsMediumMediumSoft = await simulationSilverstone.SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);

// Tady můžete vyhodnotit na základě jednotlivých simulací, která je nejlepší
// a zvolit ji pro všechny nebo některé formule.
// Stejně můžete testovat nastavení formule - turnSpeed a straightSpeed;
// skuste zachovat součet turnSpeed a straightSpeed, ale optimalizovat
// rychlost na kolo. (není součástí zadání = není nutno řešit)

// Závod
CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());
var race = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateRaceAsync(CurrentF1.Cars.All, 10);

Console.WriteLine("Final order:");
foreach (var (driver, totalTime) in race.GetOrder())
{
    Console.WriteLine($"{driver}: {totalTime.Minutes} min {totalTime.Seconds} s {totalTime.Milliseconds} ms");
}

Console.WriteLine("Fastest laps:");
foreach (var (driver, lapNum) in race.GetFastestLaps())
{
    Console.WriteLine($"{driver}: {lapNum}");
}