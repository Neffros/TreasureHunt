
using System.Text.RegularExpressions;
using TreasureHunt.Models;
using TreasureHunt.Exceptions;

namespace TreasureHunt;

public partial class MapParser : IMapParser
{
    
    /// <summary>
    /// Creates a map object with the dimensions and entities defined by the <paramref name="lines"/> parameter
    /// </summary>
    /// <param name="lines"></param>
    /// <exception cref="InitializationMapException"></exception>
    public Map CreateMap(string[] lines)
    {
        //put init verifications into a seperate method? 
        lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        if (lines.Length < 1)
            throw new InitializationMapException("No Data was provided");
        
        lines = lines.Where(line => line[0] != '#').ToArray();
        

        Dimension mapDimension = ParseMapDimension(lines[0]);
        
        List<Mountain> mountains = GenerateMountains(GetLinesOfType(lines, 'M'), mapDimension);
        
        List<Adventurer> adventurers = GenerateAdventurers(GetLinesOfType(lines, 'A'), mapDimension);
        
        VerifyEntitiesAreNotOverlapping(mountains, adventurers);
        List<Treasure> treasures = GenerateTreasures(GetLinesOfType(lines, 'T'), mapDimension);


        return new Map(mapDimension, mountains, treasures, adventurers);
    }


    /// <summary>
    /// Returns the parsed map dimension from the line in parameter.
    /// </summary>
    /// <param name="mapDimensionLine"></param>
    /// <exception cref="InitializationMapException">
    /// When <paramref name="mapDimensionLine"/> does not start with a 'C' character </exception>
    private static Dimension ParseMapDimension(string mapDimensionLine)
    {
        string[] mapDimensionData = RetrieveDataFromLine(mapDimensionLine);

        VerifyLineDataArgumentAmount(mapDimensionData, 3, "map");
        
        if (!mapDimensionData[0].Equals("C"))
            throw new InitializationMapException("First line does not contain map initialization");

        return new Dimension(int.Parse(mapDimensionData[1]), int.Parse(mapDimensionData[2]));

    }

    /// <summary>
    /// Returns the parsed mountains from the lines in parameter
    /// </summary>
    /// <param name="mountainLines"></param>
    /// <param name="mapDimension"></param>
    private static List<Mountain> GenerateMountains(string[] mountainLines, Dimension mapDimension)
    {
        List<Mountain> mountains = new ();
        
        foreach (var mountainLine in mountainLines)
        {
            string[] mountainData = RetrieveDataFromLine(mountainLine);
            
            VerifyLineDataArgumentAmount(mountainData, 3, "mountain");
            
            Position mountainPosition = new (int.Parse(mountainData[1]), int.Parse(mountainData[2]));
            
            VerifyEntityIsInDimension(mountainPosition, mapDimension, "Mountain");
            
            mountains.Add(new Mountain(mountainPosition));
        }
        
        return mountains;
    }
    
    /// <summary>
    /// Returns the parsed treasures from the lines in parameter
    /// </summary>
    /// <param name="treasureLines"></param>
    /// <param name="mapDimension"></param>
    private static List<Treasure> GenerateTreasures(string[] treasureLines, Dimension mapDimension)
    {
        List<Treasure>? treasures = new ();

        foreach (string treasureLine in treasureLines)
        {
            string[] treasureData = RetrieveDataFromLine(treasureLine);
            
            VerifyLineDataArgumentAmount(treasureData, 4, "treasure");
            
            Position treasurePosition = new (int.Parse(treasureData[1]), int.Parse(treasureData[2]));
            
            VerifyEntityIsInDimension(treasurePosition, mapDimension, "Treasure");

            treasures.AddRange(Enumerable.Repeat(new Treasure(treasurePosition), int.Parse(treasureData[3])));
        }

        return treasures;
    }
    
    /// <summary>
    /// Returns the parsed adventurers from the lines in parameter
    /// </summary>
    /// <param name="adventurerLines"></param>
    /// <param name="mapDimension"></param>
    private static List<Adventurer> GenerateAdventurers(string[] adventurerLines, Dimension mapDimension)
    {
        List<Adventurer> adventurers = new List<Adventurer>();

        foreach (var adventurerLine in adventurerLines)
        {
            string[] adventurerData = RetrieveDataFromLine(adventurerLine);

            VerifyLineDataArgumentAmount(adventurerData, 6, "adventurer");

            Position adventurerPosition = new (int.Parse(adventurerData[2]), int.Parse(adventurerData[3]));
            
            VerifyEntityIsInDimension(adventurerPosition, mapDimension, "Adventurer");

            char[] instructions = adventurerData[5].ToCharArray();
            VerifyAdventurerInstructions(instructions);
            Orientation orientation = ParseOrientation(adventurerData[4][0]);
            adventurers.Add(new Adventurer(adventurerData[1], adventurerPosition, orientation, instructions));
        }

        return adventurers;
    }

