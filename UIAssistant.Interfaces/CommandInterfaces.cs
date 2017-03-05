using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UIAssistant.Interfaces.Commands
{
    public interface ICandidate
    {
        string Name { get; }
        string Description { get; }
    }

    public interface ICandidatesGenerator
    {
        IEnumerable<ICandidate> GenerateCandidates(string word);
    }

    public interface ICommand
    {
        string Name { get; }
        string Value { get; }
        void Invoke();
    }

    public interface IParser
    {
        IEnumerable<ICommand> Parse(string statement);
    }

    public interface ICommandSyntax : IList<ICommandRule>
    {
    }

    public interface ICommandRule : IRule
    {
    }

    public interface IArgumentRule : IRule
    {

    }

    public interface IRule : ICandidate
    {
        //IRule Empty { get; }
        //string Name { get; }
        new string Description { get; set; }
        Action<ICommand> Action { get; }

        ICollection<IArgumentRule> RequiredArgs { get; }
        ICollection<IArgumentRule> OptionalArgs { get; }
    }

    public interface IValidatable<T>
    {
        ValidationResult Validate(T target);
    }

    public static partial class CommandExtensions
    {
        public static bool IsSuccess(this ValidationResult result)
        {
            return result == ValidationResult.Success;
        }
    }
}
