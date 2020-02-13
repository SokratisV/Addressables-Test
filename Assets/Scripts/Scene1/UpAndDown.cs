using UnityEngine;

public class UpAndDown : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(Random.Range(-3,3),Random.Range(-3,3),Random.Range(-3,3));        
    }
}
