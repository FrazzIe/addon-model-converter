using System.Xml.Linq;

namespace addon_model_converter;

public static class VehicleHelper
{
    public static readonly string modelVariationRoot = "CVehicleModelInfoVariation";
    public static readonly string modelVariationData = "variationData";
    public static readonly string modelVariationItemModel = "modelName";
    public static readonly string modelVariationFile = "carvariations.meta";
    

    public static readonly string modelHandlingRoot = "CHandlingDataMgr";
    public static readonly string modelHandlingData = "HandlingData";
    public static readonly string modelHandlingItemName = "handlingName";
    public static readonly string modelHandlingFile = "handling.meta";

    public static readonly string modelInfoRoot = "CVehicleModelInfo__InitDataList";
    public static readonly string modelInfoData = "InitDatas";
    public static readonly string modelInfoItemModel = "modelName";
    public static readonly string modelInfoTexture = "txdName";
    public static readonly string modelInfoItemHandling = "handlingId";
    public static readonly string modelInfoTextureRelationships = "txdRelationships";
    public static readonly string modelInfoTextureRelationshipItemModel = "child";
    public static readonly string modelInfoFile = "vehicles.meta";
    public static readonly string[] modelInfoTags = { "gameName", "vehicleMakeName", "vehicleClass", "type" };

    
    
    // <CVehicleModelInfoVariation>
    //     <variationData>
    //        <Item>
    public static XElement[] GetAllVariations(DirectoryInfo targetDir)
    {
        return GetMetadataItems(targetDir, modelVariationRoot, modelVariationData);
    }

    // <CHandlingDataMgr>
    //     <HandlingData>
    //         <Item type="CHandlingData">
    public static XElement[] GetAllHandling(DirectoryInfo targetDir)
    {
        return GetMetadataItems(targetDir, modelHandlingRoot, modelHandlingData);
    }

    // <CVehicleModelInfo__InitDataList>
    //     <InitDatas>
    //        <Item>
    public static XElement[] GetAllVehicleModelInfo(DirectoryInfo targetDir)
    {
        return GetMetadataItems(targetDir, modelInfoRoot, modelInfoData);
    }

    // <CVehicleModelInfo__InitDataList>
    //     <txdRelationships>
    //        <Item>
    public static XElement[] GetAllTextureRelationships(DirectoryInfo targetDir)
    {
        return GetMetadataItems(targetDir, modelInfoRoot, modelInfoTextureRelationships);
    }

    private static XElement[] GetMetadataItems(DirectoryInfo targetDir, string rootEl, string dataEl, string itemEl = "Item")
    {
        var items = targetDir
            .GetFiles("*.meta", SearchOption.AllDirectories)
            .Select(f => XDocument.Load(f.FullName))
            .Where(i => i.Root?.Name == rootEl)
            .Select(i => i.Root?.Element(dataEl))
            .OfType<XElement>()
            .SelectMany(i => i.Elements(itemEl))
            .ToArray();

        return items;
    }

    public static VehicleModel[] ProjectToVehicleModels(this Model[] models)
    {
        return models
            .Select(i => new VehicleModel(i.Name, i.Directory, i.Files))
            .ToArray();
    }

    public static void LoadAndAppendVehicleModelMetadata(this VehicleModel[] vehicles, DirectoryInfo metaDirectory)
    {
        var variations = GetAllVariations(metaDirectory);
        var handling = GetAllHandling(metaDirectory);
        var vehicleModelInfo = GetAllVehicleModelInfo(metaDirectory);
        var textureRelationships = GetAllTextureRelationships(metaDirectory);

        foreach (var item in vehicles)
        {
            item.Variation = variations.FirstOrDefault(i => i.Element(modelVariationItemModel)?.Value == item.Name);
            item.ModelInfo = vehicleModelInfo.FirstOrDefault(i => i.Element(modelInfoItemModel)?.Value == item.Name);
            item.TextureRelationship = textureRelationships.FirstOrDefault(i => i.Element(modelInfoTextureRelationshipItemModel)?.Value == item.Name);

            if (item.ModelInfo != null)
                item.Handling = handling.FirstOrDefault(i => i.Element(modelHandlingItemName)?.Value == item.ModelInfo?.Element(modelInfoItemHandling)?.Value);

            if (!item.MatchFound)
                Console.Error.WriteLine("Unable to find metadata for vehicle model \"{0:ModelName}\" in \"{1:Directory}\"", item.Name, item.Directory);
        }
    }

    public static void UpdateVehicleMetadataWithModelNames(this VehicleModel[] vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            vehicle.Variation?.Element(modelVariationItemModel)?.SetValue(vehicle.Name);
            vehicle.Handling?.Element(modelHandlingItemName)?.SetValue(vehicle.Name.ToUpper());
            vehicle.ModelInfo?.Element(modelInfoItemModel)?.SetValue(vehicle.Name);
            vehicle.ModelInfo?.Element(modelInfoTexture)?.SetValue(vehicle.Name);
            vehicle.ModelInfo?.Element(modelInfoItemHandling)?.SetValue(vehicle.Name.ToUpper());
            vehicle.TextureRelationship?.Element(modelInfoTextureRelationshipItemModel)?.SetValue(vehicle.Name);
        }
    }

    public static IEnumerable<VehicleModel> SaveVariations(this IEnumerable<VehicleModel> vehicles, string saveDirectory)
    {
        var path = Path.Combine(saveDirectory, modelVariationFile);
        var doc = new XDocument(
            new XElement(modelVariationRoot, 
                new XElement(modelVariationData, vehicles.Where(i => i.Variation != null).Select(i => i.Variation))
            )
        );

        doc.Save(path);

        return vehicles;
    }

    public static IEnumerable<VehicleModel> SaveHandling(this IEnumerable<VehicleModel> vehicles, string saveDirectory)
    {
        var path = Path.Combine(saveDirectory, modelHandlingFile);
        var doc = new XDocument(
            new XElement(modelHandlingRoot, 
                new XElement(modelHandlingData, vehicles.Where(i => i.Handling != null).Select(i => i.Handling))
            )
        );

        doc.Save(path);

        return vehicles;
    }

    public static IEnumerable<VehicleModel> SaveModelInfo(this IEnumerable<VehicleModel> vehicles, string saveDirectory)
    {
        var path = Path.Combine(saveDirectory, modelInfoFile);
        var doc = new XDocument(
            new XElement(modelInfoRoot, 
                new XElement(modelInfoData, vehicles.Where(i => i.ModelInfo != null).Select(i => i.ModelInfo)),
                new XElement(modelInfoTextureRelationships, vehicles.Where(i => i.TextureRelationship != null).Select(i => i.TextureRelationship))
            )
        );

        doc.Save(path);

        return vehicles;
    }
}