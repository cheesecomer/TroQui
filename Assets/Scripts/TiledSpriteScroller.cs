using UnityEngine;

public class TiledSpriteScroller : MonoBehaviour
{
    [SerializeField]
    private Transform a;

    [SerializeField]
    private Transform b;
    [SerializeField] private float width = 60f;
    [SerializeField] private float speed = 1f;

    private void Start()
    {
        a.localPosition = Vector3.zero;
        b.localPosition = Vector3.right * width;
    }

    private void Update()
    {
        float move = speed * Time.deltaTime;

        a.localPosition += Vector3.left * move;
        b.localPosition += Vector3.left * move;

        if (a.localPosition.x <= -width)
        {
            a.localPosition = b.localPosition + Vector3.right * width;
        }

        if (b.localPosition.x <= -width)
        {
            b.localPosition = a.localPosition + Vector3.right * width;
        }
    }
}
