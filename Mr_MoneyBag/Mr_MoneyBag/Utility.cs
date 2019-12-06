using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Linq.Enumerable;
using static System.String;
using static System.Console;
using EdgeList = System.Collections.Generic.List<(int node, double weight)>;

namespace Mr_MoneyBag
{
     
    public class Node
    {
        public int x, y;
        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    static class DistanceUtility
    {
        /* Find the next step from enemy to player, using the provided board
         * Provide a GameObject the next step should be
         * Uses the BFS Algorithm
         */
        static bool reach = false;
        static bool[,] vis;
        public static Node GetNextStep(MoveableObject player, MoveableObject enemy, Gameboard board)
        {
            int h = board.GetHeight();
            int w = board.GetWidth();
            //int x = obj.x;
            List<Node> path = new List<Node>();
            vis = new bool[h, w];
            reach = false;
            Console.WriteLine("abc");
            BFS(player.x, player.y, board, enemy.x, enemy.y, 0, path);
            Console.WriteLine("abcdd");
            //path.ForEach(Console.WriteLine);
            if (path.Count <= 1)
            {
                Console.WriteLine("Cannot Found Any Path from 1 to 2");
            }
            //Console.WriteLine(path[path.Count - 2].x);
            //Console.WriteLine(path[path.Count - 2].y);

            return path[path.Count - 2];

        }



        static void BFS(int x, int y, Gameboard board, int tx, int ty, int d, List<Node> path)
        {
            if (vis[x, y]) return;
            if (reach) return;
            path.Add(new Node(x, y));
            vis[x, y] = true;
            //Console.WriteLine("BFS: " + x + "," + y + " d: " + d + " tar: " + tx + "," + ty);

            if (x == tx && y == ty) { reach = true; Console.WriteLine("Reached!"); return; }
            if (board.status[x, y].isblocked) { path.RemoveAt(path.Count - 1); return;}

            
            if (x > 1) BFS(x - 1, y, board, tx, ty, d + 1, path);
            if (x < board.GetWidth() - 1) BFS(x + 1, y, board, tx, ty, d + 1, path);
            if (y > 1) BFS(x, y - 1, board, tx, ty, d + 1, path);
            if (y < board.GetHeight() - 1) BFS(x, y + 1, board, tx, ty, d + 1, path);
            if(!reach) path.RemoveAt(path.Count - 1);
            //vis[x, y] = false;
        }


        /* Get the vision distance of two objects
         * 
         */ 
         
        public static int GetDistance(GameObject obj1, GameObject obj2)
        {
            return Math.Abs(obj1.y - obj2.y) + Math.Abs(obj1.x - obj2.x);
        }

    }

    static class Level
    {
        static Random rnd = new Random();
        static int[,] dir = new int[,] { {1, 0}, {0, 1}, {-1, 0}, {0, -1} };
        static int[,] parameter = new int[,] { { 4, 0}, { 5, 0}, { 255, 1}, { 505, 1} };
        public static void GenRandomLevel(Gameboard board, int lv)
        {
            Console.WriteLine("-0-");
            GenBasicMap(board);
            Console.WriteLine("-1-");
            AddMoney(board);
            Console.WriteLine("-2-");
            AddShop(board, lv);
            Console.WriteLine("-3-");
            
        }


        public static void AddShop(Gameboard board, int lv)
        {
            for (int i = 0; i < board.shop_amount.Length; i++)
            {
                for (int j = 0; j < board.shop_amount[i]; j++)
                {
                    bool success = false;
                    while (!success)
                    {
                        int x = rnd.Next(1, board.GetHeight() - 1);
                        int y = rnd.Next(1, board.GetWidth() - 1);

                        if (board.status[x, y] is Space && board.player.x != x && board.player.y != y && 
                            !(board.status[x - 1, y].isblocked && board.status[x + 1, y].isblocked) &&
                            !(board.status[x, y - 1].isblocked && board.status[x, y + 1].isblocked)
                            )
                        {
                            switch (i)
                            {
                                case 0:
                                    board.status[x, y] = new CoinOnFloor_Shop(board, rnd.Next(1, 6), x, y);
                                    break;
                                case 1:
                                    board.status[x, y] = new NewRedGen_Shop(board, rnd.Next(1, 6), x, y);
                                    break;
                                case 2:
                                    board.status[x, y] = new RedNoticeDist_Shop(board, rnd.Next(1, 6), x, y);
                                    break;
                                case 3:
                                    board.status[x, y] = new Sight_Shop(board, rnd.Next(1, 6), x, y);
                                    break;
                                case 4:
                                    board.status[x, y] = new Damage_Shop(board, rnd.Next(1, 6), x, y);
                                    break;
                                case 5:
                                    board.status[x, y] = new MoneyLimit_Shop(board, rnd.Next(1, 6), x, y);
                                    break;

                            }
                            success = true;
                        }

                    }
                }

            }
        }

        public static void AddMoney(Gameboard board)
        {
            for(int i = 0; i < board.coinsonfloor; i++)
            {
                bool success = false;
                while (!success)
                {
                    int x = rnd.Next(0, board.GetHeight());
                    int y = rnd.Next(0, board.GetWidth());

                    if (board.status[x, y] is Space && board.player.x != x && board.player.y != y)
                    {
                        board.status[x, y] = new Money(board);
                        success = true;
                    }

                }
                
            }
        }


        public static void GenBasicMap(Gameboard board)
        {

            for (int i = 0; i < board.GetHeight(); i++)
            {
                for (int j = 0; j < board.GetWidth(); j++)
                {
                    if (i == 0 || j == 0 || i == board.GetHeight() - 1 || j == board.GetWidth() - 1)
                        board.status[i, j] = new UnbreakableWall(board, i, j);
                    else
                        board.status[i, j] = new Wall(board, i, j);
                }
            }

            bool[,] vis = new bool[board.GetHeight(), board.GetWidth()];
            GenBasicMapHelper(board, vis, Gameboard.initial_x, Gameboard.initial_y, 0);
            //board.status[Gameboard.initial_x, Gameboard.initial_y] = board.player;
        }


        private static void GenBasicMapHelper(Gameboard board, bool[,] vis, int x, int y, int step)
        {
            
            if (x > board.GetWidth() - 2 || x < 1 || y > board.GetHeight() - 2 || y < 1) return;
            if (CheckConnect(board)) return;
            if (vis[y, x]) return;
            if (board.status[y, x].GetType() == typeof(Space)) return;

            vis[y, x] = true;
            //Console.WriteLine(x + " -xy- " + y);
            board.status[y, x] = new Space(board);
            
            //int[] rndorder = order.OrderBy(t => rnd.Next()).ToArray();
            //int next = rnd.Next(0, 4);
            //GenBasicMapHelper(board, vis, x + dir[next, 0], y + dir[next, 1], step + 1);
            int prev = -1;
            for (int i = 0; i < 4; i++)
            {

                if (rnd.Next(0, (step / parameter[i,0]) + parameter[i,1]) == 0)
                {
                    int next = rnd.Next(0, 4);
                    if (i == 0) prev = next;
                    else if (next == prev)
                    {
                        while(next == prev)
                            next = rnd.Next(0, 4);
                    }
                    GenBasicMapHelper(board, vis, x + dir[next, 0], y + dir[next, 1], step + 1);
                }
            }
        }

        private static bool CheckConnect(Gameboard board)
        {
            
            for (int i = 0; i < board.GetHeight(); i+=2)
            {
                for (int j = 0; j < board.GetWidth(); j+=2)
                {
                    if ( (board.status[i, j] == null || board.status[i, j] is Wall))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }




}
