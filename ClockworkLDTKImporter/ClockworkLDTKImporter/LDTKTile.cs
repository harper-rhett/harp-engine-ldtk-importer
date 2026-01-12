using ldtk;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKTile
{
	public readonly Vector2 LocalPosition;
	public readonly int TilesetX;
	public readonly int TilesetY;
	public readonly bool XFlipped;
	public readonly bool YFlipped;

	public LDTKTile(TileInstance tileData)
	{
		// Get the position
		int localX = (int)tileData.Px[0];
		int localY = (int)tileData.Px[1];
		LocalPosition = new(localX, localY);

		// Get the tileset information
		TilesetX = (int)tileData.Src[0];
		TilesetY = (int)tileData.Src[1];
		XFlipped = (tileData.F & 1) != 0;
		YFlipped = (tileData.F & (1L << 1)) != 0;
	}
}
