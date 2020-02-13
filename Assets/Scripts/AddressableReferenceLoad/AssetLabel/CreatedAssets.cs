using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CreatedAssets : MonoBehaviour
{
    [field: SerializeField] public List<GameObject> Assets { get; } = new List<GameObject>();
    [SerializeField] private string _label;
    [SerializeField] private string _assetName;

    private void Start()
    {
        CreateAndWaitUntilCompleted();
    }

    private async Task CreateAndWaitUntilCompleted()
    {
        await CreateAddressablesLoader.InitAsset(_label, Assets);
        await CreateAddressablesLoader.InitAsset(_assetName, Assets);

        await Task.Delay(2000);
        ClearAsset(Assets[0]);
    }

    private void ClearAsset(GameObject go)
    {
        Addressables.Release(go);
    }
}