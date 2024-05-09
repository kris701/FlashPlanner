
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
dotnet run -- --domain domain.pddl --problem p01.pddl --search Classical_BestFirst --heuristic hWeighted(hFF(),10)
```

## Classical Planners

These planners is able to find solutions for SMALL problems.
The planners are:
* [GreedyBFS](FlashPlanner.Search/Search/Classical/GreedyBFS.cs): Greedy Best First Search
* [GreedyBFSUAR](FlashPlanner.Search/Search/Classical/GreedyBFSUAR.cs): Greedy Best First Search with [Under-Approximation Refinement (UAR)](https://ojs.aaai.org/index.php/ICAPS/article/view/13678)
* [GreedyBFSPO](FlashPlanner.Search/Search/Classical/GreedyBFSPO.cs): Greedy Best First Search with [Preferred Operators (PO)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf)
* [GreedyBFSDHE](FlashPlanner.Search/Search/Classical/GreedyBFSDHE.cs): Greedy Best First Search with [Deferred Heuristic Evaluation (DHE)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf)

For these planners, there is a set of heuristics as well.
* [hConstant](FlashPlanner.Search/Heuristics/hConstant.cs): Returns a given constant all the time
* [hDepth](FlashPlanner.Search/Heuristics/hDepth.cs): Simply returns a cost that is 1 lower than its parent
* [hFF](FlashPlanner.Search/Heuristics/hFF.cs): Returns a cost based on a solution to the [relaxed planning graph](https://www.youtube.com/watch?app=desktop&v=7XH60fuMlIM) for the problem
* [hAdd](FlashPlanner.Search/Heuristics/hAdd.cs): Retuns the sum of actions needed to achive every goal fact
* [hMax](FlashPlanner.Search/Heuristics/hMax.cs): Returns the highest amount of actions needed to achive a goal fact.
* [hGoal](FlashPlanner.Search/Heuristics/hGoal.cs): Returns the amount of goals that are achived in the given state, i.e. `h = allGoals - achivedGoals`
* [hPath](FlashPlanner.Search/Heuristics/hPath.cs): Returns the cost of the current branch being evaluated
* [hWeighted](FlashPlanner.Search/Heuristics/hWeighted.cs): Takes one of the previously given heuristics, and weights its result from a constant.

There are also a set of "collection heuristics" that runs on a set of heuristics:
* [hColMax](FlashPlanner.Search/HeuristicsCollections/hColMax.cs): Gets the highest heuristic value from a set of heuristics
* [hColSom](FlashPlanner.Search/HeuristicsCollections/hColSum.cs): Gets the sum of the heuristic values from a set of heuristics

Again do note, that these planners are very inefficient, and will run out of memory with larger problems.

### Examples
To find a plan using the Greedy Best First Search engine:
```csharp
PDDLDecl decl = new PDDLDecl(...);
ITranslator translator = new PDDLToSASTranslator(true);
SASDecl sas = translator.Translate(decl);
using (var greedyBFS = new GreedyBFS(sas, new hFF(decl)))
{
   var plan = greedyBFS.Solve();
}
```

## Black Box Planners
This is a type of planners where the heuristic gets seriously limited, by not allowing it to know any structural knowledge of the domain.

The current planners are:
* [GreedyBFS](FlashPlanner.Search/Search/BlackBox/GreedyBFS.cs): Greedy Best First Search
* [GreedyBFSFocused](FlashPlanner.Search/Search/BlackBox/GreedyBFSFocused.cs): Greedy Best First Search with [Focused Macros](https://arxiv.org/abs/2004.13242). 

This also only support the [hGoal](FlashPlanner.Search/Heuristics/hGoal.cs) heuristic