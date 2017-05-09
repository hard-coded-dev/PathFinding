using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public class AStar
    {
        public static List<Direction4Way> Search4WayNode( Node start, Node end, IMovable grid, out int distance )
        {
            Console.Error.WriteLine( "Finding from {0} to {1} by A*", start, end );
            distance = -1;

            List<Direction4Way> pathWay = new List<Direction4Way>();
            if( start.Equals( end ) )
            {
                distance = 0;
                return pathWay;
            }

            List<SearchNode> frontier = new List<SearchNode>();
            List<SearchNode> explored = new List<SearchNode>();

            SearchNode startNode = new SearchNode( start, null, 0, PathFinding.GetManhattanHeuristic( start, end ) );
            frontier.Add( startNode );

            bool found = false;
            while( frontier.Count > 0 )
            {
                SearchNode current = ( SearchNode )frontier[0];
                frontier.RemoveAt( 0 );
                explored.Add( current );

                if( current.Pos.Equals( end ) )
                {
                    distance = current.CostSoFar + current.CostToEnd;
                    SearchNode parent = current;
                    while( parent != null && parent.Pos.Equals( start ) == false )
                    {
                        Direction4Way dir = PathFinding.Get4WayDirection( parent.Parent.Pos, parent.Pos );
                        pathWay.Add( dir );
                        parent = parent.Parent;
                    }
                    found = true;
                    break;
                }

                List<SearchNode> neighbors = PathFinding.Get4WayNeighbors( current, grid );
                foreach( SearchNode node in neighbors )
                {
                    if( explored.Contains( node ) )
                        continue;

                    node.CostSoFar = current.CostSoFar + 1;
                    node.CostToEnd = PathFinding.GetManhattanHeuristic( node.Pos, end );

                    int index = frontier.IndexOf( node );
                    if( index > 0 )
                    {
                        if( node.CostSoFar < frontier[index].CostSoFar )
                        {
                            // if found better way
                            frontier[index].Parent = current;
                            frontier[index].CostSoFar = node.CostSoFar;
                            frontier[index].CostToEnd = node.CostToEnd;
                        }
                    }
                    else
                    {
                        frontier.Add( node );
                    }
                }
                frontier.Sort( ( item1, item2 ) => ( item1.CostSoFar + item1.CostToEnd ) - ( item2.CostSoFar + item2.CostToEnd ) );
                //Console.Error.WriteLine( "frontier = {0}", frontier.ToDebugString() );
            }

            pathWay.Reverse();

            if( found )
                Console.Error.WriteLine( "Found : {0} to {1} : {2} of distance {3}", start, end, pathWay.ToDebugString(), distance );
            else
                Console.Error.WriteLine( "No Way! : {0} to {1}", start, end );
            return pathWay;
        }
    }
}
