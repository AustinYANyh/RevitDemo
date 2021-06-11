using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AddinTest.Updater;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace AddinTest
{
    public class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Entry : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Func<Document> GetActiveDoc = () =>
            {
                return commandData.Application.ActiveUIDocument.Document;
            };
            Func<UIApplication> GetApplication = () =>
            {
                return commandData.Application;
            };

            var tmp = CurveUpdater.GetInstance(GetApplication());
            tmp.m_monitor_document = GetActiveDoc();

            UpdaterRegistry.RegisterUpdater(tmp, true);
            UpdaterRegistry.EnableUpdater(tmp.GetUpdaterId());
            UpdaterRegistry.AddTrigger(tmp.GetUpdaterId(), new ElementClassFilter(typeof(CurveElement)),
                 Element.GetChangeTypeElementAddition());

            TestPrismWindow window = new TestPrismWindow();
            window.Show();

            RevitCommandId drawCurveId = RevitCommandId.LookupPostableCommandId(PostableCommand.ModelLine);
            GetApplication().PostCommand(drawCurveId);

            CurveUpdater.m_add_elementid_list.Clear();
            window.m_view_model.DoWork = new Action(() =>
            {
                List<Curve> curve_list = GetNewAddElements<CurveElement>
                    (GetApplication(), GetActiveDoc()).Select((x) => x.GeometryCurve).ToList();

                List<CurveLoop> closed_max_region = HWTransCommon.Algorithm.CloseLoopAlgorithm.GetClosedRegions_Max(curve_list);

                UpdaterRegistry.RemoveDocumentTriggers(tmp.GetUpdaterId(), GetActiveDoc());
            });

            return Result.Succeeded;
        }


        //获取合法的新增Element
        public List<T> GetNewAddElements<T>(UIApplication app, Document doc)
            where T : Element
        {
            List<T> newAddElements = new List<T>();
            foreach (ElementId cid in CurveUpdater.m_add_elementid_list)
            {
                try
                {
                    T newC = doc.GetElement(cid) as T;
                    if (newC != null && newC.IsValidObject)
                    {
                        newAddElements.Add(newC);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return newAddElements;
        }
    }
}