    /// <summary>
    /// Returns the parsed orientation from the <paramref name="orientationChar"/> in parameter.
    /// Throws InitializationMapException if the character is invalid.
    /// </summary>
    /// <param name="orientationChar"></param>
    /// <exception cref="InitializationMapException"></exception>
    private static Orientation ParseOrientation(char orientationChar)
    {
        return orientationChar switch
        {
            'N' => Orientation.North,
            'S' => Orientation.South,
            'E' => Orientation.East,
            'W' => Orientation.West,
            _ => throw new InitializationMapException("Orientation character is invalid")
        };
    }
    
    /// <summary>
    /// Throws InitializationMapException if the <paramref name="entityPosition"/> is not in the
    /// <paramref name="dimension"/> in parameter
    /// </summary>
    /// <param name="entityPosition"></param>
    /// <param name="dimension"></param>
    /// <param name="entityType">The type of entity to display in the InitializationMapException's message</param>
    /// <exception cref="InitializationMapException"></exception>
    private static void VerifyEntityIsInDimension(Position entityPosition, Dimension dimension, string entityType)
    {
        if (Math.Max(entityPosition.X, dimension.Width) == entityPosition.X || 
            Math.Max(entityPosition.Y, dimension.Height) == entityPosition.Y)
            throw new InitializationMapException($"{entityType} at position {entityPosition.X}" +
                                                 $" - {entityPosition.Y} is out of map's bounds.");
    }
    
    /// <summary>
    /// Returns true if any entity (mountains and adventurers) are not colliding with each other. 
    /// </summary>
    /// <param name="mountains"></param>
    /// <param name="adventurers"></param>
    /// <exception cref="InitializationMapException"></exception>

    private static void VerifyEntitiesAreNotOverlapping(List<Mountain> mountains, List<Adventurer> adventurers)
    {
        List<Position> allPositions = new ();
        
        IEnumerable<Position> mountainPositions = mountains.Select(mountain => mountain.Position);
        IEnumerable<Position> adventurerPositions = adventurers.Select(adventurer => adventurer.Position);
        
        allPositions.AddRange(mountainPositions);
        allPositions.AddRange(adventurerPositions);
        
        var hashset = new HashSet<Position>();
        if (allPositions.Any(position => !hashset.Add(position)))
            throw new InitializationMapException("Entities are overlapping");
    }

    /// <summary>
    /// Throws InitializationMapException if the <paramref name="instructions"/> contains an invalid instruction
    /// </summary>
    /// <param name="instructions"></param>
    /// <exception cref="InitializationMapException"></exception>
    private static void VerifyAdventurerInstructions(IEnumerable<char> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction is not ('A' or 'G' or 'D'))
                throw new InitializationMapException($"{instruction} is not a valid instruction");
        }
    }
    
    /// <summary>
    /// Throws InitializationmapException when there are more or less arguments than <paramref name="expectedLength"/>
    /// in <paramref name="lineData"/>.
    /// </summary>
    /// <param name="lineData">Each word of the line split by each '-' character</param>
    /// <param name="expectedLength"></param>
    /// <param name="entityType">The type of entity to display in the InitializationMapException's message</param>
    /// <exception cref="InitializationMapException"></exception>
    private static void VerifyLineDataArgumentAmount(string[] lineData, int expectedLength, string entityType)
    {
        if(lineData.Length > expectedLength)
            throw new InitializationMapException($"Too many arguments in a {entityType} line: {lineData}");
        if (lineData.Length < expectedLength)
            throw new InitializationMapException($"Not enough arguments in a {entityType} line: {lineData}");
    }
    
    /// <summary>
    /// Returns all lines starting with the char <paramref name="type"/> in the <paramref name="lines"/> parameter.
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="type"></param>
    private static string[] GetLinesOfType(string[] lines, char type)
    {
        return (from line in lines where line[0] == type select line).ToArray();
    }
    
    /// <summary>
    /// Returns the <paramref name="line"/> parameter split and trimmed into an array of string.
    /// Throws InitializationMapException if the line contains a negative number.
    /// </summary>
    /// <param name="line"></param>
    /// <exception cref="InitializationMapException"></exception>
    private static string[] RetrieveDataFromLine(string line)
    {
        if (NegativeNumberRegex().IsMatch(line))
            throw new InitializationMapException($"Unauthorized negative number at line: {line}");
        
        string[] data = line.Split('-').Select(characters => characters.Trim()).ToArray();

        return data;
    }

    [GeneratedRegex("[-][1-9]+")]
    private static partial Regex NegativeNumberRegex();
}