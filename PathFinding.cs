using System;
using System.Collections.Generic;

public interface IMovable
{
    bool IsMovableNode( Node pos );
}

public interface ISearch
{
    bool IsReachedDestination( Node Pos );
}

public class PathFinding
{
	public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        LENGTH,
    }
	// based on Direction order
    static int[,] Way4Directions = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // { row, col }
	
	static Direction Get4WayDirection( Node from, Node to )
    {
        int rowDiff = to.Row - from.Row;
        int colDiff = to.Col - from.Col;

        if( rowDiff == 1 )
            return Direction.DOWN;
        else if( rowDiff == -1 )
            return Direction.UP;
        else if( colDiff == 1 )
            return Direction.RIGHT;
        else if( colDiff == -1 )
            return Direction.LEFT;
        else
        {
            Console.Error.WriteLine( "### ERROR(Get4WayDirection) ### : Cannot go that way, [{0}, {1}]", rowDiff, colDiff );
            return Direction.LENGTH;
        }
    }
	
	static int GetManhattanHeuristic( Node start, Node end )
    {
        return Math.Abs( end.Row - start.Row ) + Math.Abs( end.Col - start.Col );
    }
	
    static int GetHexHeuristic( Offset pos1, Offset pos2 )
    {
        Hex a = pos1.GetHex();
        Hex b = pos2.GetHex();
        return ( Math.Abs( a.x - b.x ) + Math.Abs( a.y - b.y ) + Math.Abs( a.z - b.z ) );
    }
	
	static List<SearchNode> Get4WayNeighbors( SearchNode current, IMovable grid )
    {
        List<SearchNode> neighbors = new List<SearchNode>();

        for( Direction dir = ( Direction )0; dir < Direction.LENGTH; dir += 1 )
        {
            Node newPos = new Node( current.Pos.Row + Way4Directions[( int )dir, 0], current.Pos.Col + Way4Directions[( int )dir, 1] );

            // for specific constraints
            if( grid != null && grid.IsMovableNode( newPos ) == false )
            	continue;

            SearchNode newNode = new SearchNode( newPos, current, 0, 0 );

            neighbors.Add( newNode );
        }
        return neighbors;
    }
	
	public static List<Direction> Search4WayNodeByAstar( Node start, Node end, IMovable grid, out int distance )
    {
        Console.Error.WriteLine( "Finding from {0} to {1} by A*", start, end );
        distance = -1;

        List<Direction> pathWay = new List<Direction>();
        if( start.Equals( end ) )
        {
            distance = 0;
            return pathWay;
        }

        List<SearchNode> frontier = new List<SearchNode>();
        List<SearchNode> explored = new List<SearchNode>();

        SearchNode startNode = new SearchNode( start, null, 0, GetManhattanHeuristic( start, end ) );
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
                    Direction dir = Get4WayDirection( parent.Parent.Pos, parent.Pos );
                    pathWay.Add( dir );
                    parent = parent.Parent;
                }
                found = true;
                break;
            }

            List<SearchNode> neighbors = Get4WayNeighbors( current, grid );
            foreach( SearchNode node in neighbors )
            {
                if( explored.Contains( node ) )
                    continue;

                node.CostSoFar = current.CostSoFar + 1;
                node.CostToEnd = GetManhattanHeuristic( node.Pos, end );

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

    public static List<Direction> Search4WayNodeByBFS( Node start, ISearch searchGrid, IMovable moveGrid, out int distance )
    {
        Console.Error.WriteLine( "Finding from {0} by BFS", start );
        distance = -1;

        List<Direction> pathWay = new List<Direction>();
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
                    Direction dir = Get4WayDirection( parent.Parent.Pos, parent.Pos );
                    //Console.Error.WriteLine( "iterate back to parent {0}", parent.Pos );
                    pathWay.Add( dir );
                    parent = parent.Parent;
                }
                found = true;
                break;
            }

            List<SearchNode> neighbors = Get4WayNeighbors( current, moveGrid );
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

    static List<HexSearchNode> GetHexNeighbors( HexSearchNode center, Offset endNode )
    {
        //Console.Error.WriteLine( "Finding neighbors of {0}", center );
        List<HexSearchNode> neighbors = new List<HexSearchNode>();

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
            HexSearchNode newNode = new HexSearchNode( newPos, rotation, center.CostSoFar + gridCost, GetHeuristic( newPos, endNode ) );

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