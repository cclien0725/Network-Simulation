using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network_Simulation;
using Deployment_Simulation;

namespace Heterogenerous_Simulation
{
    public class KCutDeploymentV2
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

        public KCutDeploymentV2(double percentageOfTunnelingTracer, double percentageOfMarkingTracer, double percentageOfFilteringTracer, Type deployType)
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

            List<int> last_deploy_count;
            int satisfy_count;
            Deployment try_deploy = null;
            List<Deployment> dList = new List<Deployment>();

            for (int K = 1; K <= networkTopology.Diameter; K++)
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

                    if (try_deploy.DeployNodes.Count <= numberOfTTracer + numberOfMTracer + numberOfFTracer)
                    {
                        DataUtility.Log(string.Format("K={0},N={1},|D|={2}\n", try_deploy.K, try_deploy.N, try_deploy.DeployNodes.Count));
                        dList.Add(try_deploy);
                    }

                    if (try_deploy.DeployNodes.Except(last_deploy_count).Count() == 0 && last_deploy_count.Except(try_deploy.DeployNodes).Count() == 0)
                        satisfy_count++;
                    else
                        satisfy_count = 0;
                } while (satisfy_count < 2);
            }

            dList.Sort((x, y) => x.DeployNodes.Count.CompareTo(y.DeployNodes.Count));
            m_deployment = dList.Last();

            //int c, e;
            List<NetworkTopology.Node> dNode = new List<NetworkTopology.Node>(networkTopology.Nodes.Where(n => m_deployment.DeployNodes.Contains(n.ID)));

            //foreach (var scope in m_deployment.AllRoundScopeList)
            //{
            //    if (scope.FindCenterNodeID(out c, out e, true))
            //    {
            //        DataUtility.Log(string.Format("center ID: {0}\n", c));
            //        centerNode.Add(networkTopology.Nodes.Find(n => n.ID == c));
            //    }
            //    else
            //    {
            //        DataUtility.Log(string.Format("center ID: {0}\n", scope.Nodes[0].ID));
            //        centerNode.Add(networkTopology.Nodes.Find(n => n.ID == scope.Nodes[0].ID));
            //    }
            //}

            //networkTopology.Reset();

            // Clear the deployment method.
            foreach (NetworkTopology.Node node in networkTopology.Nodes)
                node.Tracer = NetworkTopology.TracerType.None;

            dNode.Sort((x, y) => networkTopology.NodeIDPathDistrib[y.ID].CompareTo(networkTopology.NodeIDPathDistrib[x.ID]));

            m_deployment.FilteringTracerID = new List<int>();
            m_deployment.MarkingTracerID = new List<int>();

            for (int i = 0; i < numberOfFTracer; i++)
            {
                NetworkTopology.Node node = dNode.First(n => n.Tracer == NetworkTopology.TracerType.None);
                
                if (node == null)
                    break;

                node.Tracer = NetworkTopology.TracerType.Filtering;
                m_deployment.FilteringTracerID.Add(node.ID);
            }

            for (int i = 0; i < numberOfMTracer; i++)
            {
                NetworkTopology.Node node = dNode.First(n => n.Tracer == NetworkTopology.TracerType.None);

                if (node == null)
                    break;

                node.Tracer = NetworkTopology.TracerType.Marking;
                m_deployment.MarkingTracerID.Add(node.ID);
            }

            //int left = 0, right = dNode.Count - 1;

            //for (int i = 0; i < numberOfFTracer; i++, left += 2, right -= 2)
            //{
            //    while (dNode[(left + left / dNode.Count) % dNode.Count].Tracer != NetworkTopology.TracerType.None)
            //        left += 2;
            //    NetworkTopology.Node leftNode = dNode[(left + left / dNode.Count) % dNode.Count];
            //    leftNode.Tracer = NetworkTopology.TracerType.Filtering;
            //    m_deployment.FilteringTracerID.Add(leftNode.ID);

            //    if (++i >= numberOfFTracer)
            //        break;

            //    while (dNode[(Math.Abs(right) + Math.Abs(right) / dNode.Count) % dNode.Count].Tracer != NetworkTopology.TracerType.None)
            //        right -= 2;
            //    NetworkTopology.Node rightNode = dNode[(Math.Abs(right) + Math.Abs(right) / dNode.Count) % dNode.Count];
            //    rightNode.Tracer = NetworkTopology.TracerType.Filtering;
            //    m_deployment.FilteringTracerID.Add(rightNode.ID);
            //}

            //for (int i = 0; i < numberOfMTracer; i++, left += 2, right -= 2)
            //{
            //    while (dNode[(left + left / dNode.Count) % dNode.Count].Tracer != NetworkTopology.TracerType.None)
            //        left += 2;
            //    NetworkTopology.Node leftNode = dNode[(left + left / dNode.Count) % dNode.Count];
            //    leftNode.Tracer = NetworkTopology.TracerType.Marking;
            //    m_deployment.MarkingTracerID.Add(leftNode.ID);

            //    if (++i >= numberOfMTracer)
            //        break;

            //    while (dNode[(Math.Abs(right) + Math.Abs(right) / dNode.Count) % dNode.Count].Tracer != NetworkTopology.TracerType.None)
            //        right -= 2;
            //    NetworkTopology.Node rightNode = dNode[(Math.Abs(right) + Math.Abs(right) / dNode.Count) % dNode.Count];
            //    rightNode.Tracer = NetworkTopology.TracerType.Marking;
            //    m_deployment.MarkingTracerID.Add(rightNode.ID);
            //}

            foreach (NetworkTopology.Node node in dNode)
            {
                if (node.Tracer == NetworkTopology.TracerType.None)
                    node.Tracer = NetworkTopology.TracerType.Tunneling;
            }
        }

        public override string ToString()
        {
            return m_deploy_type.Name;
        }
    }
}
