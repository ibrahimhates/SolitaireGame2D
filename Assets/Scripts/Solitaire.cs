using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Solitaire : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject[] bottomPos;
    public GameObject[] topPos;
    public GameObject deckButton;
    public GameObject panel;
    public GameObject scoreTime;
    private TimerControl timeControl;
    // Hearts - Kalp | Diamonds - Karo | Clubs - Maça | Spades - Sinek
    public static string[] suits = new string[] { "C", "D", "H", "S" };

    // Ace - As | King - Kral | Queen - Kraliçe | Jack - Vale
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    private readonly int BOTTOMS_COUNT = 7;
    public List<GameObject>[] bottoms;
    public List<string>[] tops;

    public List<GameObject> bottom0 = new();
    public List<GameObject> bottom1 = new();
    public List<GameObject> bottom2 = new();
    public List<GameObject> bottom3 = new();
    public List<GameObject> bottom4 = new();
    public List<GameObject> bottom5 = new();
    public List<GameObject> bottom6 = new();
    public List<string> deck;

    public int deckCount;
    public List<GameObject> visibleCardOnDeck = new();
    public List<GameObject> hideCardOnDeck = new();

    public IDictionary<string, int> suitIndex;
    private bool locked = false; 
    
    void Start()
    {
        timeControl = FindObjectOfType<TimerControl>();
        suitIndex = new Dictionary<string, int>();
        suitIndex.Add("S", 0);
        suitIndex.Add("H", 1);
        suitIndex.Add("C", 2);
        suitIndex.Add("D", 3);
        bottoms = new[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        tops = new[] { new List<string>(), new List<string>(), new List<string>(), new List<string>() };
        PlayCards();
    }

    void Update()
    {
        bool isDone = false;
        if (!locked)
        {
            foreach (var list in tops)
            {
                if (list.Count != 13)
                {
                    isDone = false;
                    break;
                }

                isDone = true;
            }
        }

        if (isDone && !locked)
        {
            timeControl.timerIsRunning = false;
            panel.SetActive(true);
            var textMesh = scoreTime.GetComponent<TextMeshProUGUI>();
            textMesh.text = timeControl.timeText.text;
            locked = true;
        }
    }

    public void ResetGame()
    {
        timeControl.timeRemaining = 0;
        timeControl.timerIsRunning = true;
        locked = false;
        foreach (var bottom in bottoms)
            bottom.Clear();
        foreach (var top in tops)
            top.Clear();

        visibleCardOnDeck.Clear();
        hideCardOnDeck.Clear();
    }

    public void PlayCards()
    {
        deck = GenerateDeck();
        Shuffle(deck);
        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SolitaireDealDeck();
        deckCount = deck.Count;
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new();
        foreach (var s in suits)
        {
            foreach (var v in values)
            {
                newDeck.Add(s + v);
            }
        }

        return newDeck;
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;

        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    IEnumerator SolitaireDeal()
    {
        for (int i = 0; i < BOTTOMS_COUNT; i++)
        {
            float yOffset = 0;
            float zOffset = 0.01f;
            foreach (var card in bottoms[i])
            {
                yield return new WaitForSeconds(0.03f);
                card.transform.position = card.GetComponent<Selectable>().SetLastPosition(new Vector3(
                    bottomPos[i].transform.position.x,
                    bottomPos[i].transform.position.y - yOffset,
                    bottomPos[i].transform.position.z - zOffset));

                if (card == bottoms[i].Last())
                {
                    card.GetComponent<Selectable>().faceUp = true;
                }

                card.GetComponent<Selectable>().row = i;
                card.GetComponent<Selectable>().deck = false;
                card.GetComponent<Selectable>().top = false;

                yOffset += 0.4f;
                zOffset += 0.01f;
            }
        }
    }

    void SolitaireSort()
    {
        for (int i = 0; i < BOTTOMS_COUNT; i++)
        {
            for (int j = i; j < BOTTOMS_COUNT; j++)
            {
                bottoms[j].Add(Instantiate(cardPrefab, cardPrefab.transform.position, bottomPos[i].transform.rotation));
                bottoms[j].Last().name = deck.Last();
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    void SolitaireDealDeck()
    {
        visibleCardOnDeck.Clear();
        hideCardOnDeck.Clear();
        float zOffset = 0.01f;
        foreach (string card in deck)
        {
            var position = deckButton.transform.position;
            GameObject newCardOnDeck = Instantiate(cardPrefab,
                new Vector3(position.x, position.y,
                    position.z + zOffset),
                deckButton.transform.rotation);
            newCardOnDeck.name = card;
            hideCardOnDeck.Add(newCardOnDeck);
            newCardOnDeck.GetComponent<Selectable>().deck = true;
            newCardOnDeck.GetComponent<Selectable>().top = false;
            newCardOnDeck.GetComponent<Selectable>().row = -1;
            zOffset += 0.01f;
        }
    }

    public void ShowCardFaceOnDeck()
    {
        float xOffset = 2f;
        if (deckCount != 0)
        {
            var card = hideCardOnDeck[0];
            visibleCardOnDeck.Add(card);

            card.GetComponent<Selectable>().faceUp = true;
            var position = deckButton.transform.position;
            var lastPosition = new Vector3(position.x + xOffset,
                position.y, (deckCount * 0.01f));

            card.transform.position = card
                .GetComponent<Selectable>()
                .SetLastPosition(lastPosition);

            hideCardOnDeck.Remove(card);
            deckCount--;
        }
        else
        {
            float zOffset = 0.01f;
            var position = deckButton.transform.position;
            visibleCardOnDeck.ForEach(card =>
            {
                card.transform.position = new Vector3(position.x, position.y, position.z + zOffset);
                card.GetComponent<Selectable>().faceUp = false;
                zOffset += 0.01f;
                hideCardOnDeck.Add(card);
            });
            visibleCardOnDeck.Clear();
            deckCount = hideCardOnDeck.Count;
        }
    }
}