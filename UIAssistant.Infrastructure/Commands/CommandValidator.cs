using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;

using UIAssistant.Infrastructure.Resource;

namespace UIAssistant.Infrastructure.Commands
{
    public class CommandValidator : IValidatable<string>
    {
        private IParser _parser;
        private ILocalizer _localizer;

        public CommandValidator(IParser parser, ILocalizer localizer)
        {
            Contract.Requires(parser != null);
            Contract.Requires(localizer != null);

            _parser = parser;
            _localizer = localizer;
        }

        const string InvalidCommand = "invalidCommand";
        public ValidationResult Validate(string command)
        {
            try
            {
                var tokens = _parser.Parse(command);
                if (tokens.Count() > 0)
                {
                    return ValidationResult.Success;
                }
            }
            catch (Exception ex)
            {
                return new ValidationResult(string.Format(_localizer.GetLocalizedText(InvalidCommand), ex.Message));
            }
            return ValidationResult.Success;
        }
    }
}
