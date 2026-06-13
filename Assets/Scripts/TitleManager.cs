using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private EndlessScrollBackground backgroundScroll;
    [SerializeField] private RectTransform cartTransform;
    [SerializeField] private GameObject startButton;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private float cartMoveDuration = 1.2f;
    [SerializeField] private float exitX = 1800f;
    [SerializeField] private string quizSceneName = "QuizScene";

    private bool started;

    private void Update()
    {
        if (started) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    public void StartGame()
    {
        if (started) return;
        started = true;

        StartCoroutine(StartGameFlow());
    }

    private IEnumerator StartGameFlow()
    {
        startButton.SetActive(false);

        if (backgroundScroll != null)
        {
            backgroundScroll.Stop();
        }

        yield return StartCoroutine(MoveCartOut());

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(quizSceneName);
    }

    private IEnumerator MoveCartOut()
    {
        Vector2 startPos = cartTransform.anchoredPosition;
        Vector2 endPos = new Vector2(exitX, startPos.y);

        for (float t = 0; t < cartMoveDuration; t += Time.deltaTime)
        {
            float rate = t / cartMoveDuration;
            cartTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, rate);
            yield return null;
        }

        cartTransform.anchoredPosition = endPos;
        cartTransform.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(1f);
    }

    private void SetFadeAlpha(float alpha)
    {
        var color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}