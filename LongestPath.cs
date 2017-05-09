using System;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding
{
    public partial class PathFinding
    {
        static void DoTopologicalSort( GraphNode current, List<GraphNode> nodes, ref Dictionary<int, bool> visited, ref List<GraphNode> topological )
        {
            visited[current.Id] = true;

            IEnumerable<GraphNode> neighbors = nodes.Where( Item => Item.Id == current.NextId1 || Item.Id == current.NextId2 );
            foreach( GraphNode neighbor in neighbors )
            {
                if( visited.ContainsKey( neighbor.Id ) == false )
                {
                    DoTopologicalSort( neighbor, nodes, ref visited, ref topological );
                }
            }
            topological.Add( current );
        }

        static List<GraphNode> TopologicalSort( List<GraphNode> nodes )
        {
            List<GraphNode> topological = new List<GraphNode>();
            Dictionary<int, bool> visited = new Dictionary<int, bool>();

            DoTopologicalSort( nodes[0], nodes, ref visited, ref topological );
            topological.Reverse();

            //Console.Error.WriteLine( "Topological Order is {0}", topological.ToDebugString() );

            return topological;
        }

        // find max or longest path distance(cost) from stat to ends( can be multiple )
        public static int FindLongestPath( GraphNode start, Dictionary<int, GraphNode> ends, Dictionary<int, GraphNode> nodes )
        {
            List<GraphNode> topologicalOrder = TopologicalSort( nodes.Values.ToList() );

            Dictionary<int, int> distance = new Dictionary<int, int>();
            foreach( GraphNode node in topologicalOrder )
            {
                distance[node.Id] = -Int32.MaxValue;
            }
            distance[start.Id] = start.Cost;

            while( topologicalOrder.Count > 0 )
            {
                GraphNode current = topologicalOrder[0];
                topologicalOrder.RemoveAt( 0 );

                // Update distances of all adjacent nodes
                if( distance[current.Id] != -Int32.MaxValue )
                {
                    if( current.NextId1 != -1 )
                    {
                        GraphNode neighbor1 = nodes[current.NextId1];
                        if( distance[neighbor1.Id] < distance[current.Id] + neighbor1.Cost )
                        {
                            distance[neighbor1.Id] = distance[current.Id] + neighbor1.Cost;
                        }
                    }
                    if( current.NextId2 != -1 )
                    {
                        GraphNode neighbor2 = nodes[current.NextId2];
                        if( distance[neighbor2.Id] < distance[current.Id] + neighbor2.Cost )
                        {
                            distance[neighbor2.Id] = distance[current.Id] + neighbor2.Cost;
                        }
                    }

                    /*
                    IEnumerable<GraphNode> neighbors = nodes.Where( Item => Item.Id == current.NextId1 || Item.Id == current.NextId2 );
                    foreach( GraphNode neighbor in neighbors )
                    {
                        if( distance[neighbor.Id] < distance[current.Id] + neighbor.Cost )
                        {
                            distance[neighbor.Id] = distance[current.Id] + neighbor.Cost;
                            //Console.Error.WriteLine( "update current of {0} to {1}", neighbor.Id, distance[neighbor.Id] );
                        }
                    }
                    */
                }
            }

            int maxDistance = start.Cost;

            foreach( var kvp in distance )
            {
                //Console.Error.WriteLine( "max distance to {0} is {1}", kvp.Key, kvp.Value );
                if( ends.ContainsKey( kvp.Key ) )
                {
                    if( kvp.Value > maxDistance )
                    {
                        maxDistance = kvp.Value;
                    }
                }

                /*
                foreach( GraphNode endNode in ends )
                {
                    if( endNode.Id == kvp.Key )
                    {
                        if( kvp.Value > maxDistance )
                        {
                            maxDistance = kvp.Value;
                        }
                    }
                }
                */
            }

            return maxDistance;
        }
    }

}