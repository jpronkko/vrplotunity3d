using UnityEngine;
using TMPro;

namespace Plotter
{
    /// <summary>
    /// Sets title texts for plotting axis (x,y,z). Attach to a child game object below the main plot game object.
    /// </summary>
    public class AxisLabelCmd : MonoBehaviour
    {
        public TextMeshPro xAxisLabel;
        public TextMeshPro yAxisLabel;
        public TextMeshPro zAxisLabel;
        // Start is called before the first frame update

        public void HandleCommand(Command cmd)
        {
            Debug.Log("Handling an axis title command...");
            xAxisLabel.SetText(cmd.getString("xAxisTitle"));
            yAxisLabel.SetText(cmd.getString("yAxisTitle"));
            zAxisLabel.SetText(cmd.getString("zAxisTitle"));
        }

        /// <summary>
        /// Clears all axis label texts.
        /// </summary>
        public void Clear()
        {
            xAxisLabel.SetText("X");
            yAxisLabel.SetText("Y");
            zAxisLabel.SetText("Z");
        }
    }
}