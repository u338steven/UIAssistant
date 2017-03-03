using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UIAssistant.Infrastructure.Commands
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

    public interface ICommandSyntax : IList<CommandRule>
    {
    }

    public interface IValidatable<T>
    {
        ValidationResult Validate(T target);
    }
}
