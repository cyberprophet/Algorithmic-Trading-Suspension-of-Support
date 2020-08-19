using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Consensus
    {
        [ForeignKey("Codes"), Column(Order = 1), MinLength(6), MaxLength(8)]
        public string Code
        {
            get; set;
        }
        [Column(Order = 2), StringLength(8)]
        public string Date
        {
            get; set;
        }
        [Column(Order = 3), StringLength(1)]
        public string Quarter
        {
            get; set;
        }
        [Column(Order = 4)]
        public long Sales
        {
            get; set;
        }
        [Column(Order = 5)]
        public double YoY
        {
            get; set;
        }
        [Column(Order = 6)]
        public long Op
        {
            get; set;
        }
        [Column(Order = 7)]
        public long Np
        {
            get; set;
        }
        [Column(Order = 8)]
        public int Eps
        {
            get; set;
        }
        [Column(Order = 9)]
        public int Bps
        {
            get; set;
        }
        [Column(Order = 10)]
        public double Per
        {
            get; set;
        }
        [Column(Order = 11)]
        public double Pbr
        {
            get; set;
        }
        [Column(Order = 12)]
        public string Roe
        {
            get; set;
        }
        [Column(Order = 13)]
        public string Ev
        {
            get; set;
        }
    }
}