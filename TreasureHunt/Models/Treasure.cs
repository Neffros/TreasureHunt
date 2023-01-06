namespace TreasureHunt.Models;

public record Treasure
{
    public Position Position { get; }

    public Treasure(Position position)
    {
        Position = position;
    }

    public Treasure(int x, int y)
    {
        Position = new Position(x, y);
    }
}