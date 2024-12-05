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
            
            new GrammarRule("ARG02", "ASIG", "CADE"),
            new GrammarRule("DECL", "PR01", "ARG01"),
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
            new GrammarRule("S", "DECL", "ARG03", " "),
            new GrammarRule("S", "DECL", "ARG10", " "),

            //PR04 ~ENTONCES~
            new GrammarRule("INSTR", "ARG01", "ARG03"),
            new GrammarRule("INSTR", "ARG01", "ARG04"),
            new GrammarRule("INSTR", "ARG01", "ARG02"),
            new GrammarRule("S", "PR04", "INSTR", " "),

            //PR06 ~FIN~
            new GrammarRule("S", "PR06", " "),

            //PR07 ~INICIO~
            new GrammarRule("S", "PR07", " "),

            //PR08 ~LEER~
            new GrammarRule("ARG05", "CE12", "ARG01", "CE13"),
            new GrammarRule("ARG05", "CE12", "NUEN", "CE13"),
            new GrammarRule("ARG05", "CE12", "NUDE", "CE13"),
            new GrammarRule("ARG05", "CE12", "CADE", "CE13"),
            new GrammarRule("S", "PR08", "ARG05", " "),

            //PR10 ~MOSTRAR~        
            new GrammarRule("S", "PR10", "ARG05", " "),

            //PR11 ~REAL~
            new GrammarRule("ARG04", "ASIG", "NUDE"),
            new GrammarRule("DECL", "PR11", "ARG01"),
            new GrammarRule("S", "DECL", "ARG04", " "),

            //PR13 ~SI~
            new GrammarRule("COND", "CE12", "ARG01", "OR01", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR01", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR01", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR01", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "ARG01", "OR02", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR02", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR02", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR02", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "ARG01", "OR03", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR03", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR03", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR03", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "ARG01", "OR04", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR04", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR04", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR04", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "ARG01", "OR05", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR05", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR05", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "ARG01", "OR05", "CADE", "CE13"),

            //

            new GrammarRule("COND", "CE12", "NUEN", "OR01", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR01", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR01", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR01", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUEN", "OR02", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR02", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR02", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR02", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUEN", "OR03", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR03", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR03", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR03", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUEN", "OR04", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR04", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR04", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR04", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUEN", "OR05", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR05", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR05", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUEN", "OR05", "CADE", "CE13"),

            //

            new GrammarRule("COND", "CE12", "NUDE", "OR01", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR01", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR01", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR01", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUDE", "OR02", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR02", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR02", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR02", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUDE", "OR03", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR03", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR03", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR03", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUDE", "OR04", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR04", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR04", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR04", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "NUDE", "OR05", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR05", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR05", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "NUDE", "OR05", "CADE", "CE13"),

            //

            new GrammarRule("COND", "CE12", "CADE", "OR01", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR01", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR01", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR01", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "CADE", "OR02", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR02", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR02", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR02", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "CADE", "OR03", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR03", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR03", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR03", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "CADE", "OR04", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR04", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR04", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR04", "CADE", "CE13"),

            new GrammarRule("COND", "CE12", "CADE", "OR05", "ARG01", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR05", "NUEN", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR05", "NUDE", "CE13"),
            new GrammarRule("COND", "CE12", "CADE", "OR05", "CADE", "CE13"),

            //bell...................  
            new GrammarRule("S", "PR13", "COND", "INSTR", " "),
            new GrammarRule("S", "PR14", "INSTR", " "),

            //Expresiones
            new GrammarRule("EXPR", "ARG02", "OA01", "CADE"), // "sqt" + "cub" °Concatenación°
            new GrammarRule("EXPR", "ARG03", "OA01", "NUEN"), // 2 + 2 °suma dos entero°
            new GrammarRule("EXPR", "ARG04", "OA01", "NUDE"), // 2.2 + 2.2 °suma dos reales°
            new GrammarRule("S", "DECL", "EXPR", " "),
            new GrammarRule("ARG01", "IDEN"),
            new GrammarRule("ARG10", "ASIG", "ARG01", "OA01", "ARG01"),
            new GrammarRule("S", "DECL", " ")


        ];
    }
}
