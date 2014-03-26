using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace Network_Simulation
{
	public partial class NetworkTopology
	{
        /// <summary>
        /// Translating to the index number on our structure with specific node ID.
        /// </summary>
        /// <param name="NodeID">The node ID that want to translating.</param>
        /// <returns>The index on our structure.</returns>
        public int NodeID2Index(int NodeID)
        {
            if (Nodes.Count == 0)
                throw new Exception("NodeID2Index() Fail: There is 0 node on the network.");
            
            if (!Nodes.Exists(x => x.ID == NodeID))
                throw new Exception(string.Format("NodeID2Index() Fail: There is no match NodeID: {0} on our structure.", NodeID));

            return Nodes.FindIndex(x => x.ID == NodeID);
        }

        /// <summary>
        /// Translating to the node ID with specific index number on our structure.
        /// </summary>
        /// <param name="NodeIndex">The index number.</param>
        /// <returns>The node ID.</returns>
        public int NodeIndex2ID(int NodeIndex)
        {
            if (Nodes.Count == 0)
                throw new Exception("NodeIndex2ID() Fail: There is 0 node on the network.");

            if (NodeIndex >= Nodes.Count || NodeIndex < 0)
                throw new Exception(string.Format("NodeIndex2ID() Fail: Out of range on our structure of the index: {0}.", NodeIndex));

            return Nodes[NodeIndex].ID;
        }

        /// <summary>
        /// Using Floyd-Warshar algorithm computing shortest path.
        /// </summary>
        public void ComputingShortestPath()
        {
            Console.WriteLine("Computing shortest path...");

            // Create the space of adjacent matrix
            AdjacentMatrix = null;
            AdjacentMatrix = new Adjacency[Nodes.Count, Nodes.Count];

            foreach (var edge in Edges)
            {
                AdjacentMatrix[NodeID2Index(edge.Node1), NodeID2Index(edge.Node2)] = new Adjacency() { Delay = edge.Delay, Length = edge.Length };
                AdjacentMatrix[NodeID2Index(edge.Node2), NodeID2Index(edge.Node1)] = new Adjacency() { Delay = edge.Delay, Length = edge.Length };
            }

            // Floyd-Warshall algorithm
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    for (int k = j + 1; k < Nodes.Count; k++)
                    {
                        if ((AdjacentMatrix[j, k] == null && AdjacentMatrix[j, i] != null && AdjacentMatrix[i, k] != null) ||
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

            List<int> path = new List<int>() { NodeID2Index(SourceNodeId), NodeID2Index(DestinationNodeId) };

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
                path[i] = NodeIndex2ID(path[i]);

            return path;
        }

        /// <summary>
        /// Return the eccentricity value of the specific node ID on the network topology.
        /// </summary>
        /// <param name="NodeID">The node ID on the network topology.</param>
        /// <returns>The eccentricity value for the node ID on the network topology.</returns>
        public double Eccentricity(int NodeID)
        {
            double result = double.MinValue;
            int nowCount;

            //for (int i = 0; i < Nodes.Count; i++)
            //    if (AdjacentMatrix[NodeID2Index(NodeID), i] != null &&
            //        result < AdjacentMatrix[NodeID2Index(NodeID), i].Length)
            //        result = AdjacentMatrix[NodeID2Index(NodeID), i].Length;

            foreach (int id in Nodes.Where(n => n.ID != NodeID).Select(n => n.ID))
            {
                nowCount = Path(NodeID, id).Count;

                if (result < nowCount)
                    result = nowCount;
            }

            return result;
        }

        /// <summary>
        /// Retrun the degree value of the specific node ID on the network topology.
        /// </summary>
        /// <param name="NodeID">The node ID on the network topology.</param>
        /// <returns>The degree value for the node ID on the network topology.</returns>
        public int Degree(int NodeID)
        {
            return Edges.Where(n => n.Node1 == NodeID || n.Node2 == NodeID).ToList().Count;
        }

        /// <summary>
        /// Computing the clustering coefficeint of the node id on the network topology.
        /// </summary>
        /// <param name="NodeID">The node id on the network topology.</param>
        /// <returns>The clustering coefficeint value.</returns>
        public double ClusteringCoefficeint(int NodeID)
        {
            List<int> neighbor = GetNeighborNodeIDs(NodeID);
            List<Edge> neighbor_edge_connect_set = new List<Edge>();
            
            for (int i = 0; i < neighbor.Count; i++)
            {
                List<Edge> tmp = new List<Edge>(Edges.Where(e => e.Node1 == neighbor[i] || e.Node2 == neighbor[i]));

                for (int j = i + 1; j < neighbor.Count; j++)
                    neighbor_edge_connect_set.AddRange(tmp.Where(e => e.Node1 == neighbor[j] || e.Node2 == neighbor[j]));
            }

            double max_edges = (neighbor.Count * (neighbor.Count - 1)) / 2;
            double neighbor_edge_count = neighbor_edge_connect_set.Count;

            if (max_edges > 0)
                return neighbor_edge_count / max_edges;
            else
                return 0;
        }

        /// <summary>
        /// Getting the neighbor node IDs list for the specific node ID on the network topology.
        /// </summary>
        /// <param name="NodeID">The node Id on the network topology.</param>
        /// <returns>The list of neighbors ID which side on the specific node ID.</returns>
        public List<int> GetNeighborNodeIDs(int NodeID)
        {
            List<int> result = Edges.Where(n => n.Node1 == NodeID).Select(n => n.Node2).ToList();

            result.AddRange(Edges.Where(n => n.Node2 == NodeID).Select(n => n.Node1).ToList());

            return result;
        }

        /// <summary>
        /// Finding the center of node id on the network topology.
        /// </summary>
        /// <param name="centerID">The center node id will output.</param>
        /// <returns>Do the network topology can find the center node id.</returns>
        public bool FindCenterNodeID(out int centerID)
        {
            double minE = int.MaxValue;
            double eccentricity;
            centerID = -1;

            foreach (var item in Nodes)
            {
                eccentricity = Eccentricity(item.ID);

                if (minE > eccentricity && eccentricity != double.MinValue)
                {
                    minE = eccentricity;
                    centerID = item.ID;
                }
                else if (minE == eccentricity)
                    if (Degree(centerID) > Degree(item.ID))
                        centerID = item.ID;
            }

            return centerID != -1;
        }

        /// <summary>
        /// Operator overloading(-, minus) that define the complement set between the two network topologies.
        /// </summary>
        /// <param name="left_n">The minuend of the network topology.</param>
        /// <param name="right_n">The subtrahend of the network topology.</param>
        /// <returns>The complement set between the two network topologies.</returns>
        public static NetworkTopology operator -(NetworkTopology left_n, NetworkTopology right_n)
        {
            NetworkTopology result = new NetworkTopology(left_n.percentageOfAttackers, left_n.percentageOfNormalUser, left_n.numberOfVictims);

            result.Nodes = left_n.Nodes.Except(right_n.Nodes).ToList();
            result.Edges = left_n.Edges.Except(right_n.Edges).ToList();
            foreach (var n in right_n.Nodes)
                result.Edges.RemoveAll(e => e.Node1 == n.ID || e.Node2 == n.ID);

            //if (result.Nodes.Count > 0)
            //{
            //    result.Initialize();
            //    result.ComputingShortestPath();
            //}

            return result;
        }

        /// <summary>
        /// Setting up the drawing control that draw network topology on the specific control.
        /// </summary>
        /// <param name="ctrl">The control to draw the network.</param>
        public void SetupDrawingControl(Control ctrl)
        {
            if (m_is_setup_control)
                throw new Exception("SetupDrawingControl() Fail: The control have been setted.");

            m_is_setup_control = true;

            // Changing the cursor.
            ctrl.Cursor = Cursors.NoMove2D;

            // Enabling DoubleBuffered.
            typeof(Control).InvokeMember("DoubleBuffered",
                                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                                null, ctrl, new object[] { true });

            // Handling the MouseWheel event to zoom in or zoom out network topology.
            ctrl.MouseWheel += (s, e) =>
            {
                // zoom in
                if (e.Delta > 0)
                {
                    m_scale_x += 0.1f;
                    m_scale_y += 0.1f;
                    (s as Control).Invalidate();
                }
                // zoom out
                else
                {
                    m_scale_x = (m_scale_x - 0.1f) > 0.1 ? (m_scale_x - 0.1f) : 0.1f;
                    m_scale_y = (m_scale_y - 0.1f) > 0.1 ? (m_scale_y - 0.1f) : 0.1f;
                    (s as Control).Invalidate();
                }
            };

            // Handling the MouseEnter event to focus the specific control.
            ctrl.MouseEnter += (s, e) =>
            {
                (s as Control).Focus();
            };

            // Handling the MouseLeave event to lost focus the specific control.
            ctrl.MouseLeave += (s, e) =>
            {
                (s as Control).Focus();
            };

            // Handling the MouseDown event to move the network topology.
            ctrl.MouseDown += (s, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    m_pre_move_x = e.Location.X;
                    m_pre_move_y = e.Location.Y;
                    m_is_mouse_down = true;
                }
            };

            // Handling the MouseUp event to move the network topology.
            ctrl.MouseUp += (s, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    m_move_x += e.Location.X - m_pre_move_x;
                    m_move_y += e.Location.Y - m_pre_move_y;
                    m_pre_move_x = e.Location.X;
                    m_pre_move_y = e.Location.Y;
                    (s as Control).Invalidate();
                    m_is_mouse_down = false;
                }
            };

            // Handling the MouseMove event to move the network topology.
            ctrl.MouseMove += (s, e) =>
            {
                if (m_is_mouse_down)
                {
                    m_move_x += e.Location.X - m_pre_move_x;
                    m_move_y += e.Location.Y - m_pre_move_y;
                    m_pre_move_x = e.Location.X;
                    m_pre_move_y = e.Location.Y;
                    (s as Control).Invalidate();
                }
            };

            // Handling the Resize event to redraw the network topology.
            ctrl.Resize += (s, e) =>
            {
                (s as Control).Invalidate();
            };

            // Handling the Paint event to pain the network topology.
            ctrl.Paint += (s, e) =>
            {
                // Using the BufferedGraphics to draw the network topology.
                using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(e.Graphics, e.ClipRectangle))
                {
                    bg.Graphics.Clear(Color.Gray);
                    bg.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                    // Setting up the transform to move or scale the network topology.
                    bg.Graphics.TranslateTransform(m_move_x, m_move_y);
                    bg.Graphics.ScaleTransform(m_scale_x, m_scale_y);

                    // Drawing the edge of the network topology.
                    foreach (var edge in Edges)
                        drawEdge(bg.Graphics, edge);

                    // Drawing the node of the network topology.
                    foreach (var node in Nodes)
                        drawNode(bg.Graphics, node);

                    // Drawing the demonstration of the symbol.
                    bg.Graphics.ResetTransform();
                    bg.Graphics.ScaleTransform(0.8f, 0.8f);
                    drawDemonstration(bg.Graphics);

                    // Starting render the graph to specific graphics.
                    bg.Render();
                }
            };
        }

        /// <summary>
        /// Drawing node of the network topology.
        /// </summary>
        /// <param name="graph">The graphic to draw on.</param>
        /// <param name="node">The node to draw on.</param>
        private void drawNode(Graphics graph, Node node)
        {
            float x = (float)node.Xpos;
            float y = (float)node.Ypos;
            string str;
            Brush brush;

            switch (node.Tracer)
            {
                case TracerType.None:
                    brush = DRAW_NORMAL_BRUSH;
                    break;
                case TracerType.Filtering:
                    brush = DRAW_FILTERING_BRUSH;
                    break;
                case TracerType.Marking:
                    brush = DRAW_MARKING_BRUSH;
                    break;
                case TracerType.Tunneling:
                    brush = DRAW_TUNNELING_BRUSH;
                    break;
                default:
                    brush = Brushes.White;
                    break;
            }

            switch (node.Type)
            {
                case NodeType.Normal:
#if DEBUG
                    str = node.ID.ToString();
#else
                    str = string.Empty;
#endif
                    break;
                case NodeType.Attacker:
                    str = "A";
                    break;
                case NodeType.Victim:
                    str = "V";
                    break;
                default:
                    str = string.Empty;
                    break;
            }

            graph.FillEllipse(brush, x, y, 30, 30);
#if DEBUG
            graph.DrawString(str, new Font(DRAW_FONT_FAMILY, 12), Brushes.White, x + 15 - 11, y + 15 - 11);
#else
            graph.DrawString(str, new Font(DRAW_FONT_FAMILY, 12), Brushes.White, x + 15 - 8, y + 15 - 11);
#endif
        }

        /// <summary>
        /// Drawing edge of the network topology.
        /// </summary>
        /// <param name="graph">The graphic to draw on.</param>
        /// <param name="edge">The edge to draw on.</param>
        private void drawEdge(Graphics graph, Edge edge)
        {
            Node node1 = Nodes[NodeID2Index(edge.Node1)];
            Node node2 = Nodes[NodeID2Index(edge.Node2)];

            graph.DrawLine(Pens.Black, (float)node1.Xpos + 15, (float)node1.Ypos + 15, (float)node2.Xpos + 15, (float)node2.Ypos + 15);
        }

        /// <summary>
        /// Drawing demonstration of the symbol on the network topology.
        /// </summary>
        /// <param name="graph">The graphic to draw on.</param>
        private void drawDemonstration(Graphics graph)
        {
            float width = 200;
            float height = 220;
            float left_top_x = graph.VisibleClipBounds.Width - width;
            float left_top_y = graph.VisibleClipBounds.Height - height;
            float margin = 10;
            float padding = 10;
            float now_row_height = 0;
            Font font = new Font(DRAW_FONT_FAMILY, 12);

            graph.FillRectangle(Brushes.DimGray, left_top_x, left_top_y, width - margin, height - margin);

            graph.FillEllipse(DRAW_NORMAL_BRUSH, left_top_x + padding, left_top_y + padding + now_row_height, 30, 30);
            graph.DrawString("Normal Node", font, Brushes.White, left_top_x + padding + 40 + 15 - 11, left_top_y + padding + now_row_height + 15 - 11);

            now_row_height += 40;
            graph.FillEllipse(DRAW_FILTERING_BRUSH, left_top_x + padding, left_top_y + padding + now_row_height, 30, 30);
            graph.DrawString("Filtering Node", font, Brushes.White, left_top_x + padding + 40 + 15 - 11, left_top_y + padding + now_row_height + 15 - 11);

            now_row_height += 40;
            graph.FillEllipse(DRAW_MARKING_BRUSH, left_top_x + padding, left_top_y + padding + now_row_height, 30, 30);
            graph.DrawString("Marking Node", font, Brushes.White, left_top_x + padding + 40 + 15 - 11, left_top_y + padding + now_row_height + 15 - 11);

            now_row_height += 40;
            graph.FillEllipse(DRAW_TUNNELING_BRUSH, left_top_x + padding, left_top_y + padding + now_row_height, 30, 30);
            graph.DrawString("Tunneling Node", font, Brushes.White, left_top_x + padding + 40 + 15 - 11, left_top_y + padding + now_row_height + 15 - 11);

            now_row_height += 40;
            graph.DrawString("V: Victim; A: Attacker", font, Brushes.White, left_top_x + padding + 15 - 11, left_top_y + padding + now_row_height + 15 - 11);
        }

        /// Return the poisson random number.
        /// </summary>
        /// <param name="lambda">Lambda.</param>
        /// <returns>Random number.</returns>
        private long Poisson(double lambda)
        {
            Random rd = new Random();
            long k = 0;
            double L = Math.Exp(-lambda), p = 1;
            double r;

            do
            {
                ++k;
                r = rd.NextDouble();
                p *= r;
            } while (p > L);

            return --k;
        }
	}
}
