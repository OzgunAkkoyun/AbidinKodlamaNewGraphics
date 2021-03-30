using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorTools
{
    [MenuItem("GAME/List Shaders")]
    static void ListShaders()
    {
        var materialGUIDs = AssetDatabase.FindAssets("t:Material");
        var materialsByShaders = new Dictionary<Shader, List<Material>>();

        foreach (var materialGUID in materialGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(materialGUID);
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material)
            {
                var shader = material.shader;
                if (!materialsByShaders.TryGetValue(shader, out var list))
                {
                    list = new List<Material>();
                    materialsByShaders.Add(shader, list);
                }
                list.Add(material);
            }
        }

        foreach (var entry in materialsByShaders)
        {
            var shader = entry.Key;
            var list = entry.Value;
            Debug.Log("Shader: " + shader.name, shader);
            foreach (var material in list)
            {
                Debug.Log("   Material: " + material.name, material);
            }
        }

    }
}
