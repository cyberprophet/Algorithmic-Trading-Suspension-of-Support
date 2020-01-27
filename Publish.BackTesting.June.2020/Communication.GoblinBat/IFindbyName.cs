namespace ShareInvest.Communication
{
    public interface IFindbyName
    {
        enum CheckBoxUsed
        {
            checkBoxShortDay = 1,
            checkBoxLongDay = 3,
            checkBoxReaction = 4,
            checkBoxHedge = 5,
            checkBoxBase = 6,
            checkBoxSigma = 7,
            checkBoxPercent = 8,
            checkBoxMax = 9,
            checkBoxQuantity = 10,
            checkBoxTime = 11
        }
        enum LabelUsed
        {
            labelST = 0,
            labelSD = 1,
            labelLT = 2,
            labelLD = 3,
            labelR = 4,
            labelH = 5,
            labelBase = 6,
            labelSigma = 7,
            labelPercent = 8,
            labelMax = 9,
            labelQuantity = 10,
            labelTime = 11
        }
        enum Numeric
        {
            numericPST = 0,
            numericIST = 1,
            numericDST = 2,
            numericPSD = 3,
            numericISD = 4,
            numericDSD = 5,
            numericPLT = 6,
            numericILT = 7,
            numericDLT = 8,
            numericPLD = 9,
            numericILD = 10,
            numericDLD = 11,
            numericPR = 12,
            numericIR = 13,
            numericDR = 14,
            numericPH = 15,
            numericIH = 16,
            numericDH = 17,
            numericPB = 18,
            numericIB = 19,
            numericDB = 20,
            numericPS = 21,
            numericIS = 22,
            numericDS = 23,
            numericPP = 24,
            numericIP = 25,
            numericDP = 26,
            numericPM = 27,
            numericIM = 28,
            numericDM = 29,
            numericPQ = 30,
            numericIQ = 31,
            numericDQ = 32,
            numericPT = 33,
            numericIT = 34,
            numericDT = 35
        }
    }
}