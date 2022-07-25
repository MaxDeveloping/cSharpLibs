using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings.Interfaces
{
    public interface IDbServerPortSettings : IDbServerSettings
    {
        int Port { get; set; }
    }
}
