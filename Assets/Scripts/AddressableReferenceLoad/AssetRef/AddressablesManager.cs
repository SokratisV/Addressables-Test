using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
    // <New>

    /// <summary>
    /// Loads an AssetReference to memory and adds its handle to the Dictionary if it does not already exist.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static async Task LoadAssetAndSaveHandle
    (
        AssetReference reference,
        Dictionary<AssetReference, AsyncOperationHandle<GameObject>> dictionary
    )
    {
        if (dictionary.ContainsKey(reference))
        {
            return;
        }

        var handle = reference.LoadAssetAsync<GameObject>();
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            dictionary.Add(reference, handle);
        }
        else
        {
            Debug.Log("Something went wrong in asset load.");
        }
    }

    /// <summary>
    /// Loads all AssetReferences to memory and adds their handle to the Dictionary if it does not already exist.
    /// </summary>
    /// <param name="references"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static async Task LoadAssetAndSaveHandle(
        List<AssetReference> references,
        Dictionary<AssetReference, AsyncOperationHandle<GameObject>> dictionary
    )
    {
        foreach (var reference in references)
        {
            if (dictionary.ContainsKey(reference))
            {
                return;
            }

            var handle = reference.LoadAssetAsync<GameObject>();
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                dictionary.Add(reference, handle);
            }
            else
            {
                Debug.Log("Something went wrong in asset load.");
            }
        }

        Debug.Log(dictionary.Count);
    }

    //TODO: Add overloads for InstantiateAsync
    /// <summary>
    /// Checks if asset is loaded in memory (its handle exists), loads it if it is not, instantiates it,
    /// and saves the instance reference to the List. The loading uses LoadAssetAndSaveHandle.
    /// </summary>
    /// <param name="reference">The AssetReference to be loaded.</param>
    /// <param name="asyncHandles">Dictionary for AssetReferences,AsyncOperationHandles</param>
    /// <param name="dictionary">Dictionary for AssetReferences,List</param>
    /// <param name="moreThanOneInstance">Default true. If it's false, overwrites the previous list.</param>
    /// <returns></returns>
    public static async void InstantiateAndSaveInstance(
        AssetReference reference,
        Dictionary<AssetReference, AsyncOperationHandle<GameObject>> asyncHandles,
        Dictionary<AssetReference, List<GameObject>> dictionary,
        bool moreThanOneInstance = true
    )
    {
        if (!asyncHandles.ContainsKey(reference))
        {
            await LoadAssetAndSaveHandle(reference, asyncHandles);
            dictionary[reference] = new List<GameObject>();
        }

        var handle = reference.InstantiateAsync();
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (moreThanOneInstance)
                dictionary[reference].Clear();

            dictionary[reference].Add(handle.Result);
        }
    }

    //TODO: Add overloads for InstantiateAsync
    /// <summary>
    /// Checks if assets in List are loaded in memory (its handle exists), loads it if it is not, instantiates it,
    /// and saves the instance reference to the List. The loading uses LoadAssetAndSaveHandle.
    /// </summary>
    /// <param name="references">List of AssetReferences to load.</param>
    /// <param name="asyncHandles">Dictionary for AssetReferences,AsyncOperationHandles</param>
    /// <param name="dictionary">Dictionary for AssetReferences,List</param>
    /// <param name="moreThanOneInstance">Default true. If it's false, overwrites the previous list.</param>
    /// <returns></returns>
    public static async void InstantiateAndSaveInstance(
        IList<AssetReference> references,
        Dictionary<AssetReference, AsyncOperationHandle<GameObject>> asyncHandles,
        Dictionary<AssetReference, List<GameObject>> dictionary,
        bool moreThanOneInstance = true
    )
    {
        foreach (var reference in references)
        {
            if (!asyncHandles.ContainsKey(reference))
            {
                await LoadAssetAndSaveHandle(reference, asyncHandles);
                dictionary[reference] = new List<GameObject>();
            }

            var handle = reference.InstantiateAsync();
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (!moreThanOneInstance)
                {
                    Debug.Log("Cleared");
                    dictionary[reference].Clear();
                }

                dictionary[reference].Add(handle.Result);
            }
        }
    }

    /// <summary>
    /// Checks if AssetReferences in Dictionary have no instantiated objects and releases them.
    /// </summary>
    /// <param name="asyncHandles">Dictionary for AssetReferences,AsyncOperationHandles</param>
    /// <param name="dictionary">Dictionary for AssetReferences,List</param>
    public static void ReleaseUnusedAssets(
        Dictionary<AssetReference, AsyncOperationHandle<GameObject>> asyncHandles,
        Dictionary<AssetReference, List<GameObject>> dictionary)
    {
        var referencesToBeRemoved = new List<AssetReference>();
        foreach (var keyValuePair in dictionary)
        {
            if (keyValuePair.Value.Count == 0)
            {
                Debug.Log($"Removing {keyValuePair.Key.Asset.name}");
                Addressables.Release(asyncHandles[keyValuePair.Key]);
                asyncHandles.Remove(keyValuePair.Key);
                referencesToBeRemoved.Add(keyValuePair.Key);
            }
        }

        foreach (var reference in referencesToBeRemoved)
        {
            dictionary.Remove(reference);
        }
    }

    //TODO: Test if this releases the AsyncOperationHandle asset as well.
    /// <summary>
    /// Releases all AssetReferences instances. The AssetReferences have to be GameObjects.
    /// </summary>
    /// <param name="asyncHandles">Dictionary for AssetReferences,AsyncOperationHandles.</param>
    /// <param name="dictionary">Dictionary for AssetReferences,List.</param>
    public static void ReleaseAllAssetInstances(
        Dictionary<AssetReference, AsyncOperationHandle<GameObject>> asyncHandles,
        Dictionary<AssetReference, List<GameObject>> dictionary)
    {
        foreach (var keyValuePair in asyncHandles)
        {
            foreach (var o in dictionary[keyValuePair.Key])
            {
                Addressables.ReleaseInstance(o);
            }

            dictionary[keyValuePair.Key].Clear();
        }
    }

    /// <summary>
    /// Checks if AssetReference exists in AsyncHandles dictionary, and removes it from List if it is.
    /// </summary>
    /// <param name="reference">The AssetReference to check.</param>
    /// <param name="asyncHandles">Dictionary for AssetReferences,AsyncOperationHandles.</param>
    /// <param name="dictionary">Dictionary for AssetReferences,List.</param>
    /// <param name="firstInstance">If true, releases first instance created, else last one.</param>
    public static void ReleaseAssetInstances(
        AssetReference reference,
        Dictionary<AssetReference, AsyncOperationHandle<GameObject>> asyncHandles,
        Dictionary<AssetReference, List<GameObject>> dictionary,
        bool firstInstance = true
    )
    {
        if (asyncHandles.ContainsKey(reference) && dictionary[reference].Count > 0)
        {
            var go = firstInstance ? dictionary[reference][0] : dictionary[reference][dictionary[reference].Count - 1];

            dictionary[reference].Remove(go);
            Addressables.ReleaseInstance(go);
        }
    }

    // <Old>
    // public static async Task InstantiateAndAddToList<T>(AssetReference reference, List<T> instantiatedObjects)
    //     where T : Object
    // {
    //     instantiatedObjects.Add(await reference.InstantiateAsync().Task as T);
    // }
    //
    // public static async Task InstantiateAndAddToList<T>(List<AssetReference> references, List<T> instantiatedObjects)
    //     where T : Object
    // {
    //     foreach (var reference in references)
    //     {
    //         instantiatedObjects.Add(await reference.InstantiateAsync().Task as T);
    //     }
    // }
    //
    // public static async Task LoadAssetAndSaveHandle<T>(List<AssetReference> references, List<T> loadedObjects)
    //     where T : Object
    // {
    //     foreach (var reference in references)
    //     {
    //         loadedObjects.Add(await reference.LoadAssetAsync<T>().Task);
    //     }
    // }
    //
    // public static async Task LoadAssetAndSaveHandle<T>(AssetReference reference, List<T> loadedObjects)
    //     where T : Object
    // {
    //     loadedObjects.Add(await reference.LoadAssetAsync<T>().Task);
    // }
    //
    // public static void ReleaseAssetsFromList<T>(List<T> objects)
    //     where T : Object
    // {
    //     foreach (var go in objects)
    //     {
    //         Addressables.Release(go);
    //     }
    // }
    //
    //
    // public static void ReleaseAsset<T>(T asset)
    // {
    //     Addressables.Release(asset);
    // }
    //
    // public static void ReleaseAssetInstance(GameObject asset)
    // {
    //     Addressables.ReleaseInstance(asset);
    // }
    //
    // public static void ReleaseAllInstances(List<GameObject> gameObjects)
    // {
    //     foreach (var go in gameObjects)
    //     {
    //         Addressables.ReleaseInstance(go);
    //     }
    // }
}