using ldtk;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKLevel
{
	public readonly string Name;
	public readonly string ID;
	public readonly Dictionary<string, LDTKLayer> LayersByName = new();
	public readonly Vector2 Position;
	public readonly LDTKField[] Fields;

	public LDTKLevel(Level levelData)
	{
		Name = levelData.Identifier;
		ID = levelData.Iid;
		Position = new(levelData.WorldX, levelData.WorldY);
		DeserializeLayers(levelData);
		Fields = LDTKUtility.DeserializeFields(levelData.FieldInstances);
	}

	private void DeserializeLayers(Level levelData)
	{
		foreach (LayerInstance layerData in levelData.LayerInstances) LayersByName[layerData.Identifier] = new(layerData);
	}
}