using System;
using System.Collections.Generic;
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
    
    private static readonly Queue<string> RecentKeys = new();
    private const int RecentHistorySize = 10;
    private const int MaxAttempts = 20;

    private static void Remember(string key)
    {
        RecentKeys.Enqueue(key);

        while (RecentKeys.Count > RecentHistorySize)
        {
            RecentKeys.Dequeue();
        }
    }

    private string _similarityKey;

    public Quiz(KanaChoiceSource[] kanaChoiceSources)
    {
        for (var i = 0; i < MaxAttempts; i++)
        {
            QuizType type = GetRandomQuizType();
            GenerateInternal(type, kanaChoiceSources);
            if (!RecentKeys.Contains(_similarityKey))
            {
                break;
            }
        }

        Remember(_similarityKey);
    }

    private void GenerateInternal(QuizType type, KanaChoiceSource[] kanaChoiceSources)
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

        int a = Mathf.Min(left, right);
        int b = Mathf.Max(left, right);
        _similarityKey = $"addition:{a}+{b}";
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
        _similarityKey = $"kana_fill:{row}";

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
        _similarityKey = $"guess_number:{ItemNum}";

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
        _similarityKey = $"kana_choice:{source.word}";

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