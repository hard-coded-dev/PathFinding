using System;
using System.Collections.Generic;

public interface IMovable
{
    bool IsMovableNode( INode pos );
}

public interface IReachable
{
    bool IsReachedDestination( INode Pos );
}

public interface IHeuristic
{
	int GetHeurisitc( INode from, INode to );
}

public abstract class PathGrid : IMovable, IReachable
{
    public abstract bool IsMovableNode( INode pos );
    public abstract bool IsReachedDestination( INode Pos );
}

public enum Direction4Way
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    LENGTH,
}

public partial class PathFinding
{
    // based on Direction order
    static int[,] Way4Directions = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // { row, col }

    public static int GetManhattanHeuristic( Node start, Node end )
    {
        return Math.Abs( end.Row - start.Row ) + Math.Abs( end.Col - start.Col );
    }

    public static Direction4Way Get4WayDirection( Node from, Node to )
    {
        int rowDiff = to.Row - from.Row;
        int colDiff = to.Col - from.Col;

        if( rowDiff == 1 )
            return Direction4Way.DOWN;
        else if( rowDiff == -1 )
            return Direction4Way.UP;
        else if( colDiff == 1 )
            return Direction4Way.RIGHT;
        else if( colDiff == -1 )
            return Direction4Way.LEFT;
        else
        {
            Console.Error.WriteLine( "### ERROR(Get4WayDirection) ### : Cannot go that way, [{0}, {1}]", rowDiff, colDiff );
            return Direction4Way.LENGTH;
        }
    }

    public static List<SearchNode> Get4WayNeighbors( SearchNode current, IMovable grid )
    {
        List<SearchNode> neighbors = new List<SearchNode>();

        for( Direction4Way dir = ( Direction4Way )0; dir < Direction4Way.LENGTH; dir += 1 )
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
}
