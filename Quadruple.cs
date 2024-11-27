namespace BSharp
{
    internal class Quadruple(string op, string op1, string op2, string res)
    {
        public string Operator { get; set; } = op;
        public string Operand1 { get; set; } = op1;
        public string Operand2 { get; set; } = op2;
        public string Result { get; set; } = res;
    }
}
