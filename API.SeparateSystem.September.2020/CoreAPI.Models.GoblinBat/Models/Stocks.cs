using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Stocks : ICharts<int>
    {
        [Required, Column(Order = 1), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [NotMapped]
        public string Retention
        {
            get; set;
        }
        [Key, Column(Order = 2)]
        public long Date
        {
            get; set;
        }
        [Required, Column(Order = 3)]
        public int Price
        {
            get; set;
        }
        [Required, Column(Order = 4)]
        public int Volume
        {
            get; set;
        }
        [ForeignKey(code)]
        public virtual Codes Codes
        {
            get; set;
        }
        const string code = "Code";
    }
}