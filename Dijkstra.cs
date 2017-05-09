using System;
using System.Collections.Generic;


public class Dijkstra
{
    public List<Direction4Way> SearchGraphByDijkstra( Node start, PathGrid grid, out int cost )
    {
        cost = -1;

        List<Direction4Way> pathWay = new List<Direction4Way>();
        if( grid.IsReachedDestination( start ) )
        {
            cost = 0;
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

            if( grid.IsReachedDestination( current.Pos ) )
            {
                cost = current.CostSoFar + current.CostToEnd;
                SearchNode parent = current;
                while( parent != null && parent.Pos.Equals( start ) == false )
                {
                    Direction4Way dir = PathFinding.Get4WayDirection( parent.Parent.Pos as Node, parent.Pos as Node );
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
                node.CostToEnd = 0;

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
            Console.Error.WriteLine( "Found from {0} : {1} of distance {2}", start, pathWay.ToDebugString(), cost );
        else
            Console.Error.WriteLine( "No Way! from {0}", start );
        return pathWay;
    }
}