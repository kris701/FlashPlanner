namespace FlashPlanner.CLI.SearchParsing
{
    public class SearchOption
    {
        public string Name { get; set; }
        public Dictionary<string, Type> Arguments { get; set; }
        public Func<Dictionary<string, object?>, object> Constructor { get; set; }

        public SearchOption(string name, Dictionary<string, Type> arguments, Func<Dictionary<string, object?>, object> constructor)
        {
            Name = name;
            Arguments = arguments;
            Constructor = constructor;
        }


    }
}
