using UnityEngine;

public class EndlessScrollBackground : MonoBehaviour
{
    [SerializeField] private RectTransform background1;
    [SerializeField] private RectTransform background2;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private float speed = 100f;

    private bool _isRunning = true;

    private float _width;

    private void Start()
    {
        _width = viewport.rect.width;

        Setup(background1, 0f);
        Setup(background2, _width);
    }

    private void Update()
    {
        if (!_isRunning) return;

        Move(background1);
        Move(background2);

        if (background1.anchoredPosition.x <= -_width)
            Setup(background1, background2.anchoredPosition.x + _width);

        if (background2.anchoredPosition.x <= -_width)
            Setup(background2, background1.anchoredPosition.x + _width);
    }

    public void Stop()
    {
        _isRunning = false;
    }

    private void Setup(RectTransform bg, float x)
    {
        bg.anchorMin = new Vector2(0.5f, 0.5f);
        bg.anchorMax = new Vector2(0.5f, 0.5f);
        bg.sizeDelta = new Vector2(_width, viewport.rect.height);
        bg.anchoredPosition = new Vector2(x, 0);    
    }

    private void Move(RectTransform bg)
    {
        bg.anchoredPosition += Vector2.left * (speed * Time.deltaTime);
    }
}