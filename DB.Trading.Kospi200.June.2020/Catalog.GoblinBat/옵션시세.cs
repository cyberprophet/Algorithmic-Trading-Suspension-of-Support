﻿using System.Collections;

namespace ShareInvest.Catalog
{
    public class 옵션시세 : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            int i, l = output.Length;

            for (i = 0; i < l; i++)
                yield return output[i];
        }
        private readonly int[] output =
        {
            20,
            10,
            11,
            12,
            27,
            28,
            15,
            13,
            14,
            16,
            17,
            18,
            195,
            182,
            186,
            190,
            191,
            193,
            192,
            194,
            181,
            25,
            26,
            137,
            187,
            197,
            246,
            247,
            248,
            219,
            196,
            188,
            189,
            30 ,
            391,
            392,
            393,
            1365,
            1366,
            1367,
            305,
            306
        };
    }
}