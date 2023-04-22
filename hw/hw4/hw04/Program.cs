using hw04;
using hw04.Race;

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
var race = await new Simulation(CurrentF1.Tracks.Silverstone).SimulateRaceAsync(CurrentF1.Cars.All, 52);

Console.WriteLine("Final order:");
foreach (var (driver, totalTime) in race.GetOrder())
{
    Console.WriteLine($"  {driver}: {totalTime.Minutes} min {totalTime.Seconds} s {totalTime.Milliseconds} ms");
}

Console.WriteLine("Fastest laps:");
int i = 0;
foreach (var (driver, lapTime) in race.GetFastestLaps())
{
    Console.WriteLine($"  Lap {++i}: {driver}: {lapTime.Minutes} min {lapTime.Seconds} s {lapTime.Milliseconds} ms");
}

int orderAt = 5;
Console.WriteLine($"Order at: {orderAt}");
foreach (var (driver, lapTime) in race.GetOrderAt(orderAt))
{
    Console.WriteLine($"  {driver}: {lapTime.Minutes} min {lapTime.Seconds} s {lapTime.Milliseconds} ms");
}

Console.WriteLine($"Track point times:");
foreach (var (trackPoint, driver1, lapNum1, drivingTime, driver2, lapNum2, waitingTime) in race.GetTrackPointTimes())
{
    Console.WriteLine($"{trackPoint}:");
    Console.WriteLine($"  Shortest driving time:");
    Console.WriteLine($"    Driver: {driver1}");
    Console.WriteLine($"    Lap: {lapNum1}");
    Console.WriteLine($"    Time: {drivingTime.Minutes} min {drivingTime.Seconds} s {drivingTime.Milliseconds} ms");
    Console.WriteLine($"  Longest waiting time:");
    Console.WriteLine($"    Driver: {driver2}");
    Console.WriteLine($"    Lap: {lapNum2}");
    Console.WriteLine($"    Time: {waitingTime.Minutes} min {waitingTime.Seconds} s {waitingTime.Milliseconds} ms");
}