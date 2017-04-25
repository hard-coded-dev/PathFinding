
public class Node
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
        return ( other.Pos.Row == Pos.Row && other.Pos.Col == Pos.Col );
    }

    public override int GetHashCode()
    {
        return Pos.Row ^ Pos.Col;
    }

    public override string ToString()
    {
        return "[" + Pos.Row + ", " + Pos.Col + " : " + ( CostSoFar + CostToEnd ) + "]";
    }
};
