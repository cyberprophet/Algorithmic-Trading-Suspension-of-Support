using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Privacy
    {
        [Key, StringLength(15)]
        public string IP
        {
            get; set;
        }
        [Required, StringLength(1)]
        public char SecuritiesAPI
        {
            get; set;
        }
        [MaxLength(8), Column(Order = 0)]
        public string Identity
        {
            get; set;
        }
        [Column(Order = 1)]
        public string Password
        {
            get; set;
        }
        [MinLength(10), MaxLength(56), Column(Order = 2)]
        public string Certificate
        {
            get; set;
        }
        [StringLength(11), Column(Order = 3)]
        public string Account
        {
            get; set;
        }
        [Column(Order = 4)]
        public string AccountPassword
        {
            get; set;
        }
        [Column(Order = 5)]
        public bool Server
        {
            get; set;
        }
        [Column(Order = 6)]
        public string Date
        {
            get; set;
        }
    }
}