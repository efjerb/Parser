using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using RestSharp;
using System.Net.Http;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System.Globalization;
using System.Threading;
namespace RevitToRDFConverter
{



    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Parser parser = new Parser(doc, uiapp, uidoc, app);

            parser.ParseLevels();
            parser.ParseSpaces();
            //parser.ParseVentilationSystems();
            //parser.ParsePipingSystems();
            parser.ParseVisibleComponents();

            //Converting to string before post request
            string reader = parser.sb.ToString();

            // Encode to ASCII basics (e.g., convert æ, ø & å to ?)
            Encoding encoding;
            encoding = Encoding.ASCII;
            //reader = encoding.GetString(encoding.GetBytes(reader));

            string fullPath = "C:\\Users\\evifj\\Desktop\\testOOP.ttl";
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                writer.WriteLine(reader);
            }

            GraphDBHTTPHelper.OverwriteGraph(reader);

            return Result.Succeeded;
        }
    }
}
