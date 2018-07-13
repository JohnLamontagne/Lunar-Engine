/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using Lunar.Server.World.Structure;
using System;
using System.Collections.Generic;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World;

namespace Lunar.Server.Utilities.Pathfinding
{
    public class Pathfinder
    {
        private Layer _layer;
        private SearchNode[,] _searchNodes;

        private Rect _mapBounds;
        private Map _map;

        // Holds search nodes that are avaliable to search.
        private List<SearchNode> openList = new List<SearchNode>();
        // Holds the nodes that have already been searched.
        private List<SearchNode> closedList = new List<SearchNode>();


        public Pathfinder(Map map, Layer layer)
        {
            _mapBounds = map.Bounds;
            _layer = layer;
            _map = map;

            this.InitilizeSearchNodes();
        }

        /// <summary>
        /// Returns an estimate of the distance between two points. (H)
        /// </summary>
        private float Heuristic(Vector point1, Vector point2)
        {
            return Math.Abs(point1.X - point2.X) +
                   Math.Abs(point1.Y - point2.Y);
        }

        /// <summary>
        /// Resets the state of the search nodes.
        /// </summary>
        private void ResetSearchNodes()
        {
            openList.Clear();
            closedList.Clear();

            for (int x = _mapBounds.Left; x < _mapBounds.Width; x++)
            {
                for (int y = _mapBounds.Top; y < _mapBounds.Height; y++)
                {
                    SearchNode node = _searchNodes[x, y];

                    if (node == null)
                    {
                        continue;
                    }

                    node.InOpenList = false;
                    node.InClosedList = false;

                    node.DistanceTraveled = float.MaxValue;
                    node.DistanceToGoal = float.MaxValue;
                }
            }
        }

