using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WorldManager))]
public class WorldManagerEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector();

		if (GUILayout.Button("Debug")) {
			((WorldManager)target).DebugTerrain();
		}
	}
}
