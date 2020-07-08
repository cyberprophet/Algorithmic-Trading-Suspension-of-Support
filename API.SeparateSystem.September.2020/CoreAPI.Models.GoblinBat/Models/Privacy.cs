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
    }
}