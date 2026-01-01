using Clockwork.Graphics;
using Clockwork.Tiles;
using Clockwork.Utilities;
using ldtk;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKImporter
{
	private LdtkJson ldtkData;
	private Dictionary<string, Texture> tilesetsByPath = new();
	private Dictionary<string, LDTKArea> areasByID = new();
	private int tileSize;
	public Action<LDTKArea> AreaImported;

	public LDTKImporter(string localPath, int tileSize)
	{
		string ldtkJSON = File.ReadAllText(localPath);
		ldtkData = LdtkJson.FromJson(ldtkJSON);
		this.tileSize = tileSize;
	}

	public LDTKWorld GenerateWorld()
	{
		DeserializeTilesets();
		LDTKArea[] areas = DeserializeLevels();
		LDTKWorld world = new(areas, tileSize);
		return world;
	}

	private void DeserializeTilesets()
	{
		foreach (TilesetDefinition tilesetData in ldtkData.Defs.Tilesets)
		{
			if (tilesetData.RelPath is null) continue;
			tilesetsByPath[tilesetData.RelPath] = Texture.Load(tilesetData.RelPath);
		}
	}

	private Dictionary<string, LayerInstance> DeserializeLayers(Level levelData)
	{
		Dictionary<string, LayerInstance> layersData = new();
		foreach (LayerInstance layerData in levelData.LayerInstances) layersData[layerData.Identifier] = layerData;
		return layersData;
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

	private LDTKEntity[] DeserializeEntities(LayerInstance layerData)
	{
		int entityCount = layerData.EntityInstances.Length;
		LDTKEntity[] ldtkEntities = new LDTKEntity[entityCount];
		for (int entityIndex  = 0; entityIndex < entityCount; entityIndex++)
		{
			EntityInstance entityData = layerData.EntityInstances[entityIndex];
			
			LDTKField[] fields = DeserializeFields(entityData.FieldInstances);
			LDTKEntity ldtkEntity = new(entityData.Identifier, fields, new((float)entityData.WorldX, (float)entityData.WorldY));
			ldtkEntities[entityIndex] = ldtkEntity;
		}
		return ldtkEntities;
	}

	private LDTKField[] DeserializeFields(FieldInstance[] fieldsData)
	{
		int fieldCount = fieldsData.Length;
		LDTKField[] fields = new LDTKField[fieldCount];
		for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
		{
			FieldInstance fieldData = fieldsData[fieldIndex];
			LDTKField field = new(fieldData.Identifier, fieldData.Value, fieldData.Type);
			fields[fieldIndex] = field;
		}
		return fields;
	}

	private LDTKArea[] DeserializeLevels()
	{
		int areaCount = ldtkData.Levels.Length;
		LDTKArea[] areas = new LDTKArea[areaCount];
		for (int areaIndex = 0; areaIndex < areaCount; areaIndex++)
		{
			// Get level
			Level levelData = ldtkData.Levels[areaIndex];

			// Get tiles layer
			Dictionary<string, LayerInstance> layersData = DeserializeLayers(levelData);
			LayerInstance tileLayerData = layersData["tiles"];
			LayerInstance entityLayerData = layersData["entities"];

			// Extract some basic information
			int widthInTiles = (int)tileLayerData.CWid;
			int heightInTiles = (int)tileLayerData.CHei;

			// Create area
			Vector2 position = new((int)levelData.WorldX, (int)levelData.WorldY);
			LDTKField[] fields = DeserializeFields(levelData.FieldInstances);
			LDTKEntity[] entities = DeserializeEntities(entityLayerData);
			LDTKArea area = new(levelData.Identifier, fields, entities, position, widthInTiles, heightInTiles, tileSize);
			area.Tiles = DeserializeTiles(tileLayerData);
			area.TilesByID = DeserializeTileTypes(tileLayerData, widthInTiles, heightInTiles);
			AreaImported.Invoke(area);

			// Register area
			areas[areaIndex] = area;
			areasByID[levelData.Iid] = area;
		}
		return areas;
	}
}
