using FluentAssertions;
using NUnit.Framework;
using TreasureHunt.Exceptions;
using TreasureHunt.Models;

namespace TreasureHunt.Tests;


public class MapParserTests
{
    
    private IMapParser _mapParser;

    [OneTimeSetUp]
    public void SetUp()
    {
        _mapParser = new MapParser();
    }

    #region MapWithoutEntitiesTests

    [Test]
    [TestCase("C - 3  - 5", 3 , 5)]
    [TestCase("C - 7 - 2", 7,2)]
    [TestCase("C - 8 - 8 ", 8,8)]
    public void CreateMap_WithDimension_ReturnsParsedMap(string line, int expectedWidth, int expectedHeight)
    {
        Map expected = new (new Dimension(expectedWidth, expectedHeight), new List<Mountain>(), new List<Treasure>(),
            new List<Adventurer>());

        string[] lines = { line };
        
        Map actual = _mapParser.CreateMap(lines);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Test]
    [TestCase(arg: new[]{""})]
    public void CreateMap_WithNoLines_ThrowsInitializationMapException(string[] lines)
    {
        Action createMap = () => _mapParser.CreateMap(lines);
        createMap.Should().Throw<InitializationMapException>();
    }
    
    [Test]
    [TestCase(new[]{"# {C comme Carte} - {Nb. de case en largeur} - {Nb. de case en hauteur}", "C - 5 - 4"}, 5, 4)]
    [TestCase(new[]{"C - 5 - 4", "# {M comme Montagne} - {Axe horizontal} - {Axe vertical}"}, 5, 4)]
    public void CreateMap_WithComment_IgnoresComments(string[] lines, int expectedWidth, int expectedHeight)
    {
        Map expected = new Map(new Dimension(expectedWidth, expectedHeight), new List<Mountain>(), new List<Treasure>(),
            new List<Adventurer>());

        
        Map actual = _mapParser.CreateMap(lines);
        actual.Should().BeEquivalentTo(expected);
    }
    
    #endregion

    #region MapWithEntitiesTests

    #region MountainTests
    private static IEnumerable<object[]> MountainTestData
    {
        get
        {
            yield return new object[]
            {
                new []{ "C - 5 - 4", "M - 1 - 0" },
                new List<Mountain>{new(1,0)}
            };
            yield return new object[]
            {
                new []{ "C - 5 - 4", "M - 1 - 0", "M - 2 - 1" },
                new List<Mountain>{new(1,0), new(2,1)}
            };
        }

    }

    [Test]
    [TestCaseSource(nameof(MountainTestData))]
    public void CreateMap_WithMountains_ReturnsParsedMap(string[] lines, List<Mountain> expectedMountains)
    {

        Map expected = new Map(TestDataUtils.SampleDimension, expectedMountains, new List<Treasure>(),
            new List<Adventurer>());

        Map actual = _mapParser.CreateMap(lines);

        actual.Should().BeEquivalentTo(expected);

    }

    [Test]
    [TestCase(arg: new[]{ "C - 5 - 4", "M - 1 - 0 - 2"})]
    [TestCase(arg: new[]{"C - 5 - 4", "M - 1"})]
    public void CreateMap_WithMountainsWhenWrongFormat_ThrowsInitializationMapException(string[] lines)
    {
        Action createMap = () => _mapParser.CreateMap(lines);
        createMap.Should().Throw<InitializationMapException>();
    }

    #endregion

    #region TreasureTests
    
    private static IEnumerable<object[]> TreasureTestData
    {
        get
        {
            yield return new object[]
            {
                new []{ "C - 5 - 4", "T - 1 - 0 - 1" },
                new List<Treasure> { new(1, 0)}
            };
            yield return new object[]
            {
                new []{ "C - 5 - 4", "T - 1 - 3 - 2" },
                new List<Treasure> { new(1, 3), new(1, 3) }
            };
            yield return new object[]
            {
                new []{ "C - 5 - 4", "T - 1 - 3 - 0" },
                new List<Treasure>()
            };
        }
    }
    
