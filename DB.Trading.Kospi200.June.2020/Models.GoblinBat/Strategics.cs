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
        [Required, StringLength(6)]
        public string Code
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string Strategy
        {
            get; set;
        }
        [Required, StringLength(4)]
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
        [Required, StringLength(4)]
        public string BaseShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string BaseLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string NonaTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string NonaShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string NonaLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string OctaTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string OctaShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string OctaLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string HeptaTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string HeptaShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string HeptaLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string HexaTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string HexaShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string HexaLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string PentaTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string PantaShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string PantaLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string QuadTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string QuadShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string QuadLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string TriTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string TriShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string TriLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string DuoTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string DuoShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string DuoLong
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string MonoTime
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string MonoShort
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string MonoLong
        {
            get; set;
        }
    }
}