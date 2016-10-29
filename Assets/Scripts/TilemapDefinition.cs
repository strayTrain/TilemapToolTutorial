using UnityEngine;
using System.Collections;

[System.Serializable]
public struct TilemapSet
{
	public string Key;
	public GameObject Prefab;
}

[CreateAssetMenu(fileName = "New Tileset", menuName = "Tileset/New", order = 1)]
public class TilemapDefinition : ScriptableObject 
{
	public TilemapSet[] Tiles;

	// This function takes a key and searches for a mathing prefab in the tilemap set. 
	// Returns null if the key doesn't exist in the set. The check is case insensitive.
	public GameObject GetPrefab(string key)
	{
		GameObject output = null;

		for (int i = 0; i < Tiles.Length; i++)
		{
			// We want the name check to be case insensitive
			if (Tiles[i].Key.ToUpper() == key.ToUpper())
			{
				output = Tiles[i].Prefab;
				break;
			}
		}

		return output;
	}
}