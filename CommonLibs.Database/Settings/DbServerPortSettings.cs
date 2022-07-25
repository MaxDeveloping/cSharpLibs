using CommonLibs.Database.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings
{
    public class DbServerPortSettings : DbServerSettings, IDbServerPortSettings
    {
        public int Port { get; set; }
    }
}
