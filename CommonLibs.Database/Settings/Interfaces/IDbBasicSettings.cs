using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Settings.Interfaces
{
    public interface IDbBasicSettings
    {
        TimeSpan CommandTimeout { get; set; }

        TimeSpan ConnectionTimeout { get; set; }
    }
}
