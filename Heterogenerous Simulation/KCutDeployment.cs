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
        public Deployment Deployment
        {
            get
            {
                return m_deployment;
            }
        }

        private Deployment m_deployment;
        private Type m_deploy_type;

        private double percentageOfTunnelingTracer;
        private double percentageOfMarkingTracer;
        private double percentageOfFilteringTracer;

        public int numberOfTTracer;
        public int numberOfMTracer;
        public int numberOfFTracer;

        public KCutDeployment(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer, Type deployType)
        {
            this.percentageOfFilteringTracer = percentageOfFilteringTracer;
            this.percentageOfMarkingTracer = percentageOfMarkingTracer;
            this.percentageOfTunnelingTracer = percentageOfTunnelingTracer;
            this.m_deploy_type = deployType;
        }

        public void Deploy(NetworkTopology networkTopology)
        {
            numberOfTTracer = Convert.ToInt32(Math.Round(percentageOfTunnelingTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));
            numberOfMTracer = Convert.ToInt32(Math.Round(percentageOfMarkingTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));
            numberOfFTracer = Convert.ToInt32(Math.Round(percentageOfFilteringTracer * networkTopology.Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));

            bool isSatisfied = false;
            List<int> last_deploy_count;
            int satisfy_count;
            Deployment try_deploy = null; 
            

            for (int K = 1; K <= networkTopology.Diameter; K += 2)
            {
                int N = 0;
                satisfy_count = 0;

                do
                {
                    if (try_deploy != null)
                        last_deploy_count = new List<int>(try_deploy.DeployNodes);
                    else
                        last_deploy_count = new List<int>();

                    try_deploy = Activator.CreateInstance(m_deploy_type, new object[] { percentageOfTunnelingTracer, percentageOfMarkingTracer, percentageOfFilteringTracer, K, ++N }) as Deployment;
                    try_deploy.Deploy(networkTopology);

                    if (try_deploy.DeployNodes.Count <= numberOfTTracer)
                    {
                        if (m_deployment == null)
                            m_deployment = try_deploy;
                        else if (m_deployment.DeployNodes.Count < try_deploy.DeployNodes.Count)
                            m_deployment = try_deploy;

                        isSatisfied = true;
                    }

                    if (try_deploy.DeployNodes.Except(last_deploy_count).Count() == 0 && last_deploy_count.Except(try_deploy.DeployNodes).Count() == 0)
                        satisfy_count++;
                    else
                        satisfy_count = 0;
                } while (satisfy_count < 2);

                if (isSatisfied)
                {
                    DataUtility.Log(string.Format("K={0},N={1},|D|={2}\n", m_deployment.K, m_deployment.N, m_deployment.DeployNodes.Count));
                    //break;
                }
            }

            int c, e;
            List<int> centerNode = new List<int>();

            foreach (var scope in m_deployment.AllRoundScopeList)
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

            // Clear the deployment method.
            foreach (NetworkTopology.Node node in networkTopology.Nodes)
                node.Tracer = NetworkTopology.TracerType.None;

            centerNode.Sort((x, y) => {
                Network_Simulation.NetworkTopology.Node n1 = networkTopology.Nodes.Find(n => n.ID == x);
                Network_Simulation.NetworkTopology.Node n2 = networkTopology.Nodes.Find(n => n.ID == y);

                return n1.Eccentricity.CompareTo(n2.Eccentricity);
            });

            int i = 0, j = 0;
            m_deployment.FilteringTracerID = new List<int>();
            m_deployment.MarkingTracerID = new List<int>();

            for (; i < centerNode.Count; i++)
            {
                if (i < numberOfFTracer)
                {
                    networkTopology.Nodes.Find(n => n.ID == centerNode[i]).Tracer = NetworkTopology.TracerType.Filtering;
                    m_deployment.FilteringTracerID.Add(centerNode[i]);
                }
                else if (i < numberOfFTracer + numberOfMTracer)
                {
                    networkTopology.Nodes.Find(n => n.ID == centerNode[i]).Tracer = NetworkTopology.TracerType.Marking;
                    m_deployment.MarkingTracerID.Add(centerNode[i]);
                }
                else
                {
                    break;
                }
            }

            for (; i < numberOfFTracer; i++, j++)
            {
                if (m_deployment.AllRoundScopeList[j % m_deployment.AllRoundScopeList.Count].Nodes.Where(n => n.Tracer == NetworkTopology.TracerType.None).Count() > 0)
                {
                    NetworkTopology.Node node = m_deployment.AllRoundScopeList[j % m_deployment.AllRoundScopeList.Count].Nodes.Where(n => n.Tracer == NetworkTopology.TracerType.None).First();
                    node.Tracer = NetworkTopology.TracerType.Filtering;
                    m_deployment.FilteringTracerID.Add(node.ID);
                }
            }

            for (; i < numberOfFTracer + numberOfMTracer; i++, j++)
            {
                if (m_deployment.AllRoundScopeList[j % m_deployment.AllRoundScopeList.Count].Nodes.Where(n => n.Tracer == NetworkTopology.TracerType.None).Count() > 0)
                {
                    NetworkTopology.Node node = m_deployment.AllRoundScopeList[j % m_deployment.AllRoundScopeList.Count].Nodes.Where(n => n.Tracer == NetworkTopology.TracerType.None).First();
                    node.Tracer = NetworkTopology.TracerType.Marking;
                    m_deployment.MarkingTracerID.Add(node.ID);
                }
                else i--;
            }

            foreach (int id in m_deployment.DeployNodes)
                networkTopology.Nodes.Find(n => n.ID == id).Tracer = NetworkTopology.TracerType.Tunneling;
        }

        public override string ToString()
        {
            return m_deploy_type.Name;
        }
    }
}
