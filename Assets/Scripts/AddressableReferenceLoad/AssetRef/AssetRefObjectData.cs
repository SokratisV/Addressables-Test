using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetRefObjectData : MonoBehaviour
{
    [SerializeField] private List<AssetReference> references = new List<AssetReference>();

    private Dictionary<AssetReference, List<GameObject>> _loadedObjects =
        new Dictionary<AssetReference, List<GameObject>>();

    private Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _asyncOperationHandles =
        new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();

    // For editor debugging
    public List<GameObject> instantiated;

    public void SyncList()
    {
        if (_loadedObjects.Count > 0)
        {
            instantiated = _loadedObjects.First().Value;
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

    //</Invoked from buttons>
}