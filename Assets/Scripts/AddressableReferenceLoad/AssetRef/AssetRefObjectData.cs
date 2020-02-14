using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetRefObjectData : MonoBehaviour
{
    [SerializeField] private List<AssetReference> references = new List<AssetReference>();

    private Dictionary<AssetReference, List<GameObject>> _loadedObjects =
        new Dictionary<AssetReference, List<GameObject>>();

    // [SerializeField] private List<GameObject> instantiatedObjects = new List<GameObject>();
    // [SerializeField] private List<GameObject> loadedObjects = new List<GameObject>();

    //<Invoked from buttons>
    public void InstantiateAssets()
    {
        StartCoroutine(InstantiateAndWaitUntilComplete());
    }

    public void LoadAssets()
    {
        StartCoroutine(LoadAndWaitUntilComplete());
    }

    public void ReleaseAssets()
    {
        // _ReleaseAssets(loadedObjects);
        _ReleaseAssets(_loadedObjects);
    }

    public void ReleaseFirstAsset()
    {
        // if (loadedObjects.Count <= 0) return;
        //
        // ReleaseAsset(loadedObjects[0]);
        // loadedObjects.RemoveAt(0);
    }

    public void ReleaseAllInstances()
    {
        // _ReleaseAllInstances(instantiatedObjects);
        _ReleaseAllInstances(_loadedObjects);
    }

    public void ReleaseFirstInstance()
    {
        _ReleaseInstance(_loadedObjects, references[0]);
        // _loadedObjects[references[0]].RemoveAt(0);
    }

    public void Instantiate()
    {
    }
    //</Invoked from buttons>

    private static void _ReleaseAllInstances(List<GameObject> gameObjects)
    {
        if (gameObjects.Count <= 0) return;

        AssetRefLoader.ReleaseAllInstances(gameObjects);
        gameObjects.Clear();
    }

    private static void _ReleaseAllInstances(Dictionary<AssetReference, List<GameObject>> dictionary)
    {
        //Assets are being unloaded when none is in use, apparently
        AssetRefLoader.ReleaseAssetInstancesFromList(dictionary);
    }

    private static void _ReleaseInstance(GameObject go)
    {
        AssetRefLoader.ReleaseAssetInstance(go);
    }

    private static void _ReleaseInstance(Dictionary<AssetReference, List<GameObject>> dictionary,
        AssetReference reference)
    {
        AssetRefLoader.ReleaseAssetFromList(dictionary, reference);
    }

    private static void _ReleaseAssets(List<GameObject> gameObjects)
    {
        AssetRefLoader.ReleaseAssetsFromList(gameObjects);
        gameObjects.Clear();
    }

    private static void _ReleaseAssets(Dictionary<AssetReference, List<GameObject>> dictionary)
    {
        AssetRefLoader.ReleaseUnusedAssetsFromList(dictionary);
    }

    private static void ReleaseAsset(GameObject go)
    {
        AssetRefLoader.ReleaseAsset(go);
    }

    //No need for Coroutine (for now, TODO: Change to normal)
    private IEnumerator InstantiateAndWaitUntilComplete()
    {
        // yield return AssetRefLoader.InstantiateAndAddToList(references, instantiatedObjects);
        yield return AssetRefLoader.InstantiateFromList(_loadedObjects, references);
    }

    private IEnumerator LoadAndWaitUntilComplete()
    {
        // yield return AssetRefLoader.LoadAndAddToList(references, loadedObjects);
        yield return AssetRefLoader.LoadAndAddToList(_loadedObjects, references);
    }
}