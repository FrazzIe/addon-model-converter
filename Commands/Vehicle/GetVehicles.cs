using System.CommandLine;

namespace addon_model_converter;

public partial class Program
{
    Command vehicleInfoCmd = new Command(
        name: "info", 
        description: "Get a list of vehicle models with metadata tags"
    );

    void GetVehicles(DirectoryInfo metaDirectory)
    {
        var modelInfoList = VehicleHelper.GetAllVehicleModelInfo(metaDirectory);
        int count = 0;

        foreach (var modelInfo in modelInfoList)
        {
            var name = modelInfo.Element(VehicleHelper.modelInfoItemModel)?.Value;
            var tags = VehicleHelper.modelInfoTags
                .ToDictionary(k => k, v => modelInfo.Element(v)?.Value ?? string.Empty);

            ConsoleColor.Yellow.Write($"[ {++count} / {modelInfoList.Length} ]");
            ConsoleColor.White.Write(" - model: ");
            ConsoleColor.DarkCyan.Write(name ?? "NULL");
            ConsoleColor.White.Write(", tags: ");
            ConsoleColor.Green.Write(string.Join(',', tags));
            Console.Write('\n');
        }
    }
}