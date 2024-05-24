
<p align="center">
    <img src="https://github.com/kris701/FlashPlanner/assets/22596587/d3f387a6-e0b5-4118-9801-c125a4e64100" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/FlashPlanner/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/FlashPlanner/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/FlashPlanner)
![Nuget](https://img.shields.io/nuget/dt/FlashPlanner)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/FlashPlanner/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/FlashPlanner)
![Static Badge](https://img.shields.io/badge/Platform-Windows-blue)
![Static Badge](https://img.shields.io/badge/Platform-Linux-blue)
![Static Badge](https://img.shields.io/badge/Framework-dotnet--8.0-green)

# Flash Planner

This is a project that contains a simple planner, operating on a grounded representation.
You can either use the planner as the C# code or use the CLI interface to get plans.
The planner expects a grounded representation of a PDDL domain+problem, that can be obtained from the Translator.

The following is an example of how to use the CLI interface:
```
dotnet run -- --domain domain.pddl --problem p01.pddl --search "greedy(hGoal())"
```

The available search engines are:
* [`beam`](FlashPlanner/Search/Classical/BeamS.cs): Beam Search
* [`greedy`](FlashPlanner/Search/Classical/GreedyBFS.cs): Greedy Best First Search
* [`greedy_underaprox`](FlashPlanner/Search/Classical/GreedyBFSUAR.cs): Greedy Best First Search with [Under-Approximation Refinement (UAR)](https://ojs.aaai.org/index.php/ICAPS/article/view/13678)
* [`greedy_prefered`](FlashPlanner/Search/Classical/GreedyBFSPO.cs): Greedy Best First Search with [Preferred Operators (PO)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf)
* [`greedy_defered`](FlashPlanner/Search/Classical/GreedyBFSDHE.cs): Greedy Best First Search with [Deferred Heuristic Evaluation (DHE)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf)
* [`greedy_bb`](FlashPlanner/Search/BlackBox/GreedyBFS.cs): Black box Greedy Best First Search
* [`greedy_bb_focused`](FlashPlanner/Search/BlackBox/GreedyBFSFocused.cs): Black box Greedy Best First Search with [Focused Macros](https://arxiv.org/abs/2004.13242). 

Do note that the black box planners only support the [hGoal](FlashPlanner/Heuristics/hGoal.cs) heuristic.

The available heuristics are:
* [`hConstant(n)`](FlashPlanner/Heuristics/hConstant.cs): Returns a given constant all the time
* [`hDepth()`](FlashPlanner/Heuristics/hDepth.cs): Simply returns a cost that is 1 lower than its parent
* [`hFF()`](FlashPlanner/Heuristics/hFF.cs): Returns a cost based on a solution to the [relaxed planning graph](https://www.youtube.com/watch?app=desktop&v=7XH60fuMlIM) for the problem
* [`hAdd()`](FlashPlanner/Heuristics/hAdd.cs): Retuns the sum of actions needed to achive every goal fact
* [`hMax()`](FlashPlanner/Heuristics/hMax.cs): Returns the highest amount of actions needed to achive a goal fact.
* [`hGoal()`](FlashPlanner/Heuristics/hGoal.cs): Returns the amount of goals that are achived in the given state, i.e. `h = allGoals - achivedGoals`
* [`hPath()`](FlashPlanner/Heuristics/hPath.cs): Returns the cost of the current branch being evaluated
* [`hWeighted(h,w)`](FlashPlanner/Heuristics/hWeighted.cs): Takes one of the previously given heuristics, and weights its result from a constant.
* [`hColMax(h)`](FlashPlanner/HeuristicsCollections/hColMax.cs): Takes a set of other heuristics and returns the highest value from any of the heuristics.
* [`hColSum(h)`](FlashPlanner/HeuristicsCollections/hColSum.cs): Same as the previous one, but takes the sum of all the heuristics.

There is also a set of aliases, that can be used as a single value to setup both the search and the translator.
The options are:
* `fast()`: Normal translator configuration with Greedy Best First search and the heuristic hFF.

This project is also available as a package on the [NuGet Package Manager](https://www.nuget.org/packages/FlashPlanner).

## Examples
To find a plan using the Greedy Best First Search engine:
```csharp
var decl = new PDDLDecl(...);
var translator = new PDDLToSASTranslator(true);
var sas = translator.Translate(decl);
using (var greedyBFS = new GreedyBFS(sas, new hFF(decl)))
{
   var plan = greedyBFS.Solve();
}
```

## Supported PDDL Requirements
Here is the set of requirements that the planner supports.

- [x] STRIPS (`:strips`)
- [x] Typing (`:typing`)
- [X] Disjunctive Preconditions (`:disjunctive-preconditions`)
- [X] Equality (`:equality`)
- [x] Quantified Preconditions (`:quantified-preconditions`)
    - [x] Existential Preconditions (`:existential-preconditions`)
    - [x] Universal Preconditions (`:universal-preconditions`)
- [X] Conditional Effects (`:conditional-effects`)
- [ ] Domain Axioms (`:domain-axioms`)
    - [ ] Subgoals Through Axioms (`:subgoals-through-axioms`)
    - [ ] Expression Evaluation (`:expression-evaluation`)
- [X] ADL (`:adl`)
- [ ] Fluents (`:fluents`)
- [ ] Durative Actions (`:durative-actions`)
    - [ ] Durative Inequalities (`:durative-inequalities`)
    - [ ] Continuous Effects (`:continuous-effects`)
- [X] Negative Preconditions (`:negative-preconditions`)
- [ ] Derived Predicates (`:derived-predicates`)
- [ ] Timed Initial Literals (`:timed-initial-literals`)
- [ ] Action Expansions (`:action-expansions`)
- [ ] Foreach Expansions (`:forach-expansions`)
- [ ] DAG Expansions (`:dag-expansions`)
- [ ] Safety Constraints (`:safety-constraints`)
- [ ] Open World (`:open-world`)
- [ ] True Negation (`:true-negation`)
- [ ] UCPOP (`:ucpop`)
- [ ] Constraints (`:constraints`)
- [ ] Preferences (`:preferences`)

## Performance
Here are some simple coverage benchmarks to get an idea of the performance of this planner.
It is compared against [Fast Downward](https://github.com/aibasel/downward).
Both are run with greedy best first search with hFF.
Benchmarks are only run on the first 20 problems.
The planners have a time limit of 60 seconds and a memory limit of 4GB.

<!-- This section is auto generated. -->
| Domain | Problems | Fast Downward | Flash Planner |
| - | - | - | - |
| blocks | 20 | 20 | 20 |
| depot | 20 | 15 | 10 |
| gripper | 20 | 20 | 20 |
| logistics00 | 20 | 20 | 20 |
| satellite | 20 | 20 | 17 |
| miconic | 20 | 20 | 20 |
| mystery | 20 | 12 | 6 |
| rovers | 20 | 20 | 14 |
| tpp | 20 | 20 | 13 |
| zenotravel | 20 | 20 | 15 |
