﻿using System;
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
        /* Find the next path from obj1 to other positions, using the provided board
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


        /* Get the vision distance of two object
         * 
         */ 
         
        public static int GetDistance(MoveableObject obj1, MoveableObject obj2)
        {
            return Math.Abs(obj1.y - obj2.y) + Math.Abs(obj1.x - obj2.x);
        }

    }




}
