using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private EndlessScrollBackground backgroundScroll;
    [SerializeField] private RectTransform cartTransform;
    [SerializeField] private GameObject startButton;

    [SerializeField] private float cartMoveDuration = 1.2f;
    [SerializeField] private float exitX = 1800f;
    [SerializeField] private string quizSceneName = "QuizScene";

    private bool started;

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

        Vector2 startPos = cartTransform.anchoredPosition;
        Vector2 endPos = new Vector2(exitX, startPos.y);

        for (float t = 0; t < cartMoveDuration; t += Time.deltaTime)
        {
            float rate = t / cartMoveDuration;
            cartTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, rate);
            yield return null;
        }

        cartTransform.anchoredPosition = endPos;

        SceneManager.LoadScene(quizSceneName);
    }
}