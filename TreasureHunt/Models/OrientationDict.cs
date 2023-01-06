namespace TreasureHunt.Models;

public readonly struct OrientationDict
{
    public static readonly Dictionary<Orientation, Position> OrientationToPosition = new()
    {
        { Orientation.North, new Position(0, -1) },
        { Orientation.East,  new Position(1, 0) },
        { Orientation.South, new Position(0, 1)},
        { Orientation.West,  new Position(-1 ,0)}
    };
}
