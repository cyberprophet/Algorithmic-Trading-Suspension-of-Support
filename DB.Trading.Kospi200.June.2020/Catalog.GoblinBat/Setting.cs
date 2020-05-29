using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace ShareInvest.Catalog
{
    public struct Setting
    {
        public long Assets
        {
            get; set;
        }
        public string Strategy
        {
            get; set;
        }
        public double Commission
        {
            get; set;
        }
        public CheckState RollOver
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
        [Range(2, 20)]
        public int BaseShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int BaseLong
        {
            get; set;
        }
        [Range(0, 180)]
        public int NonaTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int NonaShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int NonaLong
        {
            get; set;
        }
        [Range(0, 150)]
        public int OctaTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int OctaShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int OctaLong
        {
            get; set;
        }
        [Range(0, 120)]
        public int HeptaTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int HeptaShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int HeptaLong
        {
            get; set;
        }
        [Range(0, 90)]
        public int HexaTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int HexaShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int HexaLong
        {
            get; set;
        }
        [Range(0, 60)]
        public int PentaTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int PentaShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int PentaLong
        {
            get; set;
        }
        [Range(0, 45)]
        public int QuadTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int QuadShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int QuadLong
        {
            get; set;
        }
        [Range(0, 30)]
        public int TriTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int TriShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int TriLong
        {
            get; set;
        }
        [Range(0, 15)]
        public int DuoTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int DuoShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int DuoLong
        {
            get; set;
        }
        [Range(1, 90)]
        public int MonoTime
        {
            get; set;
        }
        [Range(2, 20)]
        public int MonoShort
        {
            get; set;
        }
        [Range(5, 120)]
        public int MonoLong
        {
            get; set;
        }
    }
}