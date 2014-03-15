using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace Network_Simulation
{
    public partial class NetworkTopology
    {
        public List<Node> Nodes;
        public List<Edge> Edges;
        public Adjacency[,] AdjacentMatrix;
        
        public int numberOfAttackers;
        public int numberOfVictims;
        public List<int> idOfVictims;

        private double percentageOfAttackers;
        private string fileName;

        // The variable of drawing network topology.
        private float m_scale_x;
        private float m_scale_y;
        private float m_pre_move_x;
        private float m_pre_move_y;
        private float m_move_x;
        private float m_move_y;
        private bool m_is_setup_control;
        private bool m_is_mouse_down;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="percentageOfAttackers">The percentage of attackers.</param>
        /// <param name="numberOfVictims">The number of victim.</param>
        public NetworkTopology(double percentageOfAttackers, int numberOfVictims)
        {
            // Create instance of nodes.
            Nodes = new List<Node>();
            Edges = new List<Edge>();

            idOfVictims = new List<int>();

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

            // Initializing the drawing network variable.
            m_scale_x = 1;
            m_scale_y = 1;
            m_pre_move_x = 0;
            m_pre_move_y = 0;
            m_move_x = 0;
            m_move_y = 0;
            m_is_setup_control = false;
            m_is_mouse_down = false;

            // Create random array.
            int[] randomArray = DataUtility.RandomArray(Nodes.Count);
            int randomArrayIndex = 0;

            numberOfAttackers = Convert.ToInt32(Math.Round(percentageOfAttackers * Nodes.Count / 100, 0, MidpointRounding.AwayFromZero));

            // Select victims.
            for (; randomArrayIndex < numberOfVictims; randomArrayIndex++)
            {
                idOfVictims.Add(randomArray[randomArrayIndex]);
                Nodes[randomArray[randomArrayIndex]].Type = NodeType.Victim;
            }

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

            Random rd = new Random();
            int victim;
            List<int> path;

            foreach (Node node in Nodes)
            {
                victim = idOfVictims[rd.Next(idOfVictims.Count)];
                path = Path(node.ID, Nodes[victim].ID);
                switch (node.Type)
                {
                    case NodeType.Attacker:
                        
                        break;

                    case NodeType.Normal:

                        break;
                }
            }

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
                            string[] data = lines[i].Split(' ', '\t');
                            Nodes.Add(new Node() { ID = Convert.ToInt32(data[0]), Xpos = Convert.ToDouble(data[1]), Ypos = Convert.ToDouble(data[2]) });
                        }
                    }
                    // Reading edges
                    else if (Regex.IsMatch(lines[i], @"(^Edges)"))
                    {
                        numberOfEdges = Convert.ToInt32(Regex.Match(lines[i], @"\d+").Value);
                        for (int j = ++i; i < j + numberOfEdges; i++)
                        {
                            string[] data = lines[i].Split(' ', '\t');
                            int node1 = Convert.ToInt32(data[1]);
                            int node2 = Convert.ToInt32(data[2]);
                            double length = Convert.ToDouble(data[3]);
                            double delay = Convert.ToDouble(data[4]);

                            Edges.Add(new Edge() { Node1 = node1, Node2 = node2, Length = length, Delay = delay });
                        }

                        // If the file of shortest path is exist, then load into memory
                        if (File.Exists(shortestPathFileName))
                        {
                            ReadShortestPathFile(shortestPathFileName);
                            break;
                        }
                        // Computing shortest path
                        else
                        {
                            ComputingShortestPath();

                            // Output the result to the file. (XXX.ShortestPath)
                            WriteShortestPathFile(shortestPathFileName);
                        }
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

            // Create the space of adjacent matrix
            if (AdjacentMatrix == null)
                AdjacentMatrix = new Adjacency[Nodes.Count, Nodes.Count];

            using (BufferedStream bs = new BufferedStream(File.OpenRead(fileName)))
            {
                using (StreamReader reader = new StreamReader(bs))
                {
                    int i = 0;

                    while (!reader.EndOfStream)
                    {
                        string[] data = reader.ReadLine().Split(' ');

                        for (int j = 0; j < data.Length; j++)
                            if (!data[j].Equals("null"))
                                AdjacentMatrix[i, j] = new Adjacency(data[j]);

                        i++;
                    }
                }
            }
            //try
            //{
            //    using (BufferedStream bs = new BufferedStream(File.OpenRead(fileName)))
            //    {
            //        BinaryFormatter formatter = new BinaryFormatter();
            //        AdjacentMatrix = formatter.Deserialize(bs) as Adjacency[,];
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        /// <summary>
        /// Output the shortest path file without re-computing next time.
        /// </summary>
        private void WriteShortestPathFile(string fileName)
        {
            Console.WriteLine("Writing shortest path file...");

            using (BufferedStream bs = new BufferedStream(File.OpenWrite(fileName)))
            {
                using (StreamWriter writer = new StreamWriter(bs))
                {
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        for (int j = 0; j < Nodes.Count; j++)
                        {
                            if (AdjacentMatrix[i, j] == null)
                                writer.Write("null");
                            else
                                writer.Write(AdjacentMatrix[i, j].GetString());

                            if (j != Nodes.Count - 1)
                                writer.Write(" ");
                        }
                        writer.WriteLine();
                    }
                }
            }
            //try
            //{
            //    using (FileStream fStream = File.OpenWrite(fileName))
            //    {
            //        BinaryFormatter formatter = new BinaryFormatter();
            //        formatter.Serialize(fStream, AdjacentMatrix);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }
}
