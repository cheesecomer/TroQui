using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    private enum QuizState
    {
        WaitingNextQuestion,
        QuestionIntro,
        ShowLeftChoice,
        ShowRightChoice,
        Countdown,
        Correct,
        Wrong,
        GameOver
    }
    [SerializeField] private float firstQuestionDelay = 1f;
    [SerializeField] private float nextQuestionDelay = 0.5f;


    [Header("Question")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private GameObject leftChoicePanel;
    [SerializeField] private GameObject rightChoicePanel;
    [SerializeField] private TMP_Text leftChoiceText;
    [SerializeField] private TMP_Text rightChoiceText;

    [Header("Status")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text lifeText;

    [Header("Highlight")]
    [SerializeField] private Image leftChoiceImage;
    [SerializeField] private Image rightChoiceImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Cart")]
    [SerializeField] private RectTransform cartTransform;
    [SerializeField] private float tiltThreshold = 0.25f;
    [SerializeField] private float cartMaxAngle = 30f;

    [Header("Result Marks")]
    [SerializeField] private GameObject circleMark;
    [SerializeField] private GameObject crossMark;
    [SerializeField] private float resultFadeTime = 0.25f;

    [SerializeField] private GameObject cave;
    [SerializeField] private GameObject rail;
    [SerializeField] private GameObject retryButton;
    private QuizState state;
    private float timer;

    private int score = 0;
    private int life = 3;
    private Quiz.Side? selectedSide = null;
    private Quiz quiz;
    private GameObject currentResultMark;
    private bool transitioning;

    public void Retry()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void Start()
    {
        scoreText.gameObject.SetActive(false);
        WaitNextQuestion(firstQuestionDelay);
        // StartCoroutine(DerailAnimation());
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        switch (state)
        {
            case QuizState.WaitingNextQuestion:
                if (timer <= 0f)
                {
                    StartQuestion();
                }
                break;
            case QuizState.QuestionIntro:
                if (timer <= 0f) ShowLeftChoice();
                break;

            case QuizState.ShowLeftChoice:
                if (timer <= 0f) ShowRightChoice();
                break;

            case QuizState.ShowRightChoice:
                if (timer <= 0f) StartCountdown();
                break;

            case QuizState.Countdown:
                UpdateTilt();
                UpdateCountdown();

                if (timer <= 0f) Judge();
                break;

            case QuizState.Correct:
                if (timer <= 0f) {
                    StartCoroutine(HideResultMarkThenNext());
                }

                break;

            case QuizState.Wrong:
                if (timer <= 0f)
                {
                    StartCoroutine(HideResultMarkThenNext());
                }
                break;
        }
    }

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void WaitNextQuestion(float delay)
    {
        state = QuizState.WaitingNextQuestion;
        timer = delay;

        questionText.gameObject.SetActive(false);
        leftChoicePanel.SetActive(false);
        rightChoicePanel.SetActive(false);
        countdownText.gameObject.SetActive(false);
        HideResultMarksImmediate();
    }

    private void StartQuestion()
    {
        HideResultMarksImmediate();
        state = QuizState.QuestionIntro;
        timer = 1.5f;
        this.selectedSide = null;
        this.quiz = new Quiz();
        score++;

        cartTransform.localRotation = Quaternion.Euler(0, 0, 0);
        scoreText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
        questionText.text = this.quiz.Question;

        leftChoiceText.text = this.quiz.Left;
        rightChoiceText.text = this.quiz.Right;

        leftChoicePanel.SetActive(false);
        rightChoicePanel.SetActive(false);
        countdownText.gameObject.SetActive(false);

        UpdateStatus();
        UpdateHighlight();
    }

    private void ShowRightChoice()
    {
        state = QuizState.ShowRightChoice;
        timer = 1.0f;

        rightChoicePanel.SetActive(true);
    }

    private void ShowLeftChoice()
    {
        state = QuizState.ShowLeftChoice;
        timer = 1.0f;

        leftChoicePanel.SetActive(true);
    }

    private void StartCountdown()
    {
        state = QuizState.Countdown;
        timer = GetTimeLimit();

        countdownText.gameObject.SetActive(true);
        UpdateCountdown();
    }

    private void UpdateTilt()
    {
        float x = Input.acceleration.x;

        if (x < -tiltThreshold) selectedSide = Quiz.Side.Left;
        else if (x > tiltThreshold) selectedSide = Quiz.Side.Right;
        else selectedSide = null;

        cartTransform.localRotation = Quaternion.Euler(0, 0, -x * cartMaxAngle);

        UpdateHighlight();
    }

    private void UpdateCountdown()
    {
        countdownText.text = Mathf.CeilToInt(timer).ToString();
    }

    private void Judge()
    {
        bool isCorrect = selectedSide == this.quiz.CorrectSide;

        if (isCorrect)
        {
            ShowResultMark(circleMark);
            state = QuizState.Correct;
            timer = 0.5f;
        }
        else
        {
            life--;
            ShowResultMark(crossMark);
            state = QuizState.Wrong;
            timer = 0.8f;
        }

        UpdateStatus();
    }

    private IEnumerator ShowGameOver()
    {
        state = QuizState.GameOver;

        yield return StartCoroutine(DerailAnimation());

        questionText.text = "ゲームオーバー";
        leftChoicePanel.SetActive(false);
        rightChoicePanel.SetActive(false);
        countdownText.gameObject.SetActive(false);
    }

    private float GetTimeLimit()
    {
        if (score >= 10) return 5f;
        if (score >= 5) return 8f;
        return 10f;
    }

    private void UpdateStatus()
    {
        if (scoreText != null) scoreText.text = $"{score}";
        if (lifeText != null) lifeText.text = new string('♥', life);
    }

    private void UpdateHighlight()
{
        leftChoiceImage.color =
            selectedSide == Quiz.Side.Left ? highlightColor : normalColor;
        leftChoicePanel.transform.localScale =
            selectedSide == Quiz.Side.Left
                ? Vector3.one * 1.2f
                : Vector3.one;

        rightChoiceImage.color =
            selectedSide == Quiz.Side.Right ? highlightColor : normalColor;
        rightChoicePanel.transform.localScale =
            selectedSide == Quiz.Side.Right
                ? Vector3.one * 1.2f
                : Vector3.one;
    }

    private void ShowResultMark(GameObject mark)
    {
        HideResultMarksImmediate();

        currentResultMark = mark;
        currentResultMark.SetActive(true);
        currentResultMark.transform.localScale = Vector3.one;
    }

    private void HideResultMarksImmediate()
    {
        if (circleMark != null) circleMark.SetActive(false);
        if (crossMark != null) crossMark.SetActive(false);
        currentResultMark = null;
    }

    private IEnumerator HideResultMarkThenNext()
    {
        if (transitioning) yield break;
        transitioning = true;

        yield return AnimateResultMarkOut();

        transitioning = false;

        if (life <= 0)
        {
            StartCoroutine(ShowGameOver());
        }
        else
        {
            WaitNextQuestion(nextQuestionDelay);
        }
    }

    private IEnumerator AnimateResultMarkOut()
    {
        if (currentResultMark == null) yield break;

        var t = currentResultMark.transform;
        float elapsed = 0f;

        Vector3 fromScale = Vector3.one;
        Vector3 toScale = Vector3.zero;

        while (elapsed < resultFadeTime)
        {
            elapsed += Time.deltaTime;
            float rate = elapsed / resultFadeTime;

            t.localScale = Vector3.Lerp(fromScale, toScale, rate);

            yield return null;
        }

        currentResultMark.SetActive(false);
        currentResultMark = null;
    }

    private IEnumerator DerailAnimation()
    {
        cartTransform.GetComponent<CartBob>().enabled = false;
        Vector3 startPos = cartTransform.localPosition;

        // ガタン！と跳ねる
        cartTransform.localPosition = startPos + new Vector3(0, 15, 0);
        yield return new WaitForSeconds(0.1f);

        cartTransform.localPosition = startPos + new Vector3(0, 30, 0);
        yield return new WaitForSeconds(0.1f);

        // 左に傾き始める
        cartTransform.localRotation = Quaternion.Euler(0, 0, -5);
        yield return new WaitForSeconds(0.1f);

        cartTransform.localRotation = Quaternion.Euler(0, 0, -15);
        cartTransform.localPosition =
            startPos + new Vector3(-10, 30, 0);
        yield return new WaitForSeconds(0.1f);

        cartTransform.localPosition = startPos + new Vector3(-10, 15, 0);
        yield return new WaitForSeconds(0.1f);

        // 脱輪
        cartTransform.localRotation = Quaternion.Euler(0, 0, -30);
        cartTransform.localPosition =
            startPos + new Vector3(-10, -10, 0);

        yield return new WaitForSeconds(0.1f);

        // 最後に少し沈む
        cartTransform.localPosition =
            startPos + new Vector3(20, -20, 0);

        cartTransform.localRotation =
            Quaternion.Euler(0, 0, -10);

        cave.GetComponent<CaveRoteter>().enabled = false;
        rail.GetComponent<RailScroller>().enabled = false;

        yield return new WaitForSeconds(1f);

        retryButton.SetActive(true);
    }
}