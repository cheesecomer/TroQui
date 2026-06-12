using UnityEngine;

public class CartBob : MonoBehaviour
{
    [SerializeField] private float fps = 8f;

    private readonly float[] pattern = {
        0f,
        2f,
        0f,
        3f,
        0f,
        2f,
        0f,
        5f
    };

    private Vector3 startPos;
    private float timer;
    private int frame;
    private int step;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < 1f / fps) return;

        timer = 0f;
        frame = (frame + 1) % 4;

        float y = pattern[step];

        transform.localPosition =
            startPos + new Vector3(0f, y, 0f);

        step = (step + 1) % pattern.Length;
    }
}