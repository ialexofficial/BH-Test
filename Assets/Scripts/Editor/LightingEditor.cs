using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public class LightingEditor : Editor
{
    [MenuItem("Tools/Lighting/Disable DS")]
    public static void DisableDS() =>
        ToggleDS(SelectComponents<MeshRenderer>(), false);
    
    [MenuItem("Tools/Lighting/Enable DS")]
    public static void EnableDS() =>
        ToggleDS(SelectComponents<MeshRenderer>(), true);

    private static IEnumerable<T> SelectComponents<T>()
    {
        List<T> components = new List<T>();
            
        foreach (GameObject target in Selection.gameObjects)
        {
            foreach(T component in target.GetComponentsInChildren<T>())
                components.Add(component);

            T parentComponent;

            target.TryGetComponent<T>(out parentComponent);

            if (parentComponent != null)
                components.Add(parentComponent);
        }

        return components;
    }

    private static void ToggleDS(IEnumerable<MeshRenderer> meshRenderers, bool enabled)
    {
        Undo.RecordObjects(meshRenderers.ToArray(), "Toggled shadows");
        
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.shadowCastingMode = enabled ? ShadowCastingMode.On : ShadowCastingMode.Off;
        }
    }
}