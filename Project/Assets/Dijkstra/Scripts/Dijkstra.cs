using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dijkstra.Scripts
{
    public static class Dijkstra
    {
        /// <summary>
        /// DIJKSTRA
        /// Gets the shortest path from the starting Node to the ending Node.
        /// </summary>
        /// <returns>The shortest path</returns>
        /// <param name="start">Start Node</param>
        /// <param name="end">End Node</param>
        /// <param name="nodes">List of the current nodes</param>
        public static Path GetShortestPath(Node start, Node end, List<Node> nodes)
        {
            if (start == null || end == null) throw new ArgumentNullException();

            var path = new Path();

            if (start == end)
            {
                path.PathNodes.Add(start);

                return path;
            }

            var unvisited = new List<Node>();
            var previous = new Dictionary<Node, Node>();
            var distances = new Dictionary<Node, float>();

            foreach (var node in nodes)
            {
                unvisited.Add(node);
                distances.Add(node, float.MaxValue);
            }

            distances[start] = 0f;

            while (unvisited.Count != 0)
            {
                // Ordering the unvisited list by distance, smallest distance at start and largest at end
                unvisited = unvisited.OrderBy(node => distances[node]).ToList();

                // Getting the Node with smallest distance
                var current = unvisited[0];
                unvisited.Remove(current);

                // When the current node is equal to the end node, then we can break and return the path
                if (current == end)
                {
                    // Construct the shortest path
                    while (previous.ContainsKey(current))
                    {
                        path.PathNodes.Insert(0, current);
                        current = previous[current];
                    }

                    path.PathNodes.Insert(0, current);

                    break;
                }

                foreach (var neighbor in current.Connections)
                {
                    var length = Vector3.Distance(current.transform.position, neighbor.transform.position);
                    var alt = distances[current] + length;

                    if (!(alt < distances[neighbor])) continue;

                    // A shorter path to the connection (neighbor) has been found
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
            path.Bake();

            return path;
        }
    }
}
