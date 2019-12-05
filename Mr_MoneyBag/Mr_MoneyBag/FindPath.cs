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
        /* Find the shortest path from obj1 to obj2, using the provided board
         * Uses the dijkstra algorithm
         * 
         */ 
        public static void GetDistance(MoveableObject obj, Gameboard board)
        {
            int h = board.GetHeight();
            int w = board.GetWidth();
            int x = obj.x;
            int y = obj.y;
            



        }

        static int GetPos(MoveableObject obj, int w)
        {
            return obj.y * w + obj.x;
        }
    }




}
