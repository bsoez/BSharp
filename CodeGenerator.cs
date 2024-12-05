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

            // Diccionario para manejar variables declaradas
            Dictionary<string, string> variables = new();
            int msgCounter = 0;
            int labelCounter = 0;

            string[] lines = sourceCode.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Primera pasada: Declaraciones
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Declaración de variables
                if (trimmedLine.StartsWith("ENTERO "))
                {
                    var parts = trimmedLine.Split(new[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
                    string variableName = parts[1];
                    string initialValue = parts.Length > 2 ? parts[2] : "0";
                    variables[variableName] = "db";
                    assemblyCode.AppendLine($"{variableName} db {initialValue}");
                }
                else if (trimmedLine.StartsWith("REAL "))
                {
                    var parts = trimmedLine.Split(new[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
                    string variableName = parts[1];
                    string initialValue = parts.Length > 2 ? parts[2] : "0.0";
                    variables[variableName] = "db"; // Representamos los reales como cadenas
                    assemblyCode.AppendLine($"{variableName} db '{initialValue}', '$'");
                }
                else if (trimmedLine.StartsWith("CADENA "))
                {
                    var parts = trimmedLine.Split(new[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
                    string variableName = parts[1];
                    string initialValue = parts.Length > 2 ? parts[2].Trim('"') : "";
                    variables[variableName] = "db";
                    assemblyCode.AppendLine($"{variableName} db '{initialValue}', '$'");
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

                // Mostrar mensaje (MOSTRAR)
                if (trimmedLine.StartsWith("MOSTRAR "))
                {
                    var content = trimmedLine.Substring(9).Trim().Trim('(', ')');
                    string msgName = $"msg{msgCounter++}";
                    assemblyCode.AppendLine($"{msgName} db '{content}', '$'");
                    assemblyCode.AppendLine("mov ah,09");
                    assemblyCode.AppendLine($"lea dx, {msgName}");
                    assemblyCode.AppendLine("int 21h");
                }

                // Leer variable (LEER)
                else if (trimmedLine.StartsWith("LEER "))
                {
                    var variable = trimmedLine.Substring(6).Trim().Trim('(', ')');
                    assemblyCode.AppendLine("mov ah,01");
                    assemblyCode.AppendLine("int 21h");
                    assemblyCode.AppendLine("sub al,30h");
                    assemblyCode.AppendLine($"mov {variable}, al");
                }

                // Condicional SI/SINO
                else if (trimmedLine.StartsWith("SI "))
                {
                    int conditionStart = trimmedLine.IndexOf('(') + 1;
                    int conditionEnd = trimmedLine.IndexOf(')');
                    string condition = trimmedLine.Substring(conditionStart, conditionEnd - conditionStart).Trim();

                    // Dividir la condición en partes
                    var conditionParts = condition.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    // Validar que haya exactamente 3 partes: variable, operador, valor
                    if (conditionParts.Length != 3)
                    {
                        throw new Exception($"Error en la condición: {condition}. La condición debe tener el formato [variable] [operador] [valor].");
                    }

                    string variable = conditionParts[0];
                    string operatorType = conditionParts[1];
                    string value = conditionParts[2];

                    // Validar el operador
                    if (operatorType is not (">" or "<" or "==" or "!="))
                    {
                        throw new Exception($"Operador no soportado: {operatorType}");
                    }

                    string elseLabel = $"ELSE{labelCounter}";
                    string endLabel = $"ENDIF{labelCounter++}";

                    // Generar instrucciones para la condición
                    assemblyCode.AppendLine($"mov al, {variable}"); // Cargar la variable en AL
                    assemblyCode.AppendLine($"cmp al, {value}");    // Comparar AL con el valor

                    // Generar el salto condicional adecuado
                    switch (operatorType)
                    {
                        case ">": assemblyCode.AppendLine($"jle {elseLabel}"); break;
                        case "<": assemblyCode.AppendLine($"jge {elseLabel}"); break;
                        case "==": assemblyCode.AppendLine($"jne {elseLabel}"); break;
                        case "!=": assemblyCode.AppendLine($"je {elseLabel}"); break;
                    }

                    // Generar etiquetas para el bloque SI/SINO
                    assemblyCode.AppendLine($"{elseLabel}:");
                    assemblyCode.AppendLine($"{endLabel}:");
                }


                // Asignaciones (ENTONCES y otras)
                else if (trimmedLine.StartsWith("ENTONCES "))
                {
                    var parts = trimmedLine.Substring(9).Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    string variable = parts[0].Trim();
                    string value = parts[1].Trim();
                    assemblyCode.AppendLine($"mov {variable}, {value}");
                }

                // DEVUELVE
                else if (trimmedLine.StartsWith("DEVUELVE "))
                {
                    var variable = trimmedLine.Substring(9).Trim();
                    assemblyCode.AppendLine("mov ah,09");
                    assemblyCode.AppendLine($"lea dx, {variable}");
                    assemblyCode.AppendLine("int 21h");
                }
            }

            assemblyCode.AppendLine("mov ah,4ch");
            assemblyCode.AppendLine("int 21h");
            assemblyCode.AppendLine(".exit");
            assemblyCode.AppendLine("end");

            return assemblyCode.ToString();
        }




        public void SaveAssemblyToFile(List<string> assemblyCode, string filePath)
        {
            File.WriteAllLines(filePath, assemblyCode);
        }
    }
}
