using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class FileOfGoblinBat
    {
        [Key, Column(Order = 1), StringLength(8)]
        public string Version
        {
            get; set;
        }
        [Required, Column(Order = 2)]
        public byte[] Content
        {
            get; set;
        }
    }
}