using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Network_Simulation;
using Deployment_Simulation;

namespace Heterogenerous_Simulation_Console_Version
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = args[0];
            int TunnelingTracer = Convert.ToInt32(args[1]);
            int MarkingTracer = Convert.ToInt32(args[2]);
            int FilteringTracer = Convert.ToInt32(args[3]);
            double AttackNodes = Convert.ToDouble(args[4]);
            int VictimNodes = Convert.ToInt32(args[5]);
            int TotalPacket = Convert.ToInt32(args[6]);
            int PercentageOfAttackPacket = Convert.ToInt32(args[7]);
            int AttackPacketPerSecond = Convert.ToInt32(args[8]);
            int NormalPacketPerSecond = Convert.ToInt32(args[9]);
            double ProbabilityOfPacketTunneling = Convert.ToDouble(args[10]);
            double ProbabilityOfPackeMarking = Convert.ToDouble(args[11]);
            double StartFiltering = Convert.ToDouble(args[12]);
            int InitTimeOfAttackPacket = Convert.ToInt32(args[13]);
            bool DynamicProbability = Convert.ToBoolean(args[14]);
            bool ConsiderDistance = Convert.ToBoolean(args[15]);
            double PercentageOfTracer = Convert.ToDouble(args[16]);
            string methodName = args[17];

            string dbName = string.Format("{0}_T{1}M{2}F{3}_A{4}V{5}_Pkt{6}_{7}", Path.GetFileNameWithoutExtension(filename),
                                                                               TunnelingTracer,
                                                                               MarkingTracer,
                                                                               FilteringTracer,
                                                                               AttackNodes,
                                                                               VictimNodes,
                                                                               TotalPacket,
                                                                               PercentageOfAttackPacket);


            filename = Path.Combine(Environment.CurrentDirectory, "maps", filename);

            SQLiteUtility sql = new SQLiteUtility(ref dbName);
            NetworkTopology networkTopology = new NetworkTopology(AttackNodes, VictimNodes);
            networkTopology.ReadBriteFile(filename);

            NoneDeployment noneDeploy = new NoneDeployment(0, 0, 0);
            if (sql.CreateTable(noneDeploy.ToString()))
            {
                noneDeploy.Deploy(networkTopology);
                Simulator noneSimulator = new Simulator(noneDeploy, networkTopology, sql, "None");
                noneSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
            }
            else 
            {
                // Load Attackers and Victim.
                sql.LoadAttackersAndVictim(ref networkTopology);
            }

            RandomDeployment randomDeploy;
            KCutDeployment kcutDeploy;
            KCutDeploymentV2 kcut2Deploy;

            switch (methodName) 
            {
                case "RandomDeployment":
                    randomDeploy = new RandomDeployment(TunnelingTracer * PercentageOfTracer / 100, MarkingTracer * PercentageOfTracer / 100, FilteringTracer * PercentageOfTracer / 100);
                    if (sql.CreateTable(randomDeploy.ToString()))
                    {
                        randomDeploy.Deploy(networkTopology);
                        if (MarkingTracer > 0)
                        {
                            MarkingAssistanceSimulator markSimulator = new MarkingAssistanceSimulator(randomDeploy, networkTopology, sql, "Random");
                            markSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                        else
                        {
                            Simulator randomSimulator = new Simulator(randomDeploy, networkTopology, sql, "Random");
                            randomSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                    }
                    break;

                case "KCutDeployment":
                    kcutDeploy = new KCutDeployment(TunnelingTracer * PercentageOfTracer / 100, MarkingTracer * PercentageOfTracer / 100, FilteringTracer * PercentageOfTracer / 100, typeof(KCutStartWithSideNodeConsiderCoefficient));
                    if (sql.CreateTable("KCutDeployV1")) 
                    {
                        kcutDeploy.Deploy(networkTopology);
                        if (MarkingTracer > 0)
                        {
                            MarkingAssistanceSimulator markSimulator = new MarkingAssistanceSimulator(kcutDeploy.Deployment, networkTopology, sql, "Random");
                            markSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                        else
                        {
                            Simulator kcutSimulator = new Simulator(kcutDeploy.Deployment, networkTopology, sql, "V1");
                            kcutSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                    }
                    break;

                case "KCutDeployment2":
                    kcut2Deploy = new KCutDeploymentV2(TunnelingTracer * PercentageOfTracer / 100, MarkingTracer * PercentageOfTracer / 100, FilteringTracer * PercentageOfTracer / 100, typeof(KCutStartWithSideNodeConsiderCoefficient));
                    if (sql.CreateTable("KCutDeployV2"))
                    {
                        kcut2Deploy.Deploy(networkTopology);
                        if (MarkingTracer > 0)
                        {
                            MarkingAssistanceSimulator markSimulator = new MarkingAssistanceSimulator(kcut2Deploy.Deployment, networkTopology, sql, "Random");
                            markSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                        else
                        {
                            Simulator kcut2Simulator = new Simulator(kcut2Deploy.Deployment, networkTopology, sql, "V2");
                            kcut2Simulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                    }
                    break;

                case "OPTRandomDeployment":
                    // Optimal Random
                    randomDeploy = new RandomDeployment(TunnelingTracer * PercentageOfTracer / 100, MarkingTracer * PercentageOfTracer / 100, FilteringTracer * PercentageOfTracer / 100);
                    if (sql.CreateTable("OPTRandomDeployment"))
                    {
                        randomDeploy.Deploy(networkTopology);
                        if (MarkingTracer > 0)
                        {
                            OptMarkingAssistanceSimulator markSimulator = new OptMarkingAssistanceSimulator(randomDeploy, networkTopology, sql, "Random");
                            markSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                        else
                        {
                            OptSimulator optRandomSimulator = new OptSimulator(randomDeploy, networkTopology, sql, "Random");
                            optRandomSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                    }
                    break;
                case "OPTKCutDeployment":
                    // Optimal KCutV1
                    kcutDeploy = new KCutDeployment(TunnelingTracer * PercentageOfTracer / 100, MarkingTracer * PercentageOfTracer / 100, FilteringTracer * PercentageOfTracer / 100, typeof(KCutStartWithSideNodeConsiderCoefficient));
                    if (sql.CreateTable("OPTKCutDeployV1"))
                    {
                        kcutDeploy.Deploy(networkTopology);
                        if (MarkingTracer > 0)
                        {
                            OptMarkingAssistanceSimulator markSimulator = new OptMarkingAssistanceSimulator(kcutDeploy.Deployment, networkTopology, sql, "Random");
                            markSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                        else
                        {
                            OptSimulator optKCutSimulator = new OptSimulator(kcutDeploy.Deployment, networkTopology, sql, "V1");
                            optKCutSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                    }
                    break;
                case "OPTKCutDeploymentV2":
                    // Optimal KCutV2
                    kcut2Deploy = new KCutDeploymentV2(TunnelingTracer * PercentageOfTracer / 100, MarkingTracer * PercentageOfTracer / 100, FilteringTracer * PercentageOfTracer / 100, typeof(KCutStartWithSideNodeConsiderCoefficient));
                    if (sql.CreateTable("OPTKCutDeployV2"))
                    {
                        kcut2Deploy.Deploy(networkTopology);
                        if (MarkingTracer > 0)
                        {
                            OptMarkingAssistanceSimulator markSimulator = new OptMarkingAssistanceSimulator(kcut2Deploy.Deployment, networkTopology, sql, "Random");
                            markSimulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                        else
                        {
                            OptSimulator kcut2Simulator = new OptSimulator(kcut2Deploy.Deployment, networkTopology, sql, "V2");
                            kcut2Simulator.Run(AttackPacketPerSecond, NormalPacketPerSecond, TotalPacket, PercentageOfAttackPacket, ProbabilityOfPacketTunneling, ProbabilityOfPackeMarking, StartFiltering, InitTimeOfAttackPacket, DynamicProbability, ConsiderDistance);
                        }
                    }
                    break;
            }
        }
    }
}
