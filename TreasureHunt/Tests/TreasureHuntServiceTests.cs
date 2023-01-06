using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using TreasureHunt.Models;

namespace TreasureHunt.Tests;

public class TreasureHuntServiceTests
{
    private ITreasureHuntService _treasureHuntService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _treasureHuntService = new TreasureHuntService();
    }
    #region AdventurerMoveTests
    
    [Test]
    [TestCase(Orientation.North, 1, 0)]
    [TestCase(Orientation.East, 2, 1)]
    [TestCase(Orientation.West, 0, 1)]
    [TestCase(Orientation.South, 1, 2)]
    public void StartTreasureHunt_AdventurerWithAdvanceInstruction_ReturnsUpdatedMap(Orientation adventurerOrientation, int expectedX, int expectedY)
    {
        Adventurer adventurer = new("Sophie", TestDataUtils.SamplePosition, adventurerOrientation, 
            new[] { 'A' });
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(), new List<Adventurer> {adventurer});

        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].Position.Should().BeEquivalentTo(new Position(expectedX, expectedY));
    }

    [Test]
    public void StartTreasureHunt_AdventurersWithMultipleAdvanceInstructions_ReturnsUpdatedMap()
    {
        List<Adventurer> initialAdventurers = new List<Adventurer>()
        {
            new("Sophie", TestDataUtils.SamplePosition, Orientation.South,
                new[] { 'A', 'A', 'A' }),
            new("Lara", new Position(0, 4), Orientation.East,
                new[] { 'A', 'A', 'A' })
        };

        List<Adventurer> expectedAdventurers = new List<Adventurer>()
        {
            new("Sophie", new Position(1,4), Orientation.South,
                Array.Empty<char>()),
            new("Lara", new Position(3, 4), Orientation.East,
                Array.Empty<char>())
        };
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(), initialAdventurers);
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers.Should().BeEquivalentTo(expectedAdventurers);
    }
    
    [Test]
    public void StartTreasureHunt_AdventurersWithDifferentInstructionLengths_ReturnsUpdatedMap()
    {
        List<Adventurer> initialAdventurers = new List<Adventurer>
        {
            new("Sophie", TestDataUtils.SamplePosition, Orientation.South,
                new[] { 'A' }),
            new("Lara", new Position(0, 4), Orientation.East,
                new[] { 'A', 'A', 'A', 'A' })
        };
        List<Adventurer> expectedAdventurers = new List<Adventurer>
        {
            new("Sophie", new Position(1, 2), Orientation.South, Array.Empty<char>()),
            new("Lara", new Position(4, 4), Orientation.East, Array.Empty<char>())
        };
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), new List<Treasure>(),
            initialAdventurers);
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers.Should().BeEquivalentTo(expectedAdventurers);
    }
    #endregion

    #region AdventurerTurnTests

    [Test]
    [TestCase(Orientation.North, Orientation.East)]
    [TestCase(Orientation.East, Orientation.South)]
    [TestCase(Orientation.South, Orientation.West)]
    [TestCase(Orientation.West, Orientation.North)]
    public void StartTreasureHunt_AdventurerTurnRight_ReturnsUpdatedMap(Orientation initialOrientation, 
        Orientation expectedOrientation)
    {
        Adventurer adventurer = new("Sophie", TestDataUtils.SamplePosition, initialOrientation, 
            new [] {'D'});
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(), new List<Adventurer> {adventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].Orientation.Should().Be(expectedOrientation);
    }
    
    [Test]
    [TestCase(Orientation.North, Orientation.West)]
    [TestCase(Orientation.East, Orientation.North)]
    [TestCase(Orientation.South, Orientation.East)]
    [TestCase(Orientation.West, Orientation.South)]
    public void StartTreasureHunt_AdventurerTurnLeft_ReturnsUpdatedMap(Orientation initialOrientation, 
        Orientation expectedOrientation)
    {
        Adventurer adventurer = new("Sophie", TestDataUtils.SamplePosition, initialOrientation, 
            new [] {'G'});
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(), new List<Adventurer> {adventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].Orientation.Should().Be(expectedOrientation);
    }
    
    [Test]
    public void StartTreasureHunt_AdventurerTurnsMultipleTimes_ReturnsUpdatedMap()
    {
        Adventurer adventurer = new("Sophie", TestDataUtils.SamplePosition, Orientation.North, 
            new [] {'G', 'G', 'D'});
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(), new List<Adventurer> {adventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].Orientation.Should().Be(Orientation.West);
    }

    #endregion

    #region AdventurerMoveAndTurnTests

    [Test]
    public void StartTreasureHunt_AdventurerMovesAndTurns_ReturnsUpdatedMap()
    {
        Adventurer initialAdventurer = new("Sophie", TestDataUtils.SamplePosition, Orientation.East, 
            new[] { 'A', 'D', 'A' });
        
        Adventurer expectedAdventurer = new("Sophie", new Position(2,2), Orientation.South, 
        Array.Empty<char>());
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(), new List<Adventurer> {initialAdventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].Should().BeEquivalentTo(expectedAdventurer);
    }
    
    #endregion

    #region AdventurerCollisionTests

    [Test]
    public void StartTreasureHunt_AdventurerMoveIntoMountain_ReturnsUpdatedMapWithSameAdventurerPosition()
    {
        
        Adventurer initialAdventurer = new("Sophie", TestDataUtils.SamplePosition, Orientation.East, 
            new[] { 'A' });
        
        Adventurer expectedAdventurer = new("Sophie", TestDataUtils.SamplePosition, Orientation.East, 
            Array.Empty<char>());
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(){new(2,1)}, new List<Treasure>(),
            new List<Adventurer> {initialAdventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].Should().BeEquivalentTo(expectedAdventurer);
    }


    [Test]
    public void StartTreasureHunt_AdventurerMoveIntoOtherAdventurer_ReturnsUpdatedMapWithAdventurerSamePosition()
    {
        List<Adventurer> initialAdventurers = new List<Adventurer>
        {
            new("Sophie", new Position(2, 2), Orientation.North, new[] { 'A' }),
            new("Lara", new Position(2, 1), Orientation.East, new[] { 'A' })
        };
        List<Adventurer> expectedAdventurers = new List<Adventurer>
        { 
            new ("Sophie", new Position(2, 2), Orientation.North, Array.Empty<char>()),
            new ( "Lara", new Position(3,1), Orientation.East, Array.Empty<char>())
        };
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), new List<Treasure>(),
            initialAdventurers);
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers.Should().BeEquivalentTo(expectedAdventurers);
    }

    [Test]
    [TestCase(0,0, Orientation.North)]
    [TestCase(0,0, Orientation.West)]
    [TestCase(5,0, Orientation.East)]
    [TestCase(0,4, Orientation.South)]
    public void StartTreasureHunt_AdventurerMoveOutsideMap_ReturnsUpdatedMapWithAdventurerSamePosition(int posX, int posY, Orientation adventurerOrientation)
    {
        Adventurer initialAdventurer = new Adventurer("Sophie", new Position(posX, posY), adventurerOrientation,
            new[] { 'A' }); 
        
        Adventurer expectedAdventurer = new Adventurer("Sophie", new Position(posX, posY), adventurerOrientation,
            Array.Empty<char>()); 
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(), new List<Adventurer> {initialAdventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].Should().BeEquivalentTo(expectedAdventurer);
    }
    
    [Test]
    public void StartTreasureHunt_AdventurerCollectsTreasure_ReturnsUpdatedMap()
    {
        Adventurer initialAdventurer = new Adventurer("Sophie", TestDataUtils.SamplePosition, Orientation.East, 
            new[] { 'A' }, 0);
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), 
            new List<Treasure>(){new (2,1)}, new List<Adventurer> {initialAdventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        
        actual.Adventurers[0].NbTreasures.Should().Be(1);
        actual.Treasures.Should().BeEquivalentTo(new List<Treasure>());
    }
    
    [Test]
    public void StartTreasureHunt_AdventurerCollectsOneTreasureInStackOfTreasure_ReturnsUpdatedMap()
    {
        Adventurer initialAdventurer = new Adventurer("Sophie", new Position(0, 0), Orientation.East, 
            new[] { 'A' }, 0);
        List<Treasure> initialTreasures = new List<Treasure>
        {
            new(1, 0),
            new(1, 0),
            new(1, 0)
        };
        List<Treasure> expectedTreasures = new List<Treasure>
        {
            new(1, 0),
            new(1, 0)
        };
        
        Map actual = new Map(TestDataUtils.SampleDimension, new List<Mountain>(), initialTreasures,
            new List<Adventurer>(){initialAdventurer});
        
        _treasureHuntService.StartTreasureHunt(actual);
        actual.Adventurers[0].NbTreasures.Should().Be(1);
        actual.Treasures.Should().BeEquivalentTo(expectedTreasures);
    }
    #endregion
    
}