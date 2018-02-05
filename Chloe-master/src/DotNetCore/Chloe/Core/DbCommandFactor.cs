using Chloe.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Core
{
    class DbCommandFactor
    {
        public DbCommandFactor(IObjectActivator objectActivator, string commandText, DbParam[] parameters)
        {
            this.ObjectActivator = objectActivator;
            this.CommandText = commandText;
            this.Parameters = parameters;
        }
        public IObjectActivator ObjectActivator { get; set; }
        public string CommandText { get; set; }
        public DbParam[] Parameters { get; set; }
    }
}
