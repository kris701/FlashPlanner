using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Models
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
        /// Main constructor
        /// </summary>
        /// <param name="sas"></param>
        /// <param name="pDDL"></param>
        /// <param name="factHashes"></param>
        public TranslatorContext(SASDecl sas, PDDLDecl pDDL, int[] factHashes)
        {
            SAS = sas;
            PDDL = pDDL;
            FactHashes = factHashes;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public TranslatorContext()
        {
            SAS = new SASDecl();
            PDDL = new PDDLDecl();
            FactHashes = new int[0];
        }
    }
}
