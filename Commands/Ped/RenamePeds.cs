using System.CommandLine;

namespace addon_model_converter;

public partial class Program
{
    Command renamePedCmd = new Command(
        name: "rename", 
        description: "Rename all models individually"
    );

    void RenamePeds(DirectoryInfo modelDirectory, bool deep, string prefix, bool randomNames)
    {
        var peds = ModelHelper.GetModels(modelDirectory, deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        if (peds.Length == 0)
        {
            Console.Error.WriteLine($"No models could be found in \"{modelDirectory}\"");
            ReturnCode = 1;
            return;
        }

        peds
            .PromptUserForModelNames(prefix)
            .PrefixModelNames(prefix)
            .RenameModels();

        var metadata = PedHelper.GeneratePedModelMetadata(peds);
        metadata.Save(PedHelper.GetPedMetadataSaveLocation(modelDirectory));
    }
}
