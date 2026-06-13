using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Quiz
{
    public enum QuizType
    {
        Addition,
        KanaFill,
        GuessNumber,
        KanaChoice
    }

    public enum Side
    {
        Left,
        Right
    }

    public Quiz(KanaChoiceQuestion[] kanaChoiceQuestions) : this(GetRandomQuizType(), kanaChoiceQuestions)
    {
    }

    public Quiz(QuizType type, KanaChoiceQuestion[] kanaChoiceQuestions)
    {
        switch (type)
        {
            case QuizType.Addition:
                GenerateAdditionQuiz();
                break;
            case QuizType.KanaFill:
                GenerateKanaFillQuiz();
                break;
            case QuizType.GuessNumber:
                GenerateGuessNumberQuiz();
                break;
            case QuizType.KanaChoice:
                GenerateKanaChoiceQuiz(kanaChoiceQuestions);
                break;
        }

        Type = type;
    }

    public string Question { get; private set; }
    public string Left { get; private set; }
    public string Right { get; private set; }
    public Side CorrectSide { get; private set; }
    public QuizType Type { get; private set; }
    public int ItemNum { get; private set; }
    public Sprite Image { get; private set; }

    private static QuizType GetRandomQuizType()
    {
        var values = (QuizType[])Enum.GetValues(typeof(QuizType));
        return values[Random.Range(0, values.Length)];
    }

    private void GenerateAdditionQuiz()
    {
        int answer = Random.Range(2, 11); // 2～10
        int left = Random.Range(1, 5);
        int right = answer - left;

        Question = $"{left} + {right}";

        bool correctIsLeft = Random.value < 0.5f;

        int wrongAnswer;
        do
        {
            wrongAnswer = Random.Range(1, 11);
        } while (wrongAnswer == answer);

        if (correctIsLeft)
        {
            Left = $"← {answer}";
            Right = $"{wrongAnswer} →";
            CorrectSide = Side.Left;
        }
        else
        {
            Left = $"← {wrongAnswer}";
            Right = $"{answer} →";
            CorrectSide = Side.Right;
        }
    }

    private void GenerateKanaFillQuiz()
    {
        string[] rows =
        {
            "あいうえお",
            "かきくけこ",
            "さしすせそ",
            "たちつてと",
            "なにぬねの",
            "はひふへほ",
            "まみむめも",
            "やゆよ",
            "らりるれろ",
            "わをん"
        };

        string row = rows[Random.Range(0, rows.Length)];

        int missingIndex = Random.Range(1, row.Length);

        char correctKana = row[missingIndex];

        char[] chars = row.ToCharArray();
        chars[missingIndex] = '□';

        Question = new string(chars);

        char wrongKana;
        do
        {
            string wrongRow = rows[Random.Range(0, rows.Length)];
            wrongKana = wrongRow[Random.Range(0, wrongRow.Length)];
        } while (row.IndexOf(wrongKana) != -1);

        bool correctIsLeft = Random.value < 0.5f;

        if (correctIsLeft)
        {
            Left = $"← {correctKana}";
            Right = $"{wrongKana} →";
            CorrectSide = Side.Left;
        }
        else
        {
            Left = $"← {wrongKana}";
            Right = $"{correctKana} →";
            CorrectSide = Side.Right;
        }
    }

    private void GenerateGuessNumberQuiz()
    {
        ItemNum = Random.Range(1, 11);

        Question = "なんこある？";

        int wrongAnswer;
        do
        {
            wrongAnswer = Random.Range(1, 11);
        } while (wrongAnswer == ItemNum);

        if (Random.value < 0.5f)
        {
            Left = $"← {ItemNum}";
            Right = $"{wrongAnswer} →";
            CorrectSide = Side.Left;
        }
        else
        {
            Left = $"← {wrongAnswer}";
            Right = $"{ItemNum} →";
            CorrectSide = Side.Right;
        }
    }

    private void GenerateKanaChoiceQuiz(KanaChoiceQuestion[] sources)
    {
        KanaChoiceQuestion source = sources[Random.Range(0, sources.Length)];

        string correctKana = source.word.Substring(0, 1);

        Question = "□" + source.word.Substring(1);
        Image = source.sprite;

        string wrongKana;
        do
        {
            KanaChoiceQuestion wrongSource = sources[Random.Range(0, sources.Length)];
            wrongKana = wrongSource.word.Substring(0, 1);
        } while (wrongKana == correctKana);

        if (Random.value < 0.5f)
        {
            Left = $"← {correctKana}";
            Right = $"{wrongKana} →";
            CorrectSide = Side.Left;
        }
        else
        {
            Left = $"← {wrongKana}";
            Right = $"{correctKana} →";
            CorrectSide = Side.Right;
        }
    }
}