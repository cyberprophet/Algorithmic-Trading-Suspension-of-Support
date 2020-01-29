using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareInvest.CallUpDataBase;
using ShareInvest.Strategy;

namespace ShareInvest.Analysis
{
    public class Analysize : CallUpGoblinBat
    {
        public Analysize(Specify specify)
        {
            Specify = specify;
            Short = new List<double>(512);
            Long = new List<double>(512);

            foreach (var chart in Retrieve.GetInstance(specify.Code).Chart)
            {
                
            }
        }
        private Specify Specify
        {
            get;
        }
        private EMA EMA
        {
            get;
        }
        private List<double> Short
        {
            get;
        }
        private List<double> Long
        {
            get;
        }
    }
}