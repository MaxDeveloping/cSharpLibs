using CommonLibs.Database.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings
{
    public class DbEmbeddedSettings : DbBasicSettings, IDbEmbeddedSettings
    {
        public string DbFilePath { get; set; } = string.Empty;
    }
}
