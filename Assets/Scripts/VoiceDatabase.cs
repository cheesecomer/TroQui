using System.Linq;
using UnityEngine;

public class KanaQuiz
{
    public AudioClip[] Clips;
}


[CreateAssetMenu(
    fileName = "VoiceDatabase",
    menuName = "TroQui/Voice Database"
)]
public class VoiceDatabase : ScriptableObject
{
    [SerializeField] private VoiceSequence[] guessNumberVoice;

    [SerializeField] private AdditionVoiceEntry[] additionVoices;
    [SerializeField] private KanaVoiceEntry[] kanaVoices;
    [SerializeField] private KanaChoiceVoiceEntry[] kanaChoiceVoices;

    public VoiceSequence[] GetAdditionVoice(int left, int right)
    {
        return additionVoices.FirstOrDefault(x => x.left == left && x.right == right)?.voices ?? new VoiceSequence[]{};
    }

    public VoiceSequence[] GetKanaVoice(char kana)
    {
        return kanaVoices.FirstOrDefault(x => x.kana == kana)?.voices ?? new VoiceSequence[]{};
    }

    public VoiceSequence[] GetKanaChoiceVoice(Vocabulary vocabulary)
    {
        return kanaChoiceVoices.FirstOrDefault(x => x.vocabulary == vocabulary)?.voices ?? new VoiceSequence[]{};
    }

    public VoiceSequence[] GuessNumberVoice => guessNumberVoice;
}

[System.Serializable]
public class VoiceSequence
{
    public AudioClip clip;
    public float delay;
}

[System.Serializable]
public class AdditionVoiceEntry
{
    public int left;
    public int right;
    public VoiceSequence[] voices;
}

[System.Serializable]
public class KanaVoiceEntry
{
    public char kana;
    public VoiceSequence[] voices;
}

[System.Serializable]
public class KanaChoiceVoiceEntry
{
    public Vocabulary vocabulary;
    public VoiceSequence[] voices;
}