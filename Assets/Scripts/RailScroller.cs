using UnityEngine;

public class RailScroller : MonoBehaviour
{
    [SerializeField] private float fps = 8f;
    [SerializeField] private float yMove = 24f;
    [SerializeField] private float scaleMove = 0.08f;

    private RectTransform rect;
    private Vector2 basePos;
    private Vector2 baseSize;
    private float timer;
    private int frame;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        basePos = rect.anchoredPosition;
        baseSize = rect.sizeDelta;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < 1f / fps) return;

        timer = 0f;
        frame = (frame + 1) % 4;

        float t = frame / 3f; // 0, 0.33, 0.66, 1

        rect.anchoredPosition = basePos + new Vector2(0f, -yMove * t);
        rect.sizeDelta = baseSize * (1f + scaleMove * t);
    }
}