    [Test]
    [TestCaseSource(nameof(TreasureTestData))]
    public void CreateMap_WithTreasures_ReturnsParsedMap(string[] lines, List<Treasure> expectedTreasures)
    {
        Map expected = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), expectedTreasures,
            new List<Adventurer>());

        Map actual = _mapParser.CreateMap(lines);

        actual.Should().BeEquivalentTo(expected);   
    }
    
    [Test]
    [TestCase(arg: new[]{ "C - 5 - 4", "T - 1 - 0 - 2 - 4"})]
    [TestCase(arg: new[]{"C - 5 - 4", "T - X - 1"})]
    public void CreateMap_WithTreasuresWhenWrongFormat_ThrowsInitializationMapException(string[] lines)
    {
        Action createMap = () => _mapParser.CreateMap(lines);
        createMap.Should().Throw<InitializationMapException>();
    }
    
    #endregion


    #region AdventurerTests

    private static IEnumerable<object[]> AdventurerTestData
    {
        get
        {
            yield return new object[]
            {
                new[] { "C - 5 - 4", "A - Lara - 1 - 1 - E - AADADAGGA" },
                new List<Adventurer>
                {
                    new("Lara", new Position(1, 1), Orientation.East,
                        new[] { 'A', 'A', 'D', 'A', 'D', 'A', 'G', 'G', 'A' })
                }
            };
            yield return new object[]
            {
                new[] { "C - 5 - 4", "A - Lara - 1 - 1 - E - AADADAGGA", "A - Indiana - 2 - 3 - S - AADADA" },
                new List<Adventurer>
                {
                    new("Lara", new Position(1, 1), Orientation.East,
                        new[] { 'A', 'A', 'D', 'A', 'D', 'A', 'G', 'G', 'A' }),
                    new("Indiana", new Position(2,3), Orientation.South, 
                        new []{'A', 'A', 'D','A','D','A'})
                }
            };
        }
    }
    
    [Test]
    [TestCaseSource(nameof(AdventurerTestData))]
    public void CreateMap_WithAdventurer_ReturnsParsedMap(string [] lines, List<Adventurer> expectedAdventurers)
    {
        Map expected = new (TestDataUtils.SampleDimension, new List<Mountain>() , new List<Treasure>(),
            expectedAdventurers);

        Map actual = _mapParser.CreateMap(lines);
        actual.Should().BeEquivalentTo(expected);   
    }
    
    [Test]
    [TestCase(arg: new[]{ "C - 5 - 4", "A - 1 - 0 - 2"})]
    [TestCase(arg: new[]{"C - 5 - 4", "A - X - 1"})]
    public void CreateMap_WithAdventurersWhenWrongFormat_ThrowsInitializationMapException(string[] lines)
    {
        Action createMap = () => _mapParser.CreateMap(lines);
        createMap.Should().Throw<InitializationMapException>();
    }

    #endregion
    

    [Test]
    [TestCase(arg: new[]{"C - 5 - 4", "M - 5 - 10"})]
    [TestCase(arg: new[]{"C - 5 - 4", "T - -5 - 4"})]
    public void CreateMap_WithOutOfBoundsEntities_ThrowsInitializationMapException(string[] lines)
    {
        Action createMap = () => _mapParser.CreateMap(lines);
        createMap.Should().Throw<InitializationMapException>();
    }
    
    [Test]
    [TestCase(arg: new[]{"C - 5 - 4", "A - Lara - 1 - 1 - E - P"})]
    [TestCase(arg: new[]{"C - 5 - 4",  "A - Lara - 1 - 1 - E - LU"})]
    public void CreateMap_WithAdventurersWhenInvalidInstructions_ThrowsInitializationMapException(string[] lines)
    {
        Action createMap = () => _mapParser.CreateMap(lines);
        createMap.Should().Throw<InitializationMapException>();
    }
    
    [Test]
    [TestCase(arg: new[]{"C - 5 - 4", "A - Sophie - 1 - 1 - E - A", "A - Lara - 1 - 1 - E - A"})]
    [TestCase(arg: new[]{"C - 5 - 4", "A - Lior - 5 - 4 - E - A", " Sophie - 4 - 4 - E - A", "A - Lara - 4 - 4 - E - A"})]
    public void CreateMap_MapWithEntitiesOverlapping_ThrowsInitializationMapException(string[] lines)
    {
        Action createMap = () => _mapParser.CreateMap(lines);
        createMap.Should().Throw<InitializationMapException>();
    }
    
    [Test]
    public void CreateMap_WithAllEntities_ReturnsParsedMap()
    {
        string[] lines =
        {
            "C - 5 - 4",
            "M - 1 - 1 ",
            "T - 2 - 2 - 2",
            "A - Lara - 3 - 3 - E - AADADAGGA"
        };

        
        Map expected = new (TestDataUtils.SampleDimension, 
            new List<Mountain>(){new(1,1)}, 
            new List<Treasure>(){new(2,2), new (2,2)},
            new List<Adventurer>{new Adventurer("Lara", new Position(3, 3), 
                Orientation.East, new []{'A', 'A', 'D', 'A', 'D', 'A', 'G', 'G', 'A'})});
            
        Map actual = _mapParser.CreateMap(lines);
        actual.Should().BeEquivalentTo(expected);   
    }
    
    #endregion
    

}