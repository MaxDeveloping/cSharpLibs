using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database.Interfaces
{
    public interface ISchemaMetaData
    {
        string SourceVersion { get; }

        string TargetVersion { get; }

        DateTime CreationDateTime { get; }
    }
}
