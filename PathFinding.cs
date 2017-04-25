using System;
using System.Collections.Generic;

public class SearchNode : IEquatable<SearchNode>
{
    public Offset Pos { get; set; }
    public int Rotation { get; set; }
    public int CostSoFar { get; set; }
    public int CostToEnd { get; set; }
    public SearchNode Parent { get; set; }

    public SearchNode( Offset pos, int rotation, int costFrom, int costEnd )
    {
        Pos = pos;
        Rotation = rotation;
        CostSoFar = costFrom;
        CostToEnd = costEnd;
    }

    public override bool Equals( object obj )
    {
        return Equals( obj as SearchNode );
    }

    public bool Equals( SearchNode other )
    {
        return ( other.Pos.Equals( Pos ) );
    }

    public override int GetHashCode()
    {
        return Pos.row ^ Pos.col;
    }

    public override string ToString()
    {
        return "[" + Pos.row + " " + Pos.col + " : " + ( CostSoFar + CostToEnd ) + "]";
    }
};

public enum Grid
{
    WIDTH = 23,
    HEIGHT = 21,
}

public class PathFinding
{
    static int GetHeuristic( Offset pos1, Offset pos2 )
    {
        Hex a = pos1.GetHex();
        Hex b = pos2.GetHex();
        return ( Math.Abs( a.x - b.x ) + Math.Abs( a.y - b.y ) + Math.Abs( a.z - b.z ) );
    }

    static List<SearchNode> GetHexNeighbors( SearchNode center, Offset endNode )
    {
        //Console.Error.WriteLine( "Finding neighbors of {0}", center );
        List<SearchNode> neighbors = new List<SearchNode>();

        var parity = center.Pos.row & 1;    // 0 for even line, 1 for odd line
        var dir = Hex.OffsetDirections[parity];

		// 0 : Right
		// 1 : Upper Right
		// 2 : Upper Left
		// 3 : Left
		// 4 : Lower left
		// 5 : Lower Right
        for( int i = 0; i < 6; ++i )
        {
            // not allowed 2,3,4 since they are only able to go ahead.
            if( i >= 2 && i <= 4 )
                continue;
            // to sustain the direction of its previous direction
            int rotation = ( 6 + i + center.Rotation ) % 6;
            Offset newPos = new Offset( center.Pos.row + dir[rotation, 1], center.Pos.col + dir[rotation, 0] );
            int direction = Offset.GetRotation( center.Pos, newPos );

            if( newPos.row < 0 || newPos.row >= ( int )Grid.HEIGHT )
                continue;
            if( newPos.col < 0 || newPos.col >= ( int )Grid.WIDTH )
                continue;

			// for specific constraints
			//if( IsMovable( newPos, direction ) == false )
			//	continue;
		
            int gridCost = 1; // + ( int )Caribbean.GetGridProperty( newPos );
            SearchNode newNode = new SearchNode( newPos, rotation, center.CostSoFar + gridCost, GetHeuristic( newPos, endNode ) );

            neighbors.Add( newNode );
        }
        return neighbors;
    }

    public static List<Offset> FindClosedPosByAStar( Offset startNode, int startDirection, Offset endNode, out List<int> rotationPathWay )
    {
        Console.Error.WriteLine( "Path finding from {0} to {1} at direction of {2}", startNode, endNode, startDirection );

        rotationPathWay = new List<int>();
        List<Offset> pathWay = new List<Offset>();

        if( startNode.Equals( endNode ) )
            return pathWay;

        List<SearchNode> frontier = new List<SearchNode>();
        List<SearchNode> explored = new List<SearchNode>();

        SearchNode start = new SearchNode( startNode, startDirection, 0, GetHeuristic( startNode, endNode ) );
        frontier.Add( start );

        bool found = false;
        while( frontier.Count > 0 )
        {
            SearchNode current = frontier[0];
            frontier.RemoveAt( 0 );
            explored.Add( current );

            if( current.Pos.Equals( endNode ) )
            {
                SearchNode parent = current;

                while( parent != null && parent != start )
                {
                    //Console.Error.WriteLine( "cost to {0} is {1}", parent.Pos, parent.CostSoFar );
                    int rotation = Offset.GetRotation( parent.Parent.Pos, parent.Pos );
                    rotationPathWay.Add( rotation );
                    pathWay.Add( parent.Pos );
                    parent = parent.Parent;
                }

                found = true;
                break;
            }

			// constraints for early exit such as line of sight
            if( Offset.GetDistance( current.Pos, start.Pos ) < 5 )
            {
                List<SearchNode> neighbors = GetHexNeighbors( current, endNode );
                foreach( SearchNode node in neighbors )
                {
                    if( explored.Contains( node ) )
                        continue;

                    int costSoFar = current.CostSoFar;
                    int costToEnd = GetHeuristic( node.Pos, endNode );

                    int index = frontier.IndexOf( node );
                    if( index > 0 )
                    {
                        if( costSoFar < frontier[index].CostSoFar )
                        {
                            // already exist in the container and found better way
                            frontier[index].Parent = current;
                            frontier[index].Rotation = current.Rotation;
                            frontier[index].CostSoFar = costSoFar;
                            frontier[index].CostToEnd = costToEnd;
                        }
                    }
                    else
                    {
                        // Not found
                        node.Parent = current;
                        node.CostSoFar = costSoFar;
                        node.CostToEnd = costToEnd;
                        frontier.Add( node );
                    }
                }
            }
            frontier.Sort( ( item1, item2 ) => ( item1.CostSoFar + item1.CostToEnd ) - ( item2.CostSoFar + item2.CostToEnd ) );
            //Console.Error.WriteLine( "Frontier is {0}", frontier.ToDebugString() );
        }

        if( pathWay.Count == 0 && explored.Count > 0 )
        {
            explored.Sort( ( item1, item2 ) => ( item1.CostSoFar + item1.CostToEnd ) - ( item2.CostSoFar + item2.CostToEnd ) );
            SearchNode parent = explored[0];
            while( parent != null && parent != start )
            {
                rotationPathWay.Add( Offset.GetRotation( parent.Parent.Pos, parent.Pos ) );
                pathWay.Add( parent.Pos );
                parent = parent.Parent;
            }
        }

        pathWay.Reverse();
        rotationPathWay.Reverse();

        if( found )
            Console.Error.WriteLine( "Found : Pathway from {0} to target({1}) is {2}, rotation = {3} at distance of {4}", startNode, endNode, pathWay.ToDebugString(), rotationPathWay.ToDebugString(), pathWay.Count );
        else
            Console.Error.WriteLine( "Not Found : Closest Pathway from {0} to target({1}) is {2}, rotation = {3} at distance of {4}", startNode, endNode, pathWay.ToDebugString(), rotationPathWay.ToDebugString(), pathWay.Count );

        return pathWay;
    }
}