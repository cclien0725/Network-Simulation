using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Network_Simulation
{
    public partial class NetworkTopology
    {
        private Brush DRAW_NORMAL_BRUSH = Brushes.DarkSlateGray;
        private Brush DRAW_MARKING_BRUSH = Brushes.LightCoral;
        private Brush DRAW_FILTERING_BRUSH = Brushes.DarkTurquoise;
        private Brush DRAW_TUNNELING_BRUSH = Brushes.Violet;
        private const string DRAW_FONT_FAMILY = "微軟正黑體";

        private const int ATTACK_PACKET_PER_SEC = 1;
        private const int NORMAL_PACKET_PER_SEC = 10;
        private const int NUMBER_OF_ATTACK_PACKET = 300;
        private const int NUMBER_OF_NORMAL_PACKET = 30;
        private const double PROBIBILITY_OF_PACKET_TUNNELING = 0.04;
        private const double PROBIBILITY_OF_PACKET_MARKING = 0.04;
    }
}
