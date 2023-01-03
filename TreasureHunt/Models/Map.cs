namespace TreasureHunt.Models;

public record Map(Dimension Dimension, List<Mountain> Mountains, List<Treasure> Treasures, List<Adventurer> Adventurers);