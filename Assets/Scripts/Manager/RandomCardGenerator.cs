using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardList;
using Helpers;
using UnityEngine;
using Random = System.Random;

namespace Manager
{
    public class RandomCardGenerator
    {
        private Random _random;
        private Dictionary<CardTypeBehaviour, List<int>> foundCard;
        private float START_X = -5f;
        private float START_Y = 0.7f;
        private int CARD_TYPE_COUNT = 4;
        private int CARD_COUNT_EVERY_TYPE = 13;
        public RandomCardGenerator()
        {
            foundCard = new();
            foundCard.Add(CardTypeBehaviour.KARO, new());
            foundCard.Add(CardTypeBehaviour.SINEK, new());
            foundCard.Add(CardTypeBehaviour.MACO, new());
            foundCard.Add(CardTypeBehaviour.KUPA, new());
            _random = new();
        }

        private void GeneratorStack()
        {
            for (int i = 0,stack = 0; i < CARD_TYPE_COUNT; i++)
            {
                CardTypeBehaviour cardType = (CardTypeBehaviour)i;
                for (int cardNumber = 0; cardNumber < CARD_COUNT_EVERY_TYPE; cardNumber++)
                {
                    if (foundCard[cardType].Any(x => x == cardNumber))
                        continue;
                    stack++;
                    foundCard[cardType].Add(cardNumber); // eleman karalisteye alinir ve tekrar secilmez
                    Card card = new(new GameObject($"{cardType.ToString()}{cardNumber}"));
                    card.CardColor = (cardType is CardTypeBehaviour.MACO | cardType is CardTypeBehaviour.SINEK)
                        ? CardColor.BLACK
                        : CardColor.RED;

                    card.SetSprite(PlayCards.GetCardsSprite($"{cardType.ToString().ToLowerInvariant()}_{cardNumber}"));
                    card.CardType = cardType;
                    card.CardTurn = cardNumber;
                    card.isVisible = false;
                    card.SetPosition(new Vector3(-5f, 3.4f,(stack*-0.1f)));
                    Members.stackCard.Add(card);
                }
            }
        }

        public IEnumerator GeneratorTab()
        {
            for (int i = 1; i <= Members.tabListCard.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    yield return new WaitForSeconds(0.01f);
                    int cardNumber = _random.Next(0, 13);
                    CardTypeBehaviour cardType = (CardTypeBehaviour)_random.Next(0, 4);
                    // Daha once oyle bir eleman secilmisse atlanicak
                    if (foundCard[cardType].Any(x => x == cardNumber))
                    {
                        j--;
                        continue;
                    }

                    foundCard[cardType].Add(cardNumber); // eleman karalisteye alinir ve tekrar secilmez
                    Card card = new(new GameObject($"{cardType.ToString()}{cardNumber}"));
                    card.CardColor = (cardType is CardTypeBehaviour.MACO | cardType is CardTypeBehaviour.SINEK)
                        ? CardColor.BLACK
                        : CardColor.RED;

                    card.SetSprite(PlayCards.GetCardsSprite($"{cardType.ToString().ToLowerInvariant()}_{cardNumber}"));
                    card.CardType = cardType;
                    card.CardTurn = cardNumber;
                    card.isVisible = i == j + 1;
                    card.SetPosition(new Vector3(START_X + (i - 1) * 2, START_Y - (j * 0.5f),(j*-0.1f)));
                    Members.tabListCard[i - 1].Push(card);
                    
                }
            }
            GeneratorStack();
        }
    }
}