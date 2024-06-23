namespace PokemonWebApi.Models
{
    public class PokemonData
    {
        public List<MoveWrapper> moves { get; set; }

        public class MoveWrapper
        {
            public Move move { get; set; }

            public class Move
            {
                public string name { get; set; }
            }
        }
    }
}
