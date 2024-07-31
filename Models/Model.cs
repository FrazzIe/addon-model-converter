namespace addon_model_converter;

public class Model
{
    public Model(string name, DirectoryInfo directory, FileInfo[] files)
    {
        Name = name;
        FileName = name;
        Directory = directory;
        Files = files;
    }

    public string Name { get; set; }
    public string FileName { get; private set; }
    public DirectoryInfo Directory { get; set; }
    public FileInfo[] Files { get; set; }

    public void Rename(string? name = null)
    {
        foreach (var item in Files)
            item.MoveTo(Path.Combine(Directory.FullName, item.Name.Replace(FileName, name ?? Name)));

        FileName = name ?? Name;
    }
}