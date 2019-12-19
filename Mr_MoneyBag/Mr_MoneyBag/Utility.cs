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
        public static Node GetNextStep(MoveableObject player, MoveableObject enemy, GameBoard board)
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
}
