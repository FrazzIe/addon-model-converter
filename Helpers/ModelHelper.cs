using System.Text.RegularExpressions;

namespace addon_model_converter;

public static class ModelHelper
{
    public static string[] allowedExtensions = { ".ydd", ".yft", ".ymt", ".ytd" };

    public static string GetModelName(this FileInfo file)
    {
        return Regex.Replace(file.Name.Replace(file.Extension, ""), @"[\+|_]hi$", "");
    }

    public static Model[] GetModels(DirectoryInfo target, SearchOption searchCritera = SearchOption.TopDirectoryOnly)
    {
        return target
            .GetFiles("*.y*", searchCritera)
            .Where(f => allowedExtensions.Any(f.Extension.ToLower().EndsWith))
            .Select(f => new InterimModel(f))
            .GroupBy(i => new { i.Name, i.Directory.FullName })
            .Select(i => new Model(i.Key.Name, i.First().Directory, i.Select(i => i.File).ToArray()))
            .ToArray();
    }

    public static T[] RandomiseModelNames<T>(this T[] models) where T : Model
    {
        foreach (var item in models)
            item.Name = Guid.NewGuid().ToBase64String();
        
        return models;
    }

    public static T[] PrefixModelNames<T>(this T[] models, string prefix) where T : Model
    {
        foreach (var item in models)
            item.Name = $"{prefix}{item.Name}";

        return models;
    }

    public static T[] RenameModels<T>(this T[] models) where T : Model
    {
        foreach (var item in models)
            item.Rename();

        return models;
    }

    public static T[] PromptUserForModelNames<T>(this T[] models, string prefix) where T : Model
    {
        var invalidFileNameChars = Path
            .GetInvalidFileNameChars()
            .ToHashSet();

        for (var i = 0; i < models.Length; i++)
        {
            var model = models[i];
            string name;

            name = AskUserForModelName(i, models.Length, model.Name, prefix);

            while (true)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Console.Write("Are you sure want to skip renaming ({0:Name}) [Y/n]: ", model.Name);
                    var skip = Console.ReadLine()?.ToLower() == "y";

                    if (skip)
                        break;
                } else if (name.Any(invalidFileNameChars.Contains) || name.EndsWith('.'))
                {
                    var trimmed = name.TrimEnd('.');
                    Console.Write("Received an invalid name, please do not use the highlighted characters: ");
                    ConsoleColor.Red.WriteHighlighted(trimmed, invalidFileNameChars);
                    ConsoleColor.Red.Write(name.Replace(trimmed, ""));
                    Console.WriteLine();
                } else
                    break;

                name = AskUserForModelName(i, models.Length, model.Name, prefix);
            }

            model.Name = name;
        }

        return models;
    }

    private static string AskUserForModelName(int modelIter, int numModels, string name, string prefix)
    {
        Console.Write("[ {0:Count}/{1:Length} ] Enter model name ({2:Name}): {3:Prefix}", (modelIter + 1).ToString(), numModels.ToString(), name, prefix);
        return Console.ReadLine() ?? "";
    }

    // TODO: Move
    public static string ToBase64String(this Guid guid)
    {
        return Convert.ToBase64String(guid.ToByteArray())
            .Replace("==", "").Replace('/', 'a').Replace('+', 'b')
            .Replace('0', 'c').Replace('1', 'd').Replace('2', 'e')
            .Replace('3', 'f').Replace('4', 'g').Replace('5', 'h')
            .Replace('6', 'i').Replace('7', 'j').Replace('8', 'k')
            .Replace('9', 'l').ToLower();
    }
}
