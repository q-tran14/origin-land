using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexMapProceduralGenerator))]
public class HexMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Add Custom Button to Generate Map in Non-Play Mode
        HexMapProceduralGenerator generator = (HexMapProceduralGenerator)target;
        if (GUILayout.Button("Generate Map"))
        {
            generator.GenerateMap();
        }
        if (GUILayout.Button("Clear Map"))
        {
            generator.ClearMap();
        }
    }
}
