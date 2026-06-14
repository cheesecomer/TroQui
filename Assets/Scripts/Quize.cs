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

    public Quiz(KanaChoiceSource[] kanaChoiceSources) : this(GetRandomQuizType(), kanaChoiceSources)
    {
    }

    public Quiz(QuizType type, KanaChoiceSource[] kanaChoiceSources)
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
                GenerateKanaChoiceQuiz(kanaChoiceSources);
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
        int left = Random.Range(1, 5 + 1);
        int right = Random.Range(1, 5 + 1);
        int answer = left + right; // 2～10

        Question = $"{left} + {right}";


        int wrongAnswer;
        do
        {
            wrongAnswer = Random.Range(2, 11);
        } while (wrongAnswer == answer);

        bool correctIsLeft = Random.value < 0.5f;
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

    private void GenerateKanaChoiceQuiz(KanaChoiceSource[] sources)
    {
        KanaChoiceSource source = sources[Random.Range(0, sources.Length)];

        var correctKana = source.word[0].ToString();

        Question = "□" + source.word[1..];
        Image = source.sprite;

        string wrongKana;
        do
        {
            KanaChoiceSource wrongSource = sources[Random.Range(0, sources.Length)];
            wrongKana = wrongSource.word[0].ToString();
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