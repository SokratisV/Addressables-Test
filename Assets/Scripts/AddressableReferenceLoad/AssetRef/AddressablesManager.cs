﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressablesManager
{
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
                if (!dictionary.ContainsKey(reference))
                {
                    dictionary[reference] = new List<GameObject>();
                }

                if (!moreThanOneInstance)
                {
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
                Addressables.Release(asyncHandles[keyValuePair.Key]);
                referencesToBeRemoved.Add(keyValuePair.Key);
            }
        }

        foreach (var reference in referencesToBeRemoved)
        {
            asyncHandles.Remove(reference);
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

    public static async void CheckForUpdates(List<AssetReference> assetList)
    {
        // await Addressables.UpdateCatalogs(await Addressables.CheckForCatalogUpdates().Task).Task;
    }

    public static async Task<float> GetTotalDownloadSize(List<AssetReference> assetList)
    {
        var totalSize = 0f;
        foreach (var asset in assetList)
        {
            var task = Addressables.GetDownloadSizeAsync(asset).Task;
            await task;
            totalSize += task.Result;
        }

        return totalSize;
    }

    public static async Task DownloadAllAssets(List<AssetReference> assetList)
    {
        foreach (var asset in assetList)
        {
            await Addressables.DownloadDependenciesAsync(asset).Task;
        }
    }

    public static void ClearCache()
    {
        Caching.ClearCache();
    }
}