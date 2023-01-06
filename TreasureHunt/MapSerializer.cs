using TreasureHunt.Models;

namespace TreasureHunt;

public class MapSerializer : IMapSerializer
{
    /// <summary>
    /// Returns a string containing the information of the map in the format:
    /// <list type="bullet">
    /// <item><description>{C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}</description></item>
    /// <item><description>{M comme Montagne} - {Axe horizontal} - {Axe vertical}</description></item>
    /// <item><description>{T comme Trésor} - {Axe horizontal} - {Axe vertical} - {Nb. de trésors restants}</description></item>
    /// <item><description>{A comme Aventurier} - {Nom de l’aventurier} - {Axe horizontal} - {Axe vertical} - {Orientation} - {Nb. trésors ramassés}</description></item>
    /// </list>
    /// </summary>
    /// <example>
    /// <list type="bullet">
    /// <item><description>C - 5 - 4 </description></item>
    /// <item><description>M - 1 - 1 </description></item>
    /// <item><description>M - 2 - 1 </description></item>
    /// <item><description>T - 2 - 2 - 1 </description></item>
    /// <item><description>A - lara - 4 - 2 - E - 0</description></item>
    /// </list></example>
    /// <param name="map"></param>
    public string SerializeMap(Map map)
    {
        string serializedMap = SerializeMapDimension(map.Dimension);

        serializedMap += SerializeMountains(map.Mountains);

        serializedMap += SerializeTreasures(map.Treasures);

        serializedMap += SerializeAdventurers(map.Adventurers);
        
        return serializedMap.TrimEnd('\n');;
    }

    /// <summary>
    /// Serializes the dimension in the format: {C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}
    /// </summary>
    /// <param name="dimension"></param>
    private static string SerializeMapDimension(Dimension dimension)
    {
        const string dimensionLines = "# {C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}\n";
        return dimensionLines + $"C - {dimension.Width} - {dimension.Height}\n";
    }

    /// <summary>
    /// Serializes the mountains in the format: {M comme Montagne} - {Axe horizontal} - {Axe vertical}
    /// </summary>
    /// <param name="mountains"></param>
    private static string SerializeMountains(IReadOnlyCollection<Mountain> mountains)
    {
        if (!mountains.Any())
            return "";
        
        const string mountainLines = "# {M comme Montagne} - {Axe horizontal} - {Axe vertical}\n";
        return mountainLines + mountains.Aggregate("", (current, mountain) => 
            current + ($"M - {mountain.Position.X} - {mountain.Position.Y}\n"));
    }

    /// <summary>
    /// Serializes the treasures in the format: {T comme Trésor} - {Axe horizontal} - {Axe vertical} - {Nb. de trésors restants}
    /// </summary>
    /// <param name="treasures"></param>
    private static string SerializeTreasures(IReadOnlyCollection<Treasure> treasures)
    {
        if (!treasures.Any())
            return "";
        const string treasureLines = "# {T comme Trésor} - {Axe horizontal} - {Axe vertical} - {Nb. de trésors restants}\n";

        Dictionary<Treasure, int> samePositionTreasures = treasures.GroupBy(treasure => treasure).
            ToDictionary(treasure => treasure.Key, treasure => treasure.Count());
        
        
        return treasureLines + samePositionTreasures.Aggregate("", (current, treasure) 
            => current + ($"T - {treasure.Key.Position.X} - {treasure.Key.Position.Y} - {treasure.Value}\n"));
    }

    /// <summary>
    /// Serializes the adventurers in the format: {A comme Aventurier} - {Nom de l’aventurier} - {Axe horizontal} - {Axe vertical} - {Orientation} - {Nb. trésors ramassés}
    /// </summary>
    /// <param name="adventurers"></param>
    private static string SerializeAdventurers(IReadOnlyCollection<Adventurer> adventurers)
    {
        if (!adventurers.Any())
            return "";
        
        const string adventurerLines = "# {A comme Aventurier} - {Nom de l’aventurier} - {Axe horizontal} - {Axe vertical} " +
                                       "- {Orientation} - {Nb. trésors ramassés}\n";
        return adventurerLines + adventurers.Aggregate("", (current, adventurer) =>
            current + ($"A - {adventurer.Name} - {adventurer.Position.X} - {adventurer.Position.Y} " +
                       $"- {adventurer.Orientation.ToString()[0]} - {adventurer.NbTreasures}\n"));
    }
}