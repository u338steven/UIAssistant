using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    public class KeyTranslator
    {
        public CommandType Type { get; set; }
        public Key[] InputKeys { get; set; }
        public Key[] OutputKeys { get; set; }
        public string CommandText { get; set; }
        public bool CanDefineOneShot { get; set; }
        public Key[] OneShot { get; set; }

        public KeyTranslator()
        {
            Type = CommandType.SwapKey;
            CommandText = null;
            CanDefineOneShot = false;
        }

        public KeyTranslator(Key[] inputKeys, Key[] outputKeys)
        {
            Type = CommandType.SwapKey;
            InputKeys = inputKeys;
            OutputKeys = outputKeys;
            CommandText = null;
            CanDefineOneShot = false;
        }

        public KeyTranslator(Key[] inputKeys, CommandType type, string commandName)
        {
            if (type == CommandType.SwapKey)
            {
                // broken settings data
            }
            Type = type;
            InputKeys = inputKeys;
            CommandText = commandName;
            CanDefineOneShot = false;
        }
    }
}
