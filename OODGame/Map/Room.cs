using OODGame.Items;
using OODGame.Items.Unequipable;
using OODGame.Items.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame.Map
{
    public class Room
    {
        public Tile[,] Grid { get; private set; }
        public readonly int Width;
        public readonly int Height;

        public Room(int width, int height, Tile[,] grid)
        {
            Width = width;
            Height = height;
            Grid = grid;
        }

        //private void InitializeMap()
        //{
        //    string[] mapTemplate = new string[]
        //    {
        //    "  ######################################",
        //    "                                    C  #",
        //    "#    ########               ######     #",
        //    "#    ########               #    #     #",
        //    "#                           #    #     #",
        //    "#                           #    #     #",
        //    "#                           ######     #",
        //    "#         U                            #",
        //    "#                                      #",
        //    "######################         #########",
        //    "#                                      #",
        //    "#                                      #",
        //    "#           ##############             #",
        //    "#           #            #             #",
        //    "#           #            #             #",
        //    "#           #            #             #",
        //    "#  ######   ##############             #",
        //    "#       #                              #",
        //    "#     A #                            S #",
        //    "########################################"
        //    };

        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            char templateChar = mapTemplate[y][x];

        //            if (templateChar == '#')
        //            {
        //                Grid[y, x] = new WallTile();
        //            }
        //            else
        //            {
        //                Grid[y, x] = new EmptyTile();
        //                PlaceItem(templateChar, x, y);
        //            }
        //        }
        //    }
        //}

        private void PlaceItem(char symbol, int x, int y)
        {
            Item item = null;

            switch (symbol)
            {
                case 'A':
                    item = new TwoHandedAxe();
                    break;
                case 'S':
                    item = new Sword();
                    break;
                case 'C':
                    item = new Crossbow();
                    break;
                case 'U':
                    item = new Unusable1();
                    break;
                default:
                    return;
            }

            if (item != null)
            {
                var itemTile = Grid[y, x];
                itemTile.PlaceItem(item); 
                itemTile.PlaceItem(new Unusable1());
                itemTile.PlaceItem(new Unusable3());
            }
        }
    }
}
