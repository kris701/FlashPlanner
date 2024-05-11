using System.Collections;

namespace FlashPlanner.CLI.ArgumentParsing
{
    public static class ArgumentBuilder
    {
        public static object Parse(string text, Dictionary<string, object?> args, List<Argument> register)
        {
            var targetItem = register.FirstOrDefault(x => text.ToUpper().StartsWith(x.Name.ToUpper()));
            if (targetItem == null)
                throw new Exception("Invalid parse target");

            var newArgs = new Dictionary<string, object?>(args);

            var subText = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
            var subArgs = GetArgumentsAtLevel(subText);
            int offset = 0;
            foreach (var arg in targetItem.Arguments.Keys)
            {
                if (newArgs.ContainsKey(arg))
                    continue;

                if (IsTypeList(targetItem.Arguments[arg]))
                {
                    if (!subArgs[offset].StartsWith("(") || !subArgs[offset].EndsWith(")"))
                        throw new Exception("Invalid parse target");
                    var listText = subArgs[offset].Substring(subArgs[offset].IndexOf('(') + 1, subArgs[offset].LastIndexOf(')') - subArgs[offset].IndexOf('(') - 1);
                    var listArgs = GetArgumentsAtLevel(listText);
                    var items = Activator.CreateInstance(targetItem.Arguments[arg]) as IList;
                    if (items == null)
                        throw new Exception("Invalid parse target");

                    foreach (var listItem in listArgs)
                    {
                        var parsed = Parse(listItem, newArgs, register);
                        items.Add(parsed);
                    }

                    newArgs.Add(arg, items);
                }
                else
                {
                    if (targetItem.Arguments[arg].IsPrimitive)
                        newArgs.Add(arg, Convert.ChangeType(subArgs[offset], targetItem.Arguments[arg]));
                    else
                    {
                        var parsed = Parse(subArgs[offset], newArgs, register);
                        if (parsed.GetType().IsAssignableTo(targetItem.Arguments[arg]))
                            newArgs.Add(arg, parsed);
                        else
                            newArgs.Add(arg, Convert.ChangeType(parsed, targetItem.Arguments[arg]));
                    }
                }
                offset++;
            }

            return targetItem.Constructor(newArgs);
        }

        private static bool IsTypeList(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        private static List<string> GetArgumentsAtLevel(string text)
        {
            var items = new List<string>();
            var current = "";
            var level = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '(')
                    level++;
                if (text[i] == ')')
                    level--;

                if (text[i] == ',' && level == 0)
                {
                    items.Add(current);
                    current = "";
                }
                else
                    current += text[i];
            }
            if (current != "")
                items.Add(current);

            return items;
        }
    }
}
