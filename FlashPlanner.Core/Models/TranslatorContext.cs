﻿using FlashPlanner.Core.Models.SAS;
using PDDLSharp.Models.PDDL;

namespace FlashPlanner.Core.Models
{
    /// <summary>
    /// The format of data that gets transfered from the translator to the search
    /// </summary>
    public class TranslatorContext
    {
        /// <summary>
        /// A reference <seealso cref="SASDecl"/> that was translated
        /// </summary>
        public SASDecl SAS;
        /// <summary>
        /// A reference <seealso cref="PDDLDecl"/> that was translated from
        /// </summary>
        public PDDLDecl PDDL;
        /// <summary>
        /// A dictionary of hash values for facts
        /// </summary>
        public int[] FactHashes;
        /// <summary>
        /// A graph saying that when a given operator have been executed, these following operators could now be applicable
        /// </summary>
        public LinkedGraph ApplicabilityGraph;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="sas"></param>
        /// <param name="pDDL"></param>
        /// <param name="factHashes"></param>
        /// <param name="applicabilityGraph"></param>
        public TranslatorContext(TranslatorContext other)
        {
            SAS = other.SAS;
            PDDL = other.PDDL;
            FactHashes = other.FactHashes;
            ApplicabilityGraph = other.ApplicabilityGraph;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public TranslatorContext()
        {
            SAS = new SASDecl();
            PDDL = new PDDLDecl();
            FactHashes = new int[0];
            ApplicabilityGraph = new LinkedGraph(0);
        }
    }
}
