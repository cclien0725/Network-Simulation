using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Network_Simulation
{
    [Serializable]
    public partial class NetworkTopology
    {
        public List<Node> Nodes;
        public List<Edge> Edges;
        public Adjacency[,] AdjacentMatrix;
        
        public int numberOfAttackers;
        public int numberOfNormalUsers;
        public int numberOfVictims;
        public List<int> idOfVictims;
        public string FileName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(fileName);
            }
        }
        public int Diameter
        {
            get
            {
                return m_diameter;
            }
        }

        private double percentageOfNormalUser;
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

        private int m_diameter;

        private List<Node> m_src_nodes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="percentageOfAttackers">The percentage of attackers.</param>
        /// <param name="percentageOfNormalUser">The percentage of normal users.</param>
        /// <param name="numberOfVictims">The number of victim.</param>
        public NetworkTopology(double percentageOfAttackers, double percentageOfNormalUser, int numberOfVictims)
        {
            // Create instance of nodes.
            Nodes = new List<Node>();
            Edges = new List<Edge>();

            idOfVictims = new List<int>();

            // Initializing the drawing network variable.
            m_scale_x = 1;
            m_scale_y = 1;
            m_pre_move_x = 0;
            m_pre_move_y = 0;
            m_move_x = 0;
            m_move_y = 0;
            m_is_setup_control = false;
            m_is_mouse_down = false;

            // Initialize environment parameters.
            this.percentageOfAttackers = percentageOfAttackers;
            this.percentageOfNormalUser = percentageOfNormalUser;
            this.numberOfVictims = numberOfVictims;
        }

        public NetworkTopology(List<Node> src_node_list)
        {
            // Create instance of nodes.
            Nodes = new List<Node>();
            Edges = new List<Edge>();

            m_src_nodes = new List<Node>(src_node_list);
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
            numberOfNormalUsers = Convert.ToInt32(Math.Round(percentageOfNormalUser * Nodes.Count / 100 , 0, MidpointRounding.AwayFromZero));

            // Select victims.
            for (; randomArrayIndex < numberOfVictims; randomArrayIndex++)
            {
                idOfVictims.Add(randomArray[randomArrayIndex]);
                Nodes[randomArray[randomArrayIndex]].Type = NodeType.Victim;
            }

            // Select attackers.
            for (; randomArrayIndex < numberOfAttackers + numberOfVictims; randomArrayIndex++)
                Nodes[randomArray[randomArrayIndex]].Type = NodeType.Attacker;

            // Select normal users.
            for (; randomArrayIndex < numberOfNormalUsers + numberOfAttackers + numberOfVictims; randomArrayIndex++)
                Nodes[randomArray[randomArrayIndex]].Type = NodeType.Normal;

            // Finding diameter of the network topology.
            m_diameter = int.MinValue;
            foreach (var node in Nodes)
                if (m_diameter < node.Eccentricity)
                    m_diameter = node.Eccentricity;
        }

        /// <summary>
        /// Reading brite file and convert to our data structure.
        /// </summary>
        /// <param name="fileName">The path of brite file.</param>
        public void ReadBriteFile(string fileName)
        {
            this.fileName = fileName;
            Nodes.Clear();
            Edges.Clear();

            //try
            //{
               DataUtility.Log("Reading brite file...");

                int numberOfNodes;
                int numberOfEdges;
                string[] lines = File.ReadAllLines(fileName);
                string shortestPathFileName = string.Format(@"{0}.ShortestPath", fileName);
                string eccDegPathFileName = string.Format(@"{0}.EccDeg", fileName);

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
                    }
                }

                DataUtility.Log("Done.\n", false);

                m_src_nodes = new List<Node>(Nodes);

                // If the file of shortest path is exist, then load into memory
                if (File.Exists(shortestPathFileName))
                    ReadShortestPathFile(shortestPathFileName);
                // Computing shortest path
                else
                {
                    ComputingShortestPath();

                    // Output the result to the file. (XXX.ShortestPath)
                    WriteShortestPathFile(shortestPathFileName);
                }

                if (File.Exists(eccDegPathFileName))
                    ReadEccentricityDegreeFile(eccDegPathFileName);
                else
                {
                    ComputingAllDegree();
                    ComputingAllEccentricity();

                    WriteEccentricityDegreeFile(eccDegPathFileName);
                }
                
                Initialize();
            //}
            //catch(Exception exception)
            //{
            //    throw exception;
            //}
        }

        public void Reset()
        {
            string shortestPathFileName = string.Format(@"{0}.ShortestPath", fileName);
            string eccDegPathFileName = string.Format(@"{0}.EccDeg", fileName);

            // If the file of shortest path is exist, then load into memory
            if (File.Exists(shortestPathFileName))
                ReadShortestPathFile(shortestPathFileName);

            if (File.Exists(eccDegPathFileName))
                ReadEccentricityDegreeFile(eccDegPathFileName);
        }

        /// <summary>
        /// Read the shortest path file of the specific brite file.
        /// </summary>
        /// <param name="fileName">Shortest path file name. EX:n1kd4_14.brite.ShortestPath</param>
        private void ReadShortestPathFile(string fileName)
        {
            DataUtility.Log("Reading shortest path file...");

            // Create the space of adjacent matrix
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

            DataUtility.Log("Done.\n", false);
        }

        /// <summary>
        /// Output the shortest path file without re-computing next time.
        /// </summary>
        private void WriteShortestPathFile(string fileName)
        {
            DataUtility.Log("Writing shortest path file...");

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
            DataUtility.Log("Done.\n", false);
        }

        private void ReadEccentricityDegreeFile(string fileName)
        {
            DataUtility.Log("Reading eccentricity and degree file...");

            using (BufferedStream bs = new BufferedStream(File.OpenRead(fileName)))
            {
                using (StreamReader reader = new StreamReader(bs))
                {
                    int i = 0;

                    while (!reader.EndOfStream)
                    {
                        string data = reader.ReadLine();

                        string[] str = data.Split(',');

                        if (str.Length == 3)
                        {
                            int id = Convert.ToInt32(str[0]);
                            int ecc = Convert.ToInt32(str[1]);
                            int deg = Convert.ToInt32(str[2]);

                            Nodes.Find(n => n.ID == id).Eccentricity = ecc;
                            Nodes.Find(n => n.ID == id).Degree = deg;
                        }

                        i++;
                    }
                }
            }

            DataUtility.Log("Done.\n", false);
        }

        private void WriteEccentricityDegreeFile(string fileName)
        {
            DataUtility.Log("Writing eccentricity and degree file...");

            using (BufferedStream bs = new BufferedStream(File.OpenWrite(fileName)))
            {
                using (StreamWriter writer = new StreamWriter(bs))
                {
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        writer.Write(string.Format("{0},{1},{2}", Nodes[i].ID, Nodes[i].Eccentricity, Nodes[i].Degree));

                        if (i != Nodes.Count - 1)
                            writer.Write("\n");
                    }
                }
            }

            DataUtility.Log("Done.\n", false);
        }
    }
}
