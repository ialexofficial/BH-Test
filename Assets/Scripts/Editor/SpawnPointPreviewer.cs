using Components;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnPoint))]
public class SpawnPointPreviewer : Editor
{
    [SerializeField] private GameObject preview;

    private GameObject _previewing;
    
    private void Awake()
    {
        _previewing ??= Instantiate(preview);
    }

    private void OnSceneGUI()
    {
        Transform transform = ((SpawnPoint) target).transform;
        _previewing.transform.position = transform.position;
        _previewing.transform.rotation = transform.rotation;
    }

    private void OnDestroy()
    {
        DestroyImmediate(_previewing);
    }
}