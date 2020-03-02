using System.Collections.Generic;

namespace ShareInvest.Struct
{
    public interface IBlock
    {
        int Occurs
        {
            get;
        }
        string Name
        {
            get;
        }
        string Field
        {
            get;
        }
        string Property
        {
            get;
        }
        Queue<IBlock> Inquiry
        {
            get;
        }
        Queue<IBlock> GetInBlock(string res);
        Queue<IBlock> GetOutBlock(string res);
    }
}