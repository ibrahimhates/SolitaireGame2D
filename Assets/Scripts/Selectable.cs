using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Helpers;
using JetBrains.Annotations;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    private bool isDragging = false;
    private bool isDraggingEnd = false;
    private Vector3 offset;
    private Vector3 lastPosition;
    private string valueString;
    private Selectable[] _selectables;
    private Solitaire _solitaire;
    public bool faceUp = false;
    private int value;
    private string suit;
    public bool deck;
    public bool top;
    public int row;
    private const float TOP_BOTTOM_AMONG_BORDER_Y = 1.0f;
    private const float BOTTOM_START_X = -5f;
    private const float BOTTOM_CARD_WIDTH = 1f;
    private const float TOP_START_X = 1f;

    private float perceptionDistance = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = this.transform.position;
        _solitaire = FindObjectOfType<Solitaire>();
        if (this.name != "Card")
        {
            suit = this.name[0].ToString();
            string stringValue = "";
            for (int i = 1; i < this.name.Length; i++)
            {
                stringValue += this.name[i];
            }

            if (stringValue == "A")
                value = 1;
            else if (stringValue == "J")
                value = 11;
            else if (stringValue == "Q")
                value = 12;
            else if (stringValue == "K")
                value = 13;
            else
                value = int.Parse(stringValue);
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (faceUp)
        {
            if (isDragging)
            {
                this.GetComponent<SpriteRenderer>().color = Color.gray;
                Vector3 mousePosition = Input.mousePosition;
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
                transform.position = new Vector3(newPosition.x, newPosition.y, -5f);
                isDraggingEnd = true;
            }
            else if (isDraggingEnd)
            {
                bool isChangePosition = false;
                this.GetComponent<SpriteRenderer>().color = Color.white;
                _selectables = FindObjectsByType<Selectable>(FindObjectsSortMode.None);

                MatchingPlace place = CheckCollision();

                if (place == MatchingPlace.TOP)
                {
                    int snapRow = GetMatchingRowInTop();
                    if (snapRow != -1)
                    {
                        if (this.value == 1)
                        {
                            var aceCollisionResult = CheckCollisiionForAce(snapRow);
                            if (aceCollisionResult != null)
                                isChangePosition = CheckRightPositionForAce(aceCollisionResult);
                        }
                        else
                        {
                            isChangePosition = CheckRightPosition(place, snapRow);
                        }
                    }
                }
                else if (this.value == 13)
                {
                    var kingCollisionResult = CheckCollisionForKing();
                    if (kingCollisionResult.selectedObject != null)
                        isChangePosition =
                            CheckRightPositionForKing(kingCollisionResult.selectedObject, kingCollisionResult.i);
                }
                else if (place == MatchingPlace.BOTTOM)
                {
                    int snapRow = GetMatchingRowInBottom();
                    if (snapRow != -1)
                    {
                        isChangePosition = CheckRightPosition(place, snapRow);
                    }
                }

                if (!isChangePosition)
                    this.transform.position = lastPosition;

                isDraggingEnd = false;
            }
        }
    }

    (GameObject? selectedObject, int i) CheckCollisionForKing()
    {
        if (this.value == 13)
        {
            int i = 0;
            foreach (GameObject bottomPos in _solitaire.bottomPos)
            {
                if (_solitaire.bottoms[i].Any())
                {
                    i++;
                    continue;
                }

                float distance = Vector2.Distance(this.transform.position, bottomPos.transform.position);
                if (distance < perceptionDistance)
                {
                    Debug.Log("Çarpışma gerçekleşti!");
                    return (bottomPos, i);
                }

                i++;
            }
        }

        return (null, 0);
    }

    GameObject? CheckCollisiionForAce(int snapIndex)
    {
        var topPos = _solitaire.topPos.FirstOrDefault(x => x.name == this.suit);
        int thisIndex = _solitaire.suitIndex[this.suit];

        if (thisIndex == snapIndex)
            return topPos;

        return null;
    }

    MatchingPlace CheckCollision()
    {
        if (this.transform.position.y < TOP_BOTTOM_AMONG_BORDER_Y)
        {
            return MatchingPlace.BOTTOM;
        }
        else if (this.transform.position.y > TOP_BOTTOM_AMONG_BORDER_Y
                 && this.transform.position.x > TOP_START_X)
        {
            return MatchingPlace.TOP;
        }

        return MatchingPlace.NOWHERE;
    }

    private int GetMatchingRowInBottom()
    {
        float xOffset = 1f;

        for (int i = 0; i < 7; i++)
        {
            float xPosition = BOTTOM_START_X + BOTTOM_CARD_WIDTH * i + xOffset * i;
            if (this.transform.position.x > xPosition
                && this.transform.position.x < xPosition + BOTTOM_CARD_WIDTH)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetMatchingRowInTop()
    {
        float xOffset = 1f;

        for (int i = 0; i < 4; i++)
        {
            float xPosition = TOP_START_X + BOTTOM_CARD_WIDTH * i + xOffset * i;
            if (this.transform.position.x > xPosition
                && this.transform.position.x < BOTTOM_CARD_WIDTH + xPosition)
            {
                return i;
            }
        }

        return -1;
    }

    bool CheckRightPosition(MatchingPlace place, int snapRow)
    {
        Selectable snap;
        if (place == MatchingPlace.BOTTOM)
        {
            if (this.row == snapRow)
                return false;

            if (!_solitaire.bottoms[snapRow].Any())
                return false;

            snap = _solitaire.bottoms[snapRow].Last().GetComponent<Selectable>();
        }
        else
        {
            snap = GetByName(_solitaire.tops[snapRow].Last());
        }

        bool snapIsRed = snap.suit == "D" || snap.suit == "H";
        bool selectedIsRed = this.suit == "D" || this.suit == "H";

        if (snapIsRed != selectedIsRed)
        {
            if (snap.value == this.value + 1)
            {
                if (snap.name == _solitaire.bottoms[snap.row].Last().name)
                {
                    if (this.deck)
                    {
                        InDeck(snap);
                        return true;
                    }
                    else if (!this.top)
                    {
                        InBottom(snap);
                        return true;
                    }
                    else
                    {
                        FromTopToBottom(snap);
                    }
                }
            }
        }
        else
        {
            if (snap.value == this.value - 1)
            {
                if (snap.suit == this.suit)
                {
                    ToTop(snap);
                    return true;
                }
            }
        }

        return false;
    }

    void FromTopToBottom(Selectable snap)
    {
        float yOffset = 0.4f;
        float zOffset = 0.03f;

        Vector3 position = new Vector3(snap.transform.position.x,
            snap.transform.position.y - yOffset, snap.transform.position.z - zOffset);


        _solitaire.tops[_solitaire.suitIndex[this.suit]].Remove(this.name);
        this.top = false;
        _solitaire.bottoms[snap.row].Add(this.gameObject);
        this.row = snap.row;
        this.transform.position = SetLastPosition(position);
    }

    void ToTop(Selectable snap)
    {
        this.top = true;

        if (!this.deck && _solitaire.bottoms[this.row].Any(x => x == this.gameObject))
        {
            _solitaire.bottoms[this.row].Remove(this.gameObject);
            if (_solitaire.bottoms[this.row].Any())
            {
                var card = GetByName(_solitaire.bottoms[this.row].Last().name);
                card.faceUp = true;
            }
        }

        this.deck = false;
        _solitaire.visibleCardOnDeck.Remove(this.gameObject);

        this.row = -1;

        Vector3 position = new Vector3(snap.transform.position.x,
            snap.transform.position.y, snap.transform.position.z - 0.03f);

        _solitaire.tops[_solitaire.suitIndex[this.suit]].Add(this.name);
        this.transform.position = SetLastPosition(position);
    }

    bool CheckRightPositionForKing(GameObject snap, int row)
    {
        if (this.deck)
        {
            this.deck = false;
            _solitaire.visibleCardOnDeck.Remove(this.gameObject);

            this.row = row;
            _solitaire.bottoms[row].Add(this.gameObject);
            Vector3 position = new Vector3(snap.transform.position.x,
                snap.transform.position.y, snap.transform.position.z - 0.03f);

            this.transform.position = SetLastPosition(position);

            return true;
        }
        else if (!this.top)
        {
            var multibleCard = InBottomTogether();

            float yOffset = 0;
            float zOffset = 0.03f;
            foreach (var name in multibleCard)
            {
                var card = GetByName(name.name);
                Vector3 position = new Vector3(snap.transform.position.x,
                    snap.transform.position.y - yOffset, snap.transform.position.z - zOffset);

                _solitaire.bottoms[row].Add(card.gameObject);
                card.row = row;
                card.transform.position = card.SetLastPosition(position);
                yOffset += 0.4f;
                zOffset += zOffset;
            }
        }

        return false;
    }

    bool CheckRightPositionForAce(GameObject snap)
    {
        if (this.deck)
        {
            this.deck = false;
            this.top = true;
            _solitaire.visibleCardOnDeck.Remove(this.gameObject);

            Vector3 position = new Vector3(snap.transform.position.x,
                snap.transform.position.y, snap.transform.position.z - 0.03f);
            this.transform.position = SetLastPosition(position);

            _solitaire.tops[_solitaire.suitIndex[this.suit]].Add(this.name);
            return true;
        }
        else if (!this.top)
        {
            _solitaire.bottoms[this.row].Remove(this.gameObject);

            if (_solitaire.bottoms[this.row].Any())
            {
                var card = GetByName(_solitaire.bottoms[this.row].Last().name);
                card.faceUp = true;
            }

            Vector3 position = new Vector3(snap.transform.position.x,
                snap.transform.position.y, snap.transform.position.z - 0.03f);

            this.top = true;
            this.row = -1;
            this.transform.position = this.SetLastPosition(position);

            _solitaire.tops[_solitaire.suitIndex[this.suit]].Add(this.name);
            return true;
        }

        return false;
    }

    void InDeck(Selectable snap)
    {
        this.deck = false;
        _solitaire.visibleCardOnDeck.Remove(this.gameObject);

        this.row = snap.row;
        _solitaire.bottoms[snap.row].Add(this.gameObject);
        Vector3 position = new Vector3(snap.transform.position.x,
            snap.transform.position.y - 0.4f, snap.transform.position.z - 0.03f);

        this.transform.position = SetLastPosition(position);
    }

    // Ortak Kullanilan alanlar
    List<GameObject> InBottomTogether()
    {
        int size = _solitaire.bottoms[this.row].Count;
        int selectedIndex = _solitaire.bottoms[this.row].FindIndex(s => s == this.gameObject);

        var multibleCard = new List<GameObject>();
        if (size - 1 > selectedIndex)
        {
            for (int i = selectedIndex; i < size; i++)
            {
                var item = _solitaire.bottoms[this.row][i];
                multibleCard.Add(item);
            }

            foreach (var removed in multibleCard)
                _solitaire.bottoms[this.row].Remove(removed);
        }
        else
        {
            multibleCard.Add(this.gameObject);
            _solitaire.bottoms[this.row].Remove(this.gameObject);
        }

        if (_solitaire.bottoms[this.row].Any())
        {
            var lastHideCard = GetByName(_solitaire.bottoms[this.row].Last().name);
            lastHideCard.faceUp = true;
        }

        return multibleCard;
    }

    void InBottom(Selectable snap)
    {
        var multibleCard = InBottomTogether();

        float yOffset = 0.4f;
        float zOffset = 0.01f;
        foreach (var cardObject in multibleCard)
        {
            var card = GetByName(cardObject.name);
            Vector3 position = new Vector3(snap.transform.position.x,
                snap.transform.position.y - yOffset, snap.transform.position.z - zOffset);

            yOffset += 0.4f;
            zOffset += 0.01f;
            _solitaire.bottoms[snap.row].Add(cardObject);
            card.row = snap.row;
            card.transform.position = SetLastPosition(position);
        }
    }

    Selectable GetByName(string name)
    {
        return _selectables.First(x => x.name == name);
    }

    public Vector3 SetLastPosition(Vector3 vector3)
    {
        lastPosition = vector3;
        return vector3;
    }
}