using System.Collections;
using UnityEngine;

public class HighlightableSprite : MonoBehaviour
{

    [SerializeField] private SpriteType type;

    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Sprite unhighlightedSprite;
    [SerializeField] private Sprite highlightedSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetHighlight(false);
        StartCoroutine(RegisterWithGM());

    }

    private IEnumerator RegisterWithGM()
    {
        while(GameManager.instance == null)
        {
            yield return null;
        }

        switch(type)
        {
            case SpriteType.Orpheus:
                GameManager.instance.ODisplay = this;
                break;
            case SpriteType.Eurydice:
                GameManager.instance.EDisplay = this;
                break;
        }
    }

    public void SetHighlight(bool highlight)
    {
        _sr.sprite = highlight ? highlightedSprite : unhighlightedSprite;
    }

    
}

public enum SpriteType
{
    Orpheus,
    Eurydice,
}
