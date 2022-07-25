using CommonLibs.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.Database
{
    public class SchemaMetaData : ISchemaMetaData
    {
        public string SourceVersion { get; set; }

        public string TargetVersion { get; set; }

        public DateTime CreationDateTime { get; set; }
    }
}
