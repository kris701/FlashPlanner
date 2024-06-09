
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
This project is also available as a dotnet tool on the [NuGet Package Manager](https://www.nuget.org/packages/FlashPlanner).

## Planner Usage

The easiest way to run Flash Planner is to install it as a dotnet tool by running the following command:
```
dotnet tool install FlashPlanner
```
The arguments that can be given to Flash Planner is heavily inspired by [Fast Downward](https://github.com/aibasel/downward), where there is seperate string arguments for the translator and the search engine.
Generally, it needs a domain and problem file in the PDDL format as well as a search engine.
The search engine, and translation, arguments is given as a string, e.g.:
```
--search "greedy(hFF())"
```
The translator also have optional arguments, howevery they are usually not needed.
Below are the different Search Engines and Heuristics that can be combined to run FlashPlanner.
There is also the option to use an alias, instead of a combination of search and translator arguments.

If you dont wish to use the dotnet tool package, you can just clone the repository and use `dotnet run` from the `FlashPlanner` folder.

### Examples
Search on a domain file called `domain.pddl` and a problem file called `p01.pddl` with greedy best first search and the Goal Count heuristic:
```
flashplanner --domain domain.pddl --problem p01.pddl --search "greedy(hGoal())"
```

Search on a domain file called `domain.pddl` and a problem file called `p01.pddl` with greedy best first search and the hColMax with Goal Count and hAdd:
```
flashplanner --domain domain.pddl --problem p01.pddl --search "greedy(hColMax([hGoal(),hAdd()]))"
```

## Search Engines

The available search engines are:
* [`beam(h, b)`](FlashPlanner.Core/Search/BeamS.cs): [Beam Search](https://en.wikipedia.org/wiki/Beam_search). H is the heuristic, b is the beta value as an integer.
* [`greedy(h)`](FlashPlanner.Core/Search/GreedyBFS.cs): [Greedy Best First Search](https://en.wikipedia.org/wiki/Best-first_search). H is the heuristic.
* [`greedy_underaprox(h)`](FlashPlanner.Core/Search/GreedyBFSUAR.cs): [Greedy Best First Search](https://en.wikipedia.org/wiki/Best-first_search) with [Under-Approximation Refinement (UAR)](https://ojs.aaai.org/index.php/ICAPS/article/view/13678). H is the heuristic.
* [`greedy_prefered(h)`](FlashPlanner.Core/Search/GreedyBFSPO.cs): [Greedy Best First Search](https://en.wikipedia.org/wiki/Best-first_search) with [Preferred Operators (PO)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf). H is the heuristic.
* [`greedy_defered(h)`](FlashPlanner.Core/Search/GreedyBFSDHE.cs): [Greedy Best First Search](https://en.wikipedia.org/wiki/Best-first_search) with [Deferred Heuristic Evaluation (DHE)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf). H is the heuristic.
* [`greedy_lazy(h)`](FlashPlanner.Core/Search/GreedyBFSLazy.cs): [Lazy Greedy Best First Search](https://www.fast-downward.org/Doc/SearchAlgorithm#Lazy_best-first_search). H is the heuristic.
* [`greedy_focused(h, n, b, p)`](FlashPlanner.Core/Search/GreedyBFSFocused.cs): [Greedy Best First Search](https://en.wikipedia.org/wiki/Best-first_search) with [Focused Macros](https://arxiv.org/abs/2004.13242). 
H is the heuristic, N is the maximum number of macros, B is the search budget in seconds and P is the parameter limit for added macros (too many parameters kills the translator).
Do note, this implementation of the macro generation is not very good, and does NOT work for all domains.

## Heuristics

The available heuristics are:
* [`hConstant(n)`](FlashPlanner.Core/Heuristics/hConstant.cs): Returns a given constant all the time. N being the constant.
* [`hDepth()`](FlashPlanner.Core/Heuristics/hDepth.cs): Simply returns a cost that is 1 lower than its parent
* [`hFF()`](FlashPlanner.Core/Heuristics/hFF.cs): Returns a cost based on a solution to the [relaxed planning graph](https://www.youtube.com/watch?app=desktop&v=7XH60fuMlIM) for the problem
* [`hAdd()`](FlashPlanner.Core/Heuristics/hAdd.cs): Is the [Additive Heuristic](https://www.cs.toronto.edu/~sheila/2542/s14/A1/bonetgeffner-heusearch-aij01.pdf) that returns the sum of actions needed to achive every goal fact.
* [`hMax()`](FlashPlanner.Core/Heuristics/hMax.cs): Is the [Max Heuristic](https://www.cs.toronto.edu/~sheila/2542/s14/A1/bonetgeffner-heusearch-aij01.pdf) that returns the highest amount of actions needed to achive a goal fact.
* [`hGoal()`](FlashPlanner.Core/Heuristics/hGoal.cs): Returns the amount of goals that are achived in the given state, i.e. `h = allGoals - achivedGoals`
* [`hPath()`](FlashPlanner.Core/Heuristics/hPath.cs): Returns the cost of the current branch being evaluated
* [`hWeighted(h,w)`](FlashPlanner.Core/Heuristics/hWeighted.cs): Takes one of the previously given heuristics, and weights its result from a constant. H is the heuristic and W is the weight.
* [`hColMax([h1,...,hn])`](FlashPlanner.Core/HeuristicsCollections/hColMax.cs): Takes a set of other heuristics and returns the highest value from any of the heuristics.
* [`hColSum([h1,...,hn])`](FlashPlanner.Core/HeuristicsCollections/hColSum.cs): Same as the previous one, but takes the sum of all the heuristics.

## Aliases

There is also a set of aliases, that can be used as a single value to setup both the search and the translator.
The options are:
* `fast()`: Normal translator configuration with Greedy Best First search and the heuristic hFF.

## Examples
To find a plan using the Greedy Best First Search engine:
```csharp
var decl = new PDDLDecl(...);
var translator = new PDDLToSASTranslator(false);
var sas = translator.Translate(decl);
var greedyBFS = new GreedyBFS(new hFF());
ar plan = greedyBFS.Solve(sas);
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
It is compared against [Fast Downward](https://github.com/aibasel/downward) with the configuration `--evaluator \"hff=ff()\" --search \"lazy_greedy([hff], preferred=[hff])\"`.
Both are run with lazy greedy best first search with hFF.
Benchmarks are only run on the first 20 problems.
The planners have a time limit of 60 seconds and a memory limit of 4GB.

<!-- This section is auto generated. -->
| Domain | Problems | Fast Downward | Flash Planner |
| - | - | - | - |
| blocks | 20 | 20 | 20 |
| depot | 20 | 15 | 11 |
| gripper | 20 | 20 | 20 |
| logistics00 | 20 | 20 | 20 |
| satellite | 20 | 20 | 18 |
| miconic | 20 | 20 | 20 |
| rovers | 20 | 20 | 18 |
| tpp | 20 | 20 | 14 |
| zenotravel | 20 | 20 | 15 |
| Total | 180 | 175 | 156 |
