using System.Collections;
using System.Collections.Generic;
using Helpers;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    private Sprite cardFace;
    private Sprite cardBack;
    private SpriteRenderer _spriteRenderer;
    private Selectable _selectable;
    private Solitaire _solitaire;
    
    // Start is called before the first frame update
    void Start()
    {
        
        cardBack = PlayCards.GetCardsBackSprite;
        List<string> deck = Solitaire.GenerateDeck();
        _solitaire = FindObjectOfType<Solitaire>();
        deck.ForEach(card =>
        {
            if (this.name == card)
            {
                cardFace = PlayCards.GetCardsSprite(this.name);
            }
        });
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _selectable = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_selectable.faceUp)
        {
            _spriteRenderer.sprite = cardFace;
        }
        else
        {
            _spriteRenderer.sprite = cardBack;
        }
    }
}
