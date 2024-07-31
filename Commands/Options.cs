using System.CommandLine;

namespace addon_model_converter;

public partial class Program 
{
    Option<DirectoryInfo> fiveDirectoryOption = new Option<DirectoryInfo>(name: "--gta5", description: "The directory containing Grand Theft Auto 5 files");

    Option<DirectoryInfo?> metaDirectoryOption = new Option<DirectoryInfo?>(
        name: "--meta", 
        description: "The directory containing reference meta files",
        parseArgument: result =>
        {
            if (result.Tokens.Count == 0)
            {
                result.ErrorMessage = "No meta directory was provided.";
                return null;
            }

            string path = result.Tokens.Single().Value;

            if (!Directory.Exists(path))
            {
                result.ErrorMessage = "The meta directory provided does not exist.";
                return null;
            }

            return new DirectoryInfo(path);
        }
    ) { IsRequired = true };

    Option<DirectoryInfo?> modelOption = new Option<DirectoryInfo?>(
        name: "--model", 
        description: "The directory containing one or more models",
        parseArgument: result =>
        {
            if (result.Tokens.Count == 0)
            {
                result.ErrorMessage = "No model directory was provided.";
                return null;
            }

            string path = result.Tokens.Single().Value;

            if (!Directory.Exists(path))
            {
                result.ErrorMessage = "The model directory provided does not exist.";
                return null;
            }

            return new DirectoryInfo(path);
        }
    ) { IsRequired = true };

    Option<bool> deepOption = new Option<bool>(
        name: "--deep", 
        description: "Search all subdirectories in the specified model directory", 
        getDefaultValue: () => false
    );

    Option<string> prefixOption = new Option<string>(
        name: "--prefix", 
        description: "Specify a prefix to be used applied to model names", 
        getDefaultValue: () => ""
    );

    Option<bool> randomNames = new Option<bool>(
        name: "--random-names",
        description: "Give all models random new names"
    );
}