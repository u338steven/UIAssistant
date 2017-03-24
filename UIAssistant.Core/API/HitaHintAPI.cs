using System.Collections.Generic;
using System.Linq;

using UIAssistant.Core.HitaHint;
using UIAssistant.Interfaces.API;

namespace UIAssistant.Core.API
{
    class HitaHintAPI : IHitaHintAPI
    {
        public IEnumerable<string> GenerateHints(string hintKeys, int quantity)
        {
            if (hintKeys.Contains('|'))
            {
                return AlternateHintGenerator.Generate(hintKeys, quantity);
            }
            return HintGenerator.Generate(hintKeys, quantity);
        }
    }
}
