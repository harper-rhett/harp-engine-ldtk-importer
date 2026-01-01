using Clockwork;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKField
{
	public readonly string ID;
	public readonly dynamic Value;
	public readonly string Type;

	public LDTKField(string id, dynamic value, string type)
	{
		ID = id; Value = value; Type = type;
	}
}

public class LDTKEntity
{
	public readonly string ID;
	private LDTKField[] fields;
	public IReadOnlyList<LDTKField> Fields => fields;
	private Dictionary<string, LDTKField> fieldsByID = new();
	public IReadOnlyDictionary<string, LDTKField> FieldsByID => fieldsByID;
	public Vector2 Position;

	public LDTKEntity(string id, LDTKField[] fields, Vector2 position)
	{
		ID = id;
		this.fields = fields;
		foreach (LDTKField field in fields) fieldsByID[field.ID] = field;
		Position = position;
	}
}
