using System.Xml.Linq;

namespace addon_model_converter;

public class VehicleModel : Model
{
    public VehicleModel(
        string name, 
        DirectoryInfo directory, 
        FileInfo[] files
    ) : base(name, directory, files) { }
    
    public XElement? Variation { get; set; }
    public XElement? Handling { get; set; }
    public XElement? ModelInfo { get; set; }
    public XElement? TextureRelationship { get; set; }

    public bool MatchFound { 
        get => Variation != null || Handling != null || 
            (ModelInfo != null && TextureRelationship != null);
    }
}