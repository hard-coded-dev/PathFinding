using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public class BFS
    {
        public static List<Direction4Way> Search4WayNode( Node start, IReachable searchGrid, IMovable moveGrid, out int distance )
        {
            Console.Error.WriteLine( "Finding from {0} by BFS", start );
            distance = -1;

            List<Direction4Way> pathWay = new List<Direction4Way>();
            if( searchGrid.IsReachedDestination( start ) )
            {
                distance = 0;
                return pathWay;
            }

            List<SearchNode> frontier = new List<SearchNode>();
            List<SearchNode> explored = new List<SearchNode>();

            SearchNode startNode = new SearchNode( start, null, 0, 0 );
            frontier.Add( startNode );

            bool found = false;
            while( frontier.Count > 0 )
            {
                SearchNode current = ( SearchNode )frontier[0];
                frontier.RemoveAt( 0 );
                explored.Add( current );

                if( searchGrid.IsReachedDestination( current.Pos ) )
                {
                    Console.Error.WriteLine( "End node found : {0}", current.Pos );
                    distance = current.CostSoFar + current.CostToEnd;
                    SearchNode parent = current;
                    while( parent != null && parent.Pos.Equals( start ) == false )
                    {
                        Direction4Way dir = PathFinding.Get4WayDirection( parent.Parent.Pos, parent.Pos );
                        //Console.Error.WriteLine( "iterate back to parent {0}", parent.Pos );
                        pathWay.Add( dir );
                        parent = parent.Parent;
                    }
                    found = true;
                    break;
                }

                List<SearchNode> neighbors = PathFinding.Get4WayNeighbors( current, moveGrid );
                foreach( SearchNode node in neighbors )
                {
                    if( explored.Contains( node ) )
                        continue;

                    node.CostSoFar = current.CostSoFar + 1;

                    int index = frontier.IndexOf( node );
                    if( index == -1 )
                    {
                        frontier.Add( node );
                    }
                }
                //Console.Error.WriteLine( "frontier = {0}", frontier.ToDebugString() );
            }

            pathWay.Reverse();
            if( found )
                Console.Error.WriteLine( "Found : from {0} through {1} of distance {2}", start, pathWay.ToDebugString(), distance );
            else
                Console.Error.WriteLine( "No Way! Found from {0}", start );
            return pathWay;
        }
    }
}
