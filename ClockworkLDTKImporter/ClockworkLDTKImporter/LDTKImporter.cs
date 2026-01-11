using Clockwork.Graphics;
using Clockwork.Tiles;
using Clockwork.Utilities;
using ldtk;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKImporter
{
	private int tileSize;
	private Dictionary<string, Texture> tilesetsByPath = new();
	private LDTKContainer ldtkContainer;
	public Action<LDTKGameArea> GameAreaImported;

	public LDTKImporter(string localPath, int tileSize)
	{
		this.tileSize = tileSize;
		ldtkContainer = new(localPath);
		ImportTilesets();
	}

	public LDTKWorld ImportWorld(string tileLayerName, string entityLayerName)
	{
		LDTKGameArea[] areas = ImportGameAreas(tileLayerName, entityLayerName);
		LDTKWorld world = new(areas, tileSize);
		return world;
	}

	private void ImportTilesets()
	{
		foreach (string tilesetPath in ldtkContainer.TilesetPaths) tilesetsByPath[tilesetPath] = Texture.Load(tilesetPath);
	}

	private Tile[] DeserializeTiles(LayerInstance layerData)
	{
		// Initialize collections
		TileInstance[] tilesData = layerData.AutoLayerTiles;
		Tile[] tiles = new Tile[tilesData.Length];

		// Populate tiles
		for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex++)
		{
			TileInstance tileData = tilesData[tileIndex];

			// Get the position
			int localX = (int)tileData.Px[0];
			int localY = (int)tileData.Px[1];
			Vector2 localPosition = new(localX, localY);

			// Get the tileset information
			int tilesetX = (int)tileData.Src[0];
			int tilesetY = (int)tileData.Src[1];
			bool xFlipped = (tileData.F & 1) != 0;
			bool yFlipped = (tileData.F & (1L << 1)) != 0;

			// Create the tile
			Texture tilesetTexture = tilesetsByPath[layerData.TilesetRelPath];
			Tile tile = new(localPosition, tilesetTexture, tilesetX, tilesetY, tileSize, xFlipped, yFlipped);
			tiles[tileIndex] = tile;
		}

		return tiles;
	}

	private int[,] DeserializeTileTypes(LayerInstance layerData, int widthInTiles, int heightInTiles)
	{
		// Initialize collections
		long[] gridData = layerData.IntGridCsv;
		int[,] tileIDs = new int[widthInTiles, heightInTiles];

		// Preprocess tile IDs
		for (int x = 0; x < widthInTiles; x++)
			for (int y = 0; y < heightInTiles; y++)
			{
				int tileIndex = x + y * widthInTiles;
				tileIDs[x, y] = (int)gridData[tileIndex];
			}

		return tileIDs;
	}

	private LDTKEntity[] ImportEntities(LayerInstance layerData)
	{
		List<LDTKEntity> ldtkEntities = new();
		foreach (EntityInstance entityData in layerData.EntityInstances) ldtkEntities.Add(new(entityData));
		return ldtkEntities.ToArray();
	}

	private LDTKGameArea[] ImportGameAreas(string tileLayerName, string entityLayerName)
	{
		List<LDTKGameArea> gameAreas = new();
		foreach (LDTKLevel level in ldtkContainer.Levels)
		{
			// Get tiles layer
			LayerInstance tileLayerData = level.LayersByName[tileLayerName];
			LayerInstance entityLayerData = level.LayersByName[entityLayerName];

			// Extract some basic information
			int widthInTiles = (int)tileLayerData.CWid;
			int heightInTiles = (int)tileLayerData.CHei;

			// Create area
			LDTKEntity[] entities = ImportEntities(entityLayerData);
			LDTKGameArea area = new(level.Name, level.Fields, entities, level.Position, widthInTiles, heightInTiles, tileSize);
			area.Tiles = DeserializeTiles(tileLayerData);
			area.TilesByID = DeserializeTileTypes(tileLayerData, widthInTiles, heightInTiles);
			GameAreaImported.Invoke(area);

			// Register area
			gameAreas.Add(area);
		}
		return gameAreas.ToArray();
	}

	public TiledDecorationLayer ImportDecorationLayer(LDTKGameArea[] areas, string layerName, int drawLayer)
	{
		TiledDecorationLayer decorationLayer = new();
		foreach (LDTKLevel level in ldtkContainer.Levels)
		{
			// Get tiles layer
			LayerInstance tileLayerData = level.LayersByName[layerName];

			// Extract some basic information
			int widthInTiles = (int)tileLayerData.CWid;
			int heightInTiles = (int)tileLayerData.CHei;

			// Create area
			TiledArea area = new(level.Position, widthInTiles, heightInTiles, tileSize);
			area.Tiles = DeserializeTiles(tileLayerData);
		}
		return decorationLayer;
	}
}
