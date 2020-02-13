using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LoadedAddressableLocations : MonoBehaviour
{
    [SerializeField] private string _label;

    private void Start()
    {
        InitAndWaitUntilLocationLoaded();
    }

    public IList<IResourceLocation> AssetLocations { get; } = new List<IResourceLocation>();

    private async Task InitAndWaitUntilLocationLoaded()
    {
        await AddressableLocationLoader.GetAll(_label, AssetLocations);

        foreach (var location in AssetLocations)
        {
        }
    }
}