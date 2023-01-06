using FluentAssertions;
using NUnit.Framework;
using TreasureHunt.Models;

namespace TreasureHunt.Tests;

public class MapSerializerTests
{
    private IMapSerializer _mapSerializer;

    [OneTimeSetUp]
    public void SetUp()
    {
        _mapSerializer = new MapSerializer();
    }
    
    [Test]
    public void ShouldSerialize_Map()
    {
        Map map = new Map(TestDataUtils.SampleDimension,
            new List<Mountain>() { new(1, 1), new(2, 1) },
            new List<Treasure>() { new(2, 2) },
            new List<Adventurer>() { new("lara", 4, 2, Orientation.East, new[] { 'A' }) });
        
        string expectedOutput = "# {C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}\n" +
                                "C - 5 - 4\n" +
                                "# {M comme Montagne} - {Axe horizontal} - {Axe vertical}\n" +
                                "M - 1 - 1\n" +
                                "M - 2 - 1\n" +
                                "# {T comme Trésor} - {Axe horizontal} - {Axe vertical} - {Nb. de trésors restants}\n" +
                                "T - 2 - 2 - 1\n" +
                                "# {A comme Aventurier} - {Nom de l’aventurier} - {Axe horizontal} - {Axe vertical} " +
                                "- {Orientation} - {Nb. trésors ramassés}\n" +
                                "A - lara - 4 - 2 - E - 0";
        
        _mapSerializer.SerializeMap(map).Should().BeEquivalentTo(expectedOutput);
    }

    [Test]
    [TestCase(3, 5)]
    [TestCase(10, 10)]
    public void SerializeMap_DimensionLines_ReturnsSerializedMap(int width, int height)
    {
        Map map = new Map(new Dimension(width, height), new List<Mountain>(), new List<Treasure>(), new List<Adventurer>());
        _mapSerializer.SerializeMap(map).Should().BeEquivalentTo($"# {{C comme Carte}} - {{Nb. de case en largeur}} - {{Nb. de case en hauteur}}\nC - {width.ToString()} - {height.ToString()}");
    }
    
    
    [Test]
    public void SerializeMap_WithMountains_ReturnsSerializedMap()
    {
        List<Mountain> mountains = new List<Mountain>
        {
            new(3, 3),
            new(2, 2),
            new(1, 1)
        };
        string expectedOutput = "# {C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}\n" +
                                "C - 5 - 4\n" +
                                "# {M comme Montagne} - {Axe horizontal} - {Axe vertical}\n" +
                                "M - 3 - 3\n" +
                                "M - 2 - 2\n" +
                                "M - 1 - 1";
        Map map = new Map(TestDataUtils.SampleDimension, mountains, new List<Treasure>(), new List<Adventurer>());
        
        _mapSerializer.SerializeMap(map).Should().BeEquivalentTo(expectedOutput);
    }
    
    
    [Test]
    public void SerializeMap_WithTreasures_ReturnsSerializedMap()
    {

        List<Treasure> treasures = new List<Treasure>
        {
            new(2, 2),
            new(2, 2),
            new(1, 1)
        };

        string expectedOutput = "# {C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}\n" +
                                "C - 5 - 4\n" +
                                "# {T comme Trésor} - {Axe horizontal} - {Axe vertical} - {Nb. de trésors restants}\n" +
                                "T - 2 - 2 - 2\n" +
                                "T - 1 - 1 - 1";
        
        Map map = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), treasures, new List<Adventurer>());
        
        _mapSerializer.SerializeMap(map).Should().BeEquivalentTo(expectedOutput);
    }


    [Test]
    public void SerializeMap_WithAdventurers_ReturnsSerializedMap()
    {

        List<Adventurer> adventurers = new List<Adventurer>
        {
            new("sophie", new Position(1, 1), Orientation.South, Array.Empty<char>()),
            new("lara", new Position(3, 3), Orientation.East, Array.Empty<char>(), 2),
        };

        string expectedOutput = "# {C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}\n" +
                                "C - 5 - 4\n" +
                                "# {A comme Aventurier} - {Nom de l’aventurier} - {Axe horizontal} - {Axe vertical} " +
                                "- {Orientation} - {Nb. trésors ramassés}\n" +
                                "A - sophie - 1 - 1 - S - 0\n" +
                                "A - lara - 3 - 3 - E - 2";
        
        Map map = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), new List<Treasure>(), adventurers);
        
        _mapSerializer.SerializeMap(map).Should().BeEquivalentTo(expectedOutput);
    }

}