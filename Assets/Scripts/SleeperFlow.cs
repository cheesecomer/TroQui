using UnityEngine;

public class SleeperSlotSubStepper : MonoBehaviour, IStoppable
{
    [System.Serializable]
    public struct SleeperSlot
    {
        public float y;
        public float scale;
    }

    [SerializeField] private GameObject sleeperPrefab;

    [SerializeField]
    private SleeperSlot[] slots =
    {
        // 画面内
        new() { y = -0.10f, scale = 0.06f },
        new() { y = -0.22f, scale = 0.08f },
        new() { y = -0.35f, scale = 0.105f },
        new() { y = -0.50f, scale = 0.13f },
        new() { y = -0.70f, scale = 0.16f },
        new() { y = -1.00f, scale = 0.19f },
        new() { y = -1.35f, scale = 0.23f },
        new() { y = -1.80f, scale = 0.29f },
        new() { y = -2.25f, scale = 0.39f },
        new() { y = -3.50f, scale = 0.53f },
        new() { y = -5.00f, scale = 0.75f },
        new() { y = -6.00f, scale = 0.90f },
    };

    [Header("Animation")]
    [SerializeField] private float stepInterval = 0.08f;
    [SerializeField] private int subdivisions = 4;
    [SerializeField] private bool playOnStart = true;

    private Transform[] _sleepers;
    private float[] _slotPositions;
    private float _timer;
    private bool _playing;

    private void Start()
    {
        Generate();
        _playing = playOnStart;
    }

    private void Update()
    {
        if (!_playing) return;

        _timer += Time.deltaTime;

        if (_timer < stepInterval) return;

        _timer = 0f;
        Step();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();

        int sleeperCount = slots.Length - 1;

        _sleepers = new Transform[sleeperCount];
        _slotPositions = new float[sleeperCount];

        for (int i = 0; i < sleeperCount; i++)
        {
            var obj = Instantiate(sleeperPrefab, transform);
            obj.name = $"Sleeper_{i:00}";

            _sleepers[i] = obj.transform;

            // 画面内スロット 1〜6 に配置
            _slotPositions[i] = i + 1;

            ApplyPosition(_sleepers[i], _slotPositions[i]);
        }
    }

    private void Step()
    {
        float step = 1f / subdivisions;
        float lastSlot = slots.Length - 1;

        for (int i = 0; i < _sleepers.Length; i++)
        {
            // 奥 → 手前
            _slotPositions[i] += step;

            // 一番手前を超えたら、奥の画面外スロットへ戻す
            if (_slotPositions[i] >= lastSlot)
            {
                _slotPositions[i] -= lastSlot;
            }

            ApplyPosition(_sleepers[i], _slotPositions[i]);
        }
    }

    private void ApplyPosition(Transform sleeper, float slotPosition)
    {
        int fromIndex = Mathf.FloorToInt(slotPosition);
        int toIndex = Mathf.Min(fromIndex + 1, slots.Length - 1);

        float t = slotPosition - fromIndex;

        SleeperSlot from = slots[fromIndex];
        SleeperSlot to = slots[toIndex];

        float y = Mathf.Lerp(from.y, to.y, t);
        float scale = Mathf.Lerp(from.scale, to.scale, t);

        sleeper.localPosition = new Vector3(0f, y, 0f);
        sleeper.localScale = Vector3.one * scale;
    }

    void IStoppable.Stop()
    {
        _playing = false;
    }

    private void Clear()
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