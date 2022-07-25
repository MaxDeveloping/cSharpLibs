using CommonLibs.Database.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings
{
    public abstract class DbBasicSettings : IDbBasicSettings
    {
        public TimeSpan CommandTimeout { get; set; }

        public TimeSpan ConnectionTimeout { get; set; }

        public DbBasicSettings()
        {
            CommandTimeout = new TimeSpan(0, 5, 0);
            ConnectionTimeout = new TimeSpan(0, 0, 15);
        }
    }
}
