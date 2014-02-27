using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Network_Simulation
{
    public partial class NetworkTopology
    {
        public Dictionary<int, int> NodeIndexTable;
        public List<Node> Nodes;
        public Adjacency[,] AdjacentMatrix;
        
        public int numberOfAttackers;
        public int numberOfVictims;

        private double percentageOfAttackers;
        private string fileName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="percentageOfAttackers">The percentage of attackers.</param>
        /// <param name="numberOfVictims">The number of victim.</param>
        public NetworkTopology(double percentageOfAttackers, int numberOfVictims)
        {
            // Create instance of nodes.
            Nodes = new List<Node>();
            NodeIndexTable = new Dictionary<int, int>();

            // Initialize environment parameters.
            this.percentageOfAttackers = percentageOfAttackers;
            this.numberOfVictims = numberOfVictims;
        }

        /// <summary>
        /// Randomly choose attackers and victim.
        /// </summary>
        private void Initialize()
        {
            if (Nodes.Count == 0)
                throw new Exception("Initilaize() Fail: There are 0 nodes in the network.");

            // Create random array.
            int[] randomArray = DataUtility.RandomArray(Nodes.Count);
            int randomArrayIndex = 0;

            numberOfAttackers = Convert.ToInt32(Math.Round(percentageOfAttackers * Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));

            // Select victims.
            for (; randomArrayIndex < numberOfVictims; randomArrayIndex++)
                Nodes[randomArray[randomArrayIndex]].Type = NodeType.Victim;
            
            // Select attackers.
            for (; randomArrayIndex < numberOfAttackers + numberOfVictims; randomArrayIndex++)
                Nodes[randomArray[randomArrayIndex]].Type = NodeType.Attacker;
        }

        /// <summary>
        /// Clear the deployment method.
        /// </summary>
        private void ResetDeployment()
        {
            if (Nodes.Count == 0)
                throw new Exception("Initilaize() Fail: There are 0 nodes in the network.");

            foreach (Node n in Nodes)
                n.Tracer = TracerType.None;
        }

        /// <summary>
        /// Start simulate.
        /// </summary>
        public void Run()
        {
            if (Nodes.Count == 0)
                throw new Exception("Run() Fail: There are 0 nodes in the network.");



        }

        /// <summary>
        /// Deploy the tracer depends on provider's deployment method.
        /// </summary>
        /// <param name="deployment">Deployment method.</param>
        public void Deploy(Deployment deployment)
        {
            ResetDeployment();

            deployment.Initialize(this);
            deployment.Deploy(this);
        }

        /// <summary>
        /// Return the shortest path from source node to destination node.
        /// </summary>
        /// <param name="SourceNodeId">The source node id.</param>
        /// <param name="DestinationNodeId">The destination node id.</param>
        /// <returns>The sequence of the node id, which is shortest path.</returns>
        public List<int> Path(int SourceNodeId, int DestinationNodeId)
        {
            if (Nodes.Count == 0)
                throw new Exception("Path() Fail: There are 0 nodes in the network.");

            List<int> path = new List<int>() { NodeIndexTable[SourceNodeId], NodeIndexTable[DestinationNodeId]};

            for (int i = 0; i < path.Count - 1; i++)
            {
                if (AdjacentMatrix[path[i], path[i + 1]] != null &&
                    AdjacentMatrix[path[i], path[i + 1]].Predecessor != -1)
                {
                    path.Insert(i + 1, AdjacentMatrix[path[i], path[i + 1]].Predecessor);
                    i--;
                }
            }

            for (int i = 0; i < path.Count; i++) 
            {
                var NodeId = NodeIndexTable.Where(item => item.Value == path[i]).Select(item => item.Key);
                path[i] = NodeId.First();
            }

            return path;
        }

        /// <summary>
        /// Reading brite file and convert to our data structure.
        /// </summary>
        /// <param name="fileName">The path of brite file.</param>
        public void ReadBriteFile(string fileName)
        {
            this.fileName = fileName;
            try
            {
                Console.WriteLine("Start reading brite file...");

                int numberOfNodes;
                int numberOfEdges;
                string[] lines = File.ReadAllLines(fileName);
                string shortestPathFileName = string.Format(@"{0}.ShortestPath", fileName);

                for (int i = 0; i < lines.Length; i++)
                {
                    // Reading nodes
                    if (Regex.IsMatch(lines[i], @"^Nodes"))
                    {
                        numberOfNodes = Convert.ToInt32(Regex.Match(lines[i], @"\d+").Value);
                        for (int j = ++i; i < j + numberOfNodes; i++)
                        {
                            string[] data = lines[i].Split(' ');
                            Nodes.Add(new Node() { Xpos = Convert.ToDouble(data[1]), Ypos = Convert.ToDouble(data[2]) });
                            NodeIndexTable.Add(Convert.ToInt32(data[0]), Nodes.Count - 1);
                        }

                        // Create the space of adjacent matrix
                        AdjacentMatrix = new Adjacency[numberOfNodes, numberOfNodes];

                        // If the file of shortest path is exist, then load into memory
                        if (File.Exists(shortestPathFileName))
                        {
                            ReadShortestPathFile(shortestPathFileName);
                            break;
                        }
                    }
                    // Reading edges
                    else if (Regex.IsMatch(lines[i], @"(^Edges)"))
                    {
                        numberOfEdges = Convert.ToInt32(Regex.Match(lines[i], @"\d+").Value);
                        for (int j = ++i; i < j + numberOfEdges; i++)
                        {
                            string[] data = lines[i].Split(' ');
                            int node1 = Convert.ToInt32(data[1]);
                            int node2 = Convert.ToInt32(data[2]);
                            double length = Convert.ToDouble(data[3]);
                            double delay = Convert.ToDouble(data[4]);

                            AdjacentMatrix[NodeIndexTable[node1], NodeIndexTable[node2]] = new Adjacency() { Delay = delay, Length = length };
                            AdjacentMatrix[NodeIndexTable[node2], NodeIndexTable[node1]] = new Adjacency() { Delay = delay, Length = length };
                        }

                        // Computing shortest path
                        ComputingShortestPath();

                        // Output the result to the file. (XXX.ShortestPath)
                        WriteShortestPathFile(shortestPathFileName);
                    }
                }

                Initialize();
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Read the shortest path file of the specific brite file.
        /// </summary>
        /// <param name="fileName">Shortest path file name. EX:n1kd4_14.brite.ShortestPath</param>
        private void ReadShortestPathFile(string fileName)
        {
            Console.WriteLine("Reading shortest path file...");
            string[] lines = File.ReadAllLines(fileName);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(' ');
                for (int j = 0; j < data.Length; j++)
                {
                    if (!data[j].Equals("null"))
                        AdjacentMatrix[i, j] = new Adjacency(data[j]);
                }
            }
        }

        /// <summary>
        /// Output the shortest path file without re-computing next time.
        /// </summary>
        private void WriteShortestPathFile(string fileName)
        {
            Console.WriteLine("Writing shortest path file...");
            string contents = string.Empty;

            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    if (AdjacentMatrix[i, j] == null)
                        contents += "null";
                    else
                        contents += AdjacentMatrix[i, j].GetString();

                    if (j != Nodes.Count - 1)
                        contents += " ";
                }
                contents += Environment.NewLine;
            }

            File.WriteAllText(fileName, contents);
        }

        /// <summary>
        /// Using Floyd-Warshar algorithm computing shortest path.
        /// </summary>
        private void ComputingShortestPath()
        {
            Console.WriteLine("Computing shortest path...");
            // Floyd-Warshall algorithm
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    for (int k = j + 1; k < Nodes.Count; k++)
                    {
                        if ((AdjacentMatrix[j, k] == null && AdjacentMatrix[j, i] !=null && AdjacentMatrix[i,k] != null) ||
                            (AdjacentMatrix[j, k] != null && AdjacentMatrix[j, i] != null && AdjacentMatrix[i, k] != null && AdjacentMatrix[j, k].Length > AdjacentMatrix[j, i].Length + AdjacentMatrix[i, k].Length))
                        {
                            if (AdjacentMatrix[k, j] == null)
                            {
                                AdjacentMatrix[k, j] = new Adjacency();
                                AdjacentMatrix[j, k] = new Adjacency();
                            }

                            AdjacentMatrix[k, j].Length = AdjacentMatrix[j, k].Length = AdjacentMatrix[j, i].Length + AdjacentMatrix[i, k].Length;
                            AdjacentMatrix[k, j].Delay = AdjacentMatrix[j, k].Delay = AdjacentMatrix[j, i].Delay + AdjacentMatrix[i, k].Delay;
                            AdjacentMatrix[k, j].Predecessor = AdjacentMatrix[j, k].Predecessor = i;
                        }
                    }
                }
            }
        }

        private long Poisson(double lambda)
        {
            Random rd = new Random();
            long k = 0;
            double L = Math.Exp(-lambda), p = 1;
            double r;

            do {
                ++k;
                r = rd.NextDouble();
                p *= r;
            } while (p > L);

            return --k;
        }
    }
}
