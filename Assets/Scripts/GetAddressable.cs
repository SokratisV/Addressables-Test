using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GetAddressable : MonoBehaviour
{
    [SerializeField] private List<AssetReference> assetsReferences;
    [SerializeField] private Text downloadSize;
    private long _totalDownloadSize;

    public void CheckForUpdates()
    {
        foreach (var assetReference in assetsReferences)
        {
            StartCoroutine(Check(assetReference));
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
                yield return null;
            }
        }

        assetReference.InstantiateAsync();
    }

    private string BytesToMegabytes(long totalDownloadSize)
    {
        var value = string.Format("{0:F1}", totalDownloadSize / 1024f / 1024f);
        print(value);
        return value;
    }
}