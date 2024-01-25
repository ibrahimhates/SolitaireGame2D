using Helpers;
using UnityEngine;

namespace CardList
{
    public class Card
    {
        public GameObject gameObject { get; private set; }
        private Sprite _sprite;
        public CardColor CardColor { get; set; }
        public CardTypeBehaviour CardType { get; set; }
        public int CardTurn { get; set; }
        private bool _isVisible;
        public bool isVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                IsChangeVisible();
            }
        }   

        public Card(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        private void IsChangeVisible()
        {
            if (!isVisible)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = PlayCards.GetCardsBackSprite;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = _sprite;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            _sprite = sprite;
            gameObject.AddComponent<SpriteRenderer>().sprite = sprite;
        }

        public void SetPosition(Vector3 vector3) => gameObject.transform.position = vector3;

        public Vector3 GetPosition() => gameObject.transform.position;
    }
}