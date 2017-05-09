using System;
using System.Collections.Generic;

namespace PathFinding
{
    public partial class PathFinding
    {
        
        public static List<Offset> FindClosedPosByAStar( Offset startNode, int startDirection, Offset endNode, out List<int> rotationPathWay )
        {
            Console.Error.WriteLine( "Path finding from {0} to {1} at direction of {2}", startNode, endNode, startDirection );

            rotationPathWay = new List<int>();
            List<Offset> pathWay = new List<Offset>();

            if( startNode.Equals( endNode ) )
                return pathWay;

            List<HexSearchNode> frontier = new List<HexSearchNode>();
            List<HexSearchNode> explored = new List<HexSearchNode>();

            HexSearchNode start = new HexSearchNode( startNode, startDirection, 0, GetHexHeuristic( startNode, endNode ) );
            frontier.Add( start );

            bool found = false;
            while( frontier.Count > 0 )
            {
                HexSearchNode current = frontier[0];
                frontier.RemoveAt( 0 );
                explored.Add( current );

                if( current.Pos.Equals( endNode ) )
                {
                    HexSearchNode parent = current;

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
                    List<HexSearchNode> neighbors = GetHexNeighbors( current, endNode );
                    foreach( HexSearchNode node in neighbors )
                    {
                        if( explored.Contains( node ) )
                            continue;

                        int costSoFar = current.CostSoFar;
                        int costToEnd = PathFinding.GetHexHeuristic( node.Pos, endNode );

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
                HexSearchNode parent = explored[0];
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
}