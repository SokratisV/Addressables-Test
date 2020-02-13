using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class CreateAddressablesLoader
{
    public static async Task InitAsset<T>(string assetNamelabel, List<T> createdObjects)
        where T : Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(assetNamelabel).Task;

        foreach (var location in locations)
        {
            createdObjects.Add(await Addressables.InstantiateAsync(location).Task as T);
        }
    }
}