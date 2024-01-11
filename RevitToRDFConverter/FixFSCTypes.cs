using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitToRDFConverter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FixFSCTypes : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector viewCollector = Parser.GetVisibleElementCollector(doc);

            List<FSCTypeComponent> componentList = new List<FSCTypeComponent>();

            foreach (Element component in viewCollector)
            {

                if (component.LookupParameter("FSC_type") != null && (component.LookupParameter("FSC_type").AsValueString() == null || component.LookupParameter("FSC_type").AsValueString() == ""))
                {
                    FSCTypeComponent fscComponent = new FSCTypeComponent();
                    string id = component.Id.ToString();
                    string type = component.Name.ToString();
                    string fsc_type = component.LookupParameter("FSC_type").AsValueString();

                    fscComponent.ID = id;
                    fscComponent.Type = type;
                    fscComponent.FSC_type = fsc_type;
                    componentList.Add(fscComponent);
                }

            }

            if (componentList.Count == 0)
            {
                TaskDialog.Show("No components", "There are no components with missing FSC types");
                return Result.Succeeded;
            }

            FSCTypeForm form = new FSCTypeForm(componentList);
            form.ShowDialog();

            using (Transaction t = new Transaction(doc, "My transaction"))
            {
                t.Start();
                for (int i = 0; i < componentList.Count; i++)
                {
                    string id = componentList[i].ID;
                    string newFsctype = componentList[i].FSC_type;

                    ElementId elementId = new ElementId(Convert.ToInt64(id));
                    Element component = doc.GetElement(elementId);

                    Parameter fscType = component.LookupParameter("FSC_type");
                    if (newFsctype != null)
                    {
                        fscType.Set(newFsctype);

                    }

                }
                t.Commit();
            }

            return Result.Succeeded;
        }

    }
    public class FSCTypeComponent
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string FSC_type { get; set; }
    }
}
