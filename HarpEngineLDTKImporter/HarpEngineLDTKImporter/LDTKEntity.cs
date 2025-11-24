using HarpEngine;

namespace HarpEngineLDTKImporter;

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

public class LDTKEntity : Entity
{
	public readonly string ID;
	private List<LDTKField> fields = new();
	public IReadOnlyList<LDTKField> Fields => fields;
	private Dictionary<string, LDTKField> fieldsByID = new();
	public IReadOnlyDictionary<string, LDTKField> FieldsByID => fieldsByID;

	public LDTKEntity(string id)
	{
		ID = id;
	}

	public void AddField(LDTKField field)
	{
		fields.Add(field);
		fieldsByID[field.ID] = field;
	}
}
