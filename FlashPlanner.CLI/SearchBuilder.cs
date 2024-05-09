﻿using FlashPlanner.Search;
using FlashPlanner.Search.Search;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.CLI
{
    public enum SearchOptions
    {
        Classical_BestFirst,
        Classical_BestFirst_DeferedHeuristic,
        Classical_BestFirst_PreferedOps,
        Classical_BestFirst_UnderAproxRefine,

        BlackBox_BestFirst,
        BlackBox_BestFirst_Focused,
    }

    public static class SearchBuilder
    {
        private static Dictionary<SearchOptions, Func<PDDLDecl, SASDecl, IHeuristic, IPlanner>> _planners = new Dictionary<SearchOptions, Func<PDDLDecl, SASDecl, IHeuristic, IPlanner>>()
        {
            { SearchOptions.Classical_BestFirst, (p, s, h) => new Search.Search.Classical.GreedyBFS(s, h) },
            { SearchOptions.Classical_BestFirst_DeferedHeuristic, (p, s, h) => new Search.Search.Classical.GreedyBFSDHE(s, h) },
            { SearchOptions.Classical_BestFirst_PreferedOps, (p, s, h) => new Search.Search.Classical.GreedyBFSPO(s, h) },
            { SearchOptions.Classical_BestFirst_UnderAproxRefine, (p, s, h) => new Search.Search.Classical.GreedyBFSUAR(s, h) },

            { SearchOptions.BlackBox_BestFirst, (p, s, h) => new Search.Search.BlackBox.GreedyBFS(s, h) },
            { SearchOptions.BlackBox_BestFirst_Focused, (p, s, h) => new Search.Search.BlackBox.GreedyBFSFocused(p, s, h) },
        };

        public static IPlanner GetPlanner(SearchOptions option, PDDLDecl pddlDecl, SASDecl sasDecl, IHeuristic h) => _planners[option](pddlDecl, sasDecl, h);
    }
}
