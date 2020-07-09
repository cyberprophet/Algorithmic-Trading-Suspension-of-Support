using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
    public class Privacy : IContext
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
        [Column(Order =3)]
        public string SecurityAPI
        {
            get; set;
        }
    }
}