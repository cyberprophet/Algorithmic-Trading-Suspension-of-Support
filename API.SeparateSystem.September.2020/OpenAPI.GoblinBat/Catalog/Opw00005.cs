using System;

using AxKHOpenAPILib;

using ShareInvest.Analysis;
using ShareInvest.Catalog;
using ShareInvest.Catalog.OpenAPI;
using ShareInvest.EventHandler;
using ShareInvest.Interface;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
    class Opw00005 : TR, ISendSecuritiesAPI<SendSecuritiesAPI>
    {
        internal override void OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            var temp = base.OnReceiveTrData(opSingle, opMultiple, e);

            if (temp.Item1 != null)
                Send?.Invoke(this, new SendSecuritiesAPI(temp.Item1[15], temp.Item1[2], temp.Item1[7]));

            if (long.TryParse(temp.Item1[7], out long available))
            {
                Connect.Cash = available;
                var now = DateTime.Now;

                while (temp.Item2?.Count > 0)
                {
                    var param = new SendSecuritiesAPI(temp.Item2.Dequeue());

                    if (param.Convey is Tuple<string, string, int, dynamic, dynamic, long, double> balance && Connect.HoldingStock.TryGetValue(balance.Item1, out Holding hs) && API.GetMasterStockState(balance.Item1).Contains(transactionSuspension) == false)
                    {
                        if (hs.Quantity == 0 && Connect.Cash > 0 && now.Hour == 8 && now.Minute > 0x35)
                        {
                            int sell, buy, upper, lower, bPrice, sPrice;
                            uint quantity = (uint)balance.Item3, price = uint.TryParse(API.GetMasterLastPrice(hs.Code), out uint before) ? before : 0;
                            var stock = API.KOA_Functions(info, hs.Code).Split(';')[0].Contains(market);
                            var connect = Connect.GetInstance(API);
                            var account = Value.Split(';')[0];

                            switch (hs.FindStrategics)
                            {
                                case TrendsInValuation tv:
                                    upper = (int)(price * 1.3);
                                    lower = (int)(price * 0.7);

                                    if (tv.ReservationSubtractionalQuantity > 0)
                                    {
                                        sell = (int)(balance.Item4 * (1 + tv.Subtraction));
                                        sPrice = hs.GetStartingPrice(sell, stock);
                                        sPrice = sPrice < lower ? lower + hs.GetQuoteUnit(sPrice, stock) : sPrice;

                                        while (sPrice < upper && quantity-- > 0)
                                        {
                                            SendMessage(sPrice.ToString("C0"), string.Empty);
                                            connect.SendOrder(new SendOrder
                                            {
                                                RQName = balance.Item2,
                                                ScreenNo = connect.LookupScreenNo,
                                                AccNo = account,
                                                OrderType = (int)OpenOrderType.신규매도,
                                                Code = hs.Code,
                                                Qty = tv.ReservationSubtractionalQuantity,
                                                Price = sPrice,
                                                HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                                OrgOrderNo = string.Empty
                                            });
                                            for (int i = 0; i < tv.SubtractionalUnit; i++)
                                                sPrice += hs.GetQuoteUnit(sPrice, stock);
                                        }
                                    }
                                    if (tv.ReservationAddtionalQuantity > 0)
                                    {
                                        buy = (int)(balance.Item4 * (1 - tv.Addition));
                                        bPrice = hs.GetStartingPrice(lower, stock);

                                        while (bPrice < upper && bPrice < buy && Connect.Cash > bPrice * (1.5e-4 + 1))
                                        {
                                            connect.SendOrder(new SendOrder
                                            {
                                                RQName = balance.Item2,
                                                ScreenNo = connect.LookupScreenNo,
                                                AccNo = account,
                                                OrderType = (int)OpenOrderType.신규매수,
                                                Code = hs.Code,
                                                Qty = tv.ReservationAddtionalQuantity,
                                                Price = bPrice,
                                                HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                                OrgOrderNo = string.Empty
                                            });
                                            for (int i = 0; i < tv.AdditionalUnit; i++)
                                                bPrice += hs.GetQuoteUnit(bPrice, stock);

                                            Connect.Cash -= (long)(bPrice * (1.5e-4 + 1));
                                            SendMessage(bPrice.ToString("C0"), Connect.Cash.ToString("C0"));
                                        }
                                    }
                                    break;

                                case TrendToCashflow tc when tc.ReservationQuantity > 0:
                                    sell = (int)(balance.Item4 * (1 + tc.ReservationRevenue));
                                    buy = (int)(balance.Item4 * (1 - tc.Addition));
                                    upper = (int)(price * 1.3);
                                    lower = (int)(price * 0.7);
                                    bPrice = hs.GetStartingPrice(lower, stock);
                                    sPrice = hs.GetStartingPrice(sell, stock);
                                    sPrice = sPrice < lower ? lower + hs.GetQuoteUnit(sPrice, stock) : sPrice;

                                    while (sPrice < upper && quantity-- > 0)
                                    {
                                        SendMessage(sPrice.ToString("C0"), string.Empty);
                                        connect.SendOrder(new SendOrder
                                        {
                                            RQName = balance.Item2,
                                            ScreenNo = connect.LookupScreenNo,
                                            AccNo = account,
                                            OrderType = (int)OpenOrderType.신규매도,
                                            Code = hs.Code,
                                            Qty = tc.ReservationQuantity,
                                            Price = sPrice,
                                            HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                            OrgOrderNo = string.Empty
                                        });
                                        for (int i = 0; i < tc.Unit; i++)
                                            sPrice += hs.GetQuoteUnit(sPrice, stock);
                                    }
                                    while (bPrice < upper && bPrice < buy && Connect.Cash > bPrice * (1.5e-4 + 1))
                                    {
                                        connect.SendOrder(new SendOrder
                                        {
                                            RQName = balance.Item2,
                                            ScreenNo = connect.LookupScreenNo,
                                            AccNo = account,
                                            OrderType = (int)OpenOrderType.신규매수,
                                            Code = hs.Code,
                                            Qty = tc.ReservationQuantity,
                                            Price = bPrice,
                                            HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                            OrgOrderNo = string.Empty
                                        });
                                        for (int i = 0; i < tc.Unit; i++)
                                            bPrice += hs.GetQuoteUnit(bPrice, stock);

                                        Connect.Cash -= (long)(bPrice * (1.5e-4 + 1));
                                        SendMessage(bPrice.ToString("C0"), Connect.Cash.ToString("C0"));
                                    }
                                    break;

                                case TrendsInStockPrices ts when ts.Setting.Equals(Setting.Reservation):
                                    sell = (int)(balance.Item4 * (1 + ts.RealizeProfit));
                                    buy = (int)(balance.Item4 * (1 - ts.AdditionalPurchase));
                                    upper = (int)(price * 1.3);
                                    lower = (int)(price * 0.7);
                                    bPrice = hs.GetStartingPrice(lower, stock);
                                    sPrice = hs.GetStartingPrice(sell, stock);
                                    sPrice = sPrice < lower ? lower + hs.GetQuoteUnit(sPrice, stock) : sPrice;

                                    while (sPrice < upper && quantity-- > 0)
                                    {
                                        SendMessage(sPrice.ToString("C0"), string.Empty);
                                        connect.SendOrder(new SendOrder
                                        {
                                            RQName = balance.Item2,
                                            ScreenNo = connect.LookupScreenNo,
                                            AccNo = account,
                                            OrderType = (int)OpenOrderType.신규매도,
                                            Code = hs.Code,
                                            Qty = ts.Quantity,
                                            Price = sPrice,
                                            HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                            OrgOrderNo = string.Empty
                                        });
                                        for (int i = 0; i < ts.QuoteUnit; i++)
                                            sPrice += hs.GetQuoteUnit(sPrice, stock);
                                    }
                                    while (bPrice < upper && bPrice < buy && Connect.Cash > bPrice * (1.5e-4 + 1))
                                    {
                                        connect.SendOrder(new SendOrder
                                        {
                                            RQName = balance.Item2,
                                            ScreenNo = connect.LookupScreenNo,
                                            AccNo = account,
                                            OrderType = (int)OpenOrderType.신규매수,
                                            Code = hs.Code,
                                            Qty = ts.Quantity,
                                            Price = bPrice,
                                            HogaGb = ((int)HogaGb.지정가).ToString("D2"),
                                            OrgOrderNo = string.Empty
                                        });
                                        for (int i = 0; i < ts.QuoteUnit; i++)
                                            bPrice += hs.GetQuoteUnit(bPrice, stock);

                                        Connect.Cash -= (long)(bPrice * (1.5e-4 + 1));
                                        SendMessage(bPrice.ToString("C0"), Connect.Cash.ToString("C0"));
                                    }
                                    break;
                            }
                        }
                        hs.Code = balance.Item1;
                        hs.Quantity = balance.Item3;
                        hs.Purchase = (int)balance.Item4;
                        hs.Current = (int)balance.Item5;
                        hs.Revenue = balance.Item6;
                        hs.Rate = balance.Item7;
                        Connect.HoldingStock[balance.Item1] = hs;
                    }
                    Send?.Invoke(this, param);
                }
            }
        }
        internal override string ID => id;
        internal override string Value
        {
            get; set;
        }
        internal override string RQName
        {
            set
            {

            }
            get => name;
        }
        internal override string TrCode => code;
        internal override int PrevNext
        {
            get; set;
        }
        internal override string ScreenNo => LookupScreenNo;
        internal override AxKHOpenAPI API
        {
            get; set;
        }
        const string code = "opw00005";
        const string name = "체결잔고요청";
        const string id = "계좌번호;비밀번호;비밀번호입력매체구분";
        const string transactionSuspension = "거래정지";
        readonly string[] opSingle = { "예수금", "예수금D+1", "예수금D+2", "출금가능금액", "미수확보금", "대용금", "권리대용금", "주문가능현금", "현금미수금", "신용이자미납금", "기타대여금", "미상환융자금", "증거금현금", "증거금대용", "주식매수총액", "평가금액합계", "총손익합계", "총손익률", "총재매수가능금액", "20주문가능금액", "30주문가능금액", "40주문가능금액", "50주문가능금액", "60주문가능금액", "100주문가능금액", "신용융자합계", "신용융자대주합계", "신용담보비율", "예탁담보대출금액", "매도담보대출금액", "조회건수" };
        readonly string[] opMultiple = { "신용구분", "대출일", "만기일", "종목번호", "종목명", "결제잔고", "현재잔고", "현재가", "매입단가", "매입금액", "평가금액", "평가손익", "손익률" };
        public override event EventHandler<SendSecuritiesAPI> Send;
    }
}