# Gang of Four Design Patterns: A Beginner's C# Course

A complete, self-study console course covering all **23 Gang of Four (GoF) design patterns** in C# 12 and .NET 8.

Each lesson follows the same rhythm:

1. Read a plain-language explanation of the pattern.
2. Learn the problem it solves and the trade-offs involved.
3. Run a small, practical example.
4. Open the source file and trace the same idea in code.
5. Change the example and observe what happens.

The project is intentionally a teaching resource, not a production framework. Examples favor clarity over cleverness.

---

## Table of Contents

- [Introduction to Design Patterns](#introduction-to-design-patterns)
- [A Short History of the Gang of Four](#a-short-history-of-the-gang-of-four)
- [The Three Pattern Categories](#the-three-pattern-categories)
- [Learning Roadmap for Beginners](#learning-roadmap-for-beginners)
- [Summary of All 23 Patterns](#summary-of-all-23-patterns)
- [Project Structure](#project-structure)
- [Requirements and Running the App](#requirements-and-running-the-app)
- [How the Interactive Course Works](#how-the-interactive-course-works)
- [Command-Line Shortcuts](#command-line-shortcuts)
- [A Suggested Four-Week Study Plan](#a-suggested-four-week-study-plan)
- [How to Practice](#how-to-practice)
- [Common Beginner Mistakes](#common-beginner-mistakes)
- [Design Principles Behind the Patterns](#design-principles-behind-the-patterns)
- [License and Use](#license-and-use)

---

## Introduction to Design Patterns

A **design pattern** is a named, reusable solution to a recurring problem in software design. It is not a finished library, a copy-paste template, or a language feature. It is a way of organizing objects and responsibilities that has proven useful in many different projects.

Design patterns give developers a shared vocabulary. Saying "wrap it in an adapter" communicates an idea faster than describing every class and method. The name also carries a set of expectations and trade-offs, which helps a team discuss design at a higher level.

Patterns are **not goals**. A program is not better merely because it contains more patterns. In fact, forcing a pattern into a simple problem often makes the code harder to read. The right question is always:

> What problem am I solving, and what is the simplest design that solves it clearly?

Design patterns are most useful when:

- The same problem keeps appearing in different parts of a system.
- A design decision needs a shared name so a team can discuss it.
- Code must remain flexible as new variants arrive.
- A dependency needs to be decoupled so components can change independently.
- A proven structure can replace a tangle of conditionals or duplicated code.

A pattern is **not**:

- A guarantee of good design.
- A replacement for understanding the domain.
- A reason to add unnecessary classes.
- A checklist that every project must complete.
- The same thing as an algorithm. An algorithm describes steps to compute something; a pattern describes how objects collaborate.

Think of patterns as tools in a workshop. A hammer is useful, but using a hammer on every task is not craftsmanship.

---

## A Short History of the Gang of Four

The phrase **Gang of Four** refers to four authors:

- **Erich Gamma**
- **Richard Helm**
- **Ralph Johnson**
- **John Vlissides**

They published *Design Patterns: Elements of Reusable Object-Oriented Software* in **1994**. The book is often called the "GoF book" or simply "the Gang of Four book."

The authors did not invent most of these patterns from scratch. They studied existing object-oriented systems, especially frameworks and applications, and documented recurring solutions. Their contribution was to name, classify, compare, and explain the patterns in a consistent format.

The original book described **23 patterns** and organized them into three categories:

1. **Creational** — how objects are created.
2. **Structural** — how objects are composed.
3. **Behavioral** — how objects communicate and distribute responsibility.

The book became influential because it gave the software industry a common vocabulary. Developers could discuss a design without drawing every class. Many modern frameworks, libraries, and language features embody these ideas, often without using the pattern names.

Some patterns are more visible in C# than others. For example:

- `IEnumerable<T>` and `foreach` are built-in support for **Iterator**.
- C# events are a language-level form of **Observer**.
- `Func<T>` and lambdas can provide lightweight **Strategy** implementations.
- Dependency injection containers often manage **Singleton** lifetimes.
- `Stream` classes commonly combine **Decorator** and **Adapter** ideas.

Modern languages and frameworks reduce the need to hand-write some patterns, but understanding the underlying ideas remains valuable. The patterns describe design forces that still appear in every codebase.

---

## The Three Pattern Categories

### 1. Creational Patterns

Creational patterns deal with **object creation**. They hide the details of how objects are constructed and make creation more flexible.

They help answer questions such as:

- Should the client know the concrete class being created?
- Should a family of related objects be created together?
- Should construction happen step by step?
- Should an object be cloned instead of rebuilt?
- Should only one instance exist?

The five creational patterns are:

- Abstract Factory
- Builder
- Factory Method
- Prototype
- Singleton

### 2. Structural Patterns

Structural patterns deal with **how classes and objects are composed** to form larger structures. They help objects work together while keeping coupling low.

They help answer questions such as:

- How can incompatible interfaces cooperate?
- How can a large class hierarchy be split into independent dimensions?
- How can individual items and groups be treated uniformly?
- How can optional behavior be layered onto an object?
- How can a complicated subsystem be simplified?
- How can shared data reduce memory use?
- How can access to an object be controlled?

The seven structural patterns are:

- Adapter
- Bridge
- Composite
- Decorator
- Facade
- Flyweight
- Proxy

### 3. Behavioral Patterns

Behavioral patterns deal with **communication, responsibility, and algorithms** among objects. They help organize complex control flow and keep objects loosely coupled.

They help answer questions such as:

- Who should handle a request?
- How can an action be stored, queued, or undone?
- How can a small language be evaluated?
- How can a collection be traversed without exposing its internals?
- How can many objects communicate without knowing each other?
- How can state be saved and restored?
- How can changes be broadcast to subscribers?
- How can an object behave differently as its lifecycle changes?
- How can algorithms be selected at runtime?
- How can a shared process allow customized steps?
- How can new operations be added to a stable object structure?

The eleven behavioral patterns are:

- Chain of Responsibility
- Command
- Interpreter
- Iterator
- Mediator
- Memento
- Observer
- State
- Strategy
- Template Method
- Visitor

---

## Learning Roadmap for Beginners

Do not try to memorize all 23 patterns in one sitting. Learn the problem each pattern solves, then connect it to code you have seen.

### Stage 1: Build the foundations

Before studying patterns, become comfortable with:

- Classes, interfaces, and inheritance.
- Composition: one object holding another object.
- Polymorphism: calling an interface method without knowing the concrete type.
- Dependency injection basics.
- Records, nullable reference types, and basic LINQ.

These ideas appear in almost every pattern.

### Stage 2: Start with the most common patterns

Suggested first group:

1. **Factory Method** — a reusable workflow with one replaceable creation step.
2. **Builder** — constructing an object with many optional parts.
3. **Adapter** — making two interfaces work together.
4. **Strategy** — choosing between interchangeable algorithms.
5. **Observer** — notifying subscribers when something changes.

These appear frequently in ordinary application code and are relatively easy to recognize.

### Stage 3: Learn composition-heavy patterns

Suggested second group:

1. **Decorator** — layering behavior around an object.
2. **Composite** — treating individual items and groups uniformly.
3. **Facade** — simplifying a complex subsystem.
4. **Command** — turning actions into objects.
5. **State** — organizing lifecycle behavior.

### Stage 4: Study coordination and lifecycle patterns

Suggested third group:

1. **Chain of Responsibility** — passing a request along handlers.
2. **Mediator** — centralizing communication.
3. **Memento** — saving and restoring state.
4. **Template Method** — fixing a process while allowing custom steps.
5. **Iterator** — separating traversal from storage.

### Stage 5: Explore advanced or specialized patterns

Suggested final group:

1. **Abstract Factory** — families of related products.
2. **Bridge** — separating two dimensions of variation.
3. **Flyweight** — sharing immutable data at scale.
4. **Proxy** — controlling access to an object.
5. **Prototype** — cloning a prepared object.
6. **Singleton** — enforcing one instance while understanding global-state costs.
7. **Interpreter** — evaluating a small grammar.
8. **Visitor** — adding operations to a stable object structure.

### Stage 6: Practice recognition

For each pattern, ask:

- What design pressure created this pattern?
- What would the code look like without it?
- What new complexity does it add?
- Which pattern is commonly confused with it?
- Have I seen this idea in a library, framework, or app I use?

---

## Summary of All 23 Patterns

The numbers match the interactive menu and the `--run` command.

| # | Pattern | Category | Purpose in one sentence | Beginner-friendly demo |
|---|---------|----------|-------------------------|------------------------|
| 1 | Abstract Factory | Creational | Creates a family of related objects through one interchangeable factory. | Light and dark UI controls |
| 2 | Builder | Creational | Builds a complex object step by step with readable, named operations. | Custom pizza order |
| 3 | Factory Method | Creational | Lets subclasses choose the concrete object used by a shared workflow. | Email and SMS notification services |
| 4 | Prototype | Creational | Creates new objects by cloning a prepared example. | Game character with an independent inventory |
| 5 | Singleton | Creational | Guarantees one shared instance and controlled access to it. | Application settings |
| 6 | Adapter | Structural | Makes incompatible interfaces work together through a translator. | European socket to US charger |
| 7 | Bridge | Structural | Separates an abstraction from its implementation so both can vary. | Remote controls and devices |
| 8 | Composite | Structural | Treats a single item and a tree of items through one interface. | Files and folders |
| 9 | Decorator | Structural | Adds behavior by wrapping an object in layers with the same interface. | Coffee with optional add-ons |
| 10 | Facade | Structural | Provides one simple interface over a group of subsystems. | Home theater movie-night button |
| 11 | Flyweight | Structural | Shares immutable common data among many objects. | Trees sharing species data |
| 12 | Proxy | Structural | Controls access to another object through a stand-in. | Lazy-loading video proxy |
| 13 | Chain of Responsibility | Behavioral | Passes a request along handlers until one can process it. | Expense approval limits |
| 14 | Command | Behavioral | Encapsulates a request as a storable, executable, undoable object. | Remote control with undo |
| 15 | Interpreter | Behavioral | Evaluates a small language represented as an object tree. | Arithmetic expression tree |
| 16 | Iterator | Behavioral | Traverses a collection without exposing its internal storage. | Playlist iterator |
| 17 | Mediator | Behavioral | Centralizes communication between many objects. | Chat room |
| 18 | Memento | Behavioral | Saves and restores an object's state without exposing internals. | Text editor undo |
| 19 | Observer | Behavioral | Notifies subscribers automatically when state changes. | Weather station displays |
| 20 | State | Behavioral | Changes behavior as an object moves through lifecycle states. | Order payment and shipping |
| 21 | Strategy | Behavioral | Makes a family of algorithms interchangeable at runtime. | Checkout discount rules |
| 22 | Template Method | Behavioral | Fixes an algorithm skeleton while allowing custom steps. | Coffee and tea preparation |
| 23 | Visitor | Behavioral | Adds operations to a stable object structure without editing it. | Sales tax and discounts on a cart |

### Quick memory hooks

| Pattern | Memory hook |
|---------|-------------|
| Abstract Factory | "A factory for a matching family." |
| Builder | "Assemble it step by step." |
| Factory Method | "Subclass chooses the product." |
| Prototype | "Clone this prepared example." |
| Singleton | "Only one exists." |
| Adapter | "Translate this plug." |
| Bridge | "Two dimensions, two hierarchies." |
| Composite | "A file and a folder are both items." |
| Decorator | "Wrap it with extra features." |
| Facade | "One button for many steps." |
| Flyweight | "Share what is common." |
| Proxy | "Stand in and control access." |
| Chain of Responsibility | "Pass it down the line." |
| Command | "An action as an object." |
| Interpreter | "A tiny language as a tree." |
| Iterator | "Walk the collection." |
| Mediator | "Everyone talks through the hub." |
| Memento | "Save a snapshot, restore later." |
| Observer | "Notify the subscribers." |
| State | "The phase decides the behavior." |
| Strategy | "Swap the algorithm." |
| Template Method | "Same recipe, different ingredients." |
| Visitor | "Add an operation from outside." |

---

## Project Structure

```text
.gitignore
GoFPatternGuide.csproj
Program.cs
README.md
Core/
  ConsoleUi.cs
  IPatternDemo.cs
  PatternCatalog.cs
  PatternCategories.cs
Creational/
  AbstractFactoryPattern.cs
  BuilderPattern.cs
  FactoryMethodPattern.cs
  PrototypePattern.cs
  SingletonPattern.cs
Structural/
  AdapterPattern.cs
  BridgePattern.cs
  CompositePattern.cs
  DecoratorPattern.cs
  FacadePattern.cs
  FlyweightPattern.cs
  ProxyPattern.cs
Behavioral/
  ChainOfResponsibilityPattern.cs
  CommandPattern.cs
  InterpreterPattern.cs
  IteratorPattern.cs
  MediatorPattern.cs
  MementoPattern.cs
  ObserverPattern.cs
  StatePattern.cs
  StrategyPattern.cs
  TemplateMethodPattern.cs
  VisitorPattern.cs
```

### What each part does

- **`Program.cs`** — the interactive main menu, category browser, pattern runner, study guide, and optional command-line shortcuts.
- **`Core/IPatternDemo.cs`** — the tiny contract every lesson implements: name, category, summary, explanation, and `Run`.
- **`Core/PatternCatalog.cs`** — one explicit list of all 23 patterns and lookup helpers.
- **`Core/PatternCategories.cs`** — category names and beginner-friendly descriptions.
- **`Core/ConsoleUi.cs`** — consistent console headings, colors, prompts, and safe input handling.
- **`Creational/`** — the five patterns about object creation.
- **`Structural/`** — the seven patterns about composition.
- **`Behavioral/`** — the eleven patterns about communication and responsibility.

Each pattern file contains:

- A lesson with all requested sections: what it is, why it exists, the problem it solves, when to use it, advantages, disadvantages, and real-world examples.
- Additional notes on related patterns and common misunderstandings.
- A small runnable example built from nested types so the file is self-contained.
- Comments that explain the roles and the flow of the example.

---

## Requirements and Running the App

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or newer.
- A terminal or IDE that can run a console application.
- No external NuGet packages are required.

### Build

```bash
dotnet build
```

### Run the interactive course

```bash
dotnet run
```

### Run from Visual Studio or JetBrains Rider

Open `GoFPatternGuide.csproj` and press Run. The project targets `net8.0` and uses C# 12.

---

## How the Interactive Course Works

The main menu offers four learning paths:

1. **Browse patterns by category** — choose Creational, Structural, or Behavioral, then pick a lesson.
2. **View all 23 patterns at a glance** — see the complete catalog and open a pattern by number.
3. **Open a pattern by number** — jump directly to a lesson.
4. **About this guide and how to study** — a short study method and suggested learning order.

When you open a pattern, the app:

1. Shows the pattern's number, category, and one-sentence summary.
2. Prints the complete written explanation.
3. Waits for you to press Enter.
4. Runs the live demo.
5. Returns you to the pattern list.

Nothing in a demo asks for input, so the examples are deterministic and easy to run repeatedly.

---

## Command-Line Shortcuts

These are useful for classroom demonstrations and quick checks.

```bash
# List all patterns with their stable numbers
dotnet run -- --list

# Show lesson 7 and run its demo
dotnet run -- --run 7

# Find a pattern by name (case-insensitive)
dotnet run -- --run Adapter

# Run every demo without printing the explanations
dotnet run -- --run-all

# Show command-line help
dotnet run -- --help
```

The `--` separates arguments for `dotnet run` from arguments for the application.

---

## A Suggested Four-Week Study Plan

This pace is a suggestion, not a race. Spend more time on patterns that feel unfamiliar.

### Week 1: Creation and simple structure

- Read the introduction and the three category descriptions.
- Study Factory Method, Builder, Adapter, and Strategy.
- Run each demo twice: once for output, once while reading the source.
- Exercise: add a `PushNotificationService` to Factory Method or a `SoyMilkDecorator` to Decorator.

### Week 2: Composition and behavior

- Study Decorator, Composite, Facade, Observer, and Command.
- Exercise: add a new expense limit to Chain of Responsibility.
- Exercise: add a new observer to the weather station.
- Write a short note explaining why each pattern is better than a long `if/else`.

### Week 3: Lifecycles and coordination

- Study State, Memento, Template Method, Mediator, and Iterator.
- Exercise: add a `CancelledState` to the order lifecycle.
- Exercise: implement a second playlist traversal order.
- Compare State and Strategy in your own words.

### Week 4: Advanced patterns and review

- Study Abstract Factory, Bridge, Flyweight, Proxy, Prototype, Singleton, Interpreter, and Visitor.
- Revisit any pattern that still feels abstract.
- Pick one pattern and find it in a real library or framework.
- Build a tiny feature in a personal project using exactly one pattern.

---

## How to Practice

### 1. Change the example

The fastest way to understand a pattern is to extend it:

- Add a product to Abstract Factory.
- Add a topping to Builder.
- Add a new notification channel to Factory Method.
- Add a new device to Bridge.
- Add a new file type to Composite.
- Add a decorator to the coffee order.
- Add a new subsystem step to Facade.
- Add another shared tree type to Flyweight.
- Add a logging proxy.
- Add an approval level.
- Add a redo stack to Command.
- Add multiplication to Interpreter.
- Add a reverse iterator.
- Add a private message to Mediator.
- Add redo to Memento.
- Add a new subscriber.
- Add a refunded state.
- Add a loyalty-points strategy.
- Add a smoothie using the beverage template.
- Add a shipping-cost visitor.

### 2. Remove the pattern

Delete the abstraction and write the simplest version. Then compare:

- Which version is shorter?
- Which version is easier to change?
- Which version is easier to test?
- Which version would survive a new requirement?
- Was the pattern actually justified?

This exercise builds judgment better than memorizing definitions.

### 3. Find the pattern in the wild

Look for these ideas in libraries you already use:

- `IEnumerable<T>` and `foreach` — Iterator.
- C# events and `EventHandler` — Observer.
- `Stream` wrappers — Decorator and Adapter.
- ASP.NET Core middleware — Chain of Responsibility and Decorator.
- `ILogger<T>` registration lifetimes — Singleton-style sharing managed by DI.
- LINQ method chaining — a fluent, composition-oriented style related to Builder and pipelines.
- `IComparer<T>` — Strategy.

### 4. Explain it without jargon

If you can explain a pattern to a friend using a non-programming analogy, you probably understand it. If you can only recite the definition, go back to the problem it solves.

---

## Common Beginner Mistakes

### Pattern fever

Adding patterns everywhere because they seem professional. This creates indirection, extra files, and more concepts without solving a real problem.

**Antidote:** Start with the simplest code. Introduce a pattern only when a specific pressure appears.

### Confusing structure with intent

Adapter, Decorator, Proxy, and Facade all involve wrapping or coordinating objects. Their class diagrams can look similar. The difference is why the code exists:

- Adapter changes an interface.
- Decorator adds responsibilities.
- Proxy controls access.
- Facade simplifies a subsystem.

**Antidote:** Name classes by intent and write down the problem before choosing the pattern.

### Treating Singleton as a universal solution

Global access is convenient, but it hides dependencies and makes tests interfere with one another.

**Antidote:** Prefer dependency injection with a singleton lifetime when the container can manage the shared instance.

### Forgetting deep vs shallow copies

Prototype and Memento can appear to work while sharing mutable state between the original and the copy.

**Antidote:** Decide explicitly which fields must be independent and test that mutating one object does not affect the other.

### Using Visitor when element types change often

Visitor makes adding operations easy but adding element types expensive.

**Antidote:** Use Visitor when the object structure is stable and operations are the changing dimension.

### Building an oversized Mediator or Facade

A central coordinator can slowly absorb every rule and become a god object.

**Antidote:** Keep the central class focused on coordination. Move domain rules into smaller collaborators.

### Assuming patterns are thread-safe

Patterns describe structure, not synchronization. Singleton initialization, shared flyweights, and state transitions may need explicit thread-safety design.

**Antidote:** Identify shared mutable state and protect it deliberately.

### Forgetting to unsubscribe observers

An observer that is never removed can keep an object alive and receive events long after it should.

**Antidote:** Define ownership of subscriptions. Use `IDisposable` subscriptions or language events carefully.

---

## Design Principles Behind the Patterns

The GoF patterns are not isolated tricks. They repeatedly apply a few design principles.

### Program to an interface, not an implementation

Clients should depend on the behavior they need, not on a specific concrete class. This makes substitution and testing easier.

### Favor composition over inheritance

Composition is often more flexible than a deep class hierarchy. Decorator, Strategy, Bridge, and State all rely heavily on holding a collaborator.

### Encapsulate what varies

Find the part of the code that changes most often and isolate it. Strategy encapsulates algorithms; State encapsulates lifecycle behavior; Builder encapsulates construction.

### Open for extension, closed for modification

Add new behavior by adding code, not by editing stable code. Many patterns provide extension points.

### Keep responsibilities focused

Small classes with clear roles are easier to understand and test. Patterns such as Command, Visitor, and Chain of Responsibility divide work into focused objects.

### Depend on abstractions, not concrete details

High-level policy should not depend on low-level implementation. Adapter, Bridge, Facade, and Abstract Factory all help enforce boundaries.

### Prefer readability

A pattern should make the design easier to explain. If it makes the code harder to understand, it may be the wrong tool or the wrong implementation.

---

## License and Use

This project is intended as an educational reference. You may use, modify, and share the examples for learning and teaching.

When using these ideas in production code, adapt them to the actual constraints of the system. A pattern is a starting point for a design conversation, not a substitute for one.
