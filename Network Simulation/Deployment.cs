using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network_Simulation
{
    public abstract class Deployment
    {
        protected double percentageOfTunnelingTracer;
        protected double percentageOfMarkingTracer;
        protected double percentageOfFilteringTracer;

        public int numberOfTTracer;
        public int numberOfMTracer;
        public int numberOfFTracer;

        public Deployment(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer)
        {
            this.percentageOfTunnelingTracer = percentageOfTunnelingTracer;
            this.percentageOfMarkingTracer = percentageOfMarkingTracer;
            this.percentageOfFilteringTracer = percentageOfFilteringTracer;
        }

        private void Initialize(NetworkTopology networkTopology)
        {
            numberOfTTracer = Convert.ToInt32(Math.Round(percentageOfTunnelingTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));
            numberOfMTracer = Convert.ToInt32(Math.Round(percentageOfMarkingTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));
            numberOfFTracer = Convert.ToInt32(Math.Round(percentageOfFilteringTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));

            // Clear the deployment method.
            foreach (NetworkTopology.Node node in networkTopology.Nodes)
                node.Tracer = NetworkTopology.TracerType.None;
        }

        virtual public void Deploy(NetworkTopology networkTopology)
        {
            Initialize(networkTopology);

        }
    }
}
