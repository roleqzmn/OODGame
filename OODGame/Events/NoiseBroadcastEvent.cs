namespace OODGame.Events
{
    public sealed class NoiseBroadcastEvent : IGameEvent
    {
        public int SourceX { get; }
        public int SourceY { get; }
        public int Range { get; }
        public string Category { get; }

        public NoiseBroadcastEvent(int sourceX, int sourceY, int range, string category)
        {
            SourceX = sourceX;
            SourceY = sourceY;
            Range = range;
            Category = category;
        }
    }
}
