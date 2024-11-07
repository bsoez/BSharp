using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSharp
{
    internal class GrammarRule(string leftSide, params string[] rightSide)
    {
        public string[] RightSide { get; set; } = rightSide;
        public string LeftSide { get; set; } = leftSide;
    }

    internal class GrammarAnalyzer
    {
        internal static readonly List<GrammarRule> grammarRules =
        [
            //PR01 ~CADENA~
            new GrammarRule("ARG01", "IDEN"),
            new GrammarRule("ARG02", "ASIG", "CADE"),
            new GrammarRule("DECL", "PR01", "ARG01"),
            new GrammarRule("S", "DECL", " "),
            new GrammarRule("S", "DECL", "ARG02", " "),

            //PR02 ~DEVUELVE~
            new GrammarRule("S", "PR02", "ARG01", " "),
            new GrammarRule("S", "PR02", "NUEN", " "),
            new GrammarRule("S", "PR02", "NUDE", " "),
            new GrammarRule("S", "PR02", "CADE", " "),

            //PR03 ~ENTERO~
            new GrammarRule("ARG03", "ASIG", "NUEN"),
            new GrammarRule("DECL", "PR03", "ARG01"),
            new GrammarRule("S", "DECL", "ARG03", " "),

            //PR04 ~ENTONCES~
            new GrammarRule("INSTR", "ARG01", "ARG03"),
            new GrammarRule("INSTR", "ARG01", "ARG04"),
            new GrammarRule("INSTR", "ARG01", "ARG02"),
            new GrammarRule("S", "PR04", "INSTR", " "),


            //PR06 ~FIN~
            new GrammarRule("S", "PR06", " "),

            //PR07 ~INICIO~
            new GrammarRule("S", "PR07", " "),

            //PR11 ~REAL~
            new GrammarRule("ARG04", "ASIG", "NUDE"),
            new GrammarRule("DECL", "PR11", "ARG01"),
            new GrammarRule("S", "DECL", "ARG04", " "),

            //Expresiones
            new GrammarRule("EXPR", "ARG02", "OA01", "CADE"), // "sqt" + "cub" °Concatenación°
            new GrammarRule("EXPR", "ARG03", "OA01", "NUEN"), // 2 + 2 °suma dos entero°
            new GrammarRule("EXPR", "ARG04", "OA01", "NUDE"), // 2.2 + 2.2 °suma dos reales°
            new GrammarRule("S", "DECL", "EXPR", " ")


        ];
    }
}
