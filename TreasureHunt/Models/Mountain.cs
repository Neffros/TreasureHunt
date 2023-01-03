namespace TreasureHunt.Models;

public record Mountain
{
    public Position Position { get; }

    public Mountain(Position position)
    {
        Position = position;
    }

    public Mountain(int x, int y)
    {
        Position = new Position(x, y);
    }
    
}