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
        static Node[,] parent;
        static int[,] dir = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
        public static Node GetNextStep(MoveableObject player, MoveableObject enemy, Gameboard board)
        {
            int h = board.GetHeight();
            int w = board.GetWidth();
            //int x = obj.x;
            vis = new bool[h, w];
            reach = false;
            var st = new Node(player.x, player.y);
            parent = new Node[h, w];
            var queue = new Queue<Node>();
            queue.Enqueue(st);
            parent[st.x, st.y] = st;
            //Console.WriteLine("Begin BFS for Enemy: " + enemy.x + ", " + enemy.y + ": ");
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                if (vis[v.x, v.y]) continue;
                vis[v.x, v.y] = true;
                if (v.x == enemy.x && v.y == enemy.y) break;

                for(int i = 0; i < 4; i++)
                {
                    int nx = v.x + dir[i, 0];
                    int ny = v.y + dir[i, 1];
                    if ((nx > 0 && nx < board.GetWidth() - 1 && ny > 0 && ny < board.GetHeight() - 1) && !vis[nx, ny] && !board.status[nx, ny].isblocked 
                        && !((nx != enemy.x || ny != enemy.y) && board.HasEnemy(nx, ny))) {
                        queue.Enqueue(new Node(nx, ny));
                        parent[nx, ny] = v;
                        //Console.WriteLine("Parent of [" + nx + ", " + ny + "] is [" + v.x + ", " + v.y + "] board has enemy: " + board.HasEnemy(nx, ny));
                    }
                }

            }
            //Console.WriteLine("BFS for Enemy: " + enemy.x + ", " + enemy.y + " End");

            if (!vis[enemy.x, enemy.y])
            {
                Console.WriteLine("Cannot Found Any Path!");
                return new Node(enemy.x, enemy.y); // remain stayed
            }

            //Console.WriteLine(path[path.Count - 2].x);
            if (parent[enemy.x, enemy.y] == st)
                return new Node(enemy.x, enemy.y);


            return parent[enemy.x, enemy.y];

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
                        board.status[x, y] = new Money(board,x,y);
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
            board.status[y, x] = new Space(board,y,x);
            
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
