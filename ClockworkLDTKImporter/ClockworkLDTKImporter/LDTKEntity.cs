using ldtk;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKEntity
{
	public readonly string Name;
	private readonly LDTKField[] fields;
	public IReadOnlyList<LDTKField> Fields => fields;
	private Dictionary<string, LDTKField> fieldsByName = new();
	public IReadOnlyDictionary<string, LDTKField> FieldsByName => fieldsByName;
	public readonly Vector2 Position;

	public LDTKEntity(EntityInstance entityData)
	{
		Name = entityData.Identifier;
		fields = LDTKContainer.DeserializeFields(entityData.FieldInstances);
		foreach (LDTKField field in fields) fieldsByName[field.Name] = field;
		Position = new((float)entityData.WorldX, (float)entityData.WorldY);
	}
}
