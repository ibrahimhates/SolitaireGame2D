using UnityEngine;

namespace Helpers
{
    public static class PlayCards
    {
        private static string cardFilePath = "PlayingCard/allCard";
        private static string cardBackFilePath = "PlayingCard/backOfCard";
        
        public static Sprite GetCardsSprite(string cardName) => Resources.Load<Sprite>($"{cardFilePath}/{cardName}");
        
        public static Sprite GetCardsBackSprite => Resources.Load<Sprite>(cardBackFilePath);
    }
}