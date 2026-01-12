using ldtk;

namespace Clockwork.LDTKImporter;

public class LDTKLayer
{
	public readonly string Name;
	public readonly string TilesetPath;
	public readonly int WidthInTiles;
	public readonly int HeightInTiles;
	public readonly List<LDTKEntity> Entities = new();
	public readonly List<LDTKTile> Tiles = new();
	public readonly int[,] TileIDs;

	public LDTKLayer(LayerInstance layerInstance)
	{
		Name = layerInstance.Identifier;
		TilesetPath = layerInstance.TilesetRelPath;
		WidthInTiles = (int)layerInstance.CWid;
		HeightInTiles = (int)layerInstance.CHei;
		TileIDs = new int[WidthInTiles, HeightInTiles];
		DeserializeTiles(layerInstance);
		DeserializeTileTypes(layerInstance);
		DeserializeEntities(layerInstance);
	}

	private void DeserializeTiles(LayerInstance layerInstance)
	{
		// Initialize collections
		TileInstance[] autoTilesData = layerInstance.AutoLayerTiles;
		TileInstance[] gridTilesData = layerInstance.GridTiles;

		// Populate tiles
		foreach (TileInstance autoTileData in autoTilesData) Tiles.Add(new(autoTileData));
		foreach (TileInstance gridTileData in gridTilesData) Tiles.Add(new(gridTileData));
	}

	private void DeserializeTileTypes(LayerInstance layerData)
	{
		long[] gridData = layerData.IntGridCsv;
		if (gridData.Length == 0) return;

		// Preprocess tile IDs
		for (int x = 0; x < WidthInTiles; x++)
			for (int y = 0; y < HeightInTiles; y++)
			{
				int tileIndex = x + y * WidthInTiles;
				TileIDs[x, y] = (int)gridData[tileIndex];
			}
	}

	private void DeserializeEntities(LayerInstance layerData)
	{
		foreach (EntityInstance entityData in layerData.EntityInstances) Entities.Add(new(entityData));
	}
}
