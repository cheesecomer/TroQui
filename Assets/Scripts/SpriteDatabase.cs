using UnityEngine;

[CreateAssetMenu(
    fileName = "SpriteDatabase",
    menuName = "TroQui/Sprite Database"
)]
public class SpriteDatabase : ScriptableObject
{
    public Sprite[] grasses;
    public Sprite[] flowers;
    public Sprite[] trees;
    public Sprite[] clouds;
    public KanaChoiceSource[] kanaChoiceSources;
}