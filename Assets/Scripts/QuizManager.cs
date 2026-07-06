using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private float firstQuestionDelay = 1f;
    [SerializeField] private float nextQuestionDelay = 0.5f;
    
    [RequireInterface(typeof(IStoppable))]
    [SerializeField] private MonoBehaviour[] stoppables;
    
    [Header("Voice")]
    [SerializeField] private VoiceDatabase voiceDatabase;

    [Header("Sprite")]
    [SerializeField]
    private SpriteDatabase spriteDb;
    
    [Header("Choices")] [SerializeField] private TMP_Text leftChoiceText;
    [SerializeField] private TMP_Text rightChoiceText;

    [Header("Question")] [SerializeField] private GameObject questionPanel;
    [SerializeField] private TMP_Text questionText;

    [Header("Guess Number")] [SerializeField]
    private GuessNumberPanel guessNumberPanel;

    [Header("Kana Choice")] [SerializeField]
    private GameObject kanaChoicePanel;

    [SerializeField] private TMP_Text kanaChoiceQuestionText;
    [SerializeField] private Image kanaChoiceQuestionImage;

    [Header("Status")] [SerializeField] private TMP_Text countdownText;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text scoreResultText;
    [SerializeField] private TMP_Text newRecordText;
    [SerializeField] private TMP_Text lifeText;

    [Header("Highlight")] [SerializeField] private Color normalColor = Color.white;

    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Cart")] [SerializeField] private Transform cartTransform;

    [SerializeField] private float tiltThreshold = 0.25f;
    // [SerializeField] private float cartMaxAngle = 30f;
    
    [SerializeField] private SpriteRenderer girl;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite wrongSprite;
    
    [Header("Result Marks")] [SerializeField]
    private GameObject circleMark;

    [SerializeField] private GameObject crossMark;
    [SerializeField] private float resultFadeTime = 0.25f;
    
    [SerializeField] private GameObject retryButton;
    [Header("SE")]
    [FormerlySerializedAs("audioSource")]
    [SerializeField] private AudioSource seAudioSource;

    [SerializeField] private AudioClip correctSe;
    [SerializeField] private AudioClip wrongSe;
    [SerializeField] private AudioClip gameOverSe;

    [Header("BGM")]
    [SerializeField] private AudioSource bgm;
    
    [Header("Voice")]
    [SerializeField] private AudioSource voiceAudioSource;
    
    [Header("On Back Button Press")] [SerializeField]
    private GameObject pauseDialog;

    [SerializeField] private string titleSceneName = "TitleScene";

    [Header("Fade")] [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeDuration = 0.5f;
    private GameObject _currentResultMark;

    private bool _isPauseDialogOpen;
    private float _leftChoiceTextFontSize;
    private int _life = 3;
    private Quiz _quiz;
    private float _rightChoiceTextFontSize;

    private int _score;
    private Quiz.Side? _selectedSide;
    private QuizState _state;
    private float _timer;
    private bool _transitioning;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());

        retryButton.gameObject.SetActive(false);
        scoreResultText.gameObject.SetActive(false);
        newRecordText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);

        _leftChoiceTextFontSize = leftChoiceText.fontSize;
        _rightChoiceTextFontSize = rightChoiceText.fontSize;
        WaitNextQuestion(firstQuestionDelay);
    }

    private void Update()
    {
        if (_isPauseDialogOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
            return;
        }

        _timer -= Time.deltaTime;

        switch (_state)
        {
            case QuizState.WaitingNextQuestion:
                if (_timer <= 0f) StartCoroutine(StartQuestion());
                break;
            
            case QuizState.PlayingVoice:
                break;
            case QuizState.QuestionIntro:
                if (_timer <= 0f) ShowLeftChoice();
                break;

            case QuizState.ShowLeftChoice:
                if (_timer <= 0f) ShowRightChoice();
                break;

            case QuizState.ShowRightChoice:
                if (_timer <= 0f) StartCountdown();
                break;

            case QuizState.Countdown:
                UpdateTilt();
                UpdateCountdown();

                if (_timer <= 0f) Judge();
                break;

            case QuizState.Correct:
                if (_timer <= 0f) StartCoroutine(HideResultMarkThenNext());

                break;

            case QuizState.Wrong:
                if (_timer <= 0f) StartCoroutine(HideResultMarkThenNext());
                break;
        }
    }

    public void Retry()
    {
        StartCoroutine(DoRetry());
    }

    private IEnumerator DoRetry()
    {
        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void WaitNextQuestion(float delay)
    {
        _state = QuizState.WaitingNextQuestion;
        _timer = delay;

        guessNumberPanel.gameObject.SetActive(false);
        questionPanel.SetActive(false);
        questionText.gameObject.SetActive(false);
        leftChoiceText.gameObject.SetActive(false);
        rightChoiceText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        HideResultMarksImmediate();
    }

    private IEnumerator StartQuestion()
    {
        HideResultMarksImmediate();
        _state = QuizState.PlayingVoice;
        _selectedSide = null;
        _quiz = new Quiz(spriteDb.kanaChoiceSources, voiceDatabase);
        _score++;

        cartTransform.localRotation = Quaternion.Euler(0, 0, 0);
        scoreText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
        questionText.text = _quiz.Question;

        leftChoiceText.text = _quiz.Left;
        rightChoiceText.text = _quiz.Right;

        girl.sprite = normalSprite;
        leftChoiceText.gameObject.SetActive(false);
        rightChoiceText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        guessNumberPanel.gameObject.SetActive(false);
        kanaChoicePanel.SetActive(false);
        questionPanel.SetActive(false);

        if (_quiz.Type == Quiz.QuizType.GuessNumber)
        {
            guessNumberPanel.ShowItems(_quiz.ItemNum);
            guessNumberPanel.gameObject.SetActive(true);
        }
        else if (_quiz.Type == Quiz.QuizType.KanaChoice)
        {
            kanaChoicePanel.SetActive(true);

            kanaChoiceQuestionText.text = _quiz.Question;
            kanaChoiceQuestionImage.sprite = _quiz.Image;
        }
        else
        {
            questionPanel.SetActive(true);
        }
        
        foreach (var sequence in _quiz.VoiceSequences)
        {
            voiceAudioSource.PlayOneShot(sequence.clip);
            yield return new WaitForSeconds(
                sequence.clip.length + sequence.delay
            );
        }

        UpdateStatus();
        UpdateHighlight();

        _state = QuizState.QuestionIntro;
        _timer = _quiz.VoiceSequences.Length >  0 ? 0.2f : 1.5f;
    }

    private void ShowRightChoice()
    {
        _state = QuizState.ShowRightChoice;
        _timer = 1.0f;

        rightChoiceText.gameObject.SetActive(true);
    }

    private void ShowLeftChoice()
    {
        _state = QuizState.ShowLeftChoice;
        _timer = 1.0f;

        leftChoiceText.gameObject.SetActive(true);
    }

    private void StartCountdown()
    {
        _state = QuizState.Countdown;
        _timer = GetTimeLimit();

        countdownText.gameObject.SetActive(true);
        UpdateCountdown();
    }

    private void UpdateTilt()
    {
#if UNITY_EDITOR
        float x = Input.GetAxisRaw("Horizontal"); // ←→ または A/D
        if (x == 0 && _selectedSide != null)
        {
            x = _selectedSide == Quiz.Side.Left ? -1 : 1;
        }
#else
        float x = Input.acceleration.x;
#endif

        if (x < -tiltThreshold) _selectedSide = Quiz.Side.Left;
        else if (x > tiltThreshold) _selectedSide = Quiz.Side.Right;
        else _selectedSide = null;

        // cartTransform.localRotation = Quaternion.Euler(0, 0, -x * cartMaxAngle);

        UpdateHighlight();
    }

    private void UpdateCountdown()
    {
        countdownText.text = Mathf.CeilToInt(_timer).ToString();
    }

    private void Judge()
    {
        bool isCorrect = _selectedSide == _quiz.CorrectSide;

        if (isCorrect)
        {
            ShowResultMark(circleMark);
            girl.sprite = correctSprite;
            seAudioSource.PlayOneShot(correctSe);
            _state = QuizState.Correct;
            _timer = 0.5f;
        }
        else
        {
            _life--;
            ShowResultMark(crossMark);
            girl.sprite = wrongSprite;
            seAudioSource.PlayOneShot(wrongSe);
            _state = QuizState.Wrong;
            _timer = 0.8f;
        }

        UpdateStatus();
    }

    private IEnumerator ShowGameOver()
    {
        _state = QuizState.GameOver;
        
        guessNumberPanel.gameObject.SetActive(false);
        kanaChoicePanel.SetActive(false);
        questionPanel.SetActive(false);
        questionText.gameObject.SetActive(false);
        leftChoiceText.gameObject.SetActive(false);
        rightChoiceText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);

        yield return StartCoroutine(DerailAnimation());
        
        bgm.Stop();

        seAudioSource.PlayOneShot(gameOverSe);

        retryButton.SetActive(true);
        scoreResultText.gameObject.SetActive(true);

        int highScore = Preference.Instance.HighScore;
        Preference.Instance.HighScore = _score;
        if (highScore < _score) newRecordText.gameObject.SetActive(true);

        questionText.text = "ざんねん！";
        scoreResultText.text = $"{_score} もん";

        questionText.gameObject.SetActive(true);
        questionPanel.SetActive(true);
    }

    private float GetTimeLimit()
    {
        if (_score >= 10) return 5f;
        if (_score >= 5) return 8f;
        return 10f;
    }

    private void UpdateStatus()
    {
        scoreText.text = $"{_score}";
        lifeText.text = new string('♥', _life);
    }

    private void UpdateHighlight()
    {
        leftChoiceText.color =
            _selectedSide == Quiz.Side.Left ? highlightColor : normalColor;
        leftChoiceText.fontSize =
            _selectedSide == Quiz.Side.Left
                ? _leftChoiceTextFontSize * 1.2f
                : _leftChoiceTextFontSize;

        rightChoiceText.color =
            _selectedSide == Quiz.Side.Right ? highlightColor : normalColor;
        rightChoiceText.fontSize =
            _selectedSide == Quiz.Side.Right
                ? _rightChoiceTextFontSize * 1.2f
                : _rightChoiceTextFontSize;

        girl.sprite =　_selectedSide switch
        {
            Quiz.Side.Left => leftSprite,
            Quiz.Side.Right => rightSprite,
            _ => normalSprite
        };
    }

    private void ShowResultMark(GameObject mark)
    {
        HideResultMarksImmediate();

        _currentResultMark = mark;
        _currentResultMark.SetActive(true);
        _currentResultMark.transform.localScale = Vector3.one;
    }

    private void HideResultMarksImmediate()
    {
        circleMark.SetActive(false);
        crossMark.SetActive(false);
        _currentResultMark = null;
    }

    private IEnumerator HideResultMarkThenNext()
    {
        if (_transitioning) yield break;
        _transitioning = true;

        yield return AnimateResultMarkOut();

        _transitioning = false;

        if (_life <= 0)
            StartCoroutine(ShowGameOver());
        else
            WaitNextQuestion(nextQuestionDelay);
    }

    private IEnumerator AnimateResultMarkOut()
    {
        if (_currentResultMark == null) yield break;

        Transform t = _currentResultMark.transform;
        var elapsed = 0f;

        Vector3 fromScale = Vector3.one;
        Vector3 toScale = Vector3.zero;

        while (elapsed < resultFadeTime)
        {
            elapsed += Time.deltaTime;
            float rate = elapsed / resultFadeTime;

            t.localScale = Vector3.Lerp(fromScale, toScale, rate);

            yield return null;
        }

        _currentResultMark.SetActive(false);
        _currentResultMark = null;
    }

    private IEnumerator DerailAnimation()
    {
        cartTransform.GetComponent<CartBob>().enabled = false;
        Vector3 startPos = cartTransform.localPosition;

        // ガタン！と跳ねる
        cartTransform.localPosition = startPos + new Vector3(0, 0.5f, 0);
        yield return new WaitForSeconds(0.05f);

        cartTransform.localPosition = startPos + new Vector3(0, 1.0f, 0);
        yield return new WaitForSeconds(0.05f);

        cartTransform.localPosition = startPos + new Vector3(0, 1.5f, 0);
        yield return new WaitForSeconds(0.1f);

        // 左に傾き始める
        cartTransform.localRotation = Quaternion.Euler(0, 0, -5);
        yield return new WaitForSeconds(0.1f);

        cartTransform.localRotation = Quaternion.Euler(0, 0, -15);
        cartTransform.localPosition = startPos + new Vector3(0.5f, 1.5f, 0);
        yield return new WaitForSeconds(0.1f);

        cartTransform.localPosition = startPos + new Vector3(0.50f, 0.5f, 0);
        yield return new WaitForSeconds(0.1f);

        // 脱輪
        cartTransform.localRotation = Quaternion.Euler(0, 0, -30);
        cartTransform.localPosition =
            startPos + new Vector3(0.5f, -0.10f, 0);

        yield return new WaitForSeconds(0.1f);

        // 最後に少し沈む
        cartTransform.localPosition =
            startPos + new Vector3(0.5f, -0.20f, 0);

        cartTransform.localRotation =
            Quaternion.Euler(0, 0, -10);

        foreach (MonoBehaviour stoppable in stoppables)
        {
            (stoppable as IStoppable)?.Stop();
        }

        yield return new WaitForSeconds(1f);
    }

    private void HandleBackButton()
    {
        if (_state == QuizState.GameOver)
        {
            SceneManager.LoadScene(titleSceneName);
            return;
        }

        if (_isPauseDialogOpen)
        {
            ClosePauseDialog();
            return;
        }

        OpenPauseDialog();
    }

    private void OpenPauseDialog()
    {
        _isPauseDialogOpen = true;
        pauseDialog.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePauseDialog()
    {
        _isPauseDialogOpen = false;
        pauseDialog.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BackToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
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

    private IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        SetFadeAlpha(1f);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = 1f - t / fadeDuration;
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(0f);
        fadeImage.gameObject.SetActive(false);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    private enum QuizState
    {
        WaitingNextQuestion,
        PlayingVoice,
        QuestionIntro,
        ShowLeftChoice,
        ShowRightChoice,
        Countdown,
        Correct,
        Wrong,
        GameOver
    }
}