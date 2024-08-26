namespace PipeGameBlazor.Services.engine
{
    public class Cell
    {
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Connected { get; set; }
        public bool Locked { get; set; }
    }
}