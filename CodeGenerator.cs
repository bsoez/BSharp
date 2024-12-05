using System.Text;

namespace BSharp
{
    internal class CodeGenerator
    {
        public string GenerateAssembly(string sourceCode, List<Symbol> symbolTable)
        {
            StringBuilder assemblyCode = new();
            assemblyCode.AppendLine(".model small");
            assemblyCode.AppendLine(".stack");
            assemblyCode.AppendLine(".data");

            int msgCounter = 0;
            Dictionary<int, string> messageMap = new();

            // Declarar variables en ensamblador
            foreach (var symbol in symbolTable)
            {
                string assemblyName = $"Numero{symbol.Name}";
                assemblyCode.AppendLine($"{assemblyName} db 0");
            }

            // Declarar mensajes en ensamblador
            string[] lines = sourceCode.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("MOSTRAR ("))
                {
                    string content = line.Substring(9).Trim('(', ')').Trim('"');
                    string msgName = $"msg{msgCounter}";
                    assemblyCode.AppendLine($"{msgName} db 10,13,7, '{content} $'");
                    messageMap[msgCounter] = msgName;
                    msgCounter++;
                }
            }

            assemblyCode.AppendLine(".code");
            assemblyCode.AppendLine(".startup");
            assemblyCode.AppendLine("mov ax, seg @data");
            assemblyCode.AppendLine("mov ds, ax");

            msgCounter = 0; // Reiniciar el contador para procesar las instrucciones

            // Procesar las líneas para generar instrucciones
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("MOSTRAR ("))
                {
                    string content = trimmedLine.Substring(9).Trim('(', ')').Trim();

                    // Verificar si el contenido es una variable declarada
                    Symbol? symbol = symbolTable.FirstOrDefault(s => s.Name == content);

                    if (symbol != null)
                    {
                        // Mostrar el valor de la variable
                        string assemblyName = $"Numero{symbol.Name}";
                        assemblyCode.AppendLine($"mov al, {assemblyName}"); // Cargar el valor de la variable en AL
                        assemblyCode.AppendLine("AAM");                    // Ajustar para mostrar el valor
                        assemblyCode.AppendLine("mov ah,02h");             // Mostrar decenas
                        assemblyCode.AppendLine("mov dl,bh");
                        assemblyCode.AppendLine("add dl,30h");
                        assemblyCode.AppendLine("int 21h");
                        assemblyCode.AppendLine("mov dl,bl");              // Mostrar unidades
                        assemblyCode.AppendLine("add dl,30h");
                        assemblyCode.AppendLine("int 21h");
                    }
                    else
                    {
                        // Mostrar el mensaje literal
                        string msgName = $"msg{msgCounter++}";
                        assemblyCode.AppendLine("mov ah,09");
                        assemblyCode.AppendLine($"lea dx, {msgName}");
                        assemblyCode.AppendLine("int 21h");
                    }
                }
                else if (trimmedLine.StartsWith("LEER ("))
                {
                    string variable = trimmedLine.Substring(6).Trim('(', ')').Trim();
                    Symbol? symbol = symbolTable.FirstOrDefault(s => s.Name == variable);

                    if (symbol is null)
                    {
                        throw new Exception($"La variable '{variable}' no ha sido declarada.");
                    }

                    string assemblyName = $"Numero{symbol.Name}";
                    assemblyCode.AppendLine("mov ah,01");
                    assemblyCode.AppendLine("int 21h");
                    assemblyCode.AppendLine("sub al,30h");
                    assemblyCode.AppendLine($"mov {assemblyName}, al");
                }
                else if (trimmedLine.Contains("="))
                {
                    var parts = trimmedLine.Split(new[] { '=' }, 2, StringSplitOptions.TrimEntries);
                    string variableName = ExtractVariableName(parts[0].Trim());
                    string expression = parts[1].Trim();

                    Symbol? symbol = symbolTable.FirstOrDefault(s => s.Name == variableName);
                    if (symbol is null)
                    {
                        throw new Exception($"La variable '{variableName}' no ha sido declarada.");
                    }

                    string assemblyName = $"Numero{symbol.Name}";
                    string[] expressionParts = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (expressionParts.Length == 1)
                    {
                        if (int.TryParse(expressionParts[0], out _)) // Literal entero
                        {
                            assemblyCode.AppendLine($"mov al, {expressionParts[0]}");
                            assemblyCode.AppendLine($"mov {assemblyName}, al");
                        }
                        else
                        {
                            Symbol? operandSymbol = symbolTable.FirstOrDefault(s => s.Name == expressionParts[0]);
                            if (operandSymbol is null)
                            {
                                throw new Exception($"La variable '{expressionParts[0]}' en la expresión '{expression}' no ha sido declarada.");
                            }

                            assemblyCode.AppendLine($"mov al, Numero{operandSymbol.Name}");
                            assemblyCode.AppendLine($"mov {assemblyName}, al");
                        }
                    }
                    else if (expressionParts.Length == 3)
                    {
                        string operand1 = expressionParts[0];
                        string operatorType = expressionParts[1];
                        string operand2 = expressionParts[2];

                        Symbol? operand1Symbol = symbolTable.FirstOrDefault(s => s.Name == operand1);
                        Symbol? operand2Symbol = symbolTable.FirstOrDefault(s => s.Name == operand2);

                        string operand1Assembly = operand1Symbol != null ? $"Numero{operand1Symbol.Name}" : operand1;
                        string operand2Assembly = operand2Symbol != null ? $"Numero{operand2Symbol.Name}" : operand2;

                        if (operatorType != "+")
                        {
                            throw new Exception($"Operador no soportado: '{operatorType}'.");
                        }

                        assemblyCode.AppendLine($"mov al, {operand1Assembly}");
                        assemblyCode.AppendLine($"add al, {operand2Assembly}");
                        assemblyCode.AppendLine($"mov {assemblyName}, al");
                    }
                    else
                    {
                        throw new Exception($"Expresión inválida en '{trimmedLine}'.");
                    }
                }
            }

            // Finalizar programa
            assemblyCode.AppendLine("mov ah,4ch");
            assemblyCode.AppendLine("int 21h");
            assemblyCode.AppendLine(".exit");
            assemblyCode.AppendLine("end");

            return assemblyCode.ToString();
        }

        private string ExtractVariableName(string leftSide)
        {
            // Si el lado izquierdo contiene un tipo, extraer solo el nombre
            if (leftSide.StartsWith("ENTERO ") || leftSide.StartsWith("REAL ") || leftSide.StartsWith("CADENA "))
            {
                return leftSide.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
            }

            // Si no contiene un tipo, retornar directamente
            return leftSide.Trim();
        }


        public void SaveAssemblyToFile(List<string> assemblyCode, string filePath)
        {
            File.WriteAllLines(filePath, assemblyCode);
        }
    }
}
