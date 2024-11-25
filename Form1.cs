using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace BSharp
{
    public partial class Form1 : Form
    {
        private static readonly string[] palabrasReservadas = ["CADENA", "DEVUELVE", "ENTERO", "ENTONCES", "FALSO", "FIN", "INICIO", "LEER", "MIENTRAS", "MOSTRAR", "REAL", "SI", "SINO", "VERDADERO"];
        private readonly string[] TiposDeDatos = ["CADENA", "ENTERO", "REAL"];
        private static readonly char[] separatorArray = [' '];
        private static DataTable dtMatriz = new();

        public Form1()
        {
            InitializeComponent();
            Initialization();
            Connection();
        }

        private void Initialization()
        {
            dgvMatriz.AllowUserToAddRows = false;
            dgvMatriz.AllowUserToDeleteRows = false;
            dgvMatriz.AllowUserToOrderColumns = false;
            dgvMatriz.AllowUserToResizeColumns = false;
            dgvMatriz.AllowUserToResizeRows = false;
            dgvMatriz.ReadOnly = true;
        }

        private void Connection()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            int maxNivelesAtras = 10;
            string? filePath = currentDirectory;
            for (int i = 0; i < maxNivelesAtras; i++)
            {
                filePath = Path.GetDirectoryName(filePath);
                if (filePath is null)
                {
                    MessageBox.Show("Error: No se puede subir más niveles en la jerarquía de directorios.");
                    return;
                }
                string candidatePath = Path.Combine(filePath, "matrizBuena.xlsx");
                if (File.Exists(candidatePath))
                {
                    filePath = candidatePath;
                    break;
                }
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Error: El archivo no se encontró en los directorios especificados.");
                return;
            }

            // Cambiamos HDR a 'YES' para que el primer renglón sea interpretado como encabezado de columna
            string cnn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1};IMEX={2}'";
            cnn = string.Format(cnn, filePath, "YES", "1");

            OleDbConnection excelConnection = new(cnn);
            excelConnection.Open();

            DataTable? dtLexico = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dtLexico is not null)
            {
                string? sheet = dtLexico.Rows[0]["TABLE_NAME"].ToString();
                OleDbCommand command = new("SELECT * FROM [" + sheet + "]", excelConnection);
                OleDbDataAdapter adapter = new(command);
                DataTable dt = new();
                adapter.Fill(dt);

                // no necesitamos remover las columnas
                dt.Columns.RemoveAt(dt.Columns.Count - 1);
                dt.Columns.RemoveAt(0);

                dt.Columns["#"]!.ColumnName = ".";

                dt.Columns["("]!.ColumnName = "[";
                dt.Columns[")"]!.ColumnName = "]";
                dt.Columns["_"]!.ColumnName = "!";

                dt.Columns["(1"]!.ColumnName = "(";
                dt.Columns[")1"]!.ColumnName = ")";
                dt.Columns["_1"]!.ColumnName = "_";
                dt.Columns["#1"]!.ColumnName = "#";

                dt.Columns["f11"]!.ColumnName = "f1";

                dtMatriz = dt;
                excelConnection.Close();
                dgvMatriz.DataSource = dt;
            }
        }
        

        public void Lexema(string InputTokens, string OutputTokens)
        {
            // Crear el DataTable con columnas adecuadas
            DataTable dtLexemas = new();
            dtLexemas.Columns.Add("Renglon", typeof(int));  // Número de renglón
            dtLexemas.Columns.Add("Columna", typeof(int));  // Posición dentro del renglón
            dtLexemas.Columns.Add("InputToken", typeof(string));  // Token del Input
            dtLexemas.Columns.Add("OutputToken", typeof(string)); // Token del Output

            // Separar las líneas de los textos de entrada
            string[] inputLines = InputTokens.Split(["\r\n", "\n"], StringSplitOptions.None);
            string[] outputLines = OutputTokens.Split(["\r\n", "\n"], StringSplitOptions.None);

            // Procesar cada renglón
            for (int i = 0; i < inputLines.Length; i++)
            {
                // Separar los tokens en cada renglón utilizando un carácter como delimitador
                string[] inputTokens = inputLines[i].Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
                string[] outputTokens = outputLines[i].Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);

                // Asegurar que ambos renglones tengan la misma cantidad de tokens
                if (inputTokens.Length != outputTokens.Length)
                {
                    throw new Exception("Error: Los tokens de entrada y salida no coinciden en el renglón " + (i + 1));
                }

                // Procesar cada token en el renglón
                for (int j = 0; j < inputTokens.Length; j++)
                {
                    DataRow row = dtLexemas.NewRow();
                    row["Renglon"] = i + 1;  // Número de renglón
                    row["Columna"] = j + 1;  // Posición en la columna
                    row["InputToken"] = inputTokens[j];  // Token en Input
                    row["OutputToken"] = outputTokens[j]; // Token en Output
                    dtLexemas.Rows.Add(row);
                }
            }

            foreach (DataRow row in dtLexemas.Rows)
            {
                string? inputToken = row["InputToken"].ToString();
                string? outputToken = row["OutputToken"].ToString();
                int renglonActual = (int)row["Renglon"];
                int columnaActual = (int)row["Columna"];
                if (outputToken is "IDEN" && columnaActual > 1)
                {
                    DataRow? columnaAnterior = dtLexemas.AsEnumerable().FirstOrDefault(r => (int)r["Renglon"] == renglonActual && (int)r["Columna"] == columnaActual - 1);
                    if (columnaAnterior != null)
                    {
                        string? inputTokenAnterior = columnaAnterior["InputToken"].ToString();
                        if (TiposDeDatos.Contains(inputTokenAnterior))
                            dgvTablaSimbolos.Rows.Add(inputToken, outputToken, inputTokenAnterior);
                    }
                }
                else if (outputToken is "NUEN")
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "ENTERO");
                // Caso 3: Si el OutputToken es 'NUDE'
                else if (outputToken is "NUDE")
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "REAL");
                // Caso 4: Si el OutputToken es 'CADE'
                else if (outputToken is "CADE")
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "CADENA");
            }
        }
        private void btnCompilar_Click(object sender, EventArgs e) 
        {
            dgvTablaSimbolos.Rows.Clear();
            txtOutput.Clear();

            bool successLexico = AnalizadorLexico();
            if (successLexico)
                Lexema(txtInput.Text, txtOutput.Text);

            bool successSintactico = AnalizadorSintactico();
            if (successSintactico)
            {

            }
            AnalizadorSemantico(txtInput.Text);
        }

        private void txtInput_TextChanged(object sender, EventArgs e) { ChangeColorWord(); }

        public bool AnalizadorLexico()
        {
            txtOutput.Clear();
            string TokensLexico = txtInput.Text;
            if (!string.IsNullOrEmpty(TokensLexico))
            {
                StringBuilder outputBuilder = new();
                StringBuilder currentToken = new(); 

                foreach (char c in TokensLexico)
                {
                    if (char.IsWhiteSpace(c)) 
                    {
                        if (currentToken.Length > 0)
                        {
                            string Result = BSharp.Lexico.AnalizarPalabra(currentToken.ToString(), dtMatriz);
                            outputBuilder.Append(Result); 
                            currentToken.Clear(); 
                        }
                        outputBuilder.Append(c);
                    }
                    else
                        currentToken.Append(c);
                }

                if (currentToken.Length > 0)
                {
                    string Result = BSharp.Lexico.AnalizarPalabra(currentToken.ToString(), dtMatriz);
                    outputBuilder.Append(Result);
                }

                txtOutput.Text = outputBuilder.ToString();
                return true;
            }

            return false;
        }

        #region Methods

        private void ChangeColorWord()
        {
            string pattern = "";
            foreach (string palabrasReservada in palabrasReservadas) pattern += palabrasReservada + "|";

            Regex regex = new(pattern);
            int index = txtInput.SelectionStart;
            foreach (Match match in regex.Matches(txtInput.Text).Cast<Match>())
            {
                txtInput.Select(match.Index, match.Value.Length);
                txtInput.SelectionColor = Color.Blue;
                txtInput.SelectionStart = index;
            }
            txtInput.SelectionColor = Color.Black;
        }
        #endregion

        #region Análisis Sintáctico
        ///Bottom-up
        public bool AnalizadorSintactico()
        {
            txtSintaxis.Clear();
            string[] sintaxisLines = txtOutput.Text.Split('\n');
            int numLinea = 1;
            foreach (string line in sintaxisLines)
            {
                List<string> lines = [.. line.Split(' ')];
                lines.Add(" ");
                string x = AnalyzeBottomUp(lines, numLinea);
                txtSintaxis.Text += x + "\n";
                numLinea++;
            }
            _ = txtSintaxis.Text.TrimEnd('\n');
            return true;
        }

        public static string AnalyzeBottomUp(List<string> tokens, int numLinea)
        {
            StringBuilder sb = new($"[{numLinea}] " + string.Join(" ", tokens) + "\n");
            Stack<string> stack = new();
            string text;
            while (tokens.Count > 0)
            {
                string token = tokens[0];
                tokens.RemoveAt(0);
                stack.Push(token);

                while (CanReduce(stack))
                {
                    string stackBeforeReduce = string.Join(" ", stack.Reverse());
                    string? reducedSymbol = Reduce(stack);
                    if(reducedSymbol is not null)
                    {
                        stack.Push(reducedSymbol);
                        string stackWaiting = (stackBeforeReduce + " " + string.Join(" ", tokens)).Trim();
                        text = stackWaiting.Replace(stackBeforeReduce, string.Join(" ", stack.Reverse()));
                        if(stack.Count == 1 && stack.Peek() == "S")
                            sb.AppendLine($"[{numLinea}] S");
                        else
                            sb.AppendLine($"[{numLinea}] {text}");
                    }
                }
            }
            if(stack.Count is 1 && stack.Peek() is "S")
                return sb.ToString();
            else
                sb.AppendLine($"[{numLinea}] ERROR");
            return sb.ToString();
        }

        private static bool CanReduce(Stack<string> stack)
        {
            string[] stackArray = [.. stack];
            foreach (GrammarRule rule in GrammarAnalyzer.grammarRules)
            {
                if (stackArray.Length >= rule.RightSide.Length)
                {
                    string[] subArray = stackArray[..rule.RightSide.Length];
                    if (subArray.SequenceEqual(rule.RightSide.Reverse()))
                        return true;
                }
            }
            return false;
        }

        private static string? Reduce(Stack<string> stack)
        {
            string[] stackArray = [.. stack];

            foreach (GrammarRule rule in GrammarAnalyzer.grammarRules)
            {
                if (stackArray.Length >= rule.RightSide.Length)
                {
                    string[] subArray = stackArray[..rule.RightSide.Length];
                    if (subArray.SequenceEqual(rule.RightSide.Reverse()))
                    {
                        for (int i = 0; i < rule.RightSide.Length; i++)
                            stack.Pop();
                        return rule.LeftSide;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Análisis Semántico

        //Analizador Semántico
        public void AnalizadorSemantico(string code)
        {
            txtErrors.Clear();
            SymbolTable.Clear();

            AnalyzeBalanceInicioFin(code);
            AnalyzeParenthesesBalance(code);

            // Dividir el código en líneas basadas en saltos de línea
            string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);


            bool foundInicio = false;
            bool foundFin = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // Detectar palabras reservadas especiales
                if (line == "INICIO" && !foundInicio)
                {
                    WriteToConsole("Inicio del programa detectado.", "Info");
                    foundInicio = true;
                    continue;
                }

                if (line == "FIN" && !foundFin)
                {
                    WriteToConsole("Fin del programa detectado.", "Info");
                    foundFin = true;
                    continue;
                }

                // Detectar declaraciones
                if (line.StartsWith("ENTERO ") || line.StartsWith("REAL ") || line.StartsWith("CADENA "))
                {
                    AnalyzeDeclaration(line, i + 1); // Enviar línea actual
                    continue;
                }

                if (line.StartsWith("DEVUELVE "))
                {
                    AnalyzeReturn(line, i + 1);
                    continue;
                }

                if (line.StartsWith("LEER ("))
                {
                    AnalyzeRead(line, i + 1);
                    continue;
                }

                if (line.StartsWith("MOSTRAR ("))
                {
                    AnalyzePrint(line, i + 1);
                    continue;
                }

                if (line.StartsWith("SI ("))
                {
                    AnalyzeIfElse(line, i + 1);
                    continue;
                }

                if (line.StartsWith("SINO "))
                {
                    AnalyzeIfElse(line, i + 1, true);
                    continue;
                }

                if (line.StartsWith("ENTONCES "))
                {
                    AnalyzeThen(line, i + 1);
                    continue;
                }

                // Detectar asignaciones
                //if (line.Contains('='))
                //{
                //    AnalyzeAssignment(line, i + 1); // Enviar línea actual
                //    continue;
                //}

                // Si no coincide con ningún patrón conocido
                WriteToConsole($"Sintaxis inválida en la línea '{line}'.", "Error", i + 1);
            }
        }

        public void AnalyzeAssignment(string codeLine, int lineNumber)
        {
            // Dividir la línea en partes por el signo "="
            var parts = codeLine.Split('=', 2, StringSplitOptions.TrimEntries);

            // Validar que la línea tenga exactamente 2 partes: identificador y expresión
            if (parts.Length != 2)
            {
                WriteToConsole($"Sintaxis inválida en la asignación '{codeLine.Trim()}'.", "Error", lineNumber);
                return;
            }

            string variableName = parts[0]; // Antes del '='
            string value = parts[1];        // Después del '='

            // Buscar la variable en la tabla de símbolos
            var symbol = SymbolTable.FirstOrDefault(s => s.Name == variableName);
            if (symbol == null)
            {
                WriteToConsole($"La variable '{variableName}' no ha sido declarada.", "Error", lineNumber);
                return;
            }

            // Validar el valor asignado según el tipo de la variable
            switch (symbol.Type)
            {
                case "ENTERO":
                    if (!IsIntegerExpression(value))
                    {
                        WriteToConsole($"El valor '{value}' no es válido para la variable '{variableName}' de tipo 'ENTERO'.", "Error", lineNumber);
                        return;
                    }
                    break;

                case "REAL":
                    if (!IsRealExpression(value))
                    {
                        WriteToConsole($"El valor '{value}' no es válido para la variable '{variableName}' de tipo 'REAL'.", "Error", lineNumber);
                        return;
                    }
                    break;

                case "CADENA":
                    if (!IsStringExpression(value))
                    {
                        WriteToConsole($"El valor '{value}' no es válido para la variable '{variableName}' de tipo 'CADENA'.", "Error", lineNumber);
                        return;
                    }
                    break;
            }

            // Si pasa todas las validaciones
            WriteToConsole($"Asignación válida: {variableName} = {value}.", "Success");
        }

        //Analizador de declaraciones
        internal static List<Symbol> SymbolTable = [];
        public void AnalyzeDeclaration(string codeLine, int lineNumber)
        {
            // Dividir la línea en partes por espacios
            var parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Validar que la línea tenga al menos 4 partes: tipo, identificador, asignación, valor
            if (parts.Length < 4 || parts[2] != "=")
            {
                WriteToConsole($"Sintaxis inválida en la declaración '{codeLine.Trim()}'.", "Error", lineNumber);
                return;
            }

            string type = parts[0];        // Tipo de dato: ENTERO, REAL, CADENA
            string variableName = parts[1]; // Identificador (variable)
            string value = string.Join(" ", parts.Skip(3)); // Valor asignado (puede incluir operaciones)

            // Verificar que el tipo sea válido
            if (type is not ("ENTERO" or "REAL" or "CADENA"))
            {
                WriteToConsole($"Tipo de dato '{type}' no reconocido en la declaración '{codeLine.Trim()}'.", "Error", lineNumber);
                return;
            }

            // Verificar que el identificador no esté ya declarado
            if (SymbolTable.Any(s => s.Name == variableName))
            {
                WriteToConsole($"La variable '{variableName}' ya fue declarada.", "Error", lineNumber);
                return;
            }

            // Validar el valor asignado según el tipo de dato
            switch (type)
            {
                case "ENTERO":
                    if (!IsIntegerExpression(value))
                    {
                        WriteToConsole($"La expresión '{value}' no es válida para una variable de tipo 'ENTERO'.", "Error", lineNumber);
                        return;
                    }
                    break;

                case "REAL":
                    if (!IsRealExpression(value))
                    {
                        WriteToConsole($"La expresión '{value}' no es válida para una variable de tipo 'REAL'.", "Error", lineNumber);
                        return;
                    }
                    break;

                case "CADENA":
                    if (!IsStringExpression(value))
                    {
                        WriteToConsole($"La expresión '{value}' no es válida para una variable de tipo 'CADENA'.", "Error", lineNumber);
                        return;
                    }
                    break;
            }

            // Agregar la variable a la tabla de símbolos
            SymbolTable.Add(new Symbol(variableName, type, true));
            WriteToConsole($"Declaración válida: {type} {variableName} = {value}.", "Success");
        }

        private static bool IsIntegerExpression(string expression)
        {
            // Dividir la expresión por espacios para separar operandos y operadores
            var tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                if (i % 2 == 0) // Operando (número)
                {
                    if (!int.TryParse(tokens[i], out _)) // Validar que sea un entero
                    {
                        return false;
                    }
                }
                else // Operador
                {
                    if (tokens[i] is not ("+" or "-" or "*" or "/")) // Validar operadores válidos
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsRealExpression(string expression)
        {
            // Dividir la expresión por espacios para separar operandos y operadores
            var tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                if (i % 2 == 0) // Operando (número)
                {
                    if (!float.TryParse(tokens[i], out _)) // Validar que sea un número real
                    {
                        return false;
                    }
                }
                else // Operador
                {
                    if (tokens[i] is not ("+" or "-" or "*" or "/")) // Validar operadores válidos
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsStringExpression(string expression)
        {
            // Validar que la expresión completa esté entre comillas dobles
            return expression.StartsWith('"') && expression.EndsWith('"');
        }

        public void AnalyzeBalanceInicioFin(string code)
        {
            // Dividir el código en líneas para analizarlas una por una
            string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int inicioCount = 0;
            int finCount = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmedLine = lines[i].Trim();

                // Contar ocurrencias de INICIO y FIN
                if (trimmedLine == "INICIO")
                {
                    inicioCount++;

                    // Verificar si hay más de un INICIO
                    if (inicioCount > 1)
                    {
                        WriteToConsole($"Más de un 'INICIO' encontrado.", "Error", i + 1);
                        return;
                    }
                }
                else if (trimmedLine == "FIN")
                {
                    finCount++;

                    // Verificar si FIN aparece antes de INICIO
                    if (inicioCount == 0)
                    {
                        WriteToConsole($"'FIN' encontrado antes de 'INICIO'.", "Error", i + 1);
                        return;
                    }

                    // Verificar si hay más de un FIN
                    if (finCount > 1)
                    {
                        WriteToConsole($"Más de un 'FIN' encontrado.", "Error", i + 1);
                        return;
                    }
                }
            }

            // Verificar si falta INICIO o FIN al final del análisis
            if (inicioCount == 0)
            {
                WriteToConsole("No se encontró 'INICIO' en el código.", "Error");
                return;
            }
            if (finCount == 0)
            {
                WriteToConsole("No se encontró 'FIN' en el código.", "Error");
                return;
            }

            // Si todo está bien
            WriteToConsole("'INICIO' y 'FIN' están correctamente equilibrados.", "Success");
        }

        private void AnalyzeReturn(string codeLine, int lineNumber)
        {
            // Validar que tenga un identificador después de 'DEVUELVE'
            var parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'DEVUELVE'.", "Error", lineNumber);
                return;
            }

            string variableName = parts[1];
            var symbol = SymbolTable.FirstOrDefault(s => s.Name == variableName);
            if (symbol == null)
            {
                WriteToConsole($"La variable '{variableName}' no ha sido declarada.", "Error", lineNumber);
                return;
            }

            WriteToConsole($"Instrucción válida: DEVUELVE {variableName}.", "Success");
        }

        private void AnalyzeRead(string codeLine, int lineNumber)
        {
            if (!codeLine.StartsWith("LEER (") || !codeLine.EndsWith(")"))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'LEER'.", "Error", lineNumber);
                return;
            }

            string variableName = codeLine[6..^1].Trim(); // Extraer lo que está dentro de los paréntesis
            var symbol = SymbolTable.FirstOrDefault(s => s.Name == variableName);
            if (symbol == null)
            {
                WriteToConsole($"La variable '{variableName}' no ha sido declarada.", "Error", lineNumber);
                return;
            }

            WriteToConsole($"Instrucción válida: LEER({variableName}).", "Success");
        }

        private void AnalyzePrint(string codeLine, int lineNumber)
        {
            if (!codeLine.StartsWith("MOSTRAR (") || !codeLine.EndsWith(")"))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'MOSTRAR'.", "Error", lineNumber);
                return;
            }

            string content = codeLine[9..^1].Trim(); // Extraer lo que está dentro de los paréntesis
            if (string.IsNullOrWhiteSpace(content))
            {
                WriteToConsole($"El contenido de 'MOSTRAR' no puede estar vacío.", "Error", lineNumber);
                return;
            }

            WriteToConsole($"Instrucción válida: MOSTRAR({content}).", "Success");
        }

        private void AnalyzeIfElse(string codeLine, int lineNumber, bool isElse = false)
        {
            if (isElse)
            {
                // Validar 'SINO'
                var parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3 || parts[0] is not "SINO")
                {
                    WriteToConsole($"Sintaxis inválida en la instrucción 'SINO'.", "Error", lineNumber);
                    return;
                }

                string elseAssignment = string.Join(" ", parts.Skip(1)); // Cambiar el nombre para evitar conflicto
                AnalyzeAssignment(elseAssignment, lineNumber);
                return;
            }

            // Validar 'SI'
            if (!codeLine.StartsWith("SI (") || !codeLine.Contains(")"))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'SI'.", "Error", lineNumber);
                return;
            }

            // Encontrar la posición del paréntesis de cierre
            int conditionEndIndex = codeLine.IndexOf(")") + 1;

            // Extraer la condición dentro de los paréntesis
            string condition = codeLine.Substring(3, conditionEndIndex - 4).Trim();

            // Extraer la asignación después del paréntesis de cierre
            string thenAssignment = codeLine.Substring(conditionEndIndex).Trim();

            // Validar la condición
            if (string.IsNullOrWhiteSpace(condition))
            {
                WriteToConsole($"La condición en la instrucción 'SI' no puede estar vacía.", "Error", lineNumber);
                return;
            }
            WriteToConsole($"Instrucción válida: SI({condition}).", "Success");

            // Validar la asignación
            AnalyzeAssignment(thenAssignment, lineNumber);
        }

        private void AnalyzeThen(string codeLine, int lineNumber)
        {
            // Validar que la línea comience con "ENTONCES"
            if (!codeLine.StartsWith("ENTONCES "))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'ENTONCES'.", "Error", lineNumber);
                return;
            }

            // Extraer el resto de la línea después de "ENTONCES"
            string assignment = codeLine.Substring(9).Trim();

            // Validar que la asignación no esté vacía
            if (string.IsNullOrWhiteSpace(assignment))
            {
                WriteToConsole($"La instrucción 'ENTONCES' debe incluir una asignación válida.", "Error", lineNumber);
                return;
            }

            // Analizar la asignación (reutilizamos el método de asignaciones)
            AnalyzeAssignment(assignment, lineNumber);
        }

        public void AnalyzeParenthesesBalance(string code)
        {
            // Dividir el código en líneas
            string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            Stack<int> parenthesesStack = new(); // Para rastrear los paréntesis abiertos

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                for (int j = 0; j < line.Length; j++)
                {
                    char currentChar = line[j];

                    // Abrir paréntesis
                    if (currentChar == '(')
                    {
                        parenthesesStack.Push(i + 1); // Guardar la línea donde se abrió
                    }

                    // Cerrar paréntesis
                    else if (currentChar == ')')
                    {
                        if (parenthesesStack.Count == 0)
                        {
                            // Error: paréntesis de cierre sin apertura
                            WriteToConsole($"Error: Paréntesis de cierre ')' sin apertura en la línea {i + 1}.", "Error");
                            return;
                        }
                        parenthesesStack.Pop();
                    }
                }
            }

            // Verificar si quedaron paréntesis abiertos
            while (parenthesesStack.Count > 0)
            {
                int lineWithUnclosedParenthesis = parenthesesStack.Pop();
                WriteToConsole($"Error: Paréntesis de apertura '(' sin cierre en la línea {lineWithUnclosedParenthesis}.", "Error");
            }

            // Si no hubo errores
            if (parenthesesStack.Count == 0)
            {
                WriteToConsole("Todos los paréntesis están correctamente equilibrados.", "Success");
            }
        }

        //Consola de errores
        public void WriteToConsole(string message, string messageType = "Info", int? lineNumber = null)
        {
            // Incluir el número de línea en el mensaje si es un error
            string lineInfo = messageType == "Error" && lineNumber.HasValue ? $" (Línea {lineNumber})" : "";
            string formattedMessage = $"[{DateTime.Now:HH:mm:ss}] [{messageType}] {message}{lineInfo}\n";

            if (txtErrors.InvokeRequired)
            {
                txtErrors.Invoke(new Action(() => WriteToConsole(message, messageType, lineNumber)));
            }
            else
            {
                txtErrors.SelectionStart = txtErrors.TextLength;
                txtErrors.SelectionLength = 0;

                // Configuración del color según el tipo de mensaje
                if (messageType is "Error")
                    txtErrors.SelectionColor = Color.Red;
                else if (messageType is "Success")
                    txtErrors.SelectionColor = Color.Green;
                else
                    txtErrors.SelectionColor = Color.Black;

                // Añadir el mensaje al TextBox
                txtErrors.AppendText(formattedMessage);
                txtErrors.SelectionColor = txtErrors.ForeColor;
                txtErrors.ScrollToCaret();
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            WriteToConsole("Aplicación iniciada correctamente.");
            WriteToConsole("Se encontró un error inesperado.", "Error");
            WriteToConsole("Proceso completado exitosamente.", "Success");
            return;
            string miString = txtOutput.Text;
            // Resto del código para generar el archivo de texto
            string rutaArchivo = "C:\\Users\\belen\\Desktop\\miArchivo.txt";

            try
            {
                using (StreamWriter sw = new(rutaArchivo))
                {
                    sw.Write(miString);
                }

                MessageBox.Show("Se ha generado correctamente el archivo .txt");
            }
            catch (Exception)
            {
                MessageBox.Show("Error en el archivo .txt");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvTablaSimbolos.Rows.Count is 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso");
                return;
            }

            try
            {
                using (StreamWriter sw = new("C:\\Users\\belen\\Desktop\\DATOS.txt"))
                {
                    for (int i = 0; i < dgvTablaSimbolos.Columns.Count; i++)
                    {
                        sw.Write(dgvTablaSimbolos.Columns[i].HeaderText);
                        if (i < dgvTablaSimbolos.Columns.Count - 1)
                            sw.Write("\t");
                    }
                    sw.WriteLine();
                    foreach (DataGridViewRow row in dgvTablaSimbolos.Rows)
                    {
                        for (int i = 0; i < dgvTablaSimbolos.Columns.Count; i++)
                        {
                            sw.Write(row.Cells[i].Value);
                            if (i < dgvTablaSimbolos.Columns.Count - 1)
                                sw.Write("\t");
                        }
                        sw.WriteLine();
                    }
                }

                MessageBox.Show("Archivo de texto generado con éxito.", "Éxito");
            }
            catch (Exception ex) { MessageBox.Show("Error al generar el archivo de texto: " + ex.Message, "Error"); }
        }
    }
}
