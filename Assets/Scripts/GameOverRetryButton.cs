using UnityEngine;

public class GameOverRetryButton : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float scaleAmount = 0.05f;

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale =
            baseScale * (1f + Mathf.Sin(Time.time * speed) * scaleAmount);
    }
}