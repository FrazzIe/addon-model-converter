using System.CommandLine;

namespace addon_model_converter;

public partial class Program
{
    Command convertVehicleCmd = new Command(
        name: "convert", 
        description: "Generate carvariations, handling & vehicles meta files for one or more vehicle models"
    );

    void ConvertVehicles(DirectoryInfo metaDirectory, DirectoryInfo modelDirectory, bool deep, string prefix, bool randomNames)
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
            vehicles.LoadAndAppendVehicleModelMetadata(metaDirectory);
        }
        catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            ReturnCode = 2;
            return;
        }

        if (randomNames)
            vehicles.RandomiseModelNames();

        if (!string.IsNullOrEmpty(prefix))
            vehicles.PrefixModelNames(prefix);

        vehicles.UpdateVehicleMetadataWithModelNames();
        vehicles.RenameModels();

        foreach (var item in vehicles.GroupBy(i => i.Directory.FullName))
            item.AsEnumerable()
                .SaveVariations(item.Key)
                .SaveHandling(item.Key)
                .SaveModelInfo(item.Key);
    }
}
