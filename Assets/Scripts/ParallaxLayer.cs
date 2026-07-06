using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Transform chunkA;
    [SerializeField] private Transform chunkB;
    [SerializeField] private int objectCount = 10;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float yJitter = 0.1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float baseY = 0f;
    [SerializeField]
    private float extraWidth = 1f;
    [SerializeField]
    private string sortingLayerName = "Background";
    [SerializeField]
    private int sortingOrderBase = 0;
    
    private float _chunkWidth;
    private readonly List<SpriteRenderer> _renderers = new();

    private void Awake()
    {
        Camera camera = Camera.main;

        float height = camera.orthographicSize * 2f;
        float width = height * camera.aspect;

        _chunkWidth = width + extraWidth;
    }
    private void Start()
    {
        BuildChunk(chunkA, 0);
        BuildChunk(chunkB, 1);

        chunkA.localPosition = Vector3.zero;
        chunkB.localPosition = Vector3.right * _chunkWidth;
    }

    private void Update()
    {
        float move = speed * Time.deltaTime;

        chunkA.localPosition += Vector3.left * move;
        chunkB.localPosition += Vector3.left * move;

        Recycle(chunkA);
        Recycle(chunkB);
    }

    private void Recycle(Transform chunk)
    {
        if (chunk.localPosition.x <= -_chunkWidth)
        {
            chunk.localPosition += _chunkWidth * 2f * Vector3.right;
        }
    }

    private void BuildChunk(Transform chunk, int seedOffset)
    {
        var random = new System.Random(1000 + seedOffset);

        for (int i = chunk.childCount - 1; i >= 0; i--)
        {
            Destroy(chunk.GetChild(i).gameObject);
        }

        for (var i = 0; i < objectCount; i++)
        {
            float x = Mathf.Lerp(
                -_chunkWidth / 2f,
                _chunkWidth / 2f,
                i / (float)(objectCount - 1)
            );
            if (i != 0 && i != objectCount - 1)
            {
                x += RandomRange(random, -0.4f, 0.4f);
            }
            
            float y = baseY + RandomRange(random, -yJitter, yJitter);

            var go = new GameObject($"Object {i + 1}");
            go.transform.SetParent(chunk, false);
            go.transform.localPosition = new Vector3(x, y, 0f);
            go.transform.localScale = Vector3.one * RandomRange(random, minScale, maxScale);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprites[random.Next(sprites.Length)];
            sr.sortingLayerName = sortingLayerName;
            _renderers.Add(sr);
        }
        _renderers.Sort((a, b) =>
            a.transform.localPosition.y.CompareTo(b.transform.localPosition.y)
        );

        for (var i = 0; i < _renderers.Count; i++)
        {
            _renderers[i].sortingOrder = sortingOrderBase + i;
        }

        _renderers.Clear();
    }

    private static float RandomRange(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }
}