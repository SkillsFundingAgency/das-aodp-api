using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Models.Jobs
{
    public class Job
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public bool Enabled { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? LastRunTime { get; set; }
    }
}
