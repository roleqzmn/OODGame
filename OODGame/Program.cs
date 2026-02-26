using OODGame;
using OODGame.Map;
using OODGame.Player;
using System.Text;

namespace ODDGame
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Game game=new Game();
            game.Run();
        }
    }
}