using TreasureHunt;
using TreasureHunt.Models;


//string[] lines = new string[] { "C - 5 - 4", "M - 1 - 0", "M - 2 - 1", "T - 3 - 2 - 2", "A - Lara - 1 - 1 - S - AADADAGGA" };
string? treasureMapDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
string[] lines = File.ReadLines($"{treasureMapDirectory}/initialMap.txt").ToArray();

IMapParser mapParser = new MapParser();
ITreasureHuntService treasureHuntService = new TreasureHuntService();

Map map = treasureHuntService.StartTreasureHunt(mapParser.CreateMap(lines));

IMapSerializer mapSerializer = new MapSerializer();
File.WriteAllText($"{treasureMapDirectory}/map.txt", mapSerializer.SerializeMap(map));