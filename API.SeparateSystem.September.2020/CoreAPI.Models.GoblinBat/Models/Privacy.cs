using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Privacy 
    {
        [Key, Column(Order = 1)]
        public string Security
        {
            get; set;
        }
        [Column(Order = 2), MaxLength(1)]
        public string SecuritiesAPI
        {
            get; set;
        }
        [Column(Order = 3)]
        public string SecurityAPI
        {
            get; set;
        }
        [Column(Order = 4), MaxLength(1)]
        public string Account
        {
            get; set;
        }
        [Column(Order = 5)]
        public double Commission
        {
            get; set;
        }
        [Column(Order = 6)]
        public string CodeStrategics
        {
            get; set;
        }
    }
}