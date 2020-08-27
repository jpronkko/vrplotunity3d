using UnityEngine;

namespace Plotter
{
    /// <summary>
    /// Directs different types of commands received from the TCPServer to do stuff in scripts attached
    /// to plot specific game objects including axis labeling and geometry generation.
    /// </summary>
    public class PlotHandler : MonoBehaviour
    {
        public string plotTarget = "Plot1";
        public PlotCmd Plotter;

        public TitleCmd Title;

        public AxisLabelCmd AxisLabels;

        public TCPServer TCPPlotServer;

        void CmdHandler(Command command)
        {
            Dbg.Log("PlotHandler got: " + command.commandType);
            switch (command.commandType)
            {
                case Command.CommandType.AxisLabels:
                    AxisLabels.HandleCommand(command);
                    break;
                case Command.CommandType.Title:
                    Title.HandleCommand(command);
                    break;
                case Command.CommandType.Points:
                    Plotter.HandleCommand(command);
                    break;
                case Command.CommandType.Clear:
                    ResetStuff();
                    break;
                case Command.CommandType.Dbg:
                    Plotter.HandleCommand(command);

                    break;
            }
        }

        // Clear stuff from the specific plot.
        void ResetStuff()
        {
            Title.Clear(plotTarget);
            AxisLabels.Clear();
            Plotter.Clear();
        }
        
        void Start()
        {
            TCPPlotServer.RegisterCmdHandler(plotTarget, CmdHandler);
            ResetStuff();
        }
    }
}