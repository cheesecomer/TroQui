using UnityEngine;

public class RailScroller : MonoBehaviour
{
    [SerializeField] private float fps = 8f;
    [SerializeField] private float yMove = 24f;
    [SerializeField] private float scaleMove = 0.08f;
    private Vector2 _basePos;
    private Vector2 _baseSize;
    private int _frame;

    private RectTransform _rect;
    private float _timer;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _basePos = _rect.anchoredPosition;
        _baseSize = _rect.sizeDelta;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < 1f / fps) return;

        _timer = 0f;
        _frame = (_frame + 1) % 4;

        float t = _frame / 3f; // 0, 0.33, 0.66, 1

        _rect.anchoredPosition = _basePos + new Vector2(0f, -yMove * t);
        _rect.sizeDelta = _baseSize * (1f + scaleMove * t);
    }
}