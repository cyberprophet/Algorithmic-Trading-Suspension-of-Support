using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Strategics
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Index
        {
            get; set;
        }
        [Required, MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Required]
        public string Strategy
        {
            get; set;
        }
        [Required]
        public string Assets
        {
            get; set;
        }
        [Required, StringLength(2)]
        public string Commission
        {
            get; set;
        }
        [Required]
        public string MarginRate
        {
            get; set;
        }
        [Required, StringLength(1)]
        public string RollOver
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string BaseTime
        {
            get; set;
        }
        [Required]
        public string BaseShort
        {
            get; set;
        }
        [Required]
        public string BaseLong
        {
            get; set;
        }
        [Required]
        public string NonaTime
        {
            get; set;
        }
        [Required]
        public string NonaShort
        {
            get; set;
        }
        [Required]
        public string NonaLong
        {
            get; set;
        }
        [Required]
        public string OctaTime
        {
            get; set;
        }
        [Required]
        public string OctaShort
        {
            get; set;
        }
        [Required]
        public string OctaLong
        {
            get; set;
        }
        [Required]
        public string HeptaTime
        {
            get; set;
        }
        [Required]
        public string HeptaShort
        {
            get; set;
        }
        [Required]
        public string HeptaLong
        {
            get; set;
        }
        [Required]
        public string HexaTime
        {
            get; set;
        }
        [Required]
        public string HexaShort
        {
            get; set;
        }
        [Required]
        public string HexaLong
        {
            get; set;
        }
        [Required]
        public string PentaTime
        {
            get; set;
        }
        [Required]
        public string PantaShort
        {
            get; set;
        }
        [Required]
        public string PantaLong
        {
            get; set;
        }
        [Required]
        public string QuadTime
        {
            get; set;
        }
        [Required]
        public string QuadShort
        {
            get; set;
        }
        [Required]
        public string QuadLong
        {
            get; set;
        }
        [Required]
        public string TriTime
        {
            get; set;
        }
        [Required]
        public string TriShort
        {
            get; set;
        }
        [Required]
        public string TriLong
        {
            get; set;
        }
        [Required]
        public string DuoTime
        {
            get; set;
        }
        [Required]
        public string DuoShort
        {
            get; set;
        }
        [Required]
        public string DuoLong
        {
            get; set;
        }
        [Required]
        public string MonoTime
        {
            get; set;
        }
        [Required]
        public string MonoShort
        {
            get; set;
        }
        [Required]
        public string MonoLong
        {
            get; set;
        }
    }
}