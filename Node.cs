using System;
using System.Collections.Generic;

namespace PathFinding
{
    public interface INode
    {
    }

    public class Node : INode
    {
        public Node() : this( 0, 0 )
        {
        }
        public Node( int row, int col )
        {
            Row = row;
            Col = col;
        }
        public int Row { get; set; }
        public int Col { get; set; }

        public override int GetHashCode()
        {
            return Row ^ Col;
        }

        public override string ToString()
        {
            return "[" + Row + ", " + Col + "]";
        }

        public bool Equals( Node other )
        {
            return ( other.Row == Row && other.Col == Col );
        }
    };

    public class SearchNode : IEquatable<SearchNode>
    {
        public SearchNode( Node item, SearchNode parent, int costFrom, int costEnd )
        {
            Pos = item;
            Parent = parent;
            CostSoFar = costFrom;
            CostToEnd = costEnd;
        }

        public SearchNode Parent { get; set; }
        public Node Pos { get; set; }
        public int CostSoFar { get; set; }
        public int CostToEnd { get; set; }

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
            return Pos.GetHashCode();
        }

        public override string ToString()
        {
            return Pos.ToString() + "-> [" + ( CostSoFar + CostToEnd ) + "]";
        }
    };


    public class GraphNode : INode
    {
        public GraphNode( int _id, int _cost, int _node1, int _node2 )
        {
            Id = _id;
            Cost = _cost;
            NextId1 = _node1;
            NextId2 = _node2;
            Neighbors = new List<GraphNode>();
        }
        public int Id;
        public int Cost;
        // adjacent node's id
        public int NextId1;
        public int NextId2;

        List<GraphNode> Neighbors;

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return "[" + Id + " : " + Cost + "]";
        }

        public bool Equals( GraphNode other )
        {
            return ( other.Id == Id );
        }
    }

}
