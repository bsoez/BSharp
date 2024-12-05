using System.Text;

namespace BSharp
{
    internal class CodeGenerator
    {
        public string GenerateAssembly(string sourceCode)
        {
            StringBuilder assemblyCode = new();
            assemblyCode.AppendLine(".model small");
            assemblyCode.AppendLine(".stack");
            assemblyCode.AppendLine(".data");

            // Diccionario para manejar variables y mensajes
            Dictionary<string, string> variables = [];
            int msgCounter = 0;

            string[] lines = sourceCode.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Primera pasada: Declaración de variables y mensajes
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("ENTERO "))
                {
                    var parts = trimmedLine.Split(new[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
                    string variableName = parts[1];
                    variables[variableName] = "db";
                    assemblyCode.AppendLine($"{variableName} db 0"); // Inicializar como 0
                }
                else if (trimmedLine.StartsWith("MOSTRAR ("))
                {
                    string content = trimmedLine[9..].Trim('(', ')').Trim('"');
                    string msgName = $"msg{msgCounter++}";
                    assemblyCode.AppendLine($"{msgName} db '{content}', '$'");
                }
            }

            assemblyCode.AppendLine(".code");
            assemblyCode.AppendLine(".startup");
            assemblyCode.AppendLine("mov ax, seg @data");
            assemblyCode.AppendLine("mov ds, ax");

            // Segunda pasada: Instrucciones
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("MOSTRAR ("))
                {
                    string msgName = $"msg{msgCounter - 1}";
                    assemblyCode.AppendLine("mov ah,09");
                    assemblyCode.AppendLine($"lea dx, {msgName}");
                    assemblyCode.AppendLine("int 21h");
                }
                else if (trimmedLine.StartsWith("LEER ("))
                {
                    string variable = trimmedLine[6..].Trim('(', ')');
                    assemblyCode.AppendLine("mov ah,01");
                    assemblyCode.AppendLine("int 21h");
                    assemblyCode.AppendLine("sub al,30h");
                    assemblyCode.AppendLine($"mov {variable}, al");
                }
                else if (trimmedLine.StartsWith("ENTERO "))
                {
                    // Ya procesado en la sección de .data
                    continue;
                }
                else if (trimmedLine.Contains(" = ") && trimmedLine.Contains(" + "))
                {
                    string[] parts = trimmedLine.Split(new[] { "=", "+" }, StringSplitOptions.RemoveEmptyEntries);
                    string target = parts[0].Trim();
                    string operand1 = parts[1].Trim();
                    string operand2 = parts[2].Trim();

                    assemblyCode.AppendLine($"mov al, {operand1}");
                    assemblyCode.AppendLine($"add al, {operand2}");
                    assemblyCode.AppendLine($"mov {target}, al");
                }
            }

            // Finalizar programa
            assemblyCode.AppendLine("mov ah,4ch");
            assemblyCode.AppendLine("int 21h");
            assemblyCode.AppendLine(".exit");
            assemblyCode.AppendLine("end");

            return assemblyCode.ToString();
        }
    }
}
