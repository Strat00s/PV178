using System;

namespace StoneMoveApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Creating scenario
            int quarryCapacity = 83;
            int truckMaxLoad = 10;

            var quarry = new Quarry("Quarry", quarryCapacity);
            var factory = new Factory("Factory");
            var truck = new Truck(truckMaxLoad, quarry);

            // Going to quarry and back with stone until quarry is empty
            while (quarry.StoneBlocks != 0)
            {
                // TODO move truck to quarry, load stone, move to factory, unload stone
            }

            // Check if all OK
            Console.WriteLine($"-- FINISHED ---");
            Console.WriteLine(factory.StoneBlocks < quarryCapacity
                ? $"Error: Upss you lost the stone somewhere. Factory has only {factory.StoneBlocks} of stone"
                : "Stone moved OK");

            Console.ReadKey();
        }
    }

}
