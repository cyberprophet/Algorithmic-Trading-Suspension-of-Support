using System.ComponentModel.DataAnnotations;

namespace ShareInvest.Models
{
    public class Codes
    {
        [Key]
        public string Code
        {
            get; set;
        }
        [Required]
        public string Name
        {
            get; set;
        }
        [Required]
        public string Info
        {
            get; set;
        }
    }
}