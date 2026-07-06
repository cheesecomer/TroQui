using UnityEngine;

public class CloudScroller : MonoBehaviour
{
    public float Speed { get; set; } = 0.1f;

    [Header("Loop Range")]
    [SerializeField] private float leftX = -12f;
    [SerializeField] private float rightX = 12f;

    private void Update()
    {
        transform.position += Speed * Time.deltaTime * -1f * Vector3.left;

        if (transform.position.x < rightX) return;
        var pos = transform.position;
        pos.x = leftX;
        transform.position = pos;
    }
}
