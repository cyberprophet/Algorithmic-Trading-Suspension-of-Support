using System.Collections.Generic;

namespace ShareInvest.Catalog
{
    public struct QuickSort
    {
        public List<long> SortedList
        {
            get;
        }
        public QuickSort(List<long> list)
        {
            SortedList = list;
            SetQuickSort(SortedList, 0, SortedList.Count - 1);
        }
        void SetQuickSort(List<long> list, int start, int end)
        {
            var pivot = list[start];
            int left = start + 1, right = end;

            while (left <= right)
            {
                while (list[left] < pivot)
                    left++;

                while (list[right] > pivot)
                    right--;

                if (left <= right)
                    SetSwap(list, left, right);
            }
            if (start < end)
            {
                SetSwap(list, start, right);
                SetQuickSort(list, start, right - 1);
                SetQuickSort(list, right + 1, end);
            }
        }
        void SetSwap(List<long> list, int first, int last)
        {
            var temp = list[first];
            list[first] = list[last];
            list[last] = temp;
        }
    }
}