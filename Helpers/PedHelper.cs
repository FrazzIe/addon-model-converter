using System.Xml.Linq;

namespace addon_model_converter;

public static class PedHelper
{
    private static readonly string fileName = "peds.meta";
    private static readonly string template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets/ped-model-template.xml");
    private static readonly string modelInfoItemModel = "Name";

    public static XElement GetPedModelMetaDataTemplate()
    {
        var doc = XDocument.Load(template);

        if (doc?.Root is null)
            throw new Exception("Unable to locate ped model metadata template");
        
        return doc.Root;
    }

    // <CPedModelInfo__InitDataList>
    //   <InitDatas>
    //     <Item>
    public static XDocument GeneratePedModelMetadata(Model[] models)
    {
        var template = GetPedModelMetaDataTemplate();
        var peds = new List<XElement>();

        foreach (var item in models)
        {
            var ped = new XElement(template);
            ped.Element(modelInfoItemModel)?.SetValue(item.Name);
            peds.Add(ped);
        }
        
        return new XDocument(
            new XElement("CPedModelInfo__InitDataList", 
                new XElement("InitDatas", peds)
            )
        );
    }

    public static string GetPedMetadataSaveLocation(DirectoryInfo directory)
    {
        return Path.Combine(directory.FullName, fileName);
    }
}