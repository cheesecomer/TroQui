using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GuessNumberPanel : MonoBehaviour
{
    [SerializeField] private Transform itemsRoot;
    [SerializeField] private GameObject[] guessNumberPrefabs;

    private readonly Dictionary<int, Vector2[]> itemPositionPatterns = new()
    {
        { 1, new[] { new Vector2(0, 0) } },

        { 2, new[]
            {
                new Vector2(-70, 20),
                new Vector2(70, -20),
            }
        },

        { 3, new[]
            {
                new Vector2(-90, -20),
                new Vector2(0, 55),
                new Vector2(90, -20),
            }
        },

        { 4, new[]
            {
                new Vector2(-90, 45),
                new Vector2(90, 45),
                new Vector2(-80, -55),
                new Vector2(80, -55),
            }
        },

        { 5, new[]
            {
                new Vector2(-140, 50),
                new Vector2(0, 65),
                new Vector2(140, 50),
                new Vector2(-70, -55),
                new Vector2(70, -55),
            }
        },

        { 6, new[]
            {
                new Vector2(-150, 55),
                new Vector2(0, 65),
                new Vector2(150, 55),
                new Vector2(-150, -55),
                new Vector2(0, -65),
                new Vector2(150, -55),
            }
        },
        {
            7, new[]
            {
                new Vector2(-140,  70),
                new Vector2(   0,  80),
                new Vector2( 140,  70),

                new Vector2(-180,   0),
                new Vector2(   0,   0),
                new Vector2( 180,   0),

                new Vector2(   0, -80),
            }
        },
        {
            8, new[]
            {
                new Vector2(-150,  80),
                new Vector2( -50,  90),
                new Vector2(  50,  90),
                new Vector2( 150,  80),

                new Vector2(-150, -20),
                new Vector2( -50, -40),
                new Vector2(  50, -40),
                new Vector2( 150, -20),
            }
        },
        {
            9, new[]
            {
                new Vector2(-150,  80),
                new Vector2(   0,  90),
                new Vector2( 150,  80),

                new Vector2(-170,   0),
                new Vector2(   0,   0),
                new Vector2( 170,   0),

                new Vector2(-150, -80),
                new Vector2(   0, -90),
                new Vector2( 150, -80),
            }
        },
        {
            10, new[]
            {
                new Vector2(-180,  80),
                new Vector2( -90,  90),
                new Vector2(   0, 100),
                new Vector2(  90,  90),
                new Vector2( 180,  80),

                new Vector2(-180, -40),
                new Vector2( -90, -60),
                new Vector2(   0, -70),
                new Vector2(  90, -60),
                new Vector2( 180, -40),
            }
        }
    };

    private float GetItemScale(int count)
    {
        return count switch
        {
            <= 1 => 1.5f,
            <= 2 => 1.35f,
            <= 3 => 1.2f,
            <= 5 => 1.05f,
            <= 7 => 0.9f,
            _ => 0.78f,
        };
    }

    public void ShowItems(int count)
    {
        foreach (Transform child in itemsRoot)
        {
            Destroy(child.gameObject);
        }

        var prefab = this.guessNumberPrefabs[UnityEngine.Random.Range(0, this.guessNumberPrefabs.Length)];
        var positions = this.itemPositionPatterns[count];

        float baseScale = GetItemScale(count);
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, itemsRoot);
            var rect = obj.GetComponent<RectTransform>();
            float randomScale = Random.Range(0.95f, 1.05f);

            rect.anchoredPosition = positions[i];
            rect.localRotation = Quaternion.Euler(0, 0, Random.Range(-12f, 12f));
            rect.localScale = Vector3.one * baseScale * randomScale;
        }
    }
}
