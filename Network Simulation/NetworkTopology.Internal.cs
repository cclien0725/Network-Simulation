using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Network_Simulation
{
    public partial class NetworkTopology
    {
        public enum NodeType { None, Normal, Attacker, Victim }
        public enum TracerType { None, Marking, Tunneling, Filtering }

        public class Node
        {
            public int ID { get; set; }
            public double Xpos { get; set; }
            public double Ypos { get; set; }
            public NodeType Type { get; set; }
            public TracerType Tracer { get; set; }

            public Node()
            {
                // Default value.
                Type = NodeType.None;
                Tracer = TracerType.None;
            }
        }

        public class Edge
        {
            public int Node1 { get; set; }
            public int Node2 { get; set; }
            public double Length { get; set; }
            public double Delay { get; set; }

            public Edge()
            {
                Node1 = -1;
                Node2 = -1;
                Length = -1;
                Delay = -1;
            }
        }

        //[Serializable]
        public class Adjacency// : ISerializable
        {
            public double Length { get; set; }
            public double Delay { get; set; }
            public int Predecessor { get; set; }

            public Adjacency()
            {
                Predecessor = -1;
            }

            public Adjacency(string String)
            {
                string[] data = String.Split(',');
                Predecessor = Convert.ToInt32(data[0]);
                Length = Convert.ToDouble(data[1]);
                Delay = Convert.ToDouble(data[2]);
            }

            public string GetString()
            {
                return string.Format("{0},{1},{2}", Predecessor, Length, Delay);
            }

            //public void GetObjectData(SerializationInfo info, StreamingContext context)
            //{
            //    info.AddValue("length", Length, typeof(double));
            //    info.AddValue("delay", Delay, typeof(double));
            //    info.AddValue("predecessor", Predecessor, typeof(int));
            //}

            //public Adjacency(SerializationInfo info, StreamingContext context)
            //{
            //    Length = (double)info.GetValue("length", typeof(double));
            //    Delay = (double)info.GetValue("delay", typeof(double));
            //    Predecessor = (int)info.GetValue("predecessor", typeof(int));
            //}
        }
    }
}
