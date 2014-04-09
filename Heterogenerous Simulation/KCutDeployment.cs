using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;
using Deployment_Simulation;

namespace Heterogenerous_Simulation
{
    public class KCutDeployment
    {
        private KCutWithClusteringDeployment deployment;
        private double percentageOfTunnelingTracer;
        private double percentageOfMarkingTracer;
        private double percentageOfFilteringTracer;

        public int numberOfTTracer;
        public int numberOfMTracer;
        public int numberOfFTracer;


        public KCutDeployment(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer)
        {
            this.percentageOfFilteringTracer = percentageOfFilteringTracer;
            this.percentageOfMarkingTracer = percentageOfMarkingTracer;
            this.percentageOfTunnelingTracer = percentageOfTunnelingTracer;

            
        }

        public void Deploy(NetworkTopology networkTopology)
        {
            numberOfTTracer = Convert.ToInt32(Math.Round(percentageOfTunnelingTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));
            numberOfMTracer = Convert.ToInt32(Math.Round(percentageOfMarkingTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));
            numberOfFTracer = Convert.ToInt32(Math.Round(percentageOfFilteringTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));

            bool isSatisfied = false;
            List<int> last_deploy_count;

            for (int K = 1; K <= networkTopology.Diameter; K++)
            {
                int N = 0;
                do
                {
                    if (deployment != null)
                        last_deploy_count = new List<int>(deployment.DeployNodes);
                    else
                        last_deploy_count = new List<int>();

                    deployment = new KCutWithClusteringDeployment(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer, K, ++N);
                    deployment.Deploy(networkTopology);

                    if (deployment.DeployNodes.Count <= numberOfFTracer)
                    {
                        isSatisfied = true;
                        break;
                    }
                } while (deployment.DeployNodes.Except(last_deploy_count).Count() != 0 || last_deploy_count.Except(deployment.DeployNodes).Count() != 0);

                if (isSatisfied)
                    break;
            }

            int c, e;
            List<int> centerNode = new List<int>();

            foreach (var scope in deployment.AllRoundScopeList)
            {
                if (scope.FindCenterNodeID(out c, out e, true))
                {
                    DataUtility.Log(string.Format("center ID: {0}\n", c));
                    centerNode.Add(c);
                }
                else
                {
                    DataUtility.Log(string.Format("center ID: {0}\n", scope.Nodes[0].ID));
                    centerNode.Add(scope.Nodes[0].ID);
                }
            }

            //centerNode.Sort((x, y) => {
            //    return 
            //});

            foreach (int id in deployment.DeployNodes)
                networkTopology.Nodes.Find(n => n.ID == id).Tracer = NetworkTopology.TracerType.Tunneling;
        }
    }
}
