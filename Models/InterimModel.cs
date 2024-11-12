namespace addon_model_converter;

public class InterimModel
{
    public InterimModel(FileInfo file)
    {            
        Name = file.GetModelName();
        Directory = file.Directory!;
        File = file;
    }

    public string Name { get; set; }
    public DirectoryInfo Directory { get; set; }
    public FileInfo File { get; set; }
}
