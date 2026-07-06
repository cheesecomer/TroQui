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

    public Quiz(KanaChoiceSource[] kanaChoiceSources, VoiceDatabase voiceDatabase)
    {
        for (var i = 0; i < MaxAttempts; i++)
        {
            QuizType type = GetRandomQuizType();
            GenerateInternal(type, kanaChoiceSources, voiceDatabase);
            if (!RecentKeys.Contains(_similarityKey))
            {
                break;
            }
        }

        Remember(_similarityKey);
    }

    private void GenerateInternal(QuizType type, KanaChoiceSource[] kanaChoiceSources, VoiceDatabase voiceDatabase)
    {
        switch (type)
        {
            case QuizType.Addition:
                GenerateAdditionQuiz(voiceDatabase);
                break;
            case QuizType.KanaFill:
                GenerateKanaFillQuiz(voiceDatabase);
                break;
            case QuizType.GuessNumber:
                GenerateGuessNumberQuiz(voiceDatabase);
                break;
            case QuizType.KanaChoice:
                GenerateKanaChoiceQuiz(kanaChoiceSources, voiceDatabase);
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
    public VoiceSequence[] VoiceSequences { get; private set; }

    private static QuizType GetRandomQuizType()
    {
        var values = (QuizType[])Enum.GetValues(typeof(QuizType));
        return values[Random.Range(0, values.Length)];
    }

    private void GenerateAdditionQuiz(VoiceDatabase voiceDatabase)
    {
        int left = Random.Range(1, 5 + 1);
        int right = Random.Range(1, 5 + 1);
        int answer = left + right; // 2～10

        Question = $"{left} + {right}";
        VoiceSequences = voiceDatabase.GetAdditionVoice(left, right);

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

    private void GenerateKanaFillQuiz(VoiceDatabase voiceDatabase)
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
        VoiceSequences = voiceDatabase.GetKanaVoice(correctKana);

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

    private void GenerateGuessNumberQuiz(VoiceDatabase voiceDatabase)
    {
        ItemNum = Random.Range(1, 11);
        _similarityKey = $"guess_number:{ItemNum}";

        Question = "なんこある？";
        VoiceSequences = voiceDatabase.GuessNumberVoice;

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

    private void GenerateKanaChoiceQuiz(KanaChoiceSource[] sources, VoiceDatabase voiceDatabase)
    {
        KanaChoiceSource source = sources[Random.Range(0, sources.Length)];
        _similarityKey = $"kana_choice:{source.vocabulary.ToText()}";

        var correctKana = source.vocabulary.ToText()[0].ToString();

        Question = "□" + source.vocabulary.ToText()[1..];
        Image = source.sprite;
        VoiceSequences = voiceDatabase.GetKanaChoiceVoice(source.vocabulary);

        string wrongKana;   
        do
        {
            KanaChoiceSource wrongSource = sources[Random.Range(0, sources.Length)];
            wrongKana = wrongSource.vocabulary.ToText()[0].ToString();
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