using OODGame;
using System.Text;

namespace ODDGame
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            GameConfig config = GameConfig.Load("GameConfig.json");
            Game game = new Game(config);
            game.Run();
        }
    }
}