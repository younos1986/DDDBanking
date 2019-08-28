using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Api.Infrastructure.BackgroundTasks
{
    public class BackgroundTaskConfiguration
    {
        public string Name { get; set; }
        public int Interval { get; set; }
        public bool IsFireAndForget { get; set; }
    }
}
