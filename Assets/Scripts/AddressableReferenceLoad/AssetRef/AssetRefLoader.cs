using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AssetRefLoader
{
    // New
    public static async Task LoadAndAddToList<T>(Dictionary<AssetReference, List<T>> dictionary,
        AssetReference reference)
        where T : Object
    {
        if (dictionary.ContainsKey(reference))
        {
            return;
        }

        dictionary[reference] = new List<T> {await reference.LoadAssetAsync<T>().Task};
    }

    public static async Task LoadAndAddToList<T>(Dictionary<AssetReference, List<T>> dictionary,
        List<AssetReference> references)
        where T : Object
    {
        foreach (var reference in references)
        {
            if (dictionary.ContainsKey(reference))
            {
                return;
            }

            dictionary[reference] = new List<T> {await reference.LoadAssetAsync<T>().Task};
        }
    }

    public static async Task InstantiateFromList<T>(Dictionary<AssetReference, List<T>> dictionary,
        AssetReference reference)
        where T : Object
    {
        if (!dictionary.ContainsKey(reference))
        {
            await LoadAndAddToList(dictionary, reference);
        }

        dictionary[reference].Add(await reference.InstantiateAsync().Task as T);
    }

    public static async Task InstantiateFromList<T>(Dictionary<AssetReference, List<T>> dictionary,
        List<AssetReference> references)
        where T : Object
    {
        foreach (var reference in references)
        {
            if (!dictionary.ContainsKey(reference))
            {
                await LoadAndAddToList(dictionary, reference);
            }

            dictionary[reference].Add(await reference.InstantiateAsync().Task as T);
        }
    }

    // If an asset references has 0 Instantiated objects (held in List) then the asset is released.
    public static void ReleaseUnusedAssetsFromList<T>(Dictionary<AssetReference, List<T>> dictionary)
        where T : Object
    {
        foreach (var pair in dictionary)
        {
            if (pair.Value.Count >= 0) return;
            Addressables.Release(pair.Key);
        }
    }

    public static void ReleaseAssetInstancesFromList(Dictionary<AssetReference, List<GameObject>> dictionary)
    {
        foreach (var pair in dictionary)
        {
            if (pair.Value.Count == 0) return;
            foreach (var gameObject in pair.Value)
            {
                Addressables.ReleaseInstance(gameObject);
            }

            pair.Value.Clear();
        }
    }

    public static void ReleaseAssetFromList(Dictionary<AssetReference, List<GameObject>> dictionary,
        AssetReference reference)
    {
        if (dictionary.ContainsKey(reference) && dictionary[reference].Count > 0)
        {
            Debug.Log("Found it " + dictionary[reference][0]);
            Addressables.ReleaseInstance(dictionary[reference][0]);
            dictionary[reference].RemoveAt(0);
        }
    }


    // Old
    public static async Task InstantiateAndAddToList<T>(AssetReference reference, List<T> instantiatedObjects)
        where T : Object
    {
        instantiatedObjects.Add(await reference.InstantiateAsync().Task as T);
    }

    public static async Task InstantiateAndAddToList<T>(List<AssetReference> references, List<T> instantiatedObjects)
        where T : Object
    {
        foreach (var reference in references)
        {
            instantiatedObjects.Add(await reference.InstantiateAsync().Task as T);
        }
    }

    public static async Task LoadAndAddToList<T>(List<AssetReference> references, List<T> loadedObjects)
        where T : Object
    {
        foreach (var reference in references)
        {
            loadedObjects.Add(await reference.LoadAssetAsync<T>().Task);
        }
    }

    public static async Task LoadAndAddToList<T>(AssetReference reference, List<T> loadedObjects)
        where T : Object
    {
        loadedObjects.Add(await reference.LoadAssetAsync<T>().Task);
    }

    public static void ReleaseAssetsFromList<T>(List<T> objects)
        where T : Object
    {
        foreach (var go in objects)
        {
            Addressables.Release(go);
        }
    }


    public static void ReleaseAsset<T>(T asset)
    {
        Addressables.Release(asset);
    }

    public static void ReleaseAssetInstance(GameObject asset)
    {
        Addressables.ReleaseInstance(asset);
    }

    public static void ReleaseAllInstances(List<GameObject> gameObjects)
    {
        foreach (var go in gameObjects)
        {
            Addressables.ReleaseInstance(go);
        }
    }
}