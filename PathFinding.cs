using System;
using System.Collections.Generic;

namespace PathFinding
{
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

    public enum Direction8Way
    {
        UPPER_LEFT,
        UP,
        UPPER_RIGHT,
        RIGHT,
        DOWN_RIGHT,
        DOWN,
        DOWN_LEFT,
        LEFT,
        NO_WAY,
    }

    public partial class PathFinding
    {

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

        static int[,] Way4Directions = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // { row, col }
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

        static int GetHexHeuristic( Offset pos1, Offset pos2 )
        {
            Hexagonal a = pos1.GetHex();
            Hexagonal b = pos2.GetHex();
            return ( Math.Abs( a.x - b.x ) + Math.Abs( a.y - b.y ) + Math.Abs( a.z - b.z ) );
        }

        static List<HexSearchNode> GetHexNeighbors( HexSearchNode center, Offset endNode )
        {
            //Console.Error.WriteLine( "Finding neighbors of {0}", center );
            List<HexSearchNode> neighbors = new List<HexSearchNode>();

            var parity = center.Pos.row & 1;    // 0 for even line, 1 for odd line
            var dir = Hexagonal.OffsetDirections[parity];

            const int WIDTH = 23;
            const int HEIGHT = 21;

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

                if( newPos.row < 0 || newPos.row >= HEIGHT )
                    continue;
                if( newPos.col < 0 || newPos.col >= WIDTH )
                    continue;

                // for specific constraints
                //if( IsMovable( newPos, direction ) == false )
                //	continue;

                int gridCost = 1; // + ( int )Caribbean.GetGridProperty( newPos );
                HexSearchNode newNode = new HexSearchNode( newPos, rotation, center.CostSoFar + gridCost, PathFinding.GetHexHeuristic( newPos, endNode ) );

                neighbors.Add( newNode );
            }
            return neighbors;
        }

        // (0,0) : TopLeft, (M,N) : BottomLeft
        public static Direction8Way Get8WayDirection( Node from, Node to )
        {
            int rowDiff = to.Row - from.Row;
            int colDiff = to.Col - from.Col;

            if( rowDiff > 0 )
            {
                if( colDiff > 0 )
                    return Direction8Way.DOWN_RIGHT;
                else if( colDiff < 0 )
                    return Direction8Way.DOWN_LEFT;
                else
                    return Direction8Way.DOWN;
            }
            else if( rowDiff < 0 )
            {
                if( colDiff > 0 )
                    return Direction8Way.UPPER_RIGHT;
                else if( colDiff < 0 )
                    return Direction8Way.UPPER_LEFT;
                else
                    return Direction8Way.UP;
            }
            else
            {
                if( colDiff > 0 )
                    return Direction8Way.RIGHT;
                else if( colDiff < 0 )
                    return Direction8Way.LEFT;
                else
                {
                    Console.Error.WriteLine( "### ERROR(Get8WayDirection) ### : Cannot go that way, [{0}, {1}]", rowDiff, colDiff );
                    return Direction8Way.NO_WAY;
                }
            }
        }

        // UPPER_LEFT, UP, UPPER_RIGHT, RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT,
        static int[,] Way8Directions = new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 } }; // { row, col }
        public static List<SearchNode> Get8WayNeighbors( SearchNode current, IMovable grid )
        {
            List<SearchNode> neighbors = new List<SearchNode>();

            for( Direction8Way dir = ( Direction8Way )0; dir < Direction8Way.NO_WAY; dir += 1 )
            {
                Node newPos = new Node( current.Pos.Row + Way8Directions[( int )dir, 0], current.Pos.Col + Way8Directions[( int )dir, 1] );

                // for specific constraints
                if( grid != null && grid.IsMovableNode( newPos ) == false )
                    continue;

                SearchNode newNode = new SearchNode( newPos, current, 0, 0 );

                neighbors.Add( newNode );
            }
            return neighbors;
        }
    }
}