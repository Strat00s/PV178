﻿using hw04;

Console.WriteLine("Start");
var simulationSilverstone = new Simulation(CurrentF1.Tracks.Silverstone);

// Testování nejlepší strategie
foreach(var car in CurrentF1.Cars.All)
{
    //Console.WriteLine("new car test");
    CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());
    var lapsMediumHard = simulationSilverstone.SimulateLapsAsync(car, 1);
}
CurrentF1.Cars.All.First().SetMediumHardStrategy();
//var lapsMediumHard = simulationSilverstone.SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);

CurrentF1.Cars.All.First().SetMediumHardSoftStrategy();
//var lapsMediumHardSoft = simulationSilverstone.SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);

CurrentF1.Cars.All.First().SetMediumMediumSoftStrategy();
//var lapsMediumMediumSoft = simulationSilverstone.SimulateLapsAsync(CurrentF1.Cars.All.First(), 52);

// Tady můžete vyhodnotit na základě jednotlivých simulací, která je nejlepší
// a zvolit ji pro všechny nebo některé formule.
// Stejně můžete testovat nastavení formule - turnSpeed a straightSpeed;
// skuste zachovat součet turnSpeed a straightSpeed, ale optimalizovat
// rychlost na kolo. (není součástí zadání = není nutno řešit)

// Závod
CurrentF1.Cars.All.ForEach(c => c.SetMediumHardStrategy());
var race = new Simulation(CurrentF1.Tracks.Silverstone).SimulateRaceAsync(CurrentF1.Cars.All, 1);

// foreach (var (car, totalTime) in race.GetOrder())
// {
//     Console.WriteLine($"{car.Driver}: {totalTime.Minutes} min {totalTime.Seconds} s {totalTime.Milliseconds} ms");
// }