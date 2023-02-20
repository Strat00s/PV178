namespace StoneMoveApp
{
    /// <summary>
    /// Quarry (ČESKY: lom) where the stone is mined
    /// </summary>
    public class Quarry
    {
        // Number of stone block to be transported by truck
        public int StoneBlocks { get; set; }

        public Quarry(string name, int stoneBlocks) : base(name)
        {
            StoneBlocks = stoneBlocks;
        }
    }
}