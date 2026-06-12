using UnityEngine;

public class EndlessScrollBackground : MonoBehaviour
{
    [SerializeField] private RectTransform background1;
    [SerializeField] private RectTransform background2;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private float speed = 100f;

    private float width;

    private bool isRunning = true;

    public void Stop()
    {
        isRunning = false;
    }

    private void Start()
    {
        width = viewport.rect.width;

        Setup(background1, 0f);
        Setup(background2, width);
    }

    private void Update()
    {
        if (!isRunning) return;

        Move(background1);
        Move(background2);

        if (background1.anchoredPosition.x <= -width)
            Setup(background1, background2.anchoredPosition.x + width);

        if (background2.anchoredPosition.x <= -width)
            Setup(background2, background1.anchoredPosition.x + width);
    }

    private void Setup(RectTransform bg, float x)
    {
        bg.anchorMin = new Vector2(0.5f, 0.5f);
        bg.anchorMax = new Vector2(0.5f, 0.5f);
        bg.sizeDelta = new Vector2(width, viewport.rect.height);
        bg.anchoredPosition = new Vector2(x, 0);
    }

    private void Move(RectTransform bg)
    {
        bg.anchoredPosition += Vector2.left * speed * Time.deltaTime;
    }
}