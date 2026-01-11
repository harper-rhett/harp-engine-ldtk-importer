using Clockwork.Tiles;

namespace Clockwork.LDTKImporter;

public class LDTKWorld : TiledWorld
{
	private Dictionary<string, LDTKGameArea> areasByName = new();
	public IReadOnlyDictionary<string, LDTKGameArea> AreasByName => areasByName;

	public LDTKWorld(IEnumerable<TiledGameArea> areas, int tileSize) : base(tileSize)
	{
		foreach (LDTKGameArea area in areas)
		{
			areasByName[area.Name] = area;
			AddArea(area);
		}
	}
}
