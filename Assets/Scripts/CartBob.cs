using UnityEngine;

public class CartBob : MonoBehaviour, IStoppable
{
    [SerializeField] private float fps = 8f;
    [SerializeField] private float small = 2f;
    [SerializeField] private float middle = 3f;
    [SerializeField] private float large = 5f;
    private int _frame;

    private float[] _pattern = { };

    private Vector3 _startPos;
    private int _step;
    private float _timer;
    private bool _playing;

    private void Start()
    {
        _startPos = transform.localPosition;
        _pattern = new[]
        {
            0f,
            small,
            0f,
            small,
            0f,
            middle,
            0f,
            small,
            0f,
            small,
            0f,
            middle,
            0f,
            small,
            0f,
            small,
            0f,
            large
        };
        
        _playing = true;
    }

    void IStoppable.Stop()
    {
        _playing = false;
    }

    private void Update()
    {
        if (!_playing) return;

        _timer += Time.deltaTime;

        if (_timer < 1f / fps) return;

        _timer = 0f;
        _frame = (_frame + 1) % 4;

        float y = _pattern[_step];

        transform.localPosition =
            _startPos + new Vector3(0f, y, 0f);

        _step = (_step + 1) % _pattern.Length;
    }
}