        /// <summary>
        /// Returns the node with the smallest distance to goal.
        /// </summary>
        private SearchNode FindBestNode()
        {
            SearchNode currentTile = openList[0];

            float smallestDistanceToGoal = float.MaxValue;

            // Find the closest node to the goal.
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].DistanceToGoal < smallestDistanceToGoal)
                {
                    currentTile = openList[i];
                    smallestDistanceToGoal = currentTile.DistanceToGoal;
                }
            }
            return currentTile;
        }

        /// <summary>
        /// Finds the optimal path from one point to another.
        /// </summary>
        public List<Vector> FindPath(Vector startPoint, Vector endPoint)
        {
            Vector normStartPoint = new Vector((int)(startPoint.X / Settings.TileSize), (int)(startPoint.Y / Settings.TileSize));
            Vector normEndPoint = new Vector((int)(endPoint.X / Settings.TileSize), (int)(endPoint.Y / Settings.TileSize));

            if (normStartPoint.X < _mapBounds.Left || normStartPoint.Y < _mapBounds.Top 
                || normEndPoint.X < _mapBounds.Left || normEndPoint.Y < _mapBounds.Top)
                return new List<Vector>();

            if (normStartPoint.X >= _mapBounds.Width || normStartPoint.Y >= _mapBounds.Height 
                || normEndPoint.X >= _mapBounds.Width || normEndPoint.Y >= _mapBounds.Height)
                return new List<Vector>();

            // Only try to find a path if the start and end points are different.
            if (normStartPoint.X == normEndPoint.X && normStartPoint.Y == normEndPoint.Y)
            {
                return new List<Vector>();
            }

            /////////////////////////////////////////////////////////////////////
            // Step 1 : Clear the Open and Closed Lists and reset each node’s F 
            //          and G values in case they are still set from the last 
            //          time we tried to find a path. 
            /////////////////////////////////////////////////////////////////////
            ResetSearchNodes();

            // Store references to the start and end nodes for convenience.
            SearchNode startNode = _searchNodes[(int)normStartPoint.X, (int)normStartPoint.Y];
            SearchNode endNode = _searchNodes[(int)normEndPoint.X, (int)normEndPoint.Y];

            /////////////////////////////////////////////////////////////////////
            // Step 2 : Set the start node’s G value to 0 and its F value to the 
            //          estimated distance between the start node and goal node 
            //          (this is where our H function comes in) and add it to the 
            //          Open List. 
            /////////////////////////////////////////////////////////////////////
            startNode.InOpenList = true;

            startNode.DistanceToGoal = Heuristic(normStartPoint, normEndPoint);
            startNode.DistanceTraveled = 0;

            openList.Add(startNode);

            /////////////////////////////////////////////////////////////////////
            // Setp 3 : While there are still nodes to look at in the Open list : 
            /////////////////////////////////////////////////////////////////////
            while (openList.Count > 0)
            {
                /////////////////////////////////////////////////////////////////
                // a) : Loop through the Open List and find the node that 
                //      has the smallest F value.
                /////////////////////////////////////////////////////////////////
                SearchNode currentNode = FindBestNode();

                /////////////////////////////////////////////////////////////////
                // b) : If the Open List empty or no node can be found, 
                //      no path can be found so the algorithm terminates.
                /////////////////////////////////////////////////////////////////
                if (currentNode == null)
                {
                    break;
                }

                /////////////////////////////////////////////////////////////////
                // c) : If the Active Node is the goal node, we will 
                //      find and return the final path.
                /////////////////////////////////////////////////////////////////

                if (currentNode.Position.X == endNode.Position.X && currentNode.Position.Y == endNode.Position.Y)
                {
                    // Trace our path back to the start.
                    return FindFinalPath(startNode, endNode);
                }

                /////////////////////////////////////////////////////////////////
                // d) : Else, for each of the Active Node’s neighbours :
                /////////////////////////////////////////////////////////////////
                for (int i = 0; i < currentNode.GetNeighbors().Length; i++)
                {
                    SearchNode neighbor = currentNode.GetNeighbors()[i];

                    //////////////////////////////////////////////////
                    // i) : Make sure that the neighbouring node can 
                    //      be walked across. 
                    //////////////////////////////////////////////////
                    if (neighbor == null || neighbor.Walkable == false)
                    {
                        continue;
                    }

                    //////////////////////////////////////////////////
                    // ii) Calculate a new G value for the neighbouring node.
                    //////////////////////////////////////////////////
                    float distanceTraveled = currentNode.DistanceTraveled + 1;

                    // An estimate of the distance from this node to the end node.
                    float heuristic = Heuristic(neighbor.Position, normEndPoint);

                    //////////////////////////////////////////////////
                    // iii) If the neighbouring node is not in either the Open 
                    //      List or the Closed List : 
                    //////////////////////////////////////////////////
                    if (neighbor.InOpenList == false && neighbor.InClosedList == false)
                    {
                        // (1) Set the neighbouring node’s G value to the G value 
                        //     we just calculated.
                        neighbor.DistanceTraveled = distanceTraveled;
                        // (2) Set the neighbouring node’s F value to the new G value + 
                        //     the estimated distance between the neighbouring node and
                        //     goal node.
                        neighbor.DistanceToGoal = distanceTraveled + heuristic;
                        // (3) Set the neighbouring node’s Parent property to point at the Active 
                        //     Node.
                        neighbor.Parent = currentNode;
                        // (4) Add the neighbouring node to the Open List.
                        neighbor.InOpenList = true;
                        openList.Add(neighbor);
                    }
                    //////////////////////////////////////////////////
                    // iv) Else if the neighbouring node is in either the Open 
                    //     List or the Closed List :
                    //////////////////////////////////////////////////
                    else if (neighbor.InOpenList || neighbor.InClosedList)
                    {
                        // (1) If our new G value is less than the neighbouring 
                        //     node’s G value, we basically do exactly the same 
                        //     steps as if the nodes are not in the Open and 
                        //     Closed Lists except we do not need to add this node 
                        //     the Open List again.
                        if (neighbor.DistanceTraveled > distanceTraveled)
                        {
                            neighbor.DistanceTraveled = distanceTraveled;
                            neighbor.DistanceToGoal = distanceTraveled + heuristic;

                            neighbor.Parent = currentNode;
                        }
                    }
                }

                /////////////////////////////////////////////////////////////////
                // e) Remove the Active Node from the Open List and add it to the 
                //    Closed List
                /////////////////////////////////////////////////////////////////
                openList.Remove(currentNode);
                currentNode.InClosedList = true;
            }

            // No path could be found.
            return new List<Vector>();
        }

        /// <summary>
        /// Use the parent field of the search nodes to trace
        /// a path from the end node to the start node.
        /// </summary>
        private List<Vector> FindFinalPath(SearchNode startNode, SearchNode endNode)
        {
            closedList.Add(endNode);

            SearchNode parentTile = endNode.Parent;

            // Trace back through the nodes using the parent fields
            // to find the best path.
            while (parentTile != startNode)
            {

                closedList.Add(parentTile);

                parentTile = parentTile.Parent;

            }

            List<Vector> finalPath = new List<Vector>();

            // Reverse the path and transform into world space.
            for (int i = closedList.Count - 1; i >= 0; i--)
            {
                // Add the path, as well as transform the positions from tile-space to world-space.
                finalPath.Add(new Vector(closedList[i].Position.X * Settings.TileSize,
                                          closedList[i].Position.Y * Settings.TileSize));
            }

            return finalPath;
        }

        private void InitilizeSearchNodes()
        {
            _searchNodes = new SearchNode[_mapBounds.Width, _mapBounds.Height];

            for (int x = 0; x < _mapBounds.Width; x++)
            {
                for (int y = 0; y < _mapBounds.Height; y++)
                {
                    var node = new SearchNode(new Vector(x, y))
                    {
                        Walkable = true
                    };

                    if (_layer.CheckCollision(new Vector(x * Settings.TileSize, y * Settings.TileSize), new Rect(0, 0, 32, 32)))
                    {
                        node.Walkable = false;
                    }

                    _searchNodes[x, y] = node;
                }
            }

            // Add all of the nodes to the SearchNode array.
            for (int x = 0; x < _searchNodes.GetLength(0); x++)
            {
                for (int y = 0; y < _searchNodes.GetLength(1); y++)
                {
                    if (_searchNodes[x, y] == null)
                    {
                        var node = new SearchNode(new Vector(x, y));
                        node.Walkable = true;
                        _searchNodes[x, y] = node;
                    }
                        
                }
            }



            // Loop back through and add the neighbors of the nodes.
            for (int x = 0; x < _searchNodes.GetLength(0); x++)
            {
                for (int y = 0; y < _searchNodes.GetLength(1); y++)
                {
                    SearchNode[] neighbors = new SearchNode[4];

                    Vector[] neighborPositions = new Vector[]
                    {
                        new Vector(x, y - 1),
                        new Vector(x, y + 1),
                        new Vector(x - 1, y),
                        new Vector(x + 1, y)
                    };

                    for (int i = 0; i < neighborPositions.Length; i++)
                    {
                        if (neighborPositions[i].X < 0 || neighborPositions[i].X >= _searchNodes.GetLength(0))
                        {
                            continue;
                        }

                        if (neighborPositions[i].Y < 0 || neighborPositions[i].Y >= _searchNodes.GetLength(1))
                        {
                            continue;
                        }

                        if (_searchNodes[(int)neighborPositions[i].X, (int)neighborPositions[i].Y].Walkable)
                        {
                            neighbors[i] = _searchNodes[(int)neighborPositions[i].X, (int)neighborPositions[i].Y];
                        }
                    }

                    _searchNodes[x, y].SetNeighbors(neighbors);
                }
            }

        }
    }
}