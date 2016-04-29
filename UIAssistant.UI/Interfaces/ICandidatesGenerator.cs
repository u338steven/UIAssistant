using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UIAssistant.UI.Interfaces
{
    public interface ICandidatesGenerator
    {
        IEnumerable<string> GenerateCandidates(string input);
        ValidationResult Validate(string input);
    }
}
