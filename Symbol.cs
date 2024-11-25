namespace BSharp
{
    public class Symbol(string name, string type, bool isInitialized = false)
    {
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
        public bool IsInitialized { get; set; } = isInitialized;
    }
}
