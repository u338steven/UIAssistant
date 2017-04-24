using System;
using System.Linq;

namespace UIAssistant.Plugin.KeybindsManiacs
{
    enum Mode : int
    {
        Normal,
        Insert,
        Visual,
        Search,
        Command,
        UserCustom,
        Unknown,
    }

    static class ModeExtensions
    {
        public static bool EqualsName(this Mode mode, string modeName)
        {
            return Enum.GetNames(typeof(Mode)).FirstOrDefault(x => x == modeName) != null;
        }

        public static Mode GetMode(string modeName)
        {
            var names = Enum.GetNames(typeof(Mode));
            var values = Enum.GetValues(typeof(Mode)).Cast<Mode>();
            if (!values.Any(x => x.EqualsName(modeName)))
            {
                return Mode.Unknown;
            }
            return Enum.GetValues(typeof(Mode)).Cast<Mode>().FirstOrDefault(x => Enum.GetName(typeof(Mode), x) == modeName);
        }
    }
}
