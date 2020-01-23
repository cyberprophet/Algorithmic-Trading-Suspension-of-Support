using System.Collections;

namespace ShareInvest.Catalog
{
    public class 옵션호가잔량 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            21,
            27,
            28,
            41,
            61,
            81,
            101,
            51,
            71,
            91,
            111,
            42,
            62,
            82,
            102,
            52,
            72,
            92,
            112,
            43,
            63,
            83,
            103,
            53,
            73,
            93,
            113,
            44,
            64,
            84,
            104,
            54,
            74,
            94,
            114,
            45,
            65,
            85,
            105,
            55,
            75,
            95,
            115,
            121,
            122,
            123,
            125,
            126,
            127,
            137,
            128,
            13,
            23,
            238,
            200,
            201,
            291,
            293,
            294,
            295
        };
    }
}