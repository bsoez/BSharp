namespace BSharp
{
    internal class CodeGenerator
    {
        public List<string> GenerateAssembly(List<Quadruple> quadruples)
        {
            List<string> assemblyCode = [];
            Dictionary<string,string> registerMap = []; // Mapear variables temporales a registros

            foreach (Quadruple quad in quadruples)
            {
                switch (quad.Operator)
                {
                    case "+":
                        assemblyCode.Add($"MOV AX, {quad.Operand1}");
                        assemblyCode.Add($"ADD AX, {quad.Operand2}");
                        assemblyCode.Add($"MOV {quad.Result}, AX");
                        break;

                    case "-":
                        assemblyCode.Add($"MOV AX, {quad.Operand1}");
                        assemblyCode.Add($"SUB AX, {quad.Operand2}");
                        assemblyCode.Add($"MOV {quad.Result}, AX");
                        break;

                    case "=":
                        assemblyCode.Add($"MOV {quad.Result}, {quad.Operand1}");
                        break;

                    case ">":
                        assemblyCode.Add($"MOV AX, {quad.Operand1}");
                        assemblyCode.Add($"CMP AX, {quad.Operand2}");
                        break;

                    case "IF":
                        assemblyCode.Add($"JLE {quad.Result}");
                        break;

                    case "LABEL":
                        assemblyCode.Add($"{quad.Result}:");
                        break;

                    default:
                        throw new Exception($"Operador no reconocido: {quad.Operator}");
                }
            }
            return assemblyCode;
        }

        public void SaveAssemblyToFile(List<string> assemblyCode, string filePath)
        {
            File.WriteAllLines(filePath, assemblyCode);
        }
    }
}
