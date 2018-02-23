//----------------------------------------------
// File: PathFinder.cs
// Copyright © 2018 InsertCoin (www.insertcoin.info)
// Author: Omer Akyol
// Algorithm by: Copyright (C) 2006 Franco, Gustavo 
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;

namespace Astar
{
    public class PathFinder 
    {
        private const int SearchLimit = 20000;

        public List<Node> Astar(int[,] map, Vector2Int startPos, Vector2Int goalPos)
        {
            bool found = false;
            bool stop  = false;

            PriorityQueueB<Node> open = new PriorityQueueB<Node>(new ComparePFNode());
            List<Node> close          = new List<Node>();

            Node parentNode  = new Node(startPos);
            Node foundNode   = null;
            parentNode.GCost = 0;
            parentNode.HCost = 0;
            parentNode.FCost = parentNode.GCost+ parentNode.HCost;
          
            open.Push(parentNode);

            while (open.Count > 0 && !stop)
            {
                parentNode = open.Pop();

                if (parentNode.Position.x == goalPos.x && parentNode.Position.y == goalPos.y)
                {
                    foundNode = parentNode;
                    close.Add(parentNode);
                    found = true;
                    break;
                }

                if (close.Count > SearchLimit)
                {
                    Debug.LogWarning("Search limit exceeded.");
                    return null;
                }

                var successors = GetAllSuccessorNodes(map, parentNode);

                //Lets calculate each successors
                foreach (var node in successors)
                {
                    float newG = parentNode.GCost + 0; // 0 is tile difficulty
                 
                    //if ((int)newG == (int)parentNode.GCost)
                    //{
                    //    Debug.Log("WHAT?");
                    //    continue;
                    //}

                    // Need a proper search method
                    int foundInOpenIndex = -1;
                    for (int j = 0; j < open.Count; j++)
                    {
                        if (open[j].Position.x == node.Position.x && open[j].Position.y == node.Position.y)
                        {
                            foundInOpenIndex = j;
                            break;
                        }
                    }
                    if (foundInOpenIndex != -1 && open[foundInOpenIndex].GCost <= newG)
                    {
                        continue;
                    }
                    
                    int foundInCloseIndex = -1;
                    for (int j = 0; j < close.Count; j++)
                    {
                        if (close[j].Position.x == node.Position.x && close[j].Position.y == node.Position.y)
                        {
                            foundInCloseIndex = j;
                            break;
                        }
                    }
                    if (foundInCloseIndex != -1 && close[foundInCloseIndex].GCost <= newG)
                        continue;

                    node.Parent = parentNode;
                    node.GCost  = newG;
                    node.HCost  = Vector2Int.Distance(node.Position, goalPos);
                    node.FCost  = node.GCost + node.HCost;

                    open.Push(node);
                }

                close.Add(parentNode);
            }

            if (found)
            {
                List<Node> path = new List<Node>();
                ConstructPathRecursive(foundNode, path);
                path.Reverse();

                return path;
            }

            return null;
        }

        private void ConstructPathRecursive(Node node, List<Node> path)
        {
            if(node != null)
            {
                path.Add(node);
                ConstructPathRecursive(node.Parent, path);
            }
        }
        
        private List<Node> GetAllSuccessorNodes(int[,] map, Node parent)
        {
            List<Node> successors = new List<Node>();
            int x = parent.Position.x;
            int y = parent.Position.y;

            Node leftNode  = GetSuccessorNode(map, parent, x - 1, y);
            Node rightNode = GetSuccessorNode(map, parent, x + 1, y);
            Node upNode    = GetSuccessorNode(map, parent, x, y + 1);
            Node downNode  = GetSuccessorNode(map, parent, x, y - 1);

            if (leftNode != null)
                successors.Add(leftNode);

            if (rightNode != null)
                successors.Add(rightNode);

            if (upNode != null)
                successors.Add(upNode);

            if (downNode != null)
                successors.Add(downNode);

            return successors;
        }

        private Node GetSuccessorNode(int[,] map, Node parent, int x, int y)
        {
            Node node = null;

            if (x >= 0 && y >= 0 && x < MapManager.MapSize && y < MapManager.MapSize &&
            (map[y, x] == (int)TileType.Empty || map[y, x] == (int)TileType.Goal))
            {
                node = new Node(new Vector2Int(x, y));
                node.Parent = parent;
            }

            return node;
        }

        internal class ComparePFNode : IComparer<Node>
        {
            #region IComparer Members
            public int Compare(Node x, Node y)
            {
                if (x.FCost > y.FCost)
                    return 1;
                else if (x.FCost < y.FCost)
                    return -1;
                return 0;
            }
            #endregion
        }
    }
}
