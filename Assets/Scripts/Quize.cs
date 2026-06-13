using System;
using UnityEngine;

public class Quiz {
    public enum QuizType {
        Addition,
        KanaFill,
        GuessNumber,
        // KanaChoice
    }

    public enum Side {
        Left,
        Right
    }

    public string Question { get; private set; }
    public string Left { get; private set; }
    public string Right { get; private set; }
    public Side CorrectSide { get; private set; }
    public QuizType Type { get; private set; }
    public int ItemNum { get; private set; }

    public Quiz() : this(GetRandomQuizType()) {
    }

    public Quiz(QuizType type) {
        switch (type) {
            case QuizType.Addition:
                GenerateAdditionQuiz();
                break;
            case QuizType.KanaFill:
                GenerateKanaFillQuiz();
                break;
            case QuizType.GuessNumber:
                GenerateGuessNumberQuiz();
                break;
        }
        this.Type = type;
    }
    private static QuizType GetRandomQuizType()
    {
        var values = (QuizType[])Enum.GetValues(typeof(QuizType));
        return values[UnityEngine.Random.Range(0, values.Length)];
    }

    private void GenerateAdditionQuiz() {
        int answer = UnityEngine.Random.Range(2, 11); // 2～10
        int left = UnityEngine.Random.Range(1, answer);
        int right = answer - left;

        Question = $"{left} + {right}";

        bool correctIsLeft = UnityEngine.Random.value < 0.5f;

        int wrongAnswer;
        do
        {
            wrongAnswer = UnityEngine.Random.Range(1, 11);
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

        string row = rows[UnityEngine.Random.Range(0, rows.Length)];

        int missingIndex = UnityEngine.Random.Range(1, row.Length);

        char correctKana = row[missingIndex];

        char[] chars = row.ToCharArray();
        chars[missingIndex] = '□';

        Question = new string(chars);

        char wrongKana;
        do
        {
            string wrongRow = rows[UnityEngine.Random.Range(0, rows.Length)];
            wrongKana = wrongRow[UnityEngine.Random.Range(0, wrongRow.Length)];
        }
        while (row.IndexOf(wrongKana) != -1);

        bool correctIsLeft = UnityEngine.Random.value < 0.5f;

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
        this.ItemNum = UnityEngine.Random.Range(1, 11);

        this.Question = "なんこある？";

        int wrongAnswer;
        do
        {
            wrongAnswer = UnityEngine.Random.Range(1, 11);
        }
        while (wrongAnswer == ItemNum);

        if (UnityEngine.Random.value < 0.5f)
        {
            Left = $"← {ItemNum}";
            Right = $"{wrongAnswer} →";
            CorrectSide = Quiz.Side.Left;
        }
        else
        {
            Left = $"← {wrongAnswer}";
            Right = $"{ItemNum} →";
            CorrectSide = Quiz.Side.Right;
        }
    }
}