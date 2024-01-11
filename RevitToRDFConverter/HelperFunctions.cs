﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace RevitToRDFConverter
{
    public class Parser
    {
        public StringBuilder sb;
        private Document doc;
        private UIApplication uiapp;
        private UIDocument uidoc;
        private Application app;
        private string buildingGuid;
        private List<string> parsedSystems;

        public Parser(Document doc, UIApplication uiapp, UIDocument uidoc, Application app)
        {
            this.sb = new StringBuilder();
            this.doc = doc;
            this.uiapp = uiapp;
            this.uidoc = uidoc;
            this.app = app;
            parsedSystems = new List<string>();

            // Add prefixes
            sb.Append(
                "@prefix owl: <http://www.w3.org/2002/07/owl#> ." + "\n" +
                "@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> ." + "\n" +
                "@prefix xml: <http://www.w3.org/XML/1998/namespace> ." + "\n" +
                "@prefix xsd: <http://www.w3.org/2001/XMLSchema#> ." + "\n" +
                "@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> ." + "\n" +
                "@prefix bot: <https://w3id.org/bot#> ." + "\n" +
                "@prefix fso: <https://w3id.org/fso#> ." + "\n" +
                "@prefix inst: <https://example.com/inst#> ." + "\n" +
                "@prefix fpo: <https://w3id.org/fpo#> ." + "\n" +
                "@prefix ex: <https://example.com/ex#> ." + "\n" +
                "@prefix ice: <https://example.com/ice#> ." + "\n");

            // Get projectName and assign it as buildingName for now. WORKING
            ProjectInfo projectInfo = doc.ProjectInformation;
            string buildingName = projectInfo.BuildingName;
            buildingGuid = System.Guid.NewGuid().ToString().Replace(' ', '-');
            sb.Append($"inst:{buildingGuid} a bot:Building ." + "\n" + $"inst:{buildingGuid} rdfs:label '{buildingName}'^^xsd:string  ." + "\n");
        }

        public void ParseLevels()
        {
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            //ICollection<Element> levels = levelCollector.OfClass(typeof(Level)).ToElements();
            //List<Level> levelList = new List<Level>();
            foreach (Level level in levelCollector)
            {
                string levelName = level.Name.Replace(' ', '-');
                string guidNumber = level.UniqueId.ToString();
                sb.Append($"inst:{guidNumber} a bot:Storey ." + "\n"
                    + $"inst:{guidNumber} rdfs:label '{levelName}'^^xsd:string ." + "\n" + $"inst:{buildingGuid} bot:hasStorey inst:{guidNumber} ." + "\n");
            }
        }

        public void ParseSpaces()
        {
            //Get all spaces and the building it is related to. WOKRING 
            FilteredElementCollector roomCollector = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement));
            foreach (SpatialElement space in roomCollector)
            {
                if (space.Category.Name == "Spaces" & space.LookupParameter("Area").AsDouble() > 0)
                {
                    string spaceName = space.Name.Replace(' ', '-').Replace('\n', '-');
                    string spaceGuid = space.UniqueId.ToString();
                    string isSpaceOf = space.Level.UniqueId;

                    sb.Append($"inst:{spaceGuid} a bot:Space ." + "\n" +
                        $"inst:{spaceGuid} rdfs:label '{spaceName}'^^xsd:string ." + "\n" +
                        $"inst:{isSpaceOf} bot:hasSpace inst:{spaceGuid} ." + "\n");
                };
            }

        }

        private void ParseVentilationSystem(MechanicalSystem system)
        {

            DuctSystemType systemType = system.SystemType;

            string systemID = system.UniqueId;
            string systemName = system.Name;
            //    ElementId superSystemType = system.GetTypeId();
            //    string superSystemName = doc.GetElement(superSystemType).LookupParameter("Family Name").AsValueString();
            //    string superSystemID = doc.GetElement(superSystemType).UniqueId;
            string fluidID = System.Guid.NewGuid().ToString().Replace(' ', '-');
            string flowTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-');

            string fluidTemperatureID = System.Guid.NewGuid().ToString().Replace(' ', '-');

            double fluidTemperature;

            //try
            //{
            //    fluidTemperature = UnitUtils.ConvertFromInternalUnits(system.LookupParameter("Fluid TemperatureX").AsDouble(), UnitTypeId.Celsius);
            //}
            //catch (Exception)
            //{
            //    fluidTemperature = 0;
            //}

            fluidTemperature = 0;

            switch (systemType)
            {
                case DuctSystemType.SupplyAir:
                    sb.Append($"inst:{systemID} a fso:SupplySystem ." + "\n" +
                        $"inst:{systemID} rdfs:label '{systemName}'^^xsd:string ." + "\n" +

                        $"inst:{systemID} fso:hasFlow inst:{fluidID} ." + "\n" +
                        $"inst:{fluidID} a fso:Flow ." + "\n" +
                        $"inst:{fluidID} fpo:hasFlowType inst:{flowTypeID} ." + "\n" +
                        $"inst:{flowTypeID} a fpo:FlowType ." + "\n" +
                        $"inst:{flowTypeID} fpo:hasValue 'Air'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasTemperature inst:{fluidTemperatureID} ." + "\n" +
                        $"inst:{fluidTemperatureID} a fpo:Temperature ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasValue '{fluidTemperature}'^^xsd:double ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasUnit 'Celcius'^^xsd:string ." + "\n");
                    break;
                case DuctSystemType.ReturnAir:
                    sb.Append($"inst:{systemID} a fso:ReturnSystem ." + "\n"
                         + $"inst:{systemID} rdfs:label '{systemName}'^^xsd:string ." + "\n" +

                         $"inst:{systemID} fso:hasFlow inst:{fluidID} ." + "\n" +
                        $"inst:{fluidID} a fso:Flow ." + "\n" +
                        $"inst:{fluidID} fpo:hasFlowType inst:{flowTypeID} ." + "\n" +
                        $"inst:{flowTypeID} a fpo:FlowType ." + "\n" +
                        $"inst:{flowTypeID} fpo:hasValue 'Air'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasTemperature inst:{fluidTemperatureID} ." + "\n" +
                        $"inst:{fluidTemperatureID} a fpo:Temperature ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasValue '{fluidTemperature}'^^xsd:double ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasUnit 'Celcius'^^xsd:string ." + "\n");
                    break;
                case
               DuctSystemType.ExhaustAir:
                    sb.Append($"inst:{systemID} a fso:ReturnSystem ." + "\n"
                         + $"inst:{systemID} rdfs:label '{systemName}'^^xsd:string ." + "\n" +
                          $"inst:{systemID} fso:hasFlow inst:{fluidID} ." + "\n" +

                        $"inst:{fluidID} a fso:Flow ." + "\n" +
                        $"inst:{fluidID} fpo:hasTemperature inst:{fluidTemperatureID} ." + "\n" +
                        $"inst:{fluidTemperatureID} a fpo:Temperature ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasValue '{fluidTemperature}'^^xsd:double ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasUnit 'Celcius'^^xsd:string ." + "\n");
                    break;
                default:
                    break;
            }

        }

        private void ParseVentilationComponents(MechanicalSystem system)
        {
            string systemID = system.UniqueId;

            ElementSet systemComponents = system.DuctNetwork;

            //Relate components to systems
            foreach (Element component in systemComponents)
            {
                string componentID = component.UniqueId;
                string revitID = component.Id.ToString();

                string componentCategory = component.Category.Name;
                string componentType;

                

                if (component.LookupParameter("FSC_type") != null)
                {
                    componentType = component.LookupParameter("FSC_type").AsValueString();
                }
                else
                {
                    componentType = component.LookupParameter("Category").AsValueString();
                }
                
                //sb.Append($"inst:{systemID} fso:hasComponent inst:{componentID} ." + "\n");

                MapComponent(new List<string> { systemID }, component);
            }
        }

        public void ParseVentilationSystems()
        {
            //Get systems
            FilteredElementCollector ventilationSystemCollector = new FilteredElementCollector(doc).OfClass(typeof(MechanicalSystem));
            foreach (MechanicalSystem system in ventilationSystemCollector)
            {
                ParseVentilationSystem(system);

                ParseVentilationComponents(system);
            }
        }

        public void ParsePipingSystem(PipingSystem system)
        {
            PipeSystemType systemType = system.SystemType;
            string systemID = system.UniqueId;
            string systemName = system.Name;
            ElementId superSystemType = system.GetTypeId();

            //Fluid
            string fluidID = System.Guid.NewGuid().ToString().Replace(' ', '-');
            string flowTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-');
            string fluidTemperatureID = System.Guid.NewGuid().ToString().Replace(' ', '-');
            string fluidViscosityID = System.Guid.NewGuid().ToString().Replace(' ', '-');
            string fluidDensityID = System.Guid.NewGuid().ToString().Replace(' ', '-');

            string flowType = doc.GetElement(superSystemType).LookupParameter("Fluid Type").AsValueString();
            double fluidTemperature = UnitUtils.ConvertFromInternalUnits(system.LookupParameter("Fluid TemperatureX").AsDouble(), UnitTypeId.Celsius);
            double fluidViscosity = UnitUtils.ConvertFromInternalUnits(doc.GetElement(superSystemType).LookupParameter("Fluid Dynamic Viscosity").AsDouble(), UnitTypeId.PascalSeconds);
            double fluidDensity = UnitUtils.ConvertFromInternalUnits(doc.GetElement(superSystemType).LookupParameter("Fluid Density").AsDouble(), UnitTypeId.KilogramsPerCubicMeter);

            switch (systemType)
            {
                case PipeSystemType.SupplyHydronic:
                    sb.Append($"inst:{systemID} a fso:SupplySystem ." + "\n"
                        + $"inst:{systemID} rdfs:label '{systemName}'^^xsd:string ." + "\n" +

                        $"inst:{systemID} fso:hasFlow inst:{fluidID} ." + "\n" +

                        $"inst:{fluidID} a fso:Flow ." + "\n" +
                        $"inst:{fluidID} fpo:hasFlowType inst:{flowTypeID} ." + "\n" +
                        $"inst:{flowTypeID} a fpo:FlowType ." + "\n" +
                         $"inst:{flowTypeID} fpo:hasValue '{flowType}'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasTemperature inst:{fluidTemperatureID} ." + "\n" +
                        $"inst:{fluidTemperatureID} a fpo:Temperature ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasValue '{fluidTemperature}'^^xsd:double ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasUnit 'Celcius'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasViscosity inst:{fluidViscosityID} ." + "\n" +
                        $"inst:{fluidViscosityID} a fpo:Viscosity ." + "\n" +
                        $"inst:{fluidViscosityID} fpo:hasValue '{fluidViscosity}'^^xsd:double ." + "\n" +
                        $"inst:{fluidViscosityID} fpo:hasUnit 'Pascal per second'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasDensity inst:{fluidDensityID} ." + "\n" +
                        $"inst:{fluidDensityID} a fpo:Density ." + "\n" +
                        $"inst:{fluidDensityID} fpo:hasValue '{fluidDensity}'^^xsd:double ." + "\n" +
                        $"inst:{fluidDensityID} fpo:hasUnit 'Kilograms per cubic meter'^^xsd:string ." + "\n"
                        );
                    break;
                case PipeSystemType.ReturnHydronic:
                    sb.Append($"inst:{systemID} a fso:ReturnSystem ." + "\n" +
                        $"inst:{systemID} rdfs:label '{systemName}'^^xsd:string ." + "\n" +

                        $"inst:{systemID} fso:hasFlow inst:{fluidID} ." + "\n" +

                       $"inst:{fluidID} a fso:Flow ." + "\n" +
                        $"inst:{fluidID} fpo:hasFlowType inst:{flowTypeID} ." + "\n" +
                        $"inst:{flowTypeID} a fpo:FlowType ." + "\n" +
                        $"inst:{flowTypeID} fpo:hasValue '{flowType}'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasTemperature inst:{fluidTemperatureID} ." + "\n" +
                        $"inst:{fluidTemperatureID} a fpo:Temperature ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasValue '{fluidTemperature}'^^xsd:double ." + "\n" +
                        $"inst:{fluidTemperatureID} fpo:hasUnit 'Celcius'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasViscosity inst:{fluidViscosityID} ." + "\n" +
                        $"inst:{fluidViscosityID} a fpo:Viscosity ." + "\n" +
                        $"inst:{fluidViscosityID} fpo:hasValue '{fluidViscosity}'^^xsd:double ." + "\n" +
                        $"inst:{fluidViscosityID} fpo:hasUnit 'Pascal per second'^^xsd:string ." + "\n" +

                        $"inst:{fluidID} fpo:hasDensity inst:{fluidDensityID} ." + "\n" +
                        $"inst:{fluidDensityID} a fpo:Density ." + "\n" +
                        $"inst:{fluidDensityID} fpo:hasValue '{fluidDensity}'^^xsd:double ." + "\n" +
                        $"inst:{fluidDensityID} fpo:hasUnit 'Kilograms per cubic meter'^^xsd:string ." + "\n"
                        );
                    break;
                default:
                    break;
            }
        
        }

        public void ParsePipingComponents(PipingSystem system)
        {
            string systemID = system.UniqueId;

            ElementSet systemComponents = system.PipingNetwork;

            //Relate components to systems
            foreach (Element component in systemComponents)
            {
                string componentID = component.UniqueId;
                sb.Append($"inst:{systemID} fso:hasComponent inst:{componentID} ." + "\n");
            }
        }

        public void ParseSystem(MEPSystem system)
        {
            if (system is MechanicalSystem)
            {
                ParseVentilationSystem((MechanicalSystem)system);
            }
            else if (system is PipingSystem)
            {
                ParsePipingSystem((PipingSystem)system);
            }
        }

        public void ParsePipingSystems()
        {
            FilteredElementCollector hydraulicSystemCollector = new FilteredElementCollector(doc).OfClass(typeof(PipingSystem));
            
            foreach (PipingSystem system in hydraulicSystemCollector)
            {
                ParsePipingSystem(system);

                ParsePipingComponents(system);
            }
        }
        
        public List<MEPSystem> GetComponentSystems(Duct component)
        {
            List<MEPSystem> systems = new List<MEPSystem>
            {
                component.MEPSystem
            };

            return systems;
        }
        public List<MEPSystem> GetComponentSystems(Pipe component)
        {
            List<MEPSystem> systems = new List<MEPSystem>
            {
                component.MEPSystem
            };

            return systems;
        }
        public List<MEPSystem> GetComponentSystems(FamilyInstance component)
        {
            List<MEPSystem> systems = new List<MEPSystem>();
            if (component.MEPModel.ConnectorManager != null)
            {
                ConnectorSet connectors = component.MEPModel.ConnectorManager.Connectors;
                foreach (Connector connector in connectors)
                {
                    if (connector.MEPSystem != null)
                    {
                        systems.Add(connector.MEPSystem);
                    }
                }
                return systems;
            }
            else return null;
            
        }
        public List<MEPSystem> GetComponentSystems(Element component)
        {
            List<MEPSystem> systems;

            if (component is Duct)
            {
                systems = GetComponentSystems((Duct)component);
            }
            else if (component is Pipe)
            {
                systems = GetComponentSystems((Pipe)component);
            }
            else if (component is FamilyInstance)
            {
                systems = GetComponentSystems((FamilyInstance)component);
            }
            else
            {
                systems = null;
            }
            return systems;
            
        }
        public static FilteredElementCollector GetVisibleElementCollector(Document doc)
        {
            BuiltInCategory[] categoriesToFilter = new BuiltInCategory[]
            {
                BuiltInCategory.OST_DuctCurves,
                BuiltInCategory.OST_DuctFitting,
                BuiltInCategory.OST_DuctTerminal,
                BuiltInCategory.OST_DuctAccessory,
                BuiltInCategory.OST_MechanicalEquipment,
                BuiltInCategory.OST_PipeCurves,
                BuiltInCategory.OST_PipeFitting,
                BuiltInCategory.OST_PipeAccessory,
                //BuiltInCategory.OST_FlexDuctCurves,
                //BuiltInCategory.OST_PlumbingFixtures,
                //BuiltInCategory.OST_SpecialityEquipment,
                //BuiltInCategory.OST_Sprinklers
            };

            IList<ElementFilter> a = new List<ElementFilter>(categoriesToFilter.Count());

            foreach (BuiltInCategory bic in categoriesToFilter)
            {
                a.Add(new ElementCategoryFilter(bic));
            }

            LogicalOrFilter categoryFilter
              = new LogicalOrFilter(a);

            FilteredElementCollector viewCollector = new FilteredElementCollector(doc, doc.ActiveView.Id);

            viewCollector.WherePasses(categoryFilter);
            return viewCollector;
        }
        public void ParseVisibleComponents()
        {
            FilteredElementCollector viewCollector = GetVisibleElementCollector(doc);

            foreach (Element component in viewCollector)
            {
                List<string> systemIDs = new List<string>();
                List<MEPSystem> systems = GetComponentSystems(component);

                if (systems != null)
                {
                    foreach (MEPSystem system in systems)
                    {
                        string systemID = system.UniqueId;
                        systemIDs.Add(systemID);
                        if (!parsedSystems.Contains(systemID))
                        {
                            ParseSystem(system);
                            parsedSystems.Add(systemID);
                        }
                    }
                }

                MapComponent(systemIDs, component);
            }

        }

        public void MapComponent(List<string> systemIDs, Element component)
        {
            string componentID = component.UniqueId;
            string componentCategory = component.Category.Name;
            string revitID = component.Id.ToString();
            
            if (componentCategory == "Ducts")
            {
                Duct duct = component as Duct;
                
                // If the duct has more than two connectors, split it into multiple ducts
                if (duct.ConnectorManager.Connectors.Size > 2)
                {
                    SplitSegment(duct);
                    return;
                }

                sb.Append(
                $"inst:{componentID} a fso:Duct ." + "\n");

                if (duct.DuctType.Roughness != null)
                {
                    //Roughness
                    string roughnessID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    double rougnessValue = duct.DuctType.Roughness;
                    sb.Append($"inst:{componentID} fpo:hasRoughness inst:{roughnessID} ." + "\n"
                     + $"inst:{roughnessID} a fpo:Roughness ." + "\n"
                     + $"inst:{roughnessID} fpo:hasValue '{rougnessValue}'^^xsd:double ." + "\n" +
                     $"inst:{roughnessID} fpo:hasUnit 'Meter'^^xsd:string ." + "\n");
                }

                if (duct.LookupParameter("Length") != null)
                {
                    //Length
                    string lengthID = System.Guid.NewGuid().ToString().Replace(' ', '-'); ;
                    double lengthValue = UnitUtils.ConvertFromInternalUnits(duct.LookupParameter("Length").AsDouble(), UnitTypeId.Meters);
                    sb.Append($"inst:{componentID} fpo:hasLength inst:{lengthID} ." + "\n"
                     + $"inst:{lengthID} a fpo:Length ." + "\n"
                     + $"inst:{lengthID} fpo:hasValue '{lengthValue}'^^xsd:double ." + "\n"
                     + $"inst:{lengthID} fpo:hasUnit 'Meter'^^xsd:string ." + "\n");
                }

                if (duct.LookupParameter("Hydraulic Diameter") != null)
                {
                    //Outside diameter
                    string outsideDiameterID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    double outsideDiameterValue = UnitUtils.ConvertFromInternalUnits(duct.LookupParameter("Hydraulic Diameter").AsDouble(), UnitTypeId.Meters);
                    sb.Append($"inst:{componentID} fpo:hasHydraulicDiameter inst:{outsideDiameterID} ." + "\n"
                     + $"inst:{outsideDiameterID} a fpo:HydraulicDiameter ." + "\n"
                     + $"inst:{outsideDiameterID} fpo:hasValue '{outsideDiameterValue}'^^xsd:double ." + "\n"
                     + $"inst:{outsideDiameterID} fpo:hasUnit 'meter'^^xsd:string ." + "\n");
                }


                //MaterialType
                string materialTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                string materialTypeValue = duct.Name;
                sb.Append($"inst:{componentID} fpo:hasMaterialType inst:{materialTypeID} ." + "\n"
                 + $"inst:{materialTypeID} a fpo:MaterialType ." + "\n"
                 + $"inst:{materialTypeID} fpo:hasValue '{materialTypeValue}'^^xsd:string ." + "\n");


                if (duct.LookupParameter("Loss Coefficient") != null)
                {
                    //frictionFactor 
                    string frictionFactorID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    double frictionFactorValue = duct.LookupParameter("Loss Coefficient").AsDouble();
                    sb.Append($"inst:{componentID} fpo:hasFrictionFactor inst:{frictionFactorID} ." + "\n"
                     + $"inst:{frictionFactorID} a fpo:FrictionFactor ." + "\n"
                     + $"inst:{frictionFactorID} fpo:hasValue '{frictionFactorValue}'^^xsd:double ." + "\n");
                }

                if (duct.LookupParameter("Friction") != null)
                {
                    //friction
                    string frictionID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    double frictionIDValue = component.LookupParameter("Friction").AsDouble();
                    sb.Append($"inst:{componentID} fpo:hasFriction inst:{frictionID} ." + "\n"
                     + $"inst:{frictionID} a fpo:Friction ." + "\n"
                     + $"inst:{frictionID} fpo:hasValue '{frictionIDValue}'^^xsd:double ." + "\n"
                     + $"inst:{frictionID} fpo:hasUnit 'Pascal per meter'^^xsd:string ." + "\n");
                }

                sb.Append(RelatedPorts.DuctConnectors(duct, componentID));


            }
            else if (componentCategory == "Pipes")
            {
                Pipe pipe = component as Pipe;

                sb.Append(
                    $"inst:{componentID} a fso:Pipe ." + "\n");

                if (pipe.PipeType.Roughness != null)
                {
                    //Roughness
                    string roughnessID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    double rougnessValue = pipe.PipeType.Roughness;
                    sb.Append($"inst:{componentID} fpo:hasRoughness inst:{roughnessID} ." + "\n"
                     + $"inst:{roughnessID} a fpo:Roughness ." + "\n"
                     + $"inst:{roughnessID} fpo:hasValue '{rougnessValue}'^^xsd:double ." + "\n" +
                     $"inst:{roughnessID} fpo:hasUnit 'Meter'^^xsd:string ." + "\n");
                }
                if (pipe.LookupParameter("Length") != null)
                {
                    //Length
                    string lengthID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    double lengthValue = UnitUtils.ConvertFromInternalUnits(pipe.LookupParameter("Length").AsDouble(), UnitTypeId.Meters);
                    sb.Append($"inst:{componentID} fpo:hasLength inst:{lengthID} ." + "\n"
                     + $"inst:{lengthID} a fpo:Length ." + "\n"
                     + $"inst:{lengthID} fpo:hasValue '{lengthValue}'^^xsd:double ." + "\n"
                     + $"inst:{lengthID} fpo:hasUnit 'Meter'^^xsd:string ." + "\n");
                }


                //MaterialType
                string materialTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                string materialTypeValue = pipe.Name;
                sb.Append($"inst:{componentID} fpo:hasMaterialType inst:{materialTypeID} ." + "\n"
                 + $"inst:{materialTypeID} a fpo:MaterialType ." + "\n"
                 + $"inst:{materialTypeID} fpo:hasValue '{materialTypeValue}'^^xsd:string ." + "\n");


                sb.Append(RelatedPorts.PipeConnectors(pipe, componentID));
            }
            else if (componentCategory == "Duct Fittings" || componentCategory == "Pipe Fittings")
            {
                MEPModel fitting = ((FamilyInstance)component).MEPModel;
                string fittingType = ((MechanicalFitting)fitting).PartType.ToString();
                if (fittingType == "TapAdjustable" || fittingType == "TapPerpendicular")
                {
                    return;
                }
                sb.Append($"inst:{componentID} a fso:{fittingType} ." + "\n");

                if (fittingType == "Tee")
                {

                    //MaterialType
                    string materialTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-'); ;
                    string materialTypeValue = component.Name;
                    sb.Append($"inst:{componentID} fpo:hasMaterialType inst:{materialTypeID} ." + "\n"
                     + $"inst:{materialTypeID} a fpo:MaterialType ." + "\n"
                     + $"inst:{materialTypeID} fpo:hasValue  '{materialTypeValue}'^^xsd:string ." + "\n");
                }

                else if (fittingType == "Elbow")
                {
                    if (component.LookupParameter("Angle") != null)
                    {
                        //Angle
                        string angleID = System.Guid.NewGuid().ToString().Replace(' ', '-'); ;
                        double angleValue = UnitUtils.ConvertFromInternalUnits(component.LookupParameter("Angle").AsDouble(), UnitTypeId.Degrees);
                        sb.Append($"inst:{componentID} fpo:hasAngle inst:{angleID} ." + "\n"
                         + $"inst:{angleID} a fpo:Angle ." + "\n"
                         + $"inst:{angleID} fpo:hasValue  '{angleValue}'^^xsd:double ." + "\n"
                         + $"inst:{angleID} fpo:hasUnit  'Degree'^^xsd:string ." + "\n");
                    }
                    //MaterialType
                    string materialTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-'); ;
                    string materialTypeValue = component.Name;
                    sb.Append($"inst:{componentID} fpo:hasMaterialType inst:{materialTypeID} ." + "\n"
                     + $"inst:{materialTypeID} a fpo:MaterialType ." + "\n"
                     + $"inst:{materialTypeID} fpo:hasValue  '{materialTypeValue}'^^xsd:string ." + "\n");

                }

                else if (fittingType == "Transition")
                {
                    //MaterialType
                    string materialTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-'); ;
                    string materialTypeValue = component.Name;
                    sb.Append($"inst:{componentID} fpo:hasMaterialType inst:{materialTypeID} ." + "\n"
                     + $"inst:{materialTypeID} a fpo:MaterialType ." + "\n"
                     + $"inst:{materialTypeID} fpo:hasValue '{materialTypeValue}'^^xsd:string ." + "\n");

                    if (component.LookupParameter("OffsetHeight") != null && component.LookupParameter("OffsetHeight").AsDouble() > 0)
                    {
                        //Length
                        string lengthID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        double lengthValue = UnitUtils.ConvertFromInternalUnits(component.LookupParameter("OffsetHeight").AsDouble(), UnitTypeId.Meters);
                        sb.Append($"inst:{componentID} fpo:hasLength inst:{lengthID} ." + "\n"
                         + $"inst:{lengthID} a fpo:Length ." + "\n"
                         + $"inst:{lengthID} fpo:hasValue '{lengthValue}'^^xsd:double ." + "\n"
                         + $"inst:{lengthID} fpo:hasUnit 'Meter'^^xsd:string ." + "\n");
                    }
                    else
                    {
                        string lengthID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        double lengthValue = 0.02;
                        sb.Append($"inst:{componentID} fpo:hasLength inst:{lengthID} ." + "\n"
                       + $"inst:{lengthID} a fpo:Length ." + "\n"
                       + $"inst:{lengthID} fpo:hasValue '{lengthValue}'^^xsd:double ." + "\n"
                       + $"inst:{lengthID} fpo:hasUnit 'Meter'^^xsd:string ." + "\n");
                    }
                }
                
                sb.Append(RelatedPorts.FamilyInstanceConnectors((FamilyInstance)component, componentID));
                
            }
            else if (componentCategory == "Air Terminals")
            {
                FamilyInstance terminal = component as FamilyInstance;

                //Type
                sb.Append($"inst:{componentID} a fso:AirTerminal ." + "\n");

                string relatedRoomID = GetSpaceID(terminal);

                if (terminal.LookupParameter("System Classification").AsString() == "Return Air")
                {
                    //AirTerminalType
                    string airTerminalTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    string airTerminalTypeValue = "outlet";
                    sb.Append($"inst:{componentID} fpo:hasAirTerminalType inst:{airTerminalTypeID} ." + "\n"
                     + $"inst:{airTerminalTypeID} a fpo:AirTerminalType ." + "\n"
                     + $"inst:{airTerminalTypeID} fpo:hasValue '{airTerminalTypeValue}'^^xsd:string ." + "\n");

                    //Relation to room and space
                    if (relatedRoomID != null)
                    {
                        sb.Append($"inst:{relatedRoomID} fso:suppliesFluidTo inst:{componentID} ." + "\n");
                    }
                    

                    //Adding a fictive port the airterminal which is not included in Revit
                    string connectorID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    sb.Append($"inst:{componentID} fso:hasPort inst:{connectorID} ." + "\n"
                        + $"inst:{connectorID} a fso:Port ." + "\n");

                    //Diameter to fictive port 

                    //FlowDirection to fictive port
                    string connectorDirectionID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    string connectorDirection = "In";

                    sb.Append($"inst:{connectorID} fpo:hasFlowDirection inst:{connectorDirectionID} ." + "\n"
                                            + $"inst:{connectorDirectionID} a fpo:FlowDirection ." + "\n"
                                            + $"inst:{connectorDirectionID} fpo:hasValue '{connectorDirection}'^^xsd:string ." + "\n");
                }


                if (terminal.LookupParameter("System Classification").AsString() == "Supply Air")
                {
                    //AirTerminalType
                    string airTerminalTypeID = System.Guid.NewGuid().ToString().Replace(' ', '-'); ;
                    string airTerminalTypeValue = "inlet";
                    sb.Append($"inst:{componentID} fpo:hasAirTerminalType inst:{airTerminalTypeID} ." + "\n"
                     + $"inst:{airTerminalTypeID} a fpo:AirTerminalType ." + "\n"
                     + $"inst:{airTerminalTypeID} fpo:hasValue '{airTerminalTypeValue}'^^xsd:string ." + "\n");

                    //Relation to room and space
                    if (relatedRoomID != null)
                    {
                        sb.Append($"inst:{componentID} fso:suppliesFluidTo inst:{relatedRoomID} ." + "\n");
                    }

                    //Adding a fictive port the airterminal which is not included in Revit
                    string connectorID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    sb.Append($"inst:{componentID} fso:hasPort inst:{connectorID} ." + "\n"
                        + $"inst:{connectorID} a fso:Port ." + "\n");

                    //FlowDirection
                    string connectorDirectionID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    string connectorDirection = "Out";

                    sb.Append($"inst:{connectorID} fpo:hasFlowDirection inst:{connectorDirectionID} ." + "\n"
                                            + $"inst:{connectorDirectionID} a fpo:FlowDirection ." + "\n"
                                            + $"inst:{connectorDirectionID} fpo:hasValue '{connectorDirection}'^^xsd:string ." + "\n");


                    //Fictive pressureDrop
                    string pressureDropID = System.Guid.NewGuid().ToString().Replace(' ', '-'); ;
                    double pressureDropValue = 5;
                    sb.Append($"inst:{connectorID} fpo:hasPressureDrop inst:{pressureDropID} ." + "\n"
                   + $"inst:{pressureDropID} a fpo:PressureDrop ." + "\n"
                   + $"inst:{pressureDropID} fpo:hasValue '{pressureDropValue}'^^xsd:double ." + "\n"
                   + $"inst:{pressureDropID} fpo:hasUnit 'Pascal'^^xsd:string ." + "\n");

                    //if (terminal.LookupParameter("Flow") != null)
                    //{
                    //    //Flow rate
                    //    string flowID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    //    double flowValue = UnitUtils.ConvertFromInternalUnits(component.LookupParameter("Flow").AsDouble(), UnitTypeId.LitersPerSecond);
                    //    sb.Append($"inst:{connectorID} fpo:flowRate inst:{flowID} ." + "\n"
                    //     + $"inst:{flowID} a fpo:FlowRate ." + "\n"
                    //     + $"inst:{flowID} fpo:hasValue '{flowValue}'^^xsd:double ." + "\n"
                    //     + $"inst:{flowID} fpo:hasUnit 'Liters per second'^^xsd:string ." + "\n");
                    //}


                }



            }
            else if (component.LookupParameter("FSC_type") != null && component.LookupParameter("FSC_type").AsValueString() != null && component.LookupParameter("FSC_type").AsValueString() != "")
            {
                string fscType = component.LookupParameter("FSC_type").AsValueString();
                FamilyInstance componentFI = component as FamilyInstance;
                
                //Type 
                sb.Append($"inst:{componentID} a fso:{fscType} ." + "\n");

                //Fan
                if (fscType == "Fan")
                {
                    
                    if (component.LookupParameter("FSC_pressureCurve") != null)
                    {
                        //PressureCurve
                        string pressureCurveID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        string pressureCurveValue = component.LookupParameter("FSC_pressureCurve").AsString();
                        sb.Append($"inst:{componentID} fpo:hasPressureCurve inst:{pressureCurveID} ." + "\n"
                         + $"inst:{pressureCurveID} a fpo:PressureCurve ." + "\n"
                         + $"inst:{pressureCurveID} fpo:hasCurve  '{pressureCurveValue}'^^xsd:string ." + "\n"
                         + $"inst:{pressureCurveID} fpo:hasUnit  'PA:m3/h'^^xsd:string ." + "\n");
                    }

                    if (component.LookupParameter("FSC_powerCurve") != null)
                    {
                        //PowerCurve
                        string powerCurveID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        string powerCurveValue = component.LookupParameter("FSC_powerCurve").AsString();
                        sb.Append($"inst:{componentID} fpo:hasPowerCurve inst:{powerCurveID} ." + "\n"
                         + $"inst:{powerCurveID} a fpo:PowerCurve ." + "\n"
                         + $"inst:{powerCurveID} fpo:hasCurve  '{powerCurveValue}'^^xsd:string ." + "\n"
                         + $"inst:{powerCurveID} fpo:hasUnit  'PA:m3/h'^^xsd:string ." + "\n");
                    }

                }
                
                //Pump
                else if (fscType == "Pump")
                {
                    if (component.LookupParameter("FSC_pressureCurve") != null)
                    {
                        //PressureCurve
                        string pressureCurveID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        string pressureCurveValue = component.LookupParameter("FSC_pressureCurve").AsString();
                        sb.Append($"inst:{componentID} fpo:hasPressureCurve inst:{pressureCurveID} ." + "\n"
                         + $"inst:{pressureCurveID} a fpo:PressureCurve ." + "\n"
                         + $"inst:{pressureCurveID} fpo:hasCurve  '{pressureCurveValue}'^^xsd:string ." + "\n"
                         + $"inst:{pressureCurveID} fpo:hasUnit  'PA:m3/h'^^xsd:string ." + "\n");
                    }

                    if (component.LookupParameter("FSC_powerCurve") != null)
                    {
                        //PowerCurve
                        string powerCurveID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        string powerCurveValue = component.LookupParameter("FSC_powerCurve").AsString();
                        sb.Append($"inst:{componentID} fpo:hasPowerCurve inst:{powerCurveID} ." + "\n"
                         + $"inst:{powerCurveID} a fpo:PowerCurve ." + "\n"
                         + $"inst:{powerCurveID} fpo:hasCurve  '{powerCurveValue}'^^xsd:string ." + "\n"
                         + $"inst:{powerCurveID} fpo:hasUnit  'PA:m3/h'^^xsd:string ." + "\n");
                    }
                }

                //Valve
                else if (fscType == "MotorizedValve" || fscType == "BalancingValve")
                {
                    //Type 
                    sb.Append($"fso:{fscType} rdfs:subClassOf fso:Valve ." + "\n");

                    if (component.LookupParameter("FSC_kv") != null)
                    {
                        //Kv
                        string kvID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        double kvValue = component.LookupParameter("FSC_kv").AsDouble();
                        sb.Append($"inst:{componentID} fpo:hasKv inst:{kvID} ." + "\n"
                         + $"inst:{kvID} a fpo:Kv ." + "\n"
                         + $"inst:{kvID} fpo:hasValue  '{kvValue}'^^xsd:double ." + "\n");
                    }

                    if (component.LookupParameter("FSC_kvs") != null)
                    {
                        //Kvs
                        string kvsID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        double kvsValue = component.LookupParameter("FSC_kvs").AsDouble();
                        sb.Append($"inst:{componentID} fpo:hasKvs inst:{kvsID} ." + "\n"
                             + $"inst:{kvsID} a fpo:Kvs ." + "\n"
                             + $"inst:{kvsID} fpo:hasValue  '{kvsValue}'^^xsd:double ." + "\n");
                    }
                }

                //Shunt
                else if (fscType == "Shunt")
                {
                    //Type 
                    sb.Append($"fso:{fscType} rdfs:subClassOf fpo:Valve ." + "\n");

                    if (component.LookupParameter("FSC_hasCheckValve") != null)
                    {
                        //hasCheckValve
                        string hasCheckValveID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        string hasCheckValveValue = component.LookupParameter("FSC_hasCheckValve").AsValueString();
                        sb.Append($"inst:{componentID} fpo:hasCheckValve inst:{hasCheckValveID} ." + "\n"
                         + $"inst:{hasCheckValveID} a fpo:CheckValve ." + "\n"
                         + $"inst:{hasCheckValveID} fpo:hasValue  '{hasCheckValveValue}'^^xsd:string ." + "\n");
                    }
                }

                //Damper
                else if (fscType == "MotorizedDamper" || fscType == "BalancingDamper")
                {
                    //Type 
                    sb.Append($"fso:{fscType} rdfs:subClassOf fpo:Damper ." + "\n");

                    if (component.LookupParameter("FSC_kv") != null)
                    {
                        //Kv
                        string kvID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        double kvValue = component.LookupParameter("FSC_kv").AsDouble();
                        sb.Append($"inst:{componentID} fpo:hasKv inst:{kvID} ." + "\n"
                         + $"inst:{kvID} a fpo:Kv ." + "\n"
                         + $"inst:{kvID} fpo:hasValue  '{kvValue}'^^xsd:double ." + "\n");
                    }

                    if (component.LookupParameter("FSC_kvs") != null)
                    {
                        //Kvs
                        string kvsID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                        double kvsValue = component.LookupParameter("FSC_kvs").AsDouble();
                        sb.Append($"inst:{componentID} fpo:hasKvs inst:{kvsID} ." + "\n"
                         + $"inst:{kvsID} a fpo:Kvs ." + "\n"
                         + $"inst:{kvsID} fpo:hasValue  '{kvsValue}'^^xsd:double ." + "\n");
                    }
                }

                //Radiator
                else if (fscType == "Radiator")
                {

                    //DesignHeatPower
                    string designHeatPowerID = System.Guid.NewGuid().ToString().Replace(' ', '-');
                    double designHeatPowerValue = UnitUtils.ConvertFromInternalUnits(component.LookupParameter("FSC_nomPower").AsDouble(), UnitTypeId.Watts);
                    sb.Append($"inst:{componentID} fpo:hasDesignHeatingPower inst:{designHeatPowerID} ." + "\n"
                     + $"inst:{designHeatPowerID} a fpo:DesignHeatingPower ." + "\n"
                     + $"inst:{designHeatPowerID} fpo:hasValue  '{designHeatPowerValue}'^^xsd:double ." + "\n"
                     + $"inst:{designHeatPowerID} fpo:hasUnit  'Watts'^^xsd:string ." + "\n");

                    string relatedRoomID = GetSpaceID((FamilyInstance)component);

                    if (relatedRoomID != null)
                    {
                        sb.Append($"inst:{componentID} fso:transfersHeatTo inst:{relatedRoomID} ." + "\n");
                    }
                }
            }
            else
            {
                string familyName = component.Name;
                sb.Append($"inst:{componentID} a fso:Component ." + "\n");
                sb.Append($"inst:{componentID} ex:rvtTypeName '{familyName}'^^xsd:string ." + "\n");
            }
            // This is added in the end, because some components should not be mapped
            foreach (string systemID in systemIDs)
            {
                sb.Append($"inst:{systemID} fso:hasComponent inst:{componentID} ." + "\n");
            }
            
            sb.Append($"inst:{componentID} ex:RevitID \"{revitID}\" ." + "\n");
        }


        public void SplitSegment(Duct duct)
        {
            string componentID = duct.UniqueId;
            string revitID = duct.Id.ToString();

            // Get the duct's connectors
            ConnectorSet connectors = duct.ConnectorManager.Connectors;

            // Get the duct's start and end connectors by the connector type
            Connector startConnector = connectors.Cast<Connector>().FirstOrDefault<Connector>(c => c.ConnectorType == ConnectorType.End);
            Connector endConnector = connectors.Cast<Connector>().LastOrDefault<Connector>(c => c.ConnectorType == ConnectorType.End);

            // Create a connector set with the start and end connectors
            ConnectorSet mainConnectors = new ConnectorSet();
            mainConnectors.Insert(startConnector);
            mainConnectors.Insert(endConnector);

            // Create a dictionary 
            Dictionary<double, Connector> connectorDistances = new Dictionary<double, Connector>();

            // Calculate the distance from each connector to the start connector
            foreach (Connector connector in connectors)
            {
                // Continue if the connector is the start connector
                if (connector == startConnector)
                {
                    continue;
                }
                // Calulate the absolute distance from the start connector to the current connector
                double distance = connector.Origin.DistanceTo(startConnector.Origin);
                connectorDistances.Add(distance, connector);
            }

            // Sort the dictionary by distance
            Dictionary<double, Connector> sortedConnectorDistances = connectorDistances.OrderBy(x => x.Key).ToDictionary(obj => obj.Key, obj => obj.Value);

            List<double> distances = sortedConnectorDistances.Keys.ToList();
            List<Connector> sortedConnectors = sortedConnectorDistances.Values.ToList();

            // Iterate through the sorted dictionary and create duct segments between each connector
            for (int i = 0; i < sortedConnectors.Count; i++)
            {
                // Get the current connector
                Connector connector = sortedConnectors[i];
                int segmentNumber = i + 1;
                string segmentID = componentID + "-seg" + segmentNumber.ToString();

                // Calculate the length of the segment
                double length;

                if (i == 0)
                {
                    length = UnitUtils.ConvertFromInternalUnits(distances[i], UnitTypeId.Meters);
                }
                else
                {
                    length = UnitUtils.ConvertFromInternalUnits(distances[i] - distances[i - 1], UnitTypeId.Meters); 
                }

                // Instantiate the segment
                CreateDuctPart(duct, componentID, segmentID, revitID, length, mainConnectors);
                
                // Create connectors for the segment, based on the start- and end connector in the "real" duct
                foreach (Connector mainConnector in mainConnectors)
                {
                    if (i == 0 && mainConnector == startConnector)
                    {
                        // If it is the first segment, use the duct's ID to create the connector ID for the start connector
                        string connecterID = componentID + "-" + mainConnector.Id;
                        sb.Append(RelatedPorts.CreateConnector(duct, connector, segmentID, connecterID));
                    }
                    else if (i == sortedConnectors.Count - 1 && mainConnector == endConnector)
                    {
                        // If it is the last segment, use the duct's ID to create the connector ID for the end connector
                        string connecterID = componentID + "-" + mainConnector.Id;
                        sb.Append(RelatedPorts.CreateConnector(duct, connector, segmentID, connecterID));
                    }
                    else
                    {
                        // Else use the segment ID to create the connector ID
                        sb.Append(RelatedPorts.CreateConnector(duct, mainConnector, segmentID));
                    }
                }

                // Instantiate the taps
                if (i != sortedConnectors.Count-1)
                {
                    CreateTap(duct, componentID, connector, segmentNumber, mainConnectors);
                }

            }

        }
        public void CreateDuctPart(Duct duct, string componentID, string segmentID, string revitID, double length, ConnectorSet mainConnectors)
        {
            
            // Instantiate in RDF (currently with "DuctPart" as the type of the segment)
            sb.Append(
                $"inst:{componentID} fso:hasPart inst:{segmentID} ." + "\n" +
                $"inst:{segmentID} a fso:DuctPart ." + "\n" +
                $"inst:{segmentID} ex:RevitID \"{revitID}\" ." + "\n");

            // Instantiate the length of the segment in RDF

            sb.Append($"inst:{segmentID} fpo:hasLength [" + "\n"
             + $"   a fpo:Length ;" + "\n"
             + $"   fpo:hasValue '{length}'^^xsd:double ;" + "\n"
             + $"   fpo:hasUnit 'Meter'^^xsd:string ] ." + "\n");
        }
        public void CreateTap(Duct duct, string componentID, Connector connector, int segmentNumber, ConnectorSet mainConnectors)
        {
            // Find and instantiate the connected tap
            FamilyInstance tap = null;
            foreach (Connector c in connector.AllRefs)
            {
                if (c.Domain != Domain.DomainUndefined)
                {

                    tap = c.Owner as FamilyInstance;
                    break;
                }
            }
            if (tap == null) return;
            
            string tapGUID = tap.UniqueId;
            string tapRevitID = tap.Id.ToString();
            
            // Instantiate the tap in RDF
            sb.Append($"inst:{tapGUID} a fso:Tap ." + "\n" +
                      $"inst:{tapGUID} ex:RevitID \"{tapRevitID}\" ." + "\n");

            foreach (Connector ductConnector in mainConnectors)
            {
                // Use the method from RelatedPorts to create the connector with the specified tapGUID
                sb.Append(RelatedPorts.CreateConnector(duct, ductConnector, tapGUID));

            }

            Connector startConnector = mainConnectors.Cast<Connector>().FirstOrDefault<Connector>();
            Connector endConnector = mainConnectors.Cast<Connector>().LastOrDefault<Connector>();


            int startConnectorID = startConnector.Id;
            int endConnectorID = endConnector.Id;
            
            // Join the tap to the duct segments on both sides
            string startConnectorPredicate = RelatedPorts.GetPredicates(startConnector);
            string endConnectorPredicate = RelatedPorts.GetPredicates(endConnector);

            // ID of the segment on one side of the tap
            string segmentId_1 = componentID + "-seg" + segmentNumber;
            string segmentConnectorId_1 = segmentId_1 + "-" + endConnectorID;
            // The corresponding tap connector's ID
            string tapConnectorId_1 = tapGUID + "-" + startConnectorID;

            // ID of the segment on the other side of the tap
            string segmentId_2 = componentID + "-seg" + (segmentNumber + 1);
            string segmentConnectorId_2 = segmentId_2 + "-" + startConnectorID;
            // The corresponding tap connector's ID
            string tapConnectorId_2 = tapGUID + "-" + endConnectorID;

            sb.Append($"inst:{tapConnectorId_1} fso:{startConnectorPredicate} inst:{segmentConnectorId_1} ." + "\n"
                    + $"inst:{segmentConnectorId_1} fso:{endConnectorPredicate} inst:{tapConnectorId_1} ." + "\n"
                    + $"inst:{segmentConnectorId_1} a fso:Port ." + "\n" // Duplicate?
                    + $"inst:{tapGUID} fso:{startConnectorPredicate} inst:{segmentId_1} ." + "\n"
                    );

            sb.Append($"inst:{tapConnectorId_2} fso:{endConnectorPredicate} inst:{segmentConnectorId_2} ." + "\n"
                    + $"inst:{segmentConnectorId_2} fso:{startConnectorPredicate} inst:{tapConnectorId_2} ." + "\n"
                    + $"inst:{segmentConnectorId_2} a fso:Port ." + "\n" // Duplicate?
                    + $"inst:{tapGUID} fso:{endConnectorPredicate} inst:{segmentId_2} ." + "\n"
                    );


            // Instantiate the tap's existing connector as a port
            ConnectorSet tapConnectors = tap.MEPModel.ConnectorManager.Connectors;
            foreach (Connector tapConnector in tapConnectors)
            {
                foreach (Connector tapConnectorAllRef in tapConnector.AllRefs)
                {
                    if (tapConnectorAllRef.Owner.UniqueId != componentID)
                    {
                        string tapConnectorID = tapGUID + tapConnector.Id;
                        sb.Append(RelatedPorts.CreateConnector(tap, tapConnector, tapGUID));
                    }
                }
            }
        }
        
        public string GetSpaceID(FamilyInstance component)
        {
            string spaceID = null;
            if (component.Space != null)
            {
                spaceID = component.Space.UniqueId;
            }
            return spaceID;
        }
    }
    


}
