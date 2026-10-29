using GoFPatternGuide.Exercises.Core;

namespace GoFPatternGuide.Exercises.Problems.SlaDeadlines;

/// <summary>
/// PRACTICE: SLA deadlines
///
/// Edit the STARTER CODE section below until every check passes.
/// Leave the CHECKS section unchanged: it is how the guide grades your work.
/// </summary>
public sealed class SlaDeadlinesExercise : IExercise
{
    public string Title => "SLA deadlines";

    public string Scenario => """
        HelpDesk Pro promises every customer a response deadline (an SLA). The
        deadline depends on the ticket priority and on the customer's contract.

        Today there are two contracts:
          Standard  Low 72h, Normal 24h, High 8h, Urgent 2h (counted around the clock)
          Gold      half of the Standard hours (counted around the clock)

        SlaCalculator.GetDeadline handles this with a switch on the contract name
        and another switch on the priority. It works, but every new contract adds
        another case to the same method, and the method is already hard to test.

        Sales just signed a Government contract. Government tickets use the
        Standard hours, but the clock only runs during business hours:
        Monday to Friday, 08:00 to 17:00. Two more contract types are planned
        for next quarter.
        """;

    public string Task => """
        1. Add the Government contract (business hours only).
        2. Restructure the code so each contract's deadline rule lives in its
           own class, and SlaCalculator picks the rule without a growing switch.
        3. Keep the Standard and Gold results exactly as they are today.

        Acceptance criteria:
        - All checks pass.
        - Adding a fourth contract means adding one new class and registering it,
          not editing the existing rules.
        """;

    public IReadOnlyList<string> Hints { get; } =
    [
        "Look at what varies (the deadline rule) and what stays the same (asking for a deadline).",
        "Define one small interface with a single method that returns a deadline for a priority and a creation time.",
        "SlaCalculator can keep a dictionary from contract name to rule object and delegate to it."
    ];

    public IReadOnlyList<string> SelfReview { get; } =
    [
        "Does SlaCalculator still change when a new contract is added? If so, how small is that change?",
        "Can you unit-test the business-hours rule without creating an SlaCalculator?",
        "Is the business-hours walking logic readable to a teammate who did not write it?"
    ];

    public void Check(ExerciseCheck check) =>
        SlaDeadlinesChecks.Run(check, (contract, priority, created) =>
            new SlaCalculator().GetDeadline(contract, priority, created));
}

// ============================== STARTER CODE ===============================
// Change anything in this section. Keep the public GetDeadline signature,
// because the checks call it.

public enum Priority
{
    Low,
    Normal,
    High,
    Urgent
}

public sealed class SlaCalculator
{
    public DateTime GetDeadline(string contract, Priority priority, DateTime created)
    {
        switch (contract)
        {
            case "Standard":
                switch (priority)
                {
                    case Priority.Low: return created.AddHours(72);
                    case Priority.Normal: return created.AddHours(24);
                    case Priority.High: return created.AddHours(8);
                    default: return created.AddHours(2);
                }

            case "Gold":
                switch (priority)
                {
                    case Priority.Low: return created.AddHours(36);
                    case Priority.Normal: return created.AddHours(12);
                    case Priority.High: return created.AddHours(4);
                    default: return created.AddHours(1);
                }

            case "Government":
                throw new NotImplementedException("Government contracts count business hours only.");

            default:
                throw new ArgumentException($"Unknown contract '{contract}'.", nameof(contract));
        }
    }
}

// ================================= CHECKS ==================================
// Do not edit below this line. The reference solution runs the same checks.

public static class SlaDeadlinesChecks
{
    public static void Run(ExerciseCheck check, Func<string, Priority, DateTime, DateTime> deadline)
    {
        // 2 January 2026 is a Friday.
        var fridayAfternoon = new DateTime(2026, 1, 2, 16, 0, 0);
        var saturdayMorning = new DateTime(2026, 1, 3, 10, 0, 0);
        var tuesdayEvening = new DateTime(2026, 1, 6, 18, 0, 0);
        var wednesdayMorning = new DateTime(2026, 1, 7, 10, 0, 0);

        check.Equal("Standard Low ticket gets 72 hours",
            new DateTime(2026, 1, 5, 16, 0, 0),
            () => deadline("Standard", Priority.Low, fridayAfternoon));
        check.Equal("Standard Urgent ticket gets 2 hours, even over the weekend",
            new DateTime(2026, 1, 2, 18, 0, 0),
            () => deadline("Standard", Priority.Urgent, fridayAfternoon));
        check.Equal("Gold Normal ticket gets 12 hours",
            new DateTime(2026, 1, 3, 4, 0, 0),
            () => deadline("Gold", Priority.Normal, fridayAfternoon));
        check.Equal("Gold Urgent ticket gets 1 hour",
            new DateTime(2026, 1, 2, 17, 0, 0),
            () => deadline("Gold", Priority.Urgent, fridayAfternoon));

        check.Equal("Government Urgent ticket inside business hours",
            new DateTime(2026, 1, 7, 12, 0, 0),
            () => deadline("Government", Priority.Urgent, wednesdayMorning));
        check.Equal("Government High ticket on Friday 16:00 continues on Monday",
            new DateTime(2026, 1, 5, 15, 0, 0),
            () => deadline("Government", Priority.High, fridayAfternoon));
        check.Equal("Government ticket created on Saturday starts Monday 08:00",
            new DateTime(2026, 1, 5, 10, 0, 0),
            () => deadline("Government", Priority.Urgent, saturdayMorning));
        check.Equal("Government Normal ticket created after hours spans three days",
            new DateTime(2026, 1, 9, 14, 0, 0),
            () => deadline("Government", Priority.Normal, tuesdayEvening));

        check.Throws<ArgumentException>("Unknown contracts are rejected",
            () => deadline("Platinum", Priority.Low, fridayAfternoon));
    }
}
