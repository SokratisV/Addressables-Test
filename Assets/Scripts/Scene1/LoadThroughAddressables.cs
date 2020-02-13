using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadThroughAddressables : MonoBehaviour
{
    [SerializeField] private List<AssetReference> assetsReferences;
    private static int _counter = 0;

    private readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _operationHandles =
        new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

    public void LoadAssets()
    {
        foreach (var asset in assetsReferences)
        {
            if (!asset.RuntimeKeyIsValid()) return;
            if (_operationHandles.ContainsKey(asset)) return;
            _operationHandles[asset] = asset.LoadAssetAsync<GameObject>();
            print(++_counter);
        }
    }
    
    public void InstantiateObjects(int amount = 1)
    {
        foreach (var operationHandle in _operationHandles)
        {
            Addressables.InstantiateAsync(operationHandle.Key);
        }
    }

    public void ReleaseAssets()
    {
        foreach (var asset in assetsReferences)
        {
            if (_operationHandles.ContainsKey(asset))
            {
                Addressables.Release(_operationHandles[asset]);
            }
        }
    }

    public void ReleaseInstances()
    {
        
    }

}