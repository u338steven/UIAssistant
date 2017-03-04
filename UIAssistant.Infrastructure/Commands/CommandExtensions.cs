using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;

namespace UIAssistant.Infrastructure.Commands
{
    public static class CommandExtensions
    {
        public static bool ContainsWord(this IEnumerable<BaseRule> rules, string word)
        {
            Contract.Requires(rules != null);

            return rules.Any(x => x.Name.EqualsWithCaseIgnored(word));
        }

        public static IEnumerable<BaseRule> FindApplyingRules(this BaseRule rule, IEnumerable<string> args)
        {
            Contract.Requires(rule != null);

            var results = new List<BaseRule>() { rule };
            if (args.Count() == 0)
            {
                return results;
            }

            var arguments = rule.RequiredArgs;
            foreach (var token in args)
            {
                var next = arguments.FindRule(token);
                if (next.IsNullOrEmpty())
                {
                    continue;
                }
                results.Add(next);
                arguments = next.RequiredArgs;
            }
            return results;
        }

        public static BaseRule FindRule(this IEnumerable<BaseRule> rules, string word)
        {
            Contract.Requires(rules != null);
            Contract.Ensures(Contract.Result<BaseRule>() != null);

            return rules.FirstOrDefault(x => x.Name.EqualsWithCaseIgnored(word)) ?? BaseRule.Empty;
        }

        public static IEnumerable<ICandidate> GetCandidates(this IEnumerable<BaseRule> rules, string word)
        {
            Contract.Requires(rules != null);
            Contract.Ensures(Contract.Result<IEnumerable<ICandidate>>() != null);

            return rules.Where(x => x.Name.StartsWithCaseIgnored(word) && !x.Name.EqualsWithCaseIgnored(word));
        }

        public static bool IsNullOrEmpty(this BaseRule rule)
        {
            return rule == null || rule == BaseRule.Empty;
        }

        public static bool IsSuccess(this ValidationResult result)
        {
            return result == ValidationResult.Success;
        }
    }
}