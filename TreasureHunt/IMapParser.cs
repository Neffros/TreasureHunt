using TreasureHunt.Models;

namespace TreasureHunt;

public interface IMapParser
{
    public Map CreateMap(string[] lines);
}