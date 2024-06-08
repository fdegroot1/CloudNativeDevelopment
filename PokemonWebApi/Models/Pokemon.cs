using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonWebApi.Models
{
    [Table("pokemon", Schema = "dbo")]
    public class Pokemon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("pokemon_id")]

        public int PokemonId { get; set; }
        [Column("pokemon_name")]
        public string PokemonName { get; set; }


    }
}
