using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings.Interfaces
{
    public interface IDbServerSettings : IDbBasicSettings
    {
        string Host { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        string DefaultDbName { get; set; }
    }
}
