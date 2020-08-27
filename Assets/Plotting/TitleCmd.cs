using UnityEngine;
using TMPro;

namespace Plotter
{
    /// <summary>
    /// Sets the main title of a plot. Attach to a child game object below the main plot game object.
    /// </summary>
    public class TitleCmd : MonoBehaviour
    {
        public TextMeshPro mainLabel;

        public void HandleCommand(Command cmd)
        {
            Debug.Log("Handling a title command...");
            mainLabel.SetText(cmd.getString("mainTitle"));
        }

        /// <summary>
        /// Clears the label text.
        /// </summary>
        public void Clear(string plotTarget = "Plot")
        {
            mainLabel.SetText(plotTarget);
        }
    }
}