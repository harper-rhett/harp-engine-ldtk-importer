using Clockwork.Tiles;

namespace Clockwork.LDTKImporter;

public class LDTKWorld : TiledWorld
{
	private Dictionary<string, LDTKGameArea> areasByID = new();
	public IReadOnlyDictionary<string, LDTKGameArea> AreasByID => areasByID;

	public LDTKWorld(IEnumerable<TiledGameArea> areas, int tileSize) : base(tileSize)
	{
		foreach (LDTKGameArea area in areas)
		{
			areasByID[area.ID] = area;
			AddArea(area);
		}
	}
}
