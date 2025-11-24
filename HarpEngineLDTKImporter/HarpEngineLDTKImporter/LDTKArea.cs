using HarpEngine.Tiles;
using System.Numerics;

namespace HarpEngineLDTKImporter;

public class LDTKArea : TiledArea
{
	public LDTKEntity[] Entities;

	public LDTKArea(Vector2 position, int widthInTiles, int heightInTiles, int tileSize) : base(position, widthInTiles, heightInTiles, tileSize)
	{

	}
}
