using CommonLibs.Database.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings
{
    public class DbServerSettings : DbBasicSettings, IDbServerSettings
    {
        public string Host { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string DefaultDbName { get; set; } = string.Empty;
    }
}
