using UnityEngine;

public class DistanceTest : MonoBehaviour
{
    public Transform object1, object2;
    [SerializeField] private float distance;
    [SerializeField] private bool startChecking;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (startChecking)
            distance = Vector2.Distance(object1.position, object2.position);
    }
}
