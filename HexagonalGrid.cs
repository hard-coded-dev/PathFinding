using System;

public class Hexagonal
{
    public int x;
    public int y;
    public int z;
	
	public static int[][,] OffsetDirections = new int[2][,] { // { col, row }
					// 0,1,2,3,4,5
		new int[,] { {+1,  0}, { 0, -1}, {-1, -1}, {-1,  0}, {-1, +1}, { 0, +1} },		// for even line
		new int[,] { {+1,  0}, {+1, -1}, { 0, -1}, {-1,  0}, { 0, +1}, {+1, +1} } };	// for odd line


    public Hexagonal( int _x, int _y, int _z )
    {
        x = _x;
        y = _y;
        z = _z;
    }

    // convert cube to odd-r offset
    public Offset GetOffset( int x, int y, int z )
    {
        int col = x + ( z - ( z & 1 ) ) / 2;
        int row = z;
        return new Offset( row, col );
    }

    public int GetDistance( Hexagonal other )
    {
        return ( Math.Abs( other.x - x ) + Math.Abs( other.y - y ) + Math.Abs( other.z - z ) ) / 2;
    }

    public static int GetDistance( Hexagonal a, Hexagonal b )
    {
        return ( Math.Abs( a.x - b.x ) + Math.Abs( a.y - b.y ) + Math.Abs( a.z - b.z ) ) / 2;
    }

    static Hexagonal[] rotations = new Hexagonal[6] {
       new Hexagonal( +1, -1, 0 ), new Hexagonal( +1, 0, -1 ), new Hexagonal( 0, +1, -1 ),
       new Hexagonal( -1, +1, 0 ), new Hexagonal( -1, 0, +1 ), new Hexagonal( 0, -1, +1 )
    };

    public static int GetRotation( Hexagonal from, Hexagonal to )
    {
        int x = to.x - from.x;
        int y = to.y - from.y;
        int z = to.z - from.z;
        for( int i = 0; i < 6; ++i )
        {
            if( rotations[i].x == x && rotations[i].y == y && rotations[i].z == z )
                return i;
        }
        return 0;
    }

}

public class Offset
{
    public int row;
    public int col;

    public Offset()
    {
    }

    public Offset( Offset other )
        : this( other.row, other.col )
    {
    }

    public Offset( int _row, int _col )
    {
        row = _row;
        col = _col;
    }

    // convert odd-r offset to cube
    public Hexagonal GetHex()
    {
        int x = col - ( row - ( row & 1 ) ) / 2;
        int z = row;
        int y = -x - z;
        return new Hexagonal( x, y, z );
    }

    static Hexagonal GetHex( Offset other )
    {
        int x = other.col - ( other.row - ( other.row & 1 ) ) / 2;
        int z = other.row;
        int y = -x - z;
        return new Hexagonal( x, y, z );
    }

    public int GetDistance( Offset other )
    {
        Hexagonal a = GetHex();
        Hexagonal b = Offset.GetHex( other );
        return Hexagonal.GetDistance( a, b );
    }

    public static int GetDistance( Offset a, Offset b )
    {
        Hexagonal ac = a.GetHex();
        Hexagonal bc = b.GetHex();
        return Hexagonal.GetDistance( ac, bc );
    }

    public override bool Equals( object obj )
    {
        return Equals( obj as Offset );
    }

    public bool Equals( Offset other )
    {
        return ( other.row == row && other.col == col );
    }

    public override int GetHashCode()
    {
        return row ^ col;
    }

    public override string ToString()
    {
        return "[" + row + ", " + col + "]";
    }

    public static int GetRotation( Offset from, Offset to )
    {
        return Hexagonal.GetRotation( from.GetHex(), to.GetHex() );
    }
}


public class HexSearchNode : IEquatable<HexSearchNode>
{
    public Offset Pos { get; set; }
    public int Rotation { get; set; }
    public int CostSoFar { get; set; }
    public int CostToEnd { get; set; }
    public HexSearchNode Parent { get; set; }

    public HexSearchNode( Offset pos, int rotation, int costFrom, int costEnd )
    {
        Pos = pos;
        Rotation = rotation;
        CostSoFar = costFrom;
        CostToEnd = costEnd;
    }

    public override bool Equals( object obj )
    {
        return Equals( obj as HexSearchNode );
    }

    public bool Equals( HexSearchNode other )
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
