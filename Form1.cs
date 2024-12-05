using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BSharp
{
    public partial class Form1 : Form
    {
        private static readonly string[] palabrasReservadas = ["CADENA", "DEVUELVE", "ENTERO", "ENTONCES", "FALSO", "FIN", "INICIO", "LEER", "MIENTRAS", "MOSTRAR", "REAL", "SI", "SINO", "VERDADERO"];
        private readonly string[] TiposDeDatos = ["CADENA", "ENTERO", "REAL"];
        private static readonly char[] separatorArray = [' '];
        private static DataTable dtMatriz = new();
        internal static List<Symbol> SymbolTable = [];

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
            DataTable dtLexemas = new();
            dtLexemas.Columns.Add("Renglon", typeof(int));
            dtLexemas.Columns.Add("Columna", typeof(int));
            dtLexemas.Columns.Add("InputToken", typeof(string));
            dtLexemas.Columns.Add("OutputToken", typeof(string));

            string[] inputLines = InputTokens.Split(["\r\n", "\n"], StringSplitOptions.None);
            string[] outputLines = OutputTokens.Split(["\r\n", "\n"], StringSplitOptions.None);

            for (int i = 0; i < inputLines.Length; i++)
            {
                List<string> inputTokens = TokenizeInput(inputLines[i]);
                List<string> outputTokens = TokenizeInput(outputLines[i]);

                if (inputTokens.Count != outputTokens.Count)
                    throw new Exception("Error: Los tokens de entrada y salida no coinciden en el renglón " + (i + 1));

                for (int j = 0; j < inputTokens.Count; j++)
                {
                    DataRow row = dtLexemas.NewRow();
                    row["Renglon"] = i + 1;
                    row["Columna"] = j + 1;
                    row["InputToken"] = inputTokens[j];
                    row["OutputToken"] = outputTokens[j];
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
                    if (columnaAnterior is not null)
                    {
                        string? inputTokenAnterior = columnaAnterior["InputToken"].ToString();
                        if (TiposDeDatos.Contains(inputTokenAnterior))
                            dgvTablaSimbolos.Rows.Add(inputToken, outputToken, inputTokenAnterior);
                    }
                }
                else if (outputToken is "NUEN")
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "ENTERO");
                else if (outputToken is "NUDE")
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "REAL");
                else if (outputToken is "CADE")
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "CADENA");
            }
        }

        public static List<string> TokenizeInput(string input)
        {
            string pattern = "\"[^\"]*\"|\\S+";
            Regex regex = new(pattern);

            MatchCollection matches = regex.Matches(input);

            List<string> tokens = [];
            foreach (Match match in matches)
                tokens.Add(match.Value);
            return tokens;
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            dgvTablaSimbolos.Rows.Clear();
            txtOutput.Clear();

            bool successLexico = AnalizadorLexico();
            if (successLexico)
                Lexema(txtInput.Text, txtOutput.Text);

            _ = AnalizadorSintactico();
            AnalizadorSemantico(txtInput.Text);

            List<Quadruple> quadruples = GenerateIntermediateCode(txtInput.Text);
            DisplayIntermediateCode(quadruples, dgvQuadruple);

            CodeGenerator generator = new();
            string assemblyCode = generator.GenerateAssembly(txtInput.Text);

            //foreach (string line in assemblyCode)
            //    txtEnsamblador.Text += line + "\n";

            txtEnsamblador.Text = assemblyCode.Trim('\n');
            txtEnsamblador.Text = txtEnsamblador.Text.Trim();

            //_ = txtEnsamblador.Text.Trim('\n');
            // Guardar el código ensamblador en un archivo
            //generator.SaveAssemblyToFile(assemblyCode, "program.asm");
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

                bool isCade = false;
                foreach (char c in TokensLexico)
                {
                    if (c is '"')
                        if (!isCade)
                            isCade = true;
                        else
                            isCade = false;
                    if (char.IsWhiteSpace(c) && !isCade)
                    {
                        if (currentToken.Length > 0)
                        {
                            string Result = Lexico.AnalizarPalabra(currentToken.ToString(), dtMatriz);
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
                    string Result = Lexico.AnalizarPalabra(currentToken.ToString(), dtMatriz);
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
                    if (reducedSymbol is not null)
                    {
                        stack.Push(reducedSymbol);
                        string stackWaiting = (stackBeforeReduce + " " + string.Join(" ", tokens)).Trim();
                        text = stackWaiting.Replace(stackBeforeReduce, string.Join(" ", stack.Reverse()));
                        if (stack.Count == 1 && stack.Peek() == "S")
                            sb.AppendLine($"[{numLinea}] S");
                        else
                            sb.AppendLine($"[{numLinea}] {text}");
                    }
                }
            }
            if (stack.Count is 1 && stack.Peek() is "S")
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

            string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            bool foundInicio = false;
            bool foundFin = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line is "INICIO" && !foundInicio)
                {
                    WriteToConsole("Inicio del programa detectado.", "Info");
                    foundInicio = true;
                    continue;
                }

                if (line is "FIN" && !foundFin)
                {
                    WriteToConsole("Fin del programa detectado.", "Info");
                    foundFin = true;
                    continue;
                }

                if (line.StartsWith("ENTERO ") || line.StartsWith("REAL ") || line.StartsWith("CADENA "))
                {
                    AnalyzeDeclaration(line, i + 1);
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
                WriteToConsole($"Sintaxis inválida en la línea '{line}'.", "Error", i + 1);
            }
        }

        public void AnalyzeAssignment(string codeLine, int lineNumber)
        {
            string[] parts = codeLine.Split('=', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                WriteToConsole($"Sintaxis inválida en la asignación '{codeLine.Trim()}'.", "Error", lineNumber);
                return;
            }

            string variableName = parts[0];
            string value = parts[1];

            Symbol? symbol = SymbolTable.FirstOrDefault(s => s.Name == variableName);
            if (symbol is null)
            {
                WriteToConsole($"La variable '{variableName}' no ha sido declarada.", "Error", lineNumber);
                return;
            }

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
            WriteToConsole($"Asignación válida: {variableName} = {value}.", "Success");
        }

        public void AnalyzeDeclaration(string codeLine, int lineNumber)
        {
            string[] parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4 || parts[2] is not "=")
            {
                WriteToConsole($"Sintaxis inválida en la declaración '{codeLine.Trim()}'.", "Error", lineNumber);
                return;
            }

            string type = parts[0];
            string variableName = parts[1];
            string value = string.Join(" ", parts.Skip(3));

            if (type is not ("ENTERO" or "REAL" or "CADENA"))
            {
                WriteToConsole($"Tipo de dato '{type}' no reconocido en la declaración '{codeLine.Trim()}'.", "Error", lineNumber);
                return;
            }

            if (SymbolTable.Any(s => s.Name == variableName))
            {
                WriteToConsole($"La variable '{variableName}' ya fue declarada.", "Error", lineNumber);
                return;
            }

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

            SymbolTable.Add(new Symbol(variableName, type, true));
            WriteToConsole($"Declaración válida: {type} {variableName} = {value}.", "Success");
        }

        private static bool IsIntegerExpression(string expression)
        {
            string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (i % 2 is 0)
                {
                    if (!int.TryParse(tokens[i], out _))
                        return false;
                }
                else
                    if (tokens[i] is not ("+" or "-" or "*" or "/"))
                    return false;
            }
            return true;
        }

        private static bool IsRealExpression(string expression)
        {
            string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (i % 2 is 0)
                {
                    if (!double.TryParse(tokens[i], out _))
                        return false;
                }
                else
                    if (tokens[i] is not ("+" or "-" or "*" or "/"))
                    return false;
            }
            return true;
        }

        private static bool IsStringExpression(string expression) { return expression.StartsWith('"') && expression.EndsWith('"'); }

        public void AnalyzeBalanceInicioFin(string code)
        {
            string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int inicioCount = 0;
            int finCount = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmedLine = lines[i].Trim();
                if (trimmedLine is "INICIO")
                {
                    inicioCount++;
                    if (inicioCount > 1)
                    {
                        WriteToConsole($"Más de un 'INICIO' encontrado.", "Error", i + 1);
                        return;
                    }
                }
                else if (trimmedLine is "FIN")
                {
                    finCount++;
                    if (inicioCount is 0)
                    {
                        WriteToConsole($"'FIN' encontrado antes de 'INICIO'.", "Error", i + 1);
                        return;
                    }

                    if (finCount > 1)
                    {
                        WriteToConsole($"Más de un 'FIN' encontrado.", "Error", i + 1);
                        return;
                    }
                }
            }
            if (inicioCount is 0)
            {
                WriteToConsole("No se encontró 'INICIO' en el código.", "Error");
                return;
            }
            if (finCount is 0)
            {
                WriteToConsole("No se encontró 'FIN' en el código.", "Error");
                return;
            }

            WriteToConsole("'INICIO' y 'FIN' están correctamente equilibrados.", "Success");
        }

        private void AnalyzeReturn(string codeLine, int lineNumber)
        {
            string[] parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'DEVUELVE'.", "Error", lineNumber);
                return;
            }

            string variableName = parts[1];
            Symbol? symbol = SymbolTable.FirstOrDefault(s => s.Name == variableName);
            if (symbol == null)
            {
                WriteToConsole($"La variable '{variableName}' no ha sido declarada.", "Error", lineNumber);
                return;
            }

            WriteToConsole($"Instrucción válida: DEVUELVE {variableName}.", "Success");
        }

        private void AnalyzeRead(string codeLine, int lineNumber)
        {
            if (!codeLine.StartsWith("LEER (") || !codeLine.EndsWith(')'))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'LEER'.", "Error", lineNumber);
                return;
            }

            string variableName = codeLine[6..^1].Trim();
            Symbol? symbol = SymbolTable.FirstOrDefault(s => s.Name == variableName);
            if (symbol is null)
            {
                WriteToConsole($"La variable '{variableName}' no ha sido declarada.", "Error", lineNumber);
                return;
            }

            WriteToConsole($"Instrucción válida: LEER({variableName}).", "Success");
        }

        private void AnalyzePrint(string codeLine, int lineNumber)
        {
            if (!codeLine.StartsWith("MOSTRAR (") || !codeLine.EndsWith(')'))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'MOSTRAR'.", "Error", lineNumber);
                return;
            }

            string content = codeLine[9..^1].Trim();
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
                string[] parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3 || parts[0] is not "SINO")
                {
                    WriteToConsole($"Sintaxis inválida en la instrucción 'SINO'.", "Error", lineNumber);
                    return;
                }

                string elseAssignment = string.Join(" ", parts.Skip(1));
                AnalyzeAssignment(elseAssignment, lineNumber);
                return;
            }

            if (!codeLine.StartsWith("SI (") || !codeLine.Contains(')'))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'SI'.", "Error", lineNumber);
                return;
            }
            int conditionEndIndex = codeLine.IndexOf(')') + 1;
            string condition = codeLine.Substring(3, conditionEndIndex - 4).Trim();
            string thenAssignment = codeLine[conditionEndIndex..].Trim();

            if (string.IsNullOrWhiteSpace(condition))
            {
                WriteToConsole($"La condición en la instrucción 'SI' no puede estar vacía.", "Error", lineNumber);
                return;
            }
            WriteToConsole($"Instrucción válida: SI({condition}).", "Success");

            AnalyzeAssignment(thenAssignment, lineNumber);
        }

        private void AnalyzeThen(string codeLine, int lineNumber)
        {
            if (!codeLine.StartsWith("ENTONCES "))
            {
                WriteToConsole($"Sintaxis inválida en la instrucción 'ENTONCES'.", "Error", lineNumber);
                return;
            }

            string assignment = codeLine[9..].Trim();

            if (string.IsNullOrWhiteSpace(assignment))
            {
                WriteToConsole($"La instrucción 'ENTONCES' debe incluir una asignación válida.", "Error", lineNumber);
                return;
            }
            AnalyzeAssignment(assignment, lineNumber);
        }

        public void AnalyzeParenthesesBalance(string code)
        {
            string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Stack<int> parenthesesStack = new();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                for (int j = 0; j < line.Length; j++)
                {
                    char currentChar = line[j];

                    if (currentChar is '(')
                    {
                        parenthesesStack.Push(i + 1);
                    }

                    else if (currentChar is ')')
                    {
                        if (parenthesesStack.Count == 0)
                        {
                            WriteToConsole($"Error: Paréntesis de cierre ')' sin apertura en la línea {i + 1}.", "Error");
                            return;
                        }
                        parenthesesStack.Pop();
                    }
                }
            }

            while (parenthesesStack.Count > 0)
            {
                int lineWithUnclosedParenthesis = parenthesesStack.Pop();
                WriteToConsole($"Error: Paréntesis de apertura '(' sin cierre en la línea {lineWithUnclosedParenthesis}.", "Error");
            }

            if (parenthesesStack.Count is 0)
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

                if (messageType is "Error")
                    txtErrors.SelectionColor = Color.Red;
                else if (messageType is "Success")
                    txtErrors.SelectionColor = Color.Green;
                else
                    txtErrors.SelectionColor = Color.Black;

                txtErrors.AppendText(formattedMessage);
                txtErrors.SelectionColor = txtErrors.ForeColor;
                txtErrors.ScrollToCaret();
            }
        }

        #endregion

        #region Código intermedio
        private static List<Quadruple> GenerateIntermediateCode(string code)
        {
            List<Quadruple> quadruples = [];
            int tempCounter = 1;

            string[] lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("ENTERO ") || trimmedLine.StartsWith("REAL ") || trimmedLine.StartsWith("CADENA "))
                {
                    var parts = trimmedLine.Split(' ', 3, StringSplitOptions.TrimEntries);
                    string variableType = parts[0];
                    string variableName = parts[1];
                    string assignment = parts.Length > 2 ? parts[2] : "";

                    // Procesar la asignación
                    if (!string.IsNullOrWhiteSpace(assignment) && assignment.Contains('='))
                    {
                        var assignmentParts = assignment.Split('=', 2, StringSplitOptions.TrimEntries);
                        string expression = assignmentParts[1];

                        // Analizar la expresión
                        string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (tokens.Length is 1)
                        {
                            // Caso simple: x = y
                            quadruples.Add(new Quadruple("=", tokens[0], "-", variableName));
                        }
                        else if (tokens.Length is 3)
                        {
                            string tempVar = $"t{tempCounter++}";
                            quadruples.Add(new Quadruple(tokens[1], tokens[0], tokens[2], tempVar));
                            quadruples.Add(new Quadruple("=", tempVar, "-", variableName));
                        }
                    }
                    continue;
                }

                if (trimmedLine.Contains('=') && !trimmedLine.StartsWith("SI") && !trimmedLine.StartsWith("SINO") && !trimmedLine.StartsWith("ENTONCES"))
                {
                    var parts = trimmedLine.Split('=', 2, StringSplitOptions.TrimEntries);
                    string left = parts[0];
                    string right = parts[1];

                    // Analizar la expresión en la parte derecha
                    string[] tokens = right.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length is 1)
                    {
                        // Caso simple: x = y
                        quadruples.Add(new Quadruple("=", tokens[0], "-", left));
                    }
                    else if (tokens.Length is 3)
                    {
                        // Caso: x = a + b
                        string tempVar = $"t{tempCounter++}";
                        quadruples.Add(new Quadruple(tokens[1], tokens[0], tokens[2], tempVar));
                        quadruples.Add(new Quadruple("=", tempVar, "-", left));
                    }
                }
            }
            return quadruples;
        }

        private static void DisplayIntermediateCode(List<Quadruple> quadruples, DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            // Configurar columnas
            dgv.Columns.Add("Operator", "Operador");
            dgv.Columns.Add("Operand1", "Operando 1");
            dgv.Columns.Add("Operand2", "Operando 2");
            dgv.Columns.Add("Result", "Resultado");

            // Agregar filas con los cuádruplos
            foreach (Quadruple quad in quadruples)
                dgv.Rows.Add(quad.Operator, quad.Operand1, quad.Operand2, quad.Result);
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            string miString = txtOutput.Text;
            string rutaArchivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "file.txt");
            try
            {
                using StreamWriter sw = new(rutaArchivo);
                sw.Write(miString);
                MessageBox.Show("Se ha generado correctamente el archivo .txt");
            }
            catch (Exception) { MessageBox.Show("Error en el archivo .txt"); }

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
                using (StreamWriter sw = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DATOS.txt")))
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

        private void btnGuardarCodigoFinal_Click(object sender, EventArgs e)
        {
            string finalCode = txtEnsamblador.Text;
            if (!string.IsNullOrEmpty(finalCode))
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Title = "Guardar Archivo ASM",
                    Filter = "Assembly Files (*.asm)|*.asm|All Files (*.*)|*.*",
                    DefaultExt = "asm",
                    FileName = $"bsharp_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.asm"
                };

                if (saveFileDialog.ShowDialog() is DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    string[] lines = finalCode.Split('\n');
                    try
                    {
                        File.WriteAllLines(filePath, lines);
                        MessageBox.Show("Archivo ASM guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
