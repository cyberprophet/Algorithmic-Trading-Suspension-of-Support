using System;
using System.Collections.Generic;

using ShareInvest.Catalog.DataBase;

namespace ShareInvest.EventHandler.BackTesting
{
    public class Statistics : EventArgs
    {
        public ImitationGame Game
        {
            get; private set;
        }
        public Catalog.Setting Setting
        {
            get; private set;
        }
        public Dictionary<DateTime, string> Information
        {
            get; private set;
        }
        public Statistics(ImitationGame game) => Game = game;
        public Statistics(Catalog.Setting setting) => Setting = setting;
        public Statistics(Dictionary<DateTime, string> information) => Information = information;
    }
}