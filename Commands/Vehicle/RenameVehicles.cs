using System.CommandLine;

namespace addon_model_converter;

public partial class Program
{
    Command renameVehicleCmd = new Command(
        name: "rename", 
        description: "Rename all models individually"
    );

    void RenameVehicles(DirectoryInfo modelDirectory, bool deep, string prefix, bool randomNames)
    {
        var vehicles = ModelHelper
            .GetModels(modelDirectory, deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
            .ProjectToVehicleModels();

        if (vehicles.Length == 0)
        {
            Console.Error.WriteLine($"No models could be found in \"{modelDirectory}\"");
            ReturnCode = 1;
            return;
        }

        try
        {
            vehicles.LoadAndAppendVehicleModelMetadata(modelDirectory);
        }
        catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            ReturnCode = 2;
            return;
        }

        vehicles
            .PromptUserForModelNames(prefix)
            .PrefixModelNames(prefix)
            .RenameModels();

        foreach (var item in vehicles.GroupBy(i => i.Directory.FullName))
            item.AsEnumerable()
                .SaveVariations(item.Key)
                .SaveHandling(item.Key)
                .SaveModelInfo(item.Key);
    }
}
