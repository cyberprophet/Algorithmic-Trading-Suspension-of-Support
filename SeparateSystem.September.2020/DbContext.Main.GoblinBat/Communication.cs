using System;
using System.Data.Entity.Migrations;

using ShareInvest.Cummunication;
using ShareInvest.Models;

namespace ShareInvest.Main
{
    public class Communication
    {
        public int UploadGoblinBat(FileGoblinBat file)
        {
            var result = int.MinValue;
            using (var db = new MainContext())
                try
                {
                    db.File.AddOrUpdate(file);
                    result = db.SaveChanges();
                }
                catch (Exception ex)
                {
                    new ExceptionMessage(ex.StackTrace, ex.TargetSite.Name);
                }
            return result;
        }
    }
}