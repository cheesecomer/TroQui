using UnityEngine;
using UnityEngine.UI;

public class CaveRoteter : MonoBehaviour
{
    [SerializeField] private Sprite Frame1;
    [SerializeField] private Sprite Frame2;
    [SerializeField] private Sprite Frame3;
    [SerializeField] private Sprite Frame4;
    [SerializeField] private Sprite Frame5;
    [SerializeField] private Sprite Frame6;

    [SerializeField] private float fps = 8f;

    private float timer;
    private int frame;
    private Sprite[] pattern;
    private int step;
    private Image image;

    private void Awake()
    {
        pattern = new Sprite[] { Frame1, Frame2, Frame3, Frame4, Frame5, Frame6 };
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 1f / fps) return;

        timer = 0f;
        frame = (frame + 1) % 4;

        Sprite sprite = pattern[step];
        image.sprite = sprite;

        // transform.localPosition =
        //     startPos + new Vector3(0f, y, 0f);

        step = (step + 1) % pattern.Length;
    }
}
