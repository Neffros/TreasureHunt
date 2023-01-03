namespace TreasureHunt.Models;

public record Adventurer
{
    private Position _position;

    public Adventurer(string name, Position position, Orientation orientation, IEnumerable<char> instructions, uint nbTreasures = 0)
    {
        Name = name;
        _position = position;
        Orientation = orientation;
        Instructions = new Queue<char>();
        foreach (var instruction in instructions)
        {
            Instructions.Enqueue(instruction);
        }

        NbTreasures = nbTreasures;
    }
    
    public Adventurer(string name, int x, int y, Orientation orientation, IEnumerable<char> instructions, uint nbTreasures = 0)
    {
        Name = name;
        _position = new Position(x, y);
        Orientation = orientation;
        Instructions = new Queue<char>();
        foreach (var instruction in instructions)
        {
            Instructions.Enqueue(instruction);
        }

        NbTreasures = nbTreasures;
    }

    public Position Position
    {
        get => _position;
        set => _position = value ?? throw new ArgumentNullException(nameof(value));
    }

    private Orientation Orientation { get; set; }

    private uint NbTreasures { get; set; }

    private string Name { get; }

    private Queue<char> Instructions { get; }

};