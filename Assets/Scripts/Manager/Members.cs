using System.Collections.Generic;
using CardList;

namespace Manager
{
    public static class Members
    {
        public static List<Stack<Card>> tabListCard = new()
        {
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new()
        };

        public static List<Card> stackCard { get; } = new();

        public static Stack<Card> aceKupa { get; } = new();
        public static Stack<Card> aceKaro { get; } = new();
        public static Stack<Card> aceMaca { get; } = new();
        public static Stack<Card> aceSinek { get; } = new();
    }
}