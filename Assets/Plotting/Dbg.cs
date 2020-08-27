using TMPro;
using UnityEngine;

namespace Plotter
{
    /// <summary>
    /// Prints debug messages within the simulation. 
    /// </summary>
    public class Dbg : MonoBehaviour
    {
        /// <summary>
        /// The title text to print the message to
        /// </summary>
        public TextMeshPro text;

        /// <summary>
        /// A global handle to this object so that one can access the
        /// functionality globally.
        /// </summary>
        static private Dbg thisHandle;
        static public void Log(string msg)
        {
            thisHandle.PrintMsg(msg);
        }

        /// <summary>
        /// Shows the message in the simulation
        /// </summary>
        /// <param name="msg">Message to be shown</param>
        private void PrintMsg(string msg)
        {
            text.text += "\n" + msg;
            Debug.Log(msg);
        }
        private void Awake()
        {
            thisHandle = this;
        }
    }
}