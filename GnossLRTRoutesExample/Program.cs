using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Exceptions;
using Gnoss.ApiWrapper.Model;
using GnossLRTRoutesExample.Model;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GnossLRTRoutesExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Entity> entityList = LeerRutas();

            SubirRutas(entityList);
        }

        private static void SubirRutas(Dictionary<string, Entity> entityList)
        {
            List<Entity> rutas = entityList.Values.Where(ent => ent.Properties.Values.Any(prop => prop.Predicate.Equals($"{Constants.Ontologies.Rdf}type") && prop.Objects.Exists(obj => obj.Value.Equals($"{Constants.Ontologies.Route}Route")))).ToList();

            ResourceApi resourceAPI = new ResourceApi(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "config.xml"));

            foreach (Entity ruta in rutas)
            {
                ComplexOntologyResource resource = CreateComplexOntologyResourceRoute(entityList, ruta, resourceAPI);

                try
                {
                    resourceAPI.LoadComplexSemanticResource(resource);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static Dictionary<string, Entity> LeerRutas()
        {
            Dictionary<string, Entity> entityList = new Dictionary<string, Entity>();
            string routesCsvFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "rtroute.csv");

            using (CsvReader csvReader = new CsvReader(new StreamReader(routesCsvFile), true))
            {
                while (csvReader.ReadNextRecord())
                {
                    string subject = csvReader[0];
                    string predicate = csvReader[1];
                    string obj = csvReader[2];
                    string lang = null;

                    if (csvReader.FieldCount > 3)
                    {
                        lang = csvReader[3];
                    }

                    if (!entityList.ContainsKey(subject))
                    {
                        entityList.Add(subject, new Entity(subject));
                    }
                    Entity entity = entityList[subject];

                    if (!entity.Properties.ContainsKey(predicate))
                    {
                        entity.Properties.Add(predicate, new Property(predicate));
                    }
                    Property property = entity.Properties[predicate];

                    property.Objects.Add(new ObjectWithLanguage() { Value = obj, Language = lang });
                }
            }
            return entityList;
        }


        public static ComplexOntologyResource CreateComplexOntologyResourceRoute(Dictionary<string, Entity> entityList, Entity route, ResourceApi resourceAPI)
        {
            ComplexOntologyResource recurso = new ComplexOntologyResource();

            // Prefijos de ontología
            List<string> prefijosOntologia = new List<string>() {
                 Constants.Prefixes.Rdf,
                 Constants.Prefixes.Rdfs,
                 Constants.Prefixes.Owl,
                 Constants.Prefixes.Xsd,
                 Constants.Prefixes.Eroute,
                 Constants.Prefixes.Route,
                 Constants.Prefixes.Wgs84_pos, 
                 Constants.Prefixes.Eharmonise
            };

            // Propiedades
            List<OntologyProperty> listPropiedades = new List<OntologyProperty>();

            if (route.Properties.ContainsKey(Constants.RouteProperties.Name))
            {
                recurso.Title = route.Properties[Constants.RouteProperties.Name].Objects[0].Value;

                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Name].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.NameWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Description))
            {
                recurso.Description = route.Properties[Constants.RouteProperties.Description].Objects[0].Value;

                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Description].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.DescriptionWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Ads))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Ads].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.AdsWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Color))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Color].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.ColorWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Date))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Date].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.DateWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.GpsRoute))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.GpsRoute].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.GpsRouteWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.HeadOf))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.HeadOf].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.HeadOfWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Hunts))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Hunts].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.HuntsWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Pdf))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Pdf].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.PdfWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Points))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Points].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.PointsWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Publication))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Publication].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.PublicationWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.RouteFeature))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.RouteFeature].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.RouteFeatureWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.RouteId))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.RouteId].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.RouteIdWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.RouteNaturalValue))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.RouteNaturalValue].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.RouteNaturalValueWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.RouteNumber))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.RouteNumber].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.RouteNumberWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Source))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Source].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.SourceWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.TouristInformation))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.TouristInformation].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.TouristInformationWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Trail))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Trail].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.TrailWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Region))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Region].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.RegionWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Duration))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Duration].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.DurationWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Length))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Length].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.LengthWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Signaling))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Signaling].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.SignalingWithNamespace, obj.Value, obj.Language));
                }
            }

            if (route.Properties.ContainsKey(Constants.RouteProperties.Url))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Url].Objects)
                {
                    listPropiedades.Add(new StringOntologyProperty(Constants.RouteProperties.UrlWithNamespace, obj.Value, obj.Language));
                }
            }
            

            List<OntologyEntity> relatedEntitiesList = new List<OntologyEntity>();
            if (route.Properties.ContainsKey(Constants.RouteProperties.Location))
            {
                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Location].Objects)
                {
                    Entity location = entityList[obj.Value];
                    List<OntologyProperty> listPropiedadesEntidad = new List<OntologyProperty>();

                    if (location.Properties.ContainsKey(Constants.RouteProperties.Lat))
                    {
                        listPropiedadesEntidad.Add(new StringOntologyProperty(Constants.RouteProperties.LatWithNamespace, location.Properties[Constants.RouteProperties.Lat].Objects[0].Value));
                    }

                    if (location.Properties.ContainsKey(Constants.RouteProperties.Long))
                    {
                        listPropiedadesEntidad.Add(new StringOntologyProperty(Constants.RouteProperties.LongWithNamespace, location.Properties[Constants.RouteProperties.Long].Objects[0].Value));
                    }
                    
                    OntologyEntity entLocation = new OntologyEntity(Constants.Ontologies.Eharmonise + "Location", Constants.Ontologies.Eharmonise + "Location", Constants.RouteProperties.LocationWithNamespace, listPropiedadesEntidad);
                    relatedEntitiesList.Add(entLocation);
                }
            }

            List<ImageToAttach> imagesToAttach = new List<ImageToAttach>();

            if (route.Properties.ContainsKey(Constants.RouteProperties.Images))
            {
                bool mainImage = true;
                byte[] imageBytes = null;

                foreach (ObjectWithLanguage obj in route.Properties[Constants.RouteProperties.Images].Objects)
                {
                    Entity image = entityList[obj.Value];
                    List<OntologyProperty> listPropiedadesEntidad = new List<OntologyProperty>();

                    if (image.Properties.ContainsKey(Constants.RouteProperties.Image))
                    {
                        string relativeImageUrl = image.Properties[Constants.RouteProperties.Image].Objects[0].Value;

                        imageBytes = DownloadRouteImage(relativeImageUrl);
                    }

                    if (image.Properties.ContainsKey(Constants.RouteProperties.Title))
                    {
                        listPropiedadesEntidad.Add(new StringOntologyProperty(Constants.RouteProperties.TitleWithNamespace, image.Properties[Constants.RouteProperties.Title].Objects[0].Value));
                    }

                    OntologyEntity entImage = new OntologyEntity(Constants.Ontologies.Eharmonise + "Image", Constants.Ontologies.Eharmonise + "Image", Constants.RouteProperties.ImagesWithNamespace, listPropiedadesEntidad);

                    List<ImageAction> listAcciones = new List<ImageAction>();
                    listAcciones.Add(new ImageAction(234, Gnoss.ApiWrapper.Helpers.ImageTransformationType.Crop, 100));
                    listAcciones.Add(new ImageAction(660, Gnoss.ApiWrapper.Helpers.ImageTransformationType.ResizeToHeight, 100));

                    imagesToAttach.Add(new ImageToAttach() { ImageBytes = imageBytes, ActionList = listAcciones, Predicate = Constants.RouteProperties.ImageWithNamespace, Extension = "jpg", IsMainImage = mainImage, Entity = entImage });

                    relatedEntitiesList.Add(entImage);

                    mainImage = false;
                }
            }

            Ontology ont = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://www.obertadecatalunya/route#Route", "http://www.obertadecatalunya/route#Route", prefijosOntologia, listPropiedades, relatedEntitiesList);

            recurso.Ontology = ont;

            recurso.CreationDate = DateTime.Now;
            recurso.Visibility = ResourceVisibility.open;

            foreach (ImageToAttach image in imagesToAttach)
            {
                recurso.AttachImage(image.ImageBytes, image.ActionList, image.Predicate, image.IsMainImage, image.Extension, image.Entity);
            }

            return (recurso);
        }

        private static byte[] DownloadRouteImage(string relativeImagePath)
        {
            byte[] bytes = null;

            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", relativeImagePath);

            if (File.Exists(imagePath))
            {
                bytes = File.ReadAllBytes(imagePath);
            }
            else
            {
                WebClient webClient = new WebClient();

                try
                {
                    bytes = webClient.DownloadData($"http://content1.lariojaturismo.com/{relativeImagePath}");

                    if (imagePath.Contains("?"))
                    {
                        imagePath = imagePath.Substring(0, imagePath.IndexOf('?'));
                    }

                    FileInfo info = new FileInfo(imagePath);

                    if (!Directory.Exists(info.Directory.FullName))
                    {
                        Directory.CreateDirectory(info.Directory.FullName);
                    }

                    File.WriteAllBytes(imagePath, bytes);
                }
                catch (Exception ex)
                {
                    Gnoss.ApiWrapper.Helpers.LogHelper.Instance.Error($"{ex.Message} Stack: {ex.StackTrace}");
                }
            }

            return bytes;
        }
    }

    public class ImageToAttach
    {
        public byte[] ImageBytes { get; set; }
        public List<ImageAction> ActionList { get; set; }
        public string Predicate { get; set; }
        public bool IsMainImage { get; set; }
        public string Extension { get; set; }
        public OntologyEntity Entity { get; set; }
    }
}
