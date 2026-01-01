using Clockwork.Tiles;

namespace Clockwork.LDTKImporter;

public class LDTKWorld : TiledWorld
{
	private Dictionary<string, LDTKArea> areasByID = new();
	public IReadOnlyDictionary<string, LDTKArea> AreasByID => areasByID;

	public LDTKWorld(IEnumerable<TiledArea> areas, int tileSize) : base(tileSize)
	{
		foreach (LDTKArea area in areas)
		{
			areasByID[area.ID] = area;
			AddArea(area);
		}
	}
}
