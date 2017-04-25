using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Helper Functions
/// </summary>
public static class DebugHelper
{
    public static string ToDebugString<TKey, TValue>( this IDictionary<TKey, TValue> dictionary )
    {
        if( typeof( TValue ) == typeof( Entity ) )
        {
            return "{" + string.Join( ",", dictionary.Select( kv => kv.Key + "=" + ( kv.Value as Entity ).ToString() ).ToArray() ) + "}";
        }
        else
        {
            return "{" + string.Join( ",", dictionary.Select( kv => kv.Key + "=" + kv.Value ) ) + "}";
        }
    }

    public static string ToDebugString<TValue>( this IList<TValue> list )
    {
        return "{" + string.Join( ",", list ) + "}";
    }
}
