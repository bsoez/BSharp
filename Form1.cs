using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Text.RegularExpressions;

namespace BSharp
{
    public partial class Form1 : Form
    {
        private static readonly string[] palabrasReservadas = ["CADENA", "DEVUELVE", "ENTERO", "ENTONCES", "FALSO", "FIN", "INICIO", "LEER", "MIENTRAS", "MOSTRAR", "REAL", "SI", "SINO", "VERDADERO"];
        private readonly string[] TiposDeDatos = ["CADENA", "ENTERO", "REAL"];
        private static readonly char[] separator = [' ', '\n', '\r'];

        public Form1()
        {
            InitializeComponent();
            Initialization();
            Connection2();
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

        private void Connection2()
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

            string cnn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1};IMEX={2}'";
            cnn = string.Format(cnn, filePath, "NO", "1");
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
                dt.Columns.RemoveAt(dt.Columns.Count-1);
                dt.Columns.RemoveAt(0);
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

                // Caso 1: Si el OutputToken es 'IDEN'
                if (outputToken == "IDEN" && columnaActual > 1)
                {
                    // Revisamos la columna anterior dentro del mismo renglón
                    DataRow? columnaAnterior = dtLexemas.AsEnumerable().FirstOrDefault(r => (int)r["Renglon"] == renglonActual && (int)r["Columna"] == columnaActual - 1);

                    if (columnaAnterior != null)
                    {
                        string? inputTokenAnterior = columnaAnterior["InputToken"].ToString();

                        // Verificamos si el token anterior es un tipo de dato válido
                        if (TiposDeDatos.Contains(inputTokenAnterior))
                        {
                            dgvTablaSimbolos.Rows.Add(inputToken, outputToken, inputTokenAnterior);
                        }
                    }
                }

                // Caso 2: Si el OutputToken es 'NUEN'
                else if (outputToken == "NUEN")
                {
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "ENTERO");
                }

                // Caso 3: Si el OutputToken es 'NUDE'
                else if (outputToken == "NUDE")
                {
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "REAL");
                }

                // Caso 4: Si el OutputToken es 'CADE'
                else if (outputToken == "CADE")
                {
                    dgvTablaSimbolos.Rows.Add(inputToken, outputToken, "CADENA");
                }
            }
        }
        private void btnCompilar_Click(object sender, EventArgs e) 
        {
            dgvTablaSimbolos.Rows.Clear();
            txtOutput.Clear();

            bool successLexico = AnalizadorLexico();
            if (successLexico)
            {
                Lexema(txtInput.Text, txtOutput.Text);
            }

            bool successSintactico = AnalizadorSintactico();
            if (successSintactico)
            {

            }
        }

        private static readonly char[] separatorArray = [' '];

        private void txtInput_TextChanged(object sender, EventArgs e) { ChangeColorWord(); }

        private static DataTable dtMatriz = new();

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
                    {
                        currentToken.Append(c);
                    }
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

        public void AddSymbol(string name, string value, string type)
        {
            dgvTablaSimbolos.Rows.Add(name, value, type);
        }

        private void AddError(string line, string type, string message)
        {
            //dtgErrores.Rows.Add(line, type, message);
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

        private void button1_Click(object sender, EventArgs e)
        {
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
            // Convertimos la pila en un array para acceder a los elementos por índice
            string[] stackArray = [.. stack];

            // Recorremos las reglas de la gramática
            foreach (GrammarRule rule in GrammarAnalyzer.grammarRules)
            {
                // Verificamos si la pila contiene suficientes elementos para comparar con la regla
                if (stackArray.Length >= rule.RightSide.Length)
                {
                    // Extraemos los elementos relevantes de la pila        .
                    string[] subArray = stackArray[..rule.RightSide.Length];

                    // Verificamos si coinciden con el lado derecho de la regla (de atrás hacia adelante)
                    if (subArray.SequenceEqual(rule.RightSide.Reverse()))
                    {
                        return true; // Se puede reducir
                    }
                }
            }
            return false; // No hay reducción posible
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
                        // Realizamos el pop de los elementos que coinciden con el lado derecho de la regla
                        for (int i = 0; i < rule.RightSide.Length; i++)
                        {
                            stack.Pop();
                        }
                        return rule.LeftSide; // Retornamos el símbolo no terminal correspondiente
                    }
                }
            }
            return null; // No se pudo reducir
        }

        #endregion

        private void sec()
        {
            dgvTablaSimbolos.Rows.Clear();
            string[] tokens = txtInput.Text.Split(' ', '\n');
            int index = 0;
            foreach (string token in tokens)
            {
                if (token.Contains('@'))
                {
                    string tipo = "";
                    if (index > 0)
                    {
                        if (tokens[index - 1] is "ENTERO")
                            tipo = "ENTERO";
                        else if (tokens[index - 1] is "REAL")
                            tipo = "REAL";
                        //else if (tokens[index - 1] is "CADENA")
                        //    tipo = "CADENA";
                    }
                    AddSymbol(token, "IDEN", tipo);
                }
                else if (token.Contains('.'))
                {
                    AddSymbol(token, "CONR", "REAL");
                }
                else if (int.TryParse(token, out _))
                {
                    AddSymbol(token, "CONE", "ENTERO");
                }
                //else if (token.Contains('"'))
                //    AddSymbol(token, "LETR", "CADENA");
                index++;
            }
        }

        public class Lexico
        {
            public string lexema { get; set; }
            public string token { get; set; }
            public string tipo { get; set; }
            public Lexico() { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvTablaSimbolos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso");
                return;
            }

            try
            {
                // Crear un objeto StreamWriter para escribir en un archivo de texto
                using (StreamWriter sw = new("C:\\Users\\belen\\Desktop\\DATOS.txt"))
                {
                    // Escribir encabezados de columnas
                    for (int i = 0; i < dgvTablaSimbolos.Columns.Count; i++)
                    {
                        sw.Write(dgvTablaSimbolos.Columns[i].HeaderText);
                        if (i < dgvTablaSimbolos.Columns.Count - 1)
                            sw.Write("\t"); // Separador de tabulación
                    }
                    sw.WriteLine(); // Nueva línea después de los encabezados

                    // Escribir datos de las filas
                    foreach (DataGridViewRow row in dgvTablaSimbolos.Rows)
                    {
                        for (int i = 0; i < dgvTablaSimbolos.Columns.Count; i++)
                        {
                            sw.Write(row.Cells[i].Value);
                            if (i < dgvTablaSimbolos.Columns.Count - 1)
                                sw.Write("\t"); // Separador de tabulación
                        }
                        sw.WriteLine(); // Nueva línea después de cada fila
                    }
                }

                MessageBox.Show("Archivo de texto generado con éxito.", "Éxito");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el archivo de texto: " + ex.Message, "Error");
            }
        }
    }
}
