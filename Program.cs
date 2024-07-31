// - take a vehicle model
//   [x] find the carvariations, handling & vehicles meta for vehicle
//   [x] output standalone carvariation, handling & vehicles meta for vehicle
//   [ ] output label.lua, with text entries
//   [x] prefix all model files
//   [x] model name randomisation
//   [ ] provide a way to name files
//   [ ] generate random text entry labels

// - take a list ped models
//   [x] rename all model files
//   [x] generate a peds.meta based on template entry


using System.CommandLine;

namespace addon_model_converter
{
    public partial class Program
    {       
        RootCommand rootCmd = new RootCommand(description: "Simple app to convert models to addons");

        Command pedCmd = new Command(
            name: "ped", 
            description: "Generate a peds.meta file for one or more ped models"
        );
        
        Command vehicleCmd = new Command(
            name: "vehicle", 
            description: "Generate carvariations, handling & vehicles meta files for one or more vehicle models"
        );

        public static int ReturnCode { get; set; } = 0;

        public Program()
        {
            rootCmd.Add(pedCmd);
            rootCmd.Add(vehicleCmd);
            
            pedCmd.Add(convertPedCmd);
            pedCmd.Add(renamePedCmd);

            vehicleCmd.Add(convertVehicleCmd);
            vehicleCmd.Add(renameVehicleCmd);
            vehicleCmd.Add(vehicleInfoCmd);

            convertPedCmd.AddOption(modelOption);
            convertPedCmd.AddOption(deepOption);
            convertPedCmd.AddOption(prefixOption);
            convertPedCmd.AddOption(randomNames);
            convertPedCmd.SetHandler(ConvertPeds, modelOption, deepOption, prefixOption, randomNames);

            renamePedCmd.AddOption(modelOption);
            renamePedCmd.AddOption(deepOption);
            renamePedCmd.AddOption(prefixOption);
            renamePedCmd.AddOption(randomNames);
            renamePedCmd.SetHandler(RenamePeds, modelOption, deepOption, prefixOption, randomNames);

            convertVehicleCmd.AddOption(metaDirectoryOption);
            convertVehicleCmd.AddOption(modelOption);
            convertVehicleCmd.AddOption(deepOption);
            convertVehicleCmd.AddOption(prefixOption);
            convertVehicleCmd.AddOption(randomNames);
            convertVehicleCmd.SetHandler(ConvertVehicles, metaDirectoryOption, modelOption, deepOption, prefixOption, randomNames);

            renameVehicleCmd.AddOption(modelOption);
            renameVehicleCmd.AddOption(deepOption);
            renameVehicleCmd.AddOption(prefixOption);
            renameVehicleCmd.AddOption(randomNames);
            renameVehicleCmd.SetHandler(RenameVehicles, modelOption, deepOption, prefixOption, randomNames);

            vehicleInfoCmd.AddOption(metaDirectoryOption);
            vehicleInfoCmd.SetHandler(GetVehicles, metaDirectoryOption);
        }

        public int Init(string[] args)
        {
            rootCmd.Invoke(args);

            return ReturnCode;
        }

        public static void Main(string[] args)
            => new Program().Init(args);
    }
}