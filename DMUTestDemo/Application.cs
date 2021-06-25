using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace DMUTest
{
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class AssociativeSectionUpdater : Autodesk.Revit.UI.IExternalApplication
    {
        private AddInId m_thisAppId;

        public Result OnStartup(Autodesk.Revit.UI.UIControlledApplication app)
        {
            m_thisAppId = app.ActiveAddInId;

            m_sectionUpdater = new SectionUpdater(m_thisAppId);
            m_sectionUpdater.Register();

         
            return Result.Succeeded;
        }

        public Result OnShutdown(Autodesk.Revit.UI.UIControlledApplication application)
        {
            m_sectionUpdater.UnRegister();
            return Result.Succeeded;
        }

        private static SectionUpdater m_sectionUpdater = null;
    }
}
