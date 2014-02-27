using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;

namespace Heterogenerous_Simulation
{
    class TomatoDeployment : Deployment
    {
        public TomatoDeployment(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer)
            : base(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer)
        { }

        public override void Deploy(NetworkTopology networkTopology)
        {
            Console.WriteLine("Tomato deployment.");
        }
    }
}
