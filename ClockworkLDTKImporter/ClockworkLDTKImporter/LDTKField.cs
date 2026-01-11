namespace Clockwork.LDTKImporter;

public class LDTKField
{
	public readonly string Name;
	public readonly dynamic Value;
	public readonly string Type;

	public LDTKField(string id, dynamic value, string type)
	{
		Name = id; Value = value; Type = type;
	}
}