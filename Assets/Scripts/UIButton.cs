using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    private Solitaire _solitaire;
    private Selectable[] _selectables;

    public GameObject autoMovePanel;
    private bool move = false;
    // Start is called before the first frame update
    void Start()
    {
        _solitaire = FindObjectOfType<Solitaire>();
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            StartCoroutine(_solitaire.AutoComplete());
            move = !_solitaire.isDone();
        }   
    }

    public void ResetAction()
    {
        _solitaire.ResetGame();
        _selectables = FindObjectsByType<Selectable>(FindObjectsSortMode.None);
        foreach (var selectable in _selectables)
        {
            if(selectable.name != "Card")
                Destroy(selectable.gameObject);
        }
        _solitaire.PlayCards();
    }

    public void AutoMove()
    {
        autoMovePanel.SetActive(false);
        move = true;
    }
    // Sadece test etmek amacli kartlarin yerin karistiriginda tekrar duzene sokmak icin
    // public void FixBottomCard()
    // {
    //     var listBottoms = _solitaire.bottoms;
    //     int i = 0;
    //     foreach (var bottoms in listBottoms)
    //     {
    //         var bottomPos = _solitaire.bottomPos[i];
    //         float yOffset = 0;
    //         float zOffset = 0.01f;
    //         foreach (var bottom in bottoms)
    //         {
    //             var card = bottom.GetComponent<Selectable>();
    //             Vector3 position = new Vector3(bottomPos.transform.position.x,
    //                 bottomPos.transform.position.y - yOffset, bottomPos.transform.position.z - zOffset);
    //             
    //             yOffset += 0.4f;
    //             zOffset += 0.01f;
    //             card.transform.position = card.SetLastPosition(position);
    //         }
    //
    //         i++;
    //     }
    // }

    public void PlayAgain()
    {
        _solitaire.panel.SetActive(false);
        _solitaire.ResetGame();
        _selectables = FindObjectsByType<Selectable>(FindObjectsSortMode.None);
        foreach (var selectable in _selectables)
        {
            if(selectable.name != "Card")
                Destroy(selectable.gameObject);
        }
        _solitaire.PlayCards();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
