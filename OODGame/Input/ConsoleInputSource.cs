namespace OODGame.Input
{
    public sealed class ConsoleInputSource : IInputSource
    {
        public ConsoleKey ReadKey()
        {
            return Console.ReadKey(intercept: true).Key;
        }
    }
}
