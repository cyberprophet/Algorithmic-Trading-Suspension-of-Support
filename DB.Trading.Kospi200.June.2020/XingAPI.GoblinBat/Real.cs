using System;
using XA_DATASETLib;

namespace ShareInvest.XingAPI
{
    class Real : XARealClass
    {
        protected internal InBlock GetInBlock(string code) => new InBlock
        {
            Block = inBlock,
            Field = field,
            Data = code
        };
        protected internal virtual void OnReceiveRealData(string szTrCode) => Console.WriteLine(szTrCode);
        protected internal string GetInBlock() => inBlock;
        protected internal string OutBlock => outBlock;
        protected internal Real() => ReceiveRealData += OnReceiveRealData;
        protected internal ConnectAPI API => ConnectAPI.GetInstance();
        protected internal const string sell = "1";
        protected internal const string buy = "2";
        protected internal const string cancel = "3";
        protected internal const string avg = "000.00";
        const string field = "futcode";
        const string inBlock = "InBlock";
        const string outBlock = "OutBlock";
    }
    enum H
    {
        hotime = 12,
        offerho = 1,
        bidho = 2,
        offerrem = 3,
        bidrem = 4,
        offercnt = 5,
        bidcnt = 6,
        totofferrem = 7,
        totbidrem = 8,
        totoffercnt = 9,
        totbidcnt = 10,
        futcode = 11,
        dangochk = 'H',
        alloc_gubun = 'G'
    }
    enum C
    {
        chetime = 1,
        sign = 2,
        change = 3,
        drate = 4,
        price = 5,
        open = 6,
        high = 7,
        low = 8,
        cgubun = 9,
        cvolume = 10,
        volume = 11,
        value = 12,
        mdvolume = 13,
        mdchecnt = 14,
        msvolume = 15,
        mschecnt = 16,
        cpower = 17,
        offerho1 = 18,
        bidho1 = 19,
        openyak = 20,
        k200jisu = 21,
        theoryprice = 22,
        kasis = 23,
        sbasis = 24,
        ibasis = 25,
        openyakcha = 26,
        jgubun = 27,
        jnilvolume = 28,
        futcode = 29,
        chetime1 = 30
    }
    enum TR
    {
        SONBT001 = 0,
        SONBT002 = 1,
        SONBT003 = 2,
        CONET801 = 3,
        CONET002 = 4,
        CONET003 = 5
    }
    enum CMO
    {
        lineseq = 0,
        accno = 1,
        user = 2,
        len = 3,
        gubun = 4,
        compress = 5,
        encrypt = 6,
        offset = 7,
        trcode = 8,
        comid = 9,
        userid = 10,
        media = 11,
        ifid = 12,
        seq = 13,
        trid = 14,
        pubip = 15,
        prvip = 16,
        pcbpno = 17,
        bpno = 18,
        termno = 19,
        lang = 20,
        proctm = 21,
        msgcode = 22,
        outgu = 23,
        compreq = 24,
        funckey = 25,
        reqcnt = 26,
        filler = 27,
        cont = 28,
        contkey = 29,
        varlen = 30,
        varhdlen = 31,
        varmsglen = 32,
        trsrc = 33,
        eventid = 34,
        ifinfo = 35,
        filler1 = 36,
        trcode1 = 37,
        firmno = 38,
        acntno = 39,
        acntno1 = 40,
        acntnm = 41,
        brnno = 42,
        ordmktcode = 43,
        ordno1 = 44,
        ordno = 45,
        orgordno1 = 46,
        orgordno = 47,
        prntordno = 48,
        prntordno1 = 49,
        isuno = 50,
        fnoIsuno = 51,
        fnoIsunm = 52,
        pdgrpcode = 53,
        fnoIsuptntp = 54,
        bnstp = 55,
        mrctp = 56,
        ordqty = 57,
        hogatype = 58,
        mmgb = 59,
        ordprc = 60,
        unercqty = 61,
        commdacode = 62,
        peeamtcode = 63,
        mgempno = 64,
        fnotrdunitamt = 65,
        trxtime = 66,
        strtgcode = 67,
        grpId = 68,
        ordseqno = 69,
        ptflno = 70,
        bskno = 71,
        trchno = 72,
        Itemno = 73,
        userId = 74,
        opdrtnno = 75,
        rjtcode = 76,
        mrccnfqty = 77,
        orgordunercqty = 78,
        orgordmrcqty = 79,
        ctrcttime = 80,
        ctrctno = 81,
        execprc = 82,
        execqty = 83,
        newqty = 84,
        qdtqty = 85,
        lastqty = 86,
        lallexecqty = 87,
        allexecamt = 88,
        fnobalevaltp = 89,
        bnsplamt = 90,
        fnoIsuno1 = 91,
        bnstp1 = 92,
        execprc1 = 93,
        newqty1 = 94,
        qdtqty1 = 95,
        allexecamt1 = 96,
        fnoIsuno2 = 97,
        bnstp2 = 98,
        execprc2 = 99,
        newqty2 = 100,
        lqdtqty2 = 101,
        allexecamt2 = 102,
        dps = 103,
        ftsubtdsgnamt = 104,
        mgn = 105,
        mnymgn = 106,
        ordableamt = 107,
        mnyordableamt = 108,
        fnoIsuno_1 = 109,
        bnstp_1 = 110,
        unsttqty_1 = 111,
        lqdtableqty_1 = 112,
        avrprc_1 = 113,
        fnoIsuno_2 = 114,
        bnstp_2 = 115,
        unsttqty_2 = 116,
        lqdtableqty_2 = 117,
        avrprc_2 = 118
    }
}