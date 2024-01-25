using System.Collections.Generic;
using CardList;

namespace Manager
{
    public class LocationManager
    {
        public void AddAce(Card card,CardTypeBehaviour type,Stack<Card> ace)
        {
            if (card.CardType == type)
            {
                var current = ace.Peek();
                if (current.CardTurn + 1 == card.CardTurn)
                {
                    card.SetPosition(current.GetPosition());
                    Members.aceKupa.Push(card);
                }
            }
        }
    }
}