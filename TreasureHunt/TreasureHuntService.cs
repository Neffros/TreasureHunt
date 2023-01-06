using TreasureHunt.Models;

namespace TreasureHunt;

public class TreasureHuntService : ITreasureHuntService
{

    /// <summary>
    /// Applies the instructions of the adventurers on the map in parameter, returns the updated map
    /// </summary>
    /// <param name="map"></param>
    public Map StartTreasureHunt(Map map)
    {
        while (map.Adventurers.Any(adventurer => adventurer.Instructions.Count != 0))
        {
            map = HuntOneTurn(map);
        }

        return map;
    }

    /// <summary>
    /// Applies the next instruction of all adventurers in the map in parameter.
    /// </summary>
    /// <param name="map"></param>
    private static Map HuntOneTurn(Map map)
    {
        foreach (var adventurer in map.Adventurers)
        {
            if (adventurer.Instructions.Count == 0)
                continue;
            char instruction = adventurer.Instructions.Dequeue();
            switch (instruction)
            {
                case 'A':
                    map = Advance(map, adventurer);
                    break;
                case 'G':
                    adventurer.Orientation = TurnLeft(adventurer.Orientation);
                    break;
                case 'D':
                    adventurer.Orientation = TurnRight(adventurer.Orientation);
                    break;
            }
        }
        return map;
    }
    
    /// <summary>
    /// Returns the map in parameter with the position of the adventurer in parameter advanced by 1 based on its orientation if not colliding with another entity(mountain or adventurer).
    /// </summary>
    /// <param name="map"></param>
    /// <param name="adventurer"></param>
    private static Map Advance(Map map, Adventurer adventurer)
    {
        Position newPosition = GetAdventurerNewPosition(adventurer);

        if (IsCollidingWithEntityOrEdge(newPosition, map.Dimension, map.Mountains, map.Adventurers))
            return map;
        adventurer.Position = newPosition;

        return !IsCollidingWithTreasure(newPosition, map.Treasures) ? map : CollectTreasure(map, adventurer);
    }
    
    /// <summary>
    /// Returns the orientation to the left of the orientation in parameter.
    /// </summary>
    /// <param name="initialOrientation"></param>
    private static Orientation TurnLeft(Orientation initialOrientation)
    {
        return (Orientation)(((int)initialOrientation - 90 + 360) % 360);
    }
    
    /// <summary>
    /// Returns the orientation to the left of the orientation in parameter.
    /// </summary>
    /// <param name="initialOrientation"></param>
    private static Orientation TurnRight(Orientation initialOrientation)
    {
        return (Orientation)(((int)initialOrientation + 90 + 360) % 360);
    }

    /// <summary>
    /// Returns true if the position in parameter is the same as a treasure.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="treasures"></param>
    private static bool IsCollidingWithTreasure(Position position, IEnumerable<Treasure> treasures)
    {
        return treasures.Any(treasure => treasure.Position == position);
    }
    
    /// <summary>
    /// Returns the new position of the adventurer when he advances.
    /// </summary>
    /// <param name="adventurer"></param>
    private static Position GetAdventurerNewPosition(Adventurer adventurer)
    {
        Position positionModifier = OrientationDict.OrientationToPosition.First(orientation => orientation.Key == adventurer.Orientation).Value;
        
        int x = adventurer.Position.X + positionModifier.X;
        int y = adventurer.Position.Y + positionModifier.Y;
        
        return new Position(x, y);
    }

    /// <summary>
    /// Returns true if the position in parameter collides with another entity (treasure or mountain) or is not in the dimension.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapDimension"></param>
    /// <param name="mountains"></param>
    /// <param name="adventurers"></param>
    private static bool IsCollidingWithEntityOrEdge(Position position, Dimension mapDimension,
        IEnumerable<Mountain> mountains, IEnumerable<Adventurer> adventurers)
    {
        return IsNotInMap(position, mapDimension) || IsCollidingWithEntity(position, mountains, adventurers);
    }
    
    /// <summary>
    /// Returns true if the position in parameter is between 0 and the dimension in parameter.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dimension"></param>
    private static bool IsNotInMap(Position position, Dimension dimension)
    {
        if (Math.Max(position.X, dimension.Width) > dimension.Width ||
            Math.Max(position.Y, dimension.Height) > dimension.Height)
            return true;
        return position.X < 0 || position.Y < 0;
    }

    /// <summary>
    /// Returns true if the position in parameter is the same as an entity (mountain or adventurer).
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mountains"></param>
    /// <param name="adventurers"></param>
    private static bool IsCollidingWithEntity(Position position, IEnumerable<Mountain> mountains,
        IEnumerable<Adventurer> adventurers)
    {
        return mountains.Any(mountain => mountain.Position == position) || adventurers.Any(adventurer => adventurer.Position == position);
    }
    
    /// <summary>
    /// Returns the map with the adventurer's treasure count increased by one and removes the treasure from the map.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="adventurer"></param>
    private static Map CollectTreasure(Map map, Adventurer adventurer)
    {
        adventurer.NbTreasures += 1;
        List<Treasure> treasures = map.Treasures;
        Treasure treasure = treasures.First(treasure => treasure.Position == adventurer.Position);
        treasures.Remove(treasure);
        
        return map with { Treasures = treasures }; 
    }
}