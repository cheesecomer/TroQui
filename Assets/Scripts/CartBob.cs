using UnityEngine;

public class CartBob : MonoBehaviour
{
    [SerializeField] private float fps = 8f;
    [SerializeField] private float small = 2f;
    [SerializeField] private float middle = 3f;
    [SerializeField] private float large = 5f;

    private float[] pattern = {};

    private Vector3 startPos;
    private float timer;
    private int frame;
    private int step;

    private void Start()
    {
        startPos = transform.localPosition;
        this.pattern = new float[] {
            0f,
            this.small,
            0f,
            this.middle,
            0f,
            this.small,
            0f,
            this.large
        };
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