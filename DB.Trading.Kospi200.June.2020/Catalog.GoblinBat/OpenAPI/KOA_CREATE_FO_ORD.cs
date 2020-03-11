using System.Collections;

namespace ShareInvest.Catalog
{
    public struct KOA_CREATE_FO_ORD : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var str in Output)
                yield return str;
        }
        private string[] Output
        {
            get
            {
                return new string[]
                {
                    "주문번호"
                };
            }
        }
    }
}