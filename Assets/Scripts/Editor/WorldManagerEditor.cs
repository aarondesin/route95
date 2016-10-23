// WorldManagerEditor.cs
// ©2016 Team 95

using UnityEditor;

using UnityEngine;

namespace Route95.World {

    /// <summary>
    /// Custom editor for WorldManager.
    /// </summary>
    [CustomEditor(typeof(WorldManager))]
    public class WorldManagerEditor : Editor {

        #region Vars

        /// <summary>
        /// Current shown x.
        /// </summary>
        int _x = 0;

        /// <summary>
        /// Current shown y.
        /// </summary>
        int _y = 0;

        float _vertBlend = 0f;

        #endregion
        #region Unity Overrides

        public override void OnInspectorGUI() {

            DrawDefaultInspector();

            // Button to show debug colors
            if (GUILayout.Button("Show constraints")) {
                ((WorldManager)target).DebugTerrain();
            }

            if (GUILayout.Button("Print vertex map")) {
                ((WorldManager)target).PrintVertexMap();
            }

            _x = EditorGUILayout.IntField("X:", _x);
            _y = EditorGUILayout.IntField("Y:", _y);

            _vertBlend = EditorGUILayout.FloatField("Blend: ", _vertBlend);

            if (GUILayout.Button("Show coordinates")) {
                WorldManager wm = target as WorldManager;
                Vertex vert = wm.terrain.vertexmap.VertexAt(_x, _y);
                wm.vertexIndicator.transform.position = vert.WorldPos();
                _vertBlend = vert.color.a;
            }
        }

        #endregion
    }
}
