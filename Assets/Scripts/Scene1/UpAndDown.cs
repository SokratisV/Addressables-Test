using UnityEngine;

public class UpAndDown : MonoBehaviour
{
    private Vector3 _randomPosition;
    private const float Speed = 1.5f;

    private void Start()
    {
        _randomPosition = new Vector3(Random.Range(-3,3),Random.Range(-3,3),Random.Range(-3,3));
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _randomPosition, Time.deltaTime * Speed);
        if (Vector3.Distance(transform.position, _randomPosition) <= .1f)
        {
            _randomPosition = new Vector3(Random.Range(-3,3),Random.Range(-3,3),Random.Range(-3,3));
        }
    }
}
