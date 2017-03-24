using System.Collections.Generic;

namespace UIAssistant.Interfaces.API
{
    public interface IHitaHintAPI
    {
        IEnumerable<string> GenerateHints(string hintKeys, int quantity);
    }
}