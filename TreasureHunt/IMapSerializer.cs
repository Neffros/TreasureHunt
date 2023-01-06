using TreasureHunt.Models;

namespace TreasureHunt;

public interface IMapSerializer
{
    public string SerializeMap(Map map);
}