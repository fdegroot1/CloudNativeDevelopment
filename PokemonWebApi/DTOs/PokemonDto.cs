
public class PokemonDto
{
    public int PokemonId { get; set; }
    public string PokemonName { get; set; }
    public string PokemonType { get; set; }
    public List<MoveDto> Moves { get; set; }
}
