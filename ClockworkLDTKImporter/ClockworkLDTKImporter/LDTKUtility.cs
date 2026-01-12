using Clockwork.Graphics;
using ldtk;

namespace Clockwork.LDTKImporter;

public class LDTKUtility
{
	private LdtkJson ldtkData;
	public readonly List<LDTKLevel> Levels = new();
	public readonly Dictionary<string, LDTKLevel> LevelsByID = new();
	public readonly List<string> TilesetPaths = new();

	public LDTKUtility(string localPath)
	{
		string ldtkJSON = File.ReadAllText(localPath);
		ldtkData = LdtkJson.FromJson(ldtkJSON);
		DeserializeLevels();
		DeserializeTilesets();
	}

	private void DeserializeLevels()
	{
		foreach (Level levelData in ldtkData.Levels)
		{
			LDTKLevel ldtkLevel = new(levelData);
			Levels.Add(ldtkLevel);
			LevelsByID[levelData.Iid] = ldtkLevel;
		}
	}

	private void DeserializeTilesets()
	{
		foreach (TilesetDefinition tilesetData in ldtkData.Defs.Tilesets)
		{
			if (tilesetData.RelPath is null) continue;
			TilesetPaths.Add(tilesetData.RelPath);
		}
	}

	public static LDTKField[] DeserializeFields(FieldInstance[] fieldsData)
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
}