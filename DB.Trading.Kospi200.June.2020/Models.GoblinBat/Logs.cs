using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Logs
    {
        [Key, Column(Order = 1), StringLength(20)]
        public string Identity
        {
            get; set;
        }
        [Key, Column(Order = 2), StringLength(6)]
        public string Date
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string Assets
        {
            get; set;
        }
        [Required, StringLength(4)]
        public string Strategy
        {
            get; set;
        }
        [Required, StringLength(2)]
        public string Commission
        {
            get; set;
        }
        [Required, StringLength(1)]
        public string RollOver
        {
            get; set;
        }
        [Required, StringLength(6)]
        public string Code
        {
            get; set;
        }
    }
}