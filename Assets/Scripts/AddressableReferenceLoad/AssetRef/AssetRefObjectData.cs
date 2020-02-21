using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AssetRefObjectData : MonoBehaviour
{
    [SerializeField] private List<AssetReference> references = new List<AssetReference>();

    private Dictionary<AssetReference, List<GameObject>> _loadedObjects =
        new Dictionary<AssetReference, List<GameObject>>();

    private Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _asyncOperationHandles =
        new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

    // For editor debugging
    // public List<GameObject> instantiated;
    public GameObject downloadPrefab;
    private Dictionary<AssetReference, float> downloads = new Dictionary<AssetReference, float>();
    private float downloadPercent = 0f;

    public void SyncList()
    {
        if (_loadedObjects.Count > 0)
        {
            // instantiated = _loadedObjects.First().Value;
        }
    }

    //<Invoked from buttons>
    public void InstantiateAssets()
    {
        AddressablesManager.InstantiateAndSaveInstance(references, _asyncOperationHandles, _loadedObjects);
    }

    public void ReleaseFirstInstance()
    {
        AddressablesManager.ReleaseAssetInstances(references[0], _asyncOperationHandles, _loadedObjects);
    }

    public void ReleaseAllInstances()
    {
        AddressablesManager.ReleaseAllAssetInstances(_asyncOperationHandles, _loadedObjects);
    }

    public void LoadAssets()
    {
        AddressablesManager.LoadAssetAndSaveHandle(references, _asyncOperationHandles);
    }

    public void ReleaseUnusedAssets()
    {
        AddressablesManager.ReleaseUnusedAssets(_asyncOperationHandles, _loadedObjects);
    }

    public void ClearCache()
    {
        AddressablesManager.ClearCache();
    }

    public async void GetDownloadSize()
    {
        var task = AddressablesManager.GetTotalDownloadSize(references);
        await task;
        // Should make event and have the object observing..
        downloadPrefab.GetComponentInChildren<Text>().text = $"{BytesToMegabytes(task.Result):F1} MB";
        // StartCoroutine(DownloadProgressTracker());
        foreach (var assetReference in references)
        {
            StartCoroutine(DownloadProgressTracker(assetReference));
        }
    }

    //</Invoked from buttons>

    private IEnumerator DownloadProgressTracker(AssetReference asset)
    {
        downloads[asset] = 0f;
        var op = Addressables.DownloadDependenciesAsync(asset);
        while (!op.IsDone)
        {
            downloads[asset] = op.PercentComplete;
            yield return null;
        }
    }

    private void Update()
    {
        var image = downloadPrefab.GetComponentInChildren<Image>();
        var tempDownload = 0f;
        foreach (var keyValuePair in downloads)
        {
            tempDownload += keyValuePair.Value;
        }

        downloadPercent = tempDownload / 3;
        if (downloadPercent * 2 > .9f)
        {
            print("HELLO");
            image.fillAmount = 1;
        }
        else
        {
            print("ITS ME");
            image.fillAmount = downloadPercent * 2;
        }
    }

    private static float BytesToMegabytes(float bytes)
    {
        return bytes / 1024f / 1024f;
    }
}