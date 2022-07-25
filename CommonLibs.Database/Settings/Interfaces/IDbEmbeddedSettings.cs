using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings.Interfaces
{
    public interface IDbEmbeddedSettings : IDbBasicSettings
    {
        string DbFilePath { get; set; }
    }
}
