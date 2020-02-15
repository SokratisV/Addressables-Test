using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;
    private Dictionary<GameObject, List<GameObject>> _activeObjects = new Dictionary<GameObject, List<GameObject>>();

    public List<GameObject> instantiated;

    public void SyncList()
    {
        if (_activeObjects.Count > 0)
        {
            instantiated = _activeObjects.First().Value;
        }
    }

    //<Invoked from buttons>
    public void InstantiatePrefabs()
    {
        foreach (var prefab in prefabs)
        {
            if (!_activeObjects.ContainsKey(prefab))
            {
                _activeObjects.Add(prefab, new List<GameObject>());
            }

            _activeObjects[prefab].Add(Instantiate(prefab));
        }
    }

    public void DestroyFirstInstance()
    {
        if (!_activeObjects.ContainsKey(prefabs[0])) return;

        var list = _activeObjects[prefabs[0]];
        if (list != null && list.Count > 0)
        {
            Destroy(list[0]);
            list.RemoveAt(0);
        }
    }

    public void DestroyAllInstances()
    {
        foreach (var keyValuePair in _activeObjects)
        {
            foreach (var o in keyValuePair.Value)
            {
                Destroy(o);
            }

            keyValuePair.Value.Clear();
        }
    }

    //</Invoked from buttons>
}