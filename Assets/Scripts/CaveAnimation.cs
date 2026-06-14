using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CaveAnimation : MonoBehaviour
{
    [SerializeField] private Sprite frame1;
    [SerializeField] private Sprite frame2;
    [SerializeField] private Sprite frame3;
    [SerializeField] private Sprite frame4;
    [SerializeField] private Sprite frame5;
    [SerializeField] private Sprite frame6;

    [SerializeField] private float fps = 8f;
    private int _frame;
    private Image _image;
    private Sprite[] _pattern;
    private int _step;

    private float _timer;

    private void Awake()
    {
        _pattern = new[] { frame1, frame2, frame3, frame4, frame5, frame6 };
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < 1f / fps) return;

        _timer = 0f;
        _frame = (_frame + 1) % 4;

        Sprite sprite = _pattern[_step];
        _image.sprite = sprite;

        // transform.localPosition =
        //     startPos + new Vector3(0f, y, 0f);

        _step = (_step + 1) % _pattern.Length;
    }
}