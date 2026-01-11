using Clockwork.Tiles;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKGameArea : TiledGameArea
{
	public readonly string Name;

	// Entities
	private LDTKEntity[] entities;
	public IReadOnlyList<LDTKEntity> Entities => entities;
	private Dictionary<string, List<LDTKEntity>> entitiesByID = new();
	public IReadOnlyDictionary<string, IReadOnlyList<LDTKEntity>> EntitiesByID =>
	entitiesByID.ToDictionary(
		keyValuePair => keyValuePair.Key,
		keyValuePair => (IReadOnlyList<LDTKEntity>)keyValuePair.Value
	);

	// Fields
	private LDTKField[] fields;
	public IReadOnlyList<LDTKField> Fields => fields;
	private Dictionary<string, LDTKField> fieldsByID = new();
	public IReadOnlyDictionary<string, LDTKField> FieldsByID => fieldsByID;

	public LDTKGameArea(string name, LDTKField[] fields, LDTKEntity[] entities, Vector2 position, int widthInTiles, int heightInTiles, int tileSize) : base(position, widthInTiles, heightInTiles, tileSize)
	{
		Name = name;

		// Fields
		this.fields = fields;
		foreach (LDTKField field in fields) fieldsByID[field.Name] = field;

		// Entities
		this.entities = entities;
		foreach (LDTKEntity entity in entities)
		{
			if (!entitiesByID.ContainsKey(entity.Name)) entitiesByID[entity.Name] = new List<LDTKEntity>();
			entitiesByID[entity.Name].Add(entity);
		}
	}
}
