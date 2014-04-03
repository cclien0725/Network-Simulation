using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;

namespace Heterogenerous_Simulation
{
    class NoneDeployment : Deployment
    {
        public NoneDeployment(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer)
            : base(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer)
        { }

        protected override void doDeploy(NetworkTopology networkTopology)
        {
            //throw new NotImplementedException();
        }

        protected override void write2SQLite(NetworkTopology networkTopology)
        {
            //throw new NotImplementedException();
        }
    }
}
