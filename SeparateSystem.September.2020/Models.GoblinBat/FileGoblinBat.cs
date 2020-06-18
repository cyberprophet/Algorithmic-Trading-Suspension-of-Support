using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class FileGoblinBat
    {
        [Key, Column(Order = 1)]
        public string Name
        {
            get; set;
        }
        [Key, StringLength(7), Column(Order = 2)]
        public string Version
        {
            get; set;
        }
        [Required]
        public byte[] Buffer
        {
            get; set;
        }
    }
}