using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace Plotter
{
    /// <summary>
    /// This class acts as a container of a plotting command received via a TCPServer
    /// </summary>
    public class Command
    {
        // All command types that can be handled
        public enum CommandType
        {
            None,
            Points,
            Title,
            AxisLabels,
            Clear,
            Dbg
        }


        public CommandType commandType { get; set; }

        // There can be several simultaneous plots, the plot target specifies
        // the plot the command is directed to.
        public string plotTarget { get; set; }

        // Various commands can contain arrays of parameters, the following dictionaries
        // hold these values. The key in the dictionary is the parameter name.
        private Dictionary<string, float[]> floatVectors = new Dictionary<string, float[]>();

        private Dictionary<string, int[]> intVectors = new Dictionary<string, int[]>();

        private Dictionary<string, string> msgStrings = new Dictionary<string, string>();
        public Command()
        {
            plotTarget = "None";
            commandType = CommandType.None;
        }
        public Command Clone()
        {
            var copy = new Command();
            copy.commandType = commandType;
            copy.plotTarget = plotTarget;

            foreach (var kvp in floatVectors)
            {
                copy.floatVectors.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in intVectors)
            {
                copy.intVectors.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in msgStrings)
            {
                copy.msgStrings.Add(kvp.Key, kvp.Value);
            }

            return copy;
        }


        public int getFloatVectorLength(string name)
        {
            if (floatVectors.ContainsKey(name))
            {
                return floatVectors[name].Length;
            }

            return 0;
        }

        public int getIntVectorLength(string name)
        {
            if (intVectors.ContainsKey(name))
            {
                return intVectors[name].Length;
            }

            return 0;
        }

        public float[] getFloatVector(string name)
        {
            if (floatVectors.ContainsKey(name))
            {
                return floatVectors[name];
            }

            return null;
        }

        public void setFloatVector(string name, float[] vector)
        {
            if (!floatVectors.ContainsKey(name))
            {
                floatVectors.Add(name, vector);
            }
            else
            {
                floatVectors[name] = vector;
            }
        }

        public int[] getIntVector(string name)
        {
            if (intVectors.ContainsKey(name))
            {
                return intVectors[name];
            }

            return null;
        }

        public void setIntVector(string name, int[] vector)
        {
            if (!intVectors.ContainsKey(name))
            {
                intVectors.Add(name, vector);
            }
            else
            {
                intVectors[name] = vector;
            }
        }

        public string getString(string name)
        {
            if (msgStrings.ContainsKey(name))
            {
                return msgStrings[name];
            }

            return "";
        }

        public void setString(string name, string theString)
        {
            if (!msgStrings.ContainsKey(name))
            {
                msgStrings.Add(name, theString);
            }
            else
            {
                msgStrings[name] = theString;
            }
        }

        // Used for debugging and testing, prints the contents of this
        // object to the console.
        public void PrintDebugInfo()
        {
            Debug.Log("Plot " + plotTarget);
            Debug.Log("Cmd " + commandType);

            foreach (var item in floatVectors)
            {
                Debug.Log(item.Key + " " + item.Value);
                foreach (var val in item.Value)
                {
                    Debug.Log(val);
                }
            }

            foreach (var item in intVectors)
            {
                Debug.Log(item.Key + " ");
                foreach (var val in item.Value)
                {
                    Debug.Log(val);
                }
            }

            foreach (var item in msgStrings)
            {
                Debug.Log(item.Key + " " + item.Value);
            }
        }

    }
}