using UnityEngine;

public class CloudLayout : MonoBehaviour
{
    [Header("Cloud")]
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private int count = 5;

    [Header("Area")]
    [SerializeField] private float leftX = -12f;
    [SerializeField] private float rightX = 12f;
    [SerializeField] private float bottomY = 2.5f;
    [SerializeField] private float topY = 5f;

    [Header("Scale")]
    [SerializeField] private Vector2 scaleRange = new(0.8f, 1.4f);

    [Header("Spacing")]
    [SerializeField] private float minGapX = 2f;

    [Header("Speed")]
    [SerializeField] private Vector2 speedRange = new(0.05f, 0.2f);

    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();

        if (cloudPrefab == null || count <= 0)
            return;

        float width = rightX - leftX;
        float segment = width / count;

        float previousX = leftX;

        for (int i = 0; i < count; i++)
        {
            float minX = leftX + segment * i;
            float maxX = minX + segment;

            float x = Random.Range(minX, maxX);

            if (i > 0)
            {
                x = Mathf.Max(x, previousX + minGapX);
            }

            float t = Random.value;

            float y = Mathf.Lerp(bottomY, topY, t);
            float scale = Mathf.Lerp(
                scaleRange.y,
                scaleRange.x,
                t);

            var cloud = Instantiate(
                cloudPrefab,
                transform);

            cloud.transform.localPosition = new Vector3(
                Mathf.Min(x, rightX),
                y,
                0);

            cloud.transform.localScale =
                Vector3.one * scale;

            if (cloud.TryGetComponent<CloudScroller>(
                    out var scroller))
            {
                scroller.Speed =
                    Mathf.Lerp(
                        speedRange.y,
                        speedRange.x,
                        t);
            }

            previousX =
                cloud.transform.localPosition.x;
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        for (int i = transform.childCount - 1;
             i >= 0;
             i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(
                    transform.GetChild(i).gameObject);
            else
#endif
                Destroy(
                    transform.GetChild(i).gameObject);
        }
    }
}