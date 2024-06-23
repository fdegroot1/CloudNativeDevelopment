using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonWebApi.Models
{
    [Table("team", Schema = "dbo")]
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("team_id")]
        public int TeamId { get; set; }

        [Column("team_name")]
        public string TeamName { get; set; }

        
        public List<Pokemon> Pokemons { get; set; }
    }
}
