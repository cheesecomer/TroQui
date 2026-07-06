using UnityEngine;

public class TreeLineGenerator : MonoBehaviour
{
    [Header("Tree Sprites")]
    [SerializeField] private Sprite[] treeSprites; // 4種類入れる

    [Header("Layout")]
    [SerializeField] private int treeCount = 60;
    [SerializeField] private float startX = -10f;
    [SerializeField] private float endX = 10f;

    [Header("Hill Curve")]
    [SerializeField] private float baseY = -1.6f;
    [SerializeField] private float hillHeight = 0.9f;

    [Header("Random")]
    [SerializeField] private int seed = 12345;
    [SerializeField] private float xJitter = 0.18f;
    [SerializeField] private float yJitter = 0.08f;
    [SerializeField] private Vector2 scaleRange = new Vector2(0.75f, 1.15f);
    [SerializeField] private float rotationRange = 4f;

    [Header("Rendering")]
    [SerializeField] private string sortingLayerName = "Background";
    [SerializeField] private int baseSortingOrder = 0;

    [ContextMenu("Generate Trees")]
    private void Generate()
    {
        ClearChildren();

        Random.InitState(seed);

        for (int i = 0; i < treeCount; i++)
        {
            float t = treeCount <= 1 ? 0f : i / (float)(treeCount - 1);

            float x = Mathf.Lerp(startX, endX, t);
            x += Random.Range(-xJitter, xJitter);

            // 画面中央が高く、端が低い丘カーブ
            float curve = Mathf.Sin(t * Mathf.PI);
            float y = baseY + curve * hillHeight;
            y += Random.Range(-yJitter, yJitter);

            Sprite sprite = treeSprites[Random.Range(0, treeSprites.Length)];

            GameObject tree = new GameObject($"Tree_{i:00}");
            tree.transform.SetParent(transform);
            tree.transform.localPosition = new Vector3(x, y, 0f);

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            tree.transform.localScale = new Vector3(scale, scale, 1f);

            tree.transform.localRotation =
                Quaternion.Euler(0f, 0f, Random.Range(-rotationRange, rotationRange));

            var renderer = tree.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingLayerName = sortingLayerName;

            // 奥行き感。下にある木ほど手前
            renderer.sortingOrder = baseSortingOrder + Mathf.RoundToInt(-y * 10f);
        }
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }
    }
}