using System;

public class Hex
{
    public int x;
    public int y;
    public int z;
	
	public static int[][,] OffsetDirections = new int[2][,] { // { col, row }
					// 0,1,2,3,4,5
		new int[,] { {+1,  0}, { 0, -1}, {-1, -1}, {-1,  0}, {-1, +1}, { 0, +1} },		// for even line
		new int[,] { {+1,  0}, {+1, -1}, { 0, -1}, {-1,  0}, { 0, +1}, {+1, +1} } };	// for odd line


    public Hex( int _x, int _y, int _z )
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

    public int GetDistance( Hex other )
    {
        return ( Math.Abs( other.x - x ) + Math.Abs( other.y - y ) + Math.Abs( other.z - z ) ) / 2;
    }

    public static int GetDistance( Hex a, Hex b )
    {
        return ( Math.Abs( a.x - b.x ) + Math.Abs( a.y - b.y ) + Math.Abs( a.z - b.z ) ) / 2;
    }

    static Hex[] rotations = new Hex[6] {
       new Hex( +1, -1, 0 ), new Hex( +1, 0, -1 ), new Hex( 0, +1, -1 ),
       new Hex( -1, +1, 0 ), new Hex( -1, 0, +1 ), new Hex( 0, -1, +1 )
    };

    public static int GetRotation( Hex from, Hex to )
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
    public Hex GetHex()
    {
        int x = col - ( row - ( row & 1 ) ) / 2;
        int z = row;
        int y = -x - z;
        return new Hex( x, y, z );
    }

    static Hex GetHex( Offset other )
    {
        int x = other.col - ( other.row - ( other.row & 1 ) ) / 2;
        int z = other.row;
        int y = -x - z;
        return new Hex( x, y, z );
    }

    public int GetDistance( Offset other )
    {
        Hex a = GetHex();
        Hex b = Offset.GetHex( other );
        return Hex.GetDistance( a, b );
    }

    public static int GetDistance( Offset a, Offset b )
    {
        Hex ac = a.GetHex();
        Hex bc = b.GetHex();
        return Hex.GetDistance( ac, bc );
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
        return Hex.GetRotation( from.GetHex(), to.GetHex() );
    }
}
