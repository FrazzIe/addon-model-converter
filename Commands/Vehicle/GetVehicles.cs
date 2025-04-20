using System.CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace addon_model_converter;

public partial class Program
{
    Command vehicleInfoCmd = new Command(
        name: "info", 
        description: "Get a list of vehicle models with metadata tags"
    );

    void GetVehicles(DirectoryInfo metaDirectory)
    {
        var modelInfoList = VehicleHelper.GetAllVehicleModelInfo(metaDirectory, true);
        int count = 0;
        
        var vehicleInfoList = new List<VehicleInfo>();
        var vehicleInfoFilename = "vehicles.json";

        foreach (var modelInfo in modelInfoList)
        {

            var info = VehicleInfo.FromXML(modelInfo);

            ConsoleColor.Yellow.Write($"[ {++count} / {modelInfoList.Length} ] ");
            ConsoleColor.White.Write(info.ToString());
            Console.Write('\n');

            vehicleInfoList.Add(info);
        }

        var saveLocation = Path.Join(metaDirectory.FullName, vehicleInfoFilename);
        var json = JsonSerializer.Serialize(vehicleInfoList, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(saveLocation, json);

        Console.Write("Saved ");
        ConsoleColor.Blue.Write(vehicleInfoFilename);
        Console.Write(" to ");
        ConsoleColor.Blue.Write(saveLocation);
        Console.Write(".\n");
    }
}

public class VehicleInfo {
    private static readonly string Unknown = "unknown";

    [JsonPropertyName("name")]
    public string Name { get; set; } = Unknown;
    [JsonPropertyName("make")]
    public string Make { get; set; } = Unknown;
    [JsonPropertyName("class")]
    public string Class { get; set; } = Unknown;
    [JsonPropertyName("type")]
    public string Type { get; set; } = Unknown;
    
    private static string Normalise(string? value)
        => string.IsNullOrWhiteSpace(value) ? Unknown : value;

    public static VehicleInfo FromXML(XElement el)
    {
        return new VehicleInfo
        {
            Name = Normalise(el.Element(VehicleHelper.modelInfoItemModel)?.Value),
            Make  = Normalise(el.Element("vehicleMakeName")?.Value.ToLower()),
            Class = Normalise(el.Element("vehicleClass")?.Value.Replace("VC_", "").ToLower()),
            Type = Normalise(
                el.Element("type")?.Value.Replace("VEHICLE_TYPE_", "").ToLower()
            )
        };
    }
    
    public override string ToString() {
        return $"name:{Name}, make:{Make}, class:{Class}, type:{Type}";
    }
}