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
     
    static class DistanceUtility
    {
        /* Find the next step from enemy to player, using the provided board
         * Provide a GameObject the next step should be
         * Uses the BFS Algorithm
         */ 
        public static GameObject GetNextStep(MoveableObject player, MoveableObject enemy, Gameboard board)
        {
            int h = board.GetHeight();
            int w = board.GetWidth();
            //int x = obj.x;
            List<GameObject> path = new List<GameObject>();
            BFS(player.x, player.y, board, enemy.x, enemy.y, 0, path);
            if(path.Count <= 1)
            {
                Console.WriteLine("Cannot Found Any Path from 1 to 2");
            }


            return path[path.Count - 1];

        }



        static void BFS(int x, int y, Gameboard board, int tx, int ty, int d, List<GameObject> path)
        {
            path.Add(board.status[x, y]);
            if (x == tx && y == ty) return;
            if (board.status[x, y].isblocked) { path.RemoveAt(path.Count - 1); return;}
            if (x > 0) BFS(x - 1, y, board, tx, ty, d + 1, path);
            if (x < board.GetWidth()) BFS(x + 1, y, board, tx, ty, d + 1, path);
            if (y > 0) BFS(x, y - 1, board, tx, ty, d + 1, path);
            if (y < board.GetHeight()) BFS(x, y + 1, board, tx, ty, d + 1, path);
            path.RemoveAt(path.Count - 1);
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
        static int[] parameter = new int[] { 22, 8, 6 };
        public static void GenRandomLevel(Gameboard board, int lv)
        {
            GenBasicMap(board);
            
        }
        public static void GenBasicMap(Gameboard board)
        {

            for (int i = 0; i < board.GetHeight(); i++)
            {
                for (int j = 0; j < board.GetWidth(); j++)
                {
                    board.status[i, j] = new Wall(board, i, j);
                }
            }

            bool[,] vis = new bool[board.GetHeight(), board.GetWidth()];
            GenBasicMapHelper(board, vis, Gameboard.initial_x, Gameboard.initial_y, 0);
            board.status[Gameboard.initial_x, Gameboard.initial_y] = board.player;
        }

        private static void GenBasicMapHelper(Gameboard board, bool[,] vis, int x, int y, int step)
        {
            
            if (x > board.GetWidth() - 2 || x < 1 || y > board.GetHeight() - 2 || y < 1) return;
            if (CheckConnect(board)) return;
            if (vis[y, x]) return;
            if (board.status[y, x].GetType() == typeof(Space)) return;

            vis[y, x] = true;
            Console.WriteLine(x + " -xy- " + y);
            board.status[y, x] = new Space(board);
            
            //int[] rndorder = order.OrderBy(t => rnd.Next()).ToArray();
            int next = rnd.Next(0, 4);
            GenBasicMapHelper(board, vis, x + dir[next, 0], y + dir[next, 1], step + 1);

            for(int i = 0; i < parameter.Length; i++)
            {
                if (rnd.Next(0, step / parameter[i]) == 0)
                {
                    next = rnd.Next(0, 4);
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
                    if ( (board.status[i, j] == null || board.status[i, j].GetType() == typeof(Wall)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }




}
