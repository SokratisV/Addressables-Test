using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GetAddressable : MonoBehaviour
{
    [SerializeField] private List<AssetReference> assetsReferences;
    [SerializeField] private Text downloadSize;
    
    private long _totalDownloadSize;
    private readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _asyncOperationHandles =
        new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

    public void CheckForUpdates()
    {
        _totalDownloadSize = 0;
        foreach (var asset in assetsReferences)
        {
            StartCoroutine(Check(asset));
        }
    }

    public void CleanUp()
    {
        foreach (var asset in assetsReferences)
        {
            if (_asyncOperationHandles.ContainsKey(asset))
            {
                Addressables.ReleaseInstance(_asyncOperationHandles[asset]);
                _asyncOperationHandles.Remove(asset);
            }
        }
    }

    public void DeepClean()
    {
        foreach (var asset in assetsReferences)
        {
            if (_asyncOperationHandles.ContainsKey(asset))
            {
                Addressables.Release(_asyncOperationHandles[asset]);
                Addressables.ClearDependencyCacheAsync(asset);
            }
        }
        Caching.ClearCache();
        downloadSize.text = "0.0";
        CleanUp();
    }

    public void InstantiateInstances()
    {
        foreach (var asset in assetsReferences)
        {
            if (_asyncOperationHandles.ContainsKey(asset)) return;
            _asyncOperationHandles[asset] = Addressables.InstantiateAsync(asset);
        }
    }

    private IEnumerator Check(AssetReference assetReference)
    {
        var op = Addressables.GetDownloadSizeAsync(assetReference);
        while (!op.IsDone)
        {
            yield return null;
        }
        _totalDownloadSize += op.Result;

        // IF updates found, download updates.
        downloadSize.text = BytesToMegabytes(_totalDownloadSize);
        
        if (op.Result > 0)
        {
            var op1 = Addressables.DownloadDependenciesAsync(assetReference);
            while (!op1.IsDone)
            {
                print(op1.PercentComplete);
                yield return null;
            }
        }
    }

    private string BytesToMegabytes(long totalDownloadSize)
    {
        var value = $"{totalDownloadSize / 1024f / 1024f:F1}";
        return value;
    }
}