using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class SatisfyConditions
    {
        [Key, ForeignKey("Privacy"), Column(Order = 1)]
        public string Security
        {
            get; set;
        }
        [Required, Column(Order = 2)]
        public string SettingValue
        {
            get; set;
        }
        [Column(Order = 4)]
        public string Ban
        {
            get; set;
        }
        [Required, Column(Order = 3)]
        public string Strategics
        {
            get; set;
        }
        [Column(Order = 5)]
        public string TempStorage
        {
            get; set;
        }
    }
}