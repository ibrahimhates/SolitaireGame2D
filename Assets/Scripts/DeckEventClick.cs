using UnityEngine;

public class DeckEventClick : MonoBehaviour
{
    // Start is called before the first frame update
    private Solitaire _solitaire;
    void Start()
    {
        _solitaire = FindObjectOfType<Solitaire>();
    }

    void OnMouseDown()
    {
        _solitaire.ShowCardFaceOnDeck();
        if (_solitaire.deckCount == 0)
            this.GetComponent<SpriteRenderer>().color = Color.gray;
        else
            this.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
