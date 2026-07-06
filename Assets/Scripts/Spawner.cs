using System.Collections.Generic;
using UnityEngine;

public enum GroundObjectType
{
    Grass,
    Flower,
    Tree,
}

public class Spawner : MonoBehaviour, IStoppable
{
    [SerializeField]
    private SpriteDatabase spriteDb;

    [SerializeField] private GroundObjectType groundObjectType = GroundObjectType.Grass;

    [Header("Area")]
    [SerializeField] private SpriteRenderer groundBase;
    
    [Header("Count")]
    [SerializeField] private int count = 80;

    [Header("Perspective")]
    [SerializeField] private float topY = 1.2f;
    [SerializeField] private float bottomY = -4.4f;
    [SerializeField] private float farHalfWidth = 0.8f;
    [SerializeField] private float nearHalfWidth = 8.5f;

    [Header("Scale")]
    [SerializeField] private Vector2 scaleRange = new(0.25f, 1.15f);

    [Header("Rail Clear Area")]
    [SerializeField] private float railClearNear = 1.6f;
    [SerializeField] private float railClearFar = 0.25f;

    [Header("Scroll")]
    [SerializeField] private bool scroll = true;
    [SerializeField] private float stepInterval = 0.08f;
    [SerializeField] private int subdivisions = 4;
    [SerializeField] private int perspectiveSegments = 7;

    [Header("Look")]
    [SerializeField] private float rotationRange = 5f;
    [SerializeField] private int seed = 12345;
    [SerializeField] private string sortingLayer = "Default";
    [Header("Sorting")]
    [SerializeField] private int farOrderInLayer = 1;
    [SerializeField] private int nearOrderInLayer = 100;
    
    private readonly List<SceneryInstance> _instances = new();
    private System.Random _random;
    private float _timer;

    private Sprite[] _sprites;

    private void Start()
    {
        Generate();
    }

    void IStoppable.Stop()
    {
        scroll = false;
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();

        if (groundBase == null)
        {
            Debug.LogError("GroundBase or grass sprites are not assigned.");
            return;
        }

        _random = new System.Random(seed);
        _sprites = groundObjectType switch
        {
            GroundObjectType.Grass => spriteDb.grasses,
            GroundObjectType.Flower => spriteDb.flowers,
            GroundObjectType.Tree => spriteDb.trees,
            _ => spriteDb.grasses
        };

        for (var i = 0; i < count; i++)
        {
            var go = new GameObject($"{groundObjectType.ToString()}_{i:000}");
            go.transform.SetParent(transform, false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _sprites[_random.Next(_sprites.Length)];
            sr.sortingLayerName = sortingLayer;
            sr.sortingOrder = farOrderInLayer;

            var instance = new SceneryInstance
            {
                transform = go.transform,
                renderer = sr,
                distance = Random01(),
                lateral = RandomSigned01(),
                baseScale = RandomRange(0.85f, 1.25f),
                flipX = _random.Next(2) == 0,
                rotation = RandomRange(-rotationRange, rotationRange),
            };

            _instances.Add(instance);
            ApplyPerspective(instance);
        }
    }

    private void Update()
    {
        if (!scroll) return;

        _timer += Time.deltaTime;

        if (_timer < stepInterval)
        {
            return;
        }

        _timer = 0f;

        var step = 1f / (perspectiveSegments * subdivisions);

        foreach (var instance in _instances)
        {
            instance.distance += step;

            if (instance.distance > 1f)
            {
                instance.distance -= 1f;
                ResetAtFar(instance);
            }

            ApplyPerspective(instance);
        }
    }

    private void ResetAtFar(SceneryInstance instance)
    {
        instance.lateral = RandomSigned01();
        instance.baseScale = RandomRange(0.85f, 1.25f);
        instance.flipX = _random.Next(2) == 0;
        instance.rotation = RandomRange(-rotationRange, rotationRange);
        instance.renderer.sprite = _sprites[_random.Next(_sprites.Length)];
    }

    private void ApplyPerspective(SceneryInstance instance)
    {
        var t = Mathf.Clamp01(instance.distance);

        var y = Mathf.Lerp(topY, bottomY, t);

        var halfWidth = Mathf.Lerp(
            farHalfWidth,
            nearHalfWidth,
            t
        );

        var x = instance.lateral * halfWidth;

        var railClear = Mathf.Lerp(
            railClearFar,
            railClearNear,
            t
        );

        if (Mathf.Abs(x) < railClear)
        {
            x = Mathf.Sign(
                instance.lateral == 0
                    ? 1f
                    : instance.lateral
            ) * railClear;
        }

        var scale =
            Mathf.Lerp(
                scaleRange.x,
                scaleRange.y,
                t
            ) * instance.baseScale;

        instance.transform.position = new Vector3(
            x,
            y,
            groundBase.transform.position.z - 0.02f
        );

        instance.transform.localScale = new Vector3(
            instance.flipX ? -scale : scale,
            scale,
            1f
        );

        instance.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                instance.rotation
            );
        instance.renderer.sortingOrder = Mathf.RoundToInt(
            Mathf.Lerp(
                farOrderInLayer,
                nearOrderInLayer,
                t
            )
        );
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        while (transform.childCount > 0)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            else
            {
                DestroyImmediate(
                    transform.GetChild(0).gameObject
                );
            }
        }

        _instances.Clear();
    }

    private float Random01()
    {
        return (float)_random.NextDouble();
    }

    private float RandomSigned01()
    {
        return RandomRange(-1f, 1f);
    }

    private float RandomRange(
        float min,
        float max
    )
    {
        return min +
               (float)_random.NextDouble() *
               (max - min);
    }

    private class SceneryInstance
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public float distance;
        public float lateral;
        public float baseScale;
        public bool flipX;
        public float rotation;
    }
}