using GoFPatternGuide.Core;
using GoFPatternGuide.Exercises.Core;
using GoFPatternGuide.Exercises.Problems.SlaDeadlines;

namespace GoFPatternGuide.Solutions.Strategy;

/// <summary>
/// REFERENCE SOLUTION: SLA deadlines, solved with Strategy.
///
/// Each contract's deadline rule is a strategy object behind ISlaPolicy. The
/// calculator (the context) looks the strategy up by contract name and delegates.
/// </summary>
public sealed class StrategySolution : IExerciseSolution
{
    public string Pattern => "Strategy";

    public string Category => PatternCategories.Behavioral;

    public string WhyThisPattern => """
        The deadline rule is the part that varies; asking for a deadline is the
        part that stays the same. Strategy gives each rule its own class behind
        one interface, so the calculator no longer knows how any rule works.

        Gains:
        - A new contract is a new class plus one registration line.
        - Each rule can be unit-tested on its own.
        - The two policies that share a shape (Standard and Gold) reuse one
          class with different hours, which a switch statement could not do.

        Costs:
        - More types to navigate than one switch statement.
        - Someone still has to register the strategies; here that is a
          dictionary, in a real app it might be dependency injection.

        Not the answer: a bigger switch, or a base class with one subclass per
        contract that overrides a method. Inheritance would work, but Strategy
        lets the rule be swapped at runtime and composed from configuration.
        """;

    public void Check(ExerciseCheck check) =>
        SlaDeadlinesChecks.Run(check, (contract, priority, created) =>
            new SlaCalculator().GetDeadline(contract, priority, created));

    // STRATEGY
    private interface ISlaPolicy
    {
        DateTime GetDeadline(Priority priority, DateTime created);
    }

    // CONCRETE STRATEGY: the clock runs around the clock. Standard and Gold
    // differ only in their hours, so they share this class.
    private sealed class AroundTheClockPolicy(IReadOnlyDictionary<Priority, int> hours) : ISlaPolicy
    {
        public DateTime GetDeadline(Priority priority, DateTime created) =>
            created.AddHours(hours[priority]);
    }

    // CONCRETE STRATEGY: the clock only runs Monday-Friday, 08:00-17:00.
    private sealed class BusinessHoursPolicy(IReadOnlyDictionary<Priority, int> hours) : ISlaPolicy
    {
        private static readonly TimeSpan DayStart = TimeSpan.FromHours(8);
        private static readonly TimeSpan DayEnd = TimeSpan.FromHours(17);

        public DateTime GetDeadline(Priority priority, DateTime created)
        {
            var remaining = TimeSpan.FromHours(hours[priority]);
            var cursor = MoveToBusinessTime(created);

            while (true)
            {
                var available = cursor.Date + DayEnd - cursor;
                if (remaining <= available)
                {
                    return cursor + remaining;
                }

                remaining -= available;
                cursor = MoveToBusinessTime(cursor.Date.AddDays(1) + DayStart);
            }
        }

        // Moves a moment forward to the next minute the clock is running.
        private static DateTime MoveToBusinessTime(DateTime moment)
        {
            if (moment.TimeOfDay >= DayEnd)
            {
                moment = moment.Date.AddDays(1) + DayStart;
            }
            else if (moment.TimeOfDay < DayStart)
            {
                moment = moment.Date + DayStart;
            }

            while (moment.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                moment = moment.Date.AddDays(1) + DayStart;
            }

            return moment;
        }
    }

    // CONTEXT
    private sealed class SlaCalculator
    {
        private static readonly Dictionary<Priority, int> StandardHours = new()
        {
            [Priority.Low] = 72,
            [Priority.Normal] = 24,
            [Priority.High] = 8,
            [Priority.Urgent] = 2
        };

        private static readonly Dictionary<Priority, int> GoldHours =
            StandardHours.ToDictionary(pair => pair.Key, pair => pair.Value / 2);

        // Adding a contract means adding one line here and, at most, one class.
        private readonly Dictionary<string, ISlaPolicy> _policies = new()
        {
            ["Standard"] = new AroundTheClockPolicy(StandardHours),
            ["Gold"] = new AroundTheClockPolicy(GoldHours),
            ["Government"] = new BusinessHoursPolicy(StandardHours)
        };

        public DateTime GetDeadline(string contract, Priority priority, DateTime created)
        {
            if (!_policies.TryGetValue(contract, out var policy))
            {
                throw new ArgumentException($"Unknown contract '{contract}'.", nameof(contract));
            }

            return policy.GetDeadline(priority, created);
        }
    }
}
