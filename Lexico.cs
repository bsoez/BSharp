using System.Data;

namespace BSharp
{
    public class Lexico
    {
        public static string AnalizarPalabra(string palabra, DataTable dtMatrizTransicion)
        {
            int estadoActual = 0; // Comenzamos desde el estado inicial (renglón 0)

            foreach (char caracter in palabra)
            {
                string columna = ObtenerColumnaDeCaracter(caracter)!;

                if (columna == null)
                {
                    return "Error: Carácter no válido";
                }

                if (char.IsLower(caracter))
                {
                    columna += "1";
                }

                // Obtenemos el valor en la celda de la matriz de transición
                int siguienteEstado = ObtenerSiguienteEstado(estadoActual, columna, dtMatrizTransicion);

                if (siguienteEstado == -1)
                {
                    return "Error: Transición no válida";
                }

                // Actualizamos el estado actual
                estadoActual = siguienteEstado;
            }

            // Después de procesar la palabra, verificamos el FDC
            int estadoFinal = ObtenerSiguienteEstado(estadoActual, "FDC", dtMatrizTransicion);

            if (estadoFinal == -1)
            {
                return "Error: No se encontró el final de cadena";
            }

            // Si el estado final es válido, obtenemos el token asociado
            string token = ObtenerToken(estadoFinal, dtMatrizTransicion);
            if (string.IsNullOrEmpty(token))
            {
                //return "Error: No se encontró el token";
                return "";
            }

            return token;
        }

        private static string? ObtenerColumnaDeCaracter(char caracter)
        {
            if (char.IsUpper(caracter))
            {
                return caracter.ToString(); // Letras mayúsculas A-Z
            }
            else if (char.IsLower(caracter))
            {
                return caracter.ToString(); // Letras minúsculas a-z
            }
            else if (char.IsDigit(caracter))
            {
                return caracter.ToString(); // Dígitos 0-9
            }
            else
            {
                // lógica para caracteres especiales                
                if (caracter == '+')
                {
                    return "+";
                }
                else if (caracter == '-')
                {
                    return "-";
                }
                else if (caracter == '*')
                {
                    return "*";
                }
                else if (caracter == '/')
                {
                    return "/";
                }
                else if (caracter == '&')
                {
                    return "&";
                }
                else if (caracter == '|')
                {
                    return "|";
                }
                else if (caracter == '!')
                {
                    return "!";
                }
                else if (caracter == '>')
                {
                    return ">";
                }
                else if (caracter == '<')
                {
                    return "<";
                }
                else if (caracter == '=')
                {
                    return "=";
                }
                else if (caracter == '@')
                {
                    return "@";
                }
                else if (caracter == '\\') // Doble barra invertida por ser un carácter de escape
                {
                    return "\\";
                }
                else if (caracter == '"')
                {
                    return "\"";
                }
                else if (caracter == '{')
                {
                    return "{";
                }
                else if (caracter == '}')
                {
                    return "}";
                }
                else if (caracter == '[')
                {
                    return "[";
                }
                else if (caracter == ']')
                {
                    return "]";
                }
                else if (caracter == ':')
                {
                    return ":";
                }
                else if (caracter == ';')
                {
                    return ";";
                }
                else if (caracter == '.')
                {
                    return ".";
                }
                else if (caracter == '(')
                {
                    return "(";
                }
                else if (caracter == ')')
                {
                    return ")";
                }
                else if (caracter == '_')
                {
                    return "_";
                }
                else if (caracter == '#')
                {
                    return "#";
                }
                else if (caracter == '$')
                {
                    return "$";
                }
                else if (caracter == '%')
                {
                    return "%";
                }
                else if (caracter == '°')
                {
                    return "°";
                }
                else if (caracter == ' ')
                {
                    return " ";
                }
            }

            return null; // Carácter no válido
        }

        private static int ObtenerSiguienteEstado(int estadoActual, string columna, DataTable dtMatrizTransicion)
        {
            // Busca el valor en la columna correspondiente y el estado actual
            DataRow filaActual = dtMatrizTransicion.Rows[estadoActual];

            if (columna is " ")
                return 96;
            if (filaActual[columna] != DBNull.Value)
            {
                return Convert.ToInt32(filaActual[columna]);
            }

            return -1; // No hay transición válida
        }

        private static string ObtenerToken(int estadoFinal, DataTable dtMatrizTransicion)
        {
            // Busca el token en la columna 'TOKEN' en el estado final
            DataRow filaFinal = dtMatrizTransicion.Rows[estadoFinal];

            if (filaFinal["TOKEN"] != DBNull.Value)
            {
                return filaFinal["TOKEN"].ToString();
            }

            return null;
        }
    }
}
