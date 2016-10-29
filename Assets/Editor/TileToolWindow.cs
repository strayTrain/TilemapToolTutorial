using UnityEngine;
using UnityEditor;

public class TileToolWindow : EditorWindow 
{
	private TextAsset mapFile;
	private TilemapDefinition mapDefinition;
	private float tileWidth = 1;
	private float tileHeight = 1;

	[MenuItem("Tools/Tile Tool")]
	public static void InitializeWindow() 
	{
		TileToolWindow window = EditorWindow.GetWindow(typeof(TileToolWindow)) as TileToolWindow;
		window.titleContent.text = "Tile Tool";
		window.titleContent.tooltip = "This tool will instantiate a map based off of a text file";
		window.Show();  
	}

	private void OnGUI() 
	{
		mapFile = EditorGUILayout.ObjectField("Map File", mapFile, typeof(TextAsset), false) as TextAsset;
		mapDefinition = EditorGUILayout.ObjectField("Map Definition", mapDefinition, typeof(TilemapDefinition), false) as TilemapDefinition;

		// Make sure that we've provided the required information before we try and parse the file
		if (mapFile != null && mapDefinition != null)
		{
			// Show the options for the tile width and tile height
			tileWidth = EditorGUILayout.FloatField("Tile Width", tileWidth);
			tileHeight = EditorGUILayout.FloatField("Tile Height", tileHeight);

			// Finally, generate the map with a button click :)
			if (GUILayout.Button("Generate Map"))
			{
				ParseMap(mapFile, mapDefinition);
			}
		}
	}

	private void ParseMap(TextAsset map, TilemapDefinition definition)
	{
		// We start off by splitting the text file into individual lines. \n is the escape sequence (symbol) for a new line.
		// By splitting the text file according to \n, we chop it up into individual lines. For our purposes, each line is a row in out Unity map. 
		string[] rows = map.text.Split('\n');

		Transform folderObject = new GameObject("Map").transform;
		Undo.RegisterCreatedObjectUndo(folderObject.gameObject, "Create Folder Object");

		// For each line in the text file (as in, for each row)
		for (int i = 0; i < rows.Length; i++)	
		{
			// We want to read the rows from the bottom row to the top row so that our map looks like its text file representation.
			// i.e If our map is 3x3 the first row to be created should be: rows[2(rows.length) - 1(to avoid index out of bounds) - i(our index in the loop)]
			// which is equal to rows[2] which is the 3rd line of rows (indexing starts at 0, so its [0, 1, 2])
			// then we instantiate rows[1] then rows[0]. Bottom to top, just like the text file :)
			string[] keys = rows[rows.Length - 1 - i].Split(' ');

			// This commented out line would generate the map upside down
			//string[] keys = rows[i].Split(' ');

			// Now we go through each key on this line. The keys are our columns.
			for (int j = 0; j < keys.Length; j++)
			{	
				// Try and fetch the prefab related to this key
				string currentKey = keys[j];
				// When we press enter in our text file editor a return escape key is inserted into the text file. We want to ignore this because when we're comparing the
				// name, the \r is included! e.g "Hello\r" != "Hello"
				currentKey = currentKey.Trim('\r');

				// If definition.GetPrefab returns null, we know that currentKey wasn't defined in the tilemap definition
				GameObject currentPrefab = definition.GetPrefab(currentKey);

				if (currentPrefab != null)
				{
					GameObject instantiatedPrefab = Instantiate(currentPrefab, new Vector3(j * tileWidth, i * tileHeight, 0), Quaternion.identity, folderObject) as GameObject;	
					Undo.RegisterCreatedObjectUndo(instantiatedPrefab, "Create Tile");
				}	
				// We'll assume an empty space is an empty tile. Only throw an error when there's an undefined key
				else if (currentKey != "") 
				{
					// Give a helpful error in case we have bad data
					Debug.LogError("Couldn't find a definition for the key: [" + currentKey + "] in the tilemap definition: " + definition.name, definition);
				}
			}
		}
	}
}