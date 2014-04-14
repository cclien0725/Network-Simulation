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
        public KCutWithClusteringDeployment Deployment
        {
            get
            {
                return deployment;
            }
        }

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
            int satisfy_count;

            for (int K = 1; K <= networkTopology.Diameter; K++)
            {
                int N = 0;
                satisfy_count = 0;

                do
                {
                    if (deployment != null)
                        last_deploy_count = new List<int>(deployment.DeployNodes);
                    else
                        last_deploy_count = new List<int>();

                    deployment = new KCutWithClusteringDeployment(percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer, K, ++N);
                    deployment.Deploy(networkTopology);

                    if (deployment.DeployNodes.Count <= numberOfTTracer)
                    {
                        isSatisfied = true;
                        break;
                    } 
                    
                    if (deployment.DeployNodes.Except(last_deploy_count).Count() == 0 && last_deploy_count.Except(deployment.DeployNodes).Count() == 0)
                    {
                        satisfy_count++;
                    }
                    else
                    {
                        satisfy_count = 0;
                    }
                } while (satisfy_count < 2);

                if (isSatisfied)
                {
                    DataUtility.Log(string.Format("K={0},N={1}", K, N));
                    break;
                }
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

            networkTopology.Reset();

            centerNode.Sort((x, y) => {
                Network_Simulation.NetworkTopology.Node n1 = networkTopology.Nodes.Find(n => n.ID == x);
                Network_Simulation.NetworkTopology.Node n2 = networkTopology.Nodes.Find(n => n.ID == y);

                return n1.Eccentricity.CompareTo(n2.Eccentricity);
            });

            int i = 0, j = 0;
            deployment.FilteringTracerID = new List<int>();
            deployment.MarkingTracerID = new List<int>();

            //for (i = 0; i < numberOfFTracer; i++)
            //{
            //    networkTopology.Nodes.Find(n => n.ID == centerNode[i]).Tracer = NetworkTopology.TracerType.Filtering;
            //    deployment.FilteringTracerID.Add(centerNode[i]);
            //}

            //for (; i < numberOfMTracer + numberOfFTracer; i++)
            //{
            //    networkTopology.Nodes.Find(n => n.ID == centerNode[i]).Tracer = NetworkTopology.TracerType.Marking;
            //    deployment.MarkingTracerID.Add(centerNode[i]);
            //}

            for (; i < centerNode.Count; i++)
            {
                if (i < numberOfFTracer)
                {
                    networkTopology.Nodes.Find(n => n.ID == centerNode[i]).Tracer = NetworkTopology.TracerType.Filtering;
                    deployment.FilteringTracerID.Add(centerNode[i]);
                }
                else if (i < numberOfFTracer + numberOfMTracer)
                {
                    networkTopology.Nodes.Find(n => n.ID == centerNode[i]).Tracer = NetworkTopology.TracerType.Marking;
                    deployment.MarkingTracerID.Add(centerNode[i]);
                }
                else
                {
                    break;
                }
            }

            for (; i < numberOfFTracer; i++, j++)
            {
                deployment.AllRoundScopeList[j % deployment.AllRoundScopeList.Count].Nodes.Where(n => !centerNode.Contains(n.ID)).First().Tracer = NetworkTopology.TracerType.Filtering;
                deployment.FilteringTracerID.Add(deployment.AllRoundScopeList[j % deployment.AllRoundScopeList.Count].Nodes.Where(n => !centerNode.Contains(n.ID)).First().ID);
            }

            for (; i < numberOfFTracer + numberOfMTracer; i++, j++)
            {
                if (deployment.AllRoundScopeList[j % deployment.AllRoundScopeList.Count].Nodes.Where(n => !centerNode.Contains(n.ID) && n.Tracer == NetworkTopology.TracerType.None).Count() > 0)
                {
                    deployment.AllRoundScopeList[j % deployment.AllRoundScopeList.Count].Nodes.Where(n => !centerNode.Contains(n.ID) && n.Tracer == NetworkTopology.TracerType.None).First().Tracer = NetworkTopology.TracerType.Marking;
                    deployment.MarkingTracerID.Add(deployment.AllRoundScopeList[j % deployment.AllRoundScopeList.Count].Nodes.Where(n => !centerNode.Contains(n.ID)).First().ID);
                }
            }

            foreach (int id in deployment.DeployNodes)
                networkTopology.Nodes.Find(n => n.ID == id).Tracer = NetworkTopology.TracerType.Tunneling;
        }
    }
}
