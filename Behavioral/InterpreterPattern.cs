using GoFPatternGuide.Core;

namespace GoFPatternGuide.Behavioral;

/// <summary>
/// INTERPRETER
///
/// Intent: represent a small language as a tree of objects and evaluate it by
/// asking each node to interpret itself.
///
/// The demo builds a tiny arithmetic language with numbers, addition, and
/// subtraction. The expression tree is assembled directly in code.
/// </summary>
public sealed class InterpreterPattern : IPatternDemo
{
    public string Name => "Interpreter";

    public string Category => PatternCategories.Behavioral;

    public string Summary =>
        "Models a small language as an object tree and evaluates it node by node.";

    public string Explanation => """
        WHAT IT IS
        Interpreter is a behavioral pattern that defines a representation for a
        small language and an interpreter that evaluates sentences in that
        language. Each grammar rule becomes a class, and the whole expression is
        represented as a tree.

        In the demo, the language has three rules:
        - a number evaluates to its numeric value;
        - an addition evaluates its left and right parts and adds them;
        - a subtraction evaluates its left and right parts and subtracts them.

        WHY IT EXISTS
        Some problems can be described with a compact set of rules: arithmetic
        expressions, boolean filters, simple search queries, configuration rules,
        or a mini command language.

        Writing a full parser and compiler is often too much. Interpreter gives
        us a clear object model for a small grammar and makes each rule easy to
        inspect and extend.

        THE PROBLEM IT SOLVES
        A long if/else evaluator quickly becomes difficult to read. Every operator
        and every combination of operands is handled in one method, and adding a
        new rule means editing that method again.

        With Interpreter, each rule knows how to evaluate itself. The program
        composes rule objects into a tree. Evaluation starts at the root and
        recursively asks the children for their values.

        WHEN TO USE IT
        - The grammar is small and stable.
        - Expressions can be represented naturally as a tree.
        - Efficiency is less important than clarity and extensibility.
        - You need to evaluate, print, or transform simple rules repeatedly.
        - You want each grammar rule isolated in its own class.

        ADVANTAGES
        - Makes a small grammar explicit and readable.
        - Each grammar rule has a focused class.
        - Adding a rule is usually straightforward.
        - The expression is a normal object tree that can be stored or traversed.
        - Evaluation, formatting, and other operations can be added cleanly.

        DISADVANTAGES
        - Class count grows quickly as the grammar grows.
        - Complex grammars become hard to maintain.
        - A hand-written parser is often still needed to turn text into a tree.
        - Performance may be poor for very large expressions.
        - The pattern is easy to overuse for languages that need a real parser,
          compiler, or existing expression library.

        REAL-WORLD EXAMPLES
        - Arithmetic or spreadsheet formulas.
        - Search filters such as "price < 100 AND inStock = true".
        - Simple rule engines and business-rule evaluation.
        - Regular expression engines and query languages.
        - Music notation or command mini-languages.
        - Configuration expressions that decide whether a feature is enabled.

        HOW THE DEMO WORKS
        1. IExpression is the abstract expression. It exposes Evaluate and
           Describe.
        2. NumberExpression is a terminal expression. It has no children.
        3. AddExpression and SubtractExpression are nonterminal expressions. Each
           holds a left and right expression.
        4. The demo builds (10 + 5) - 3 as a tree.
        5. Evaluate recursively calculates 12, and Describe prints the structure.

        WHERE IS THE PARSER?
        This demo intentionally builds the tree in code. Teaching both a parser and
        the pattern at once would hide the core idea. In a real application, a
        parser would read text such as "(10 + 5) - 3" and construct the same tree.
        If the language starts to grow operators, precedence, variables, and error
        messages, consider a parser generator or an existing expression library.

        A GOOD RULE OF THUMB
        Use Interpreter for a small, stable grammar where an object tree is clearer
        than a large evaluation method. Do not use it to build a general-purpose
        programming language.
        """;

    public void Run()
    {
        Console.WriteLine("Building a tiny arithmetic expression tree...\n");

        // This tree represents: (10 + 5) - 3
        //
        //              Subtract
        //             /        \
        //          Add          3
        //         /   \
        //       10     5
        IExpression expression = new SubtractExpression(
            new AddExpression(
                new NumberExpression(10),
                new NumberExpression(5)),
            new NumberExpression(3));

        Console.WriteLine($"Expression: {expression.Describe()}");
        Console.WriteLine($"Result:     {expression.Evaluate()}");

        Console.WriteLine();
        Console.WriteLine("The same tree can also be evaluated after changing a leaf:");

        IExpression another = new AddExpression(
            expression,
            new NumberExpression(8));

        Console.WriteLine($"Expression: {another.Describe()}");
        Console.WriteLine($"Result:     {another.Evaluate()}");
    }

    // ABSTRACT EXPRESSION
    private interface IExpression
    {
        int Evaluate();

        string Describe();
    }

    // TERMINAL EXPRESSION
    // A number is a leaf with no child expressions.
    private sealed class NumberExpression : IExpression
    {
        private readonly int _value;

        public NumberExpression(int value)
        {
            _value = value;
        }

        public int Evaluate() => _value;

        public string Describe() => _value.ToString();
    }

    // NONTERMINAL EXPRESSION 1
    private sealed class AddExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly IExpression _right;

        public AddExpression(IExpression left, IExpression right)
        {
            _left = left;
            _right = right;
        }

        public int Evaluate() => _left.Evaluate() + _right.Evaluate();

        public string Describe() => $"({_left.Describe()} + {_right.Describe()})";
    }

    // NONTERMINAL EXPRESSION 2
    private sealed class SubtractExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly IExpression _right;

        public SubtractExpression(IExpression left, IExpression right)
        {
            _left = left;
            _right = right;
        }

        public int Evaluate() => _left.Evaluate() - _right.Evaluate();

        public string Describe() => $"({_left.Describe()} - {_right.Describe()})";
    }
}
