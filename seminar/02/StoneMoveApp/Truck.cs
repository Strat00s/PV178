using System;

namespace StoneMoveApp
{
    /// <summary>
    /// Truck which is moving block of stone
    /// </summary>
    public class Truck
    {
        // Number of stone block which can be loaded to the truck
        private readonly int loadCapacity;

        // Actual place of the truck (quarry or factory)
        private Place truckPosition;

        // Number of stone block loaded on the truck
        public int LoadedStoneBlocks { get; set; }

        public Truck(int loadCapacity, Place truckPosition)
        {
            this.loadCapacity = loadCapacity;
            this.truckPosition = truckPosition;
        }


        public void Move(Place place)
        {
            truckPosition = place;
            Console.WriteLine($"Truck is moving to {truckPosition.Name}");
        }

        public void LoadStone()
        {
            Console.WriteLine("Loading stone blocks in quarry...");

            // Truck is in quarry
            if (truckPosition is Quarry quarry)
            {
                // Truck is not full 
                if (LoadedStoneBlocks < loadCapacity)
                {
                    var loadedStoneBlocks = loadCapacity - LoadedStoneBlocks;

                    // Empty quarry
                    if (quarry.StoneBlocks < loadedStoneBlocks)
                    {
                        loadedStoneBlocks = quarry.StoneBlocks;
                    }

                    // Decrease number of blocks in quarry
                    quarry.StoneBlocks -= loadedStoneBlocks;

                    // Load the truck
                    LoadedStoneBlocks = loadedStoneBlocks;
                }

                // Error: Truck is maximally loaded
                else
                {
                    Console.WriteLine("Error: Trying to load truck which maximally loaded!");
                }
            }

            // Error: Truck not in quarry!
            else
            {
                Console.WriteLine("Error: Trying to load truck which is not in the quarry!");
            }
        }


    }
}