using Clockwork.Tiles;
using System.Numerics;

namespace Clockwork.LDTKImporter;

public class LDTKGameArea : TiledGameArea
{
	public readonly string ID;

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

	public LDTKGameArea(string id, LDTKField[] fields, LDTKEntity[] entities, Vector2 position, int widthInTiles, int heightInTiles, int tileSize) : base(position, widthInTiles, heightInTiles, tileSize)
	{
		ID = id;

		// Fields
		this.fields = fields;
		foreach (LDTKField field in fields) fieldsByID[field.ID] = field;

		// Entities
		this.entities = entities;
		foreach (LDTKEntity entity in entities)
		{
			if (!entitiesByID.ContainsKey(entity.ID)) entitiesByID[entity.ID] = new List<LDTKEntity>();
			entitiesByID[entity.ID].Add(entity);
		}
	}
}
