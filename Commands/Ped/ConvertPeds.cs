using System.CommandLine;

namespace addon_model_converter;

public partial class Program
{
    Command convertPedCmd = new Command(
        name: "convert", 
        description: "Generate a peds.meta file for one or more ped models"
    );

    void ConvertPeds(DirectoryInfo modelDirectory, bool deep, string prefix, bool randomNames)
    {
        var models = ModelHelper.GetModels(modelDirectory, deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        if (models.Length == 0)
        {
            Console.Error.WriteLine($"No models could be found in \"{modelDirectory}\"");
            ReturnCode = 1;
            return;
        }

        if (randomNames)
            models.RandomiseModelNames();

        if (!string.IsNullOrEmpty(prefix))
            models.PrefixModelNames(prefix);

        models.RenameModels();

        var metadata = PedHelper.GeneratePedModelMetadata(models);

        metadata.Save(PedHelper.GetPedMetadataSaveLocation(modelDirectory));
    }
}