using System;
using UnityEngine;

public enum Vocabulary
{
    Duck,          // あひる
    Dog,           // いぬ
    Rabbit,        // うさぎ
    Shrimp,        // えび
    Ogre,          // おに
    Umbrella,      // かさ
    Giraffe,       // きりん
    Car,           // くるま
    Cake,          // ケーキ
    Koala,         // コアラ
    Fish,          // さかな
    BulletTrain,   // しんかんせん
    Watermelon,    // すいか
    Soap,          // せっけん
    VacuumCleaner, // そうじき
    Octopus,       // たこ
    Cheese,        // チーズ
    Moon,          // つき
    Television,    // テレビ
    Tomato,        // トマト
    Eggplant,      // なす
    Carrot,        // にんじん
    StuffedToy,    // ぬいぐるみ
    Cat,           // ねこ
    Saw,           // のこぎり
    Bouquet,       // はなたば
    Airplane,      // ひこうき
    Balloon,       // ふうせん
    Snake,         // へび
    Star,          // ほし
    PineCone,      // まつぼっくり,
    SatsumaMandarin,// みかん
    StrawHat,      // むぎわらぼうし
    Glasses,       // メガネ
    Peach,         // もも
    Goat,          // ヤギ
    Snow,          // ゆき
    Yacht,         // ヨット
    Lion,          // ライオン
    Apple,         // りんご
    Lemon,         // れもん
    Candle,        // ろうそく
    Crocodile      // ワニ
}

public static class VocabularyExtensions
{
    public static string ToText(this Vocabulary vocabulary)
    {
        return vocabulary switch
        {
            Vocabulary.Duck => "あひる",
            Vocabulary.Dog => "いぬ",
            Vocabulary.Rabbit => "うさぎ",
            Vocabulary.Shrimp => "えび",
            Vocabulary.Ogre => "おに",
            Vocabulary.Umbrella => "かさ",
            Vocabulary.Giraffe => "きりん",
            Vocabulary.Car => "くるま",
            Vocabulary.Cake => "けーき",
            Vocabulary.Koala => "こあら",
            Vocabulary.Fish => "さかな",
            Vocabulary.BulletTrain => "しんかんせん",
            Vocabulary.Watermelon => "すいか",
            Vocabulary.Soap => "せっけん",
            Vocabulary.VacuumCleaner => "そうじき",
            Vocabulary.Octopus => "たこ",
            Vocabulary.Cheese => "ちーず",
            Vocabulary.Moon => "つき",
            Vocabulary.Television => "てれび",
            Vocabulary.Tomato => "とまと",
            Vocabulary.Eggplant => "なす",
            Vocabulary.Carrot => "にんじん",
            Vocabulary.StuffedToy => "ぬいぐるみ",
            Vocabulary.Cat => "ねこ",
            Vocabulary.Saw => "のこぎり",
            Vocabulary.Bouquet => "はなたば",
            Vocabulary.Airplane => "ひこうき",
            Vocabulary.Balloon => "ふうせん",
            Vocabulary.Snake => "へび",
            Vocabulary.Star => "ほし",
            Vocabulary.PineCone => "まつぼっくり",
            Vocabulary.SatsumaMandarin => "みかん",
            Vocabulary.StrawHat => "むぎわらぼうし",
            Vocabulary.Glasses => "めがね",
            Vocabulary.Peach => "もも",
            Vocabulary.Goat => "やぎ",
            Vocabulary.Snow => "ゆき",
            Vocabulary.Yacht => "よっと",
            Vocabulary.Lion => "らいおん",
            Vocabulary.Apple => "りんご",
            Vocabulary.Lemon => "れもん",
            Vocabulary.Candle => "ろうそく",
            Vocabulary.Crocodile => "わに",
            _ => throw new ArgumentOutOfRangeException(nameof(vocabulary), vocabulary, null)
        };
    }
}

[Serializable]
public class KanaChoiceSource
{
    public Vocabulary vocabulary;
    public Sprite sprite;
}