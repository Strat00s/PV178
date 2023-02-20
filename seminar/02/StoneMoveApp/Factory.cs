namespace StoneMoveApp
{
    /// <summary>
    /// Factory where the stone is processed (sculptures are created)
    /// </summary>
    public class Factory
    {
        // Number of transported stone blocks by truck
        public int StoneBlocks { get; set; }
        public Factory(string name) : base(name)
        {

        }
    }
}