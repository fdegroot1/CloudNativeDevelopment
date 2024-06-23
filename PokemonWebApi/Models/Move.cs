using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonWebApi.Models
{
    [Table("move", Schema = "dbo")]
  
    public class Move
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("move_id")]
        public int MoveId { get; set; }

        [Column("move_name")]
        public string MoveName { get; set; }

        [Column("move_type")]
        public string MoveType { get; set; }

        [Column("move_power")]
        public int MovePower { get; set; }

        [ForeignKey("Pokemon")]
        [Column("pokemon_id")]
        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; }
    }

}
