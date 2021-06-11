using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddinTest.Updater
{
    public class CurveUpdater : IUpdater
    {
        public UIApplication m_app { get; set; }

        public Document m_monitor_document { get; set; }

        private static CurveUpdater m_instance;

        public static CurveUpdater GetInstance(UIApplication app)
        {
            Interlocked.CompareExchange<CurveUpdater>(ref m_instance, new CurveUpdater(app.ActiveAddInId), null);

            return m_instance;
        }

        private CurveUpdater(AddInId id)
        {
            m_updater_id = new UpdaterId(id, Guid.NewGuid());
        }

        public UpdaterId m_updater_id { get; set; }

        public static List<ElementId> m_add_elementid_list = new List<ElementId>();

        public void Execute(UpdaterData data)
        {
            List<ElementId> id_list = data.GetAddedElementIds() as List<ElementId>;
            if (id_list != null && id_list.Count > 0)
            {
                foreach (ElementId item in id_list)
                {
                    if (m_add_elementid_list.Find(p => p.Equals(item)) == null)
                    {
                        m_add_elementid_list.Add(item);
                    }
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return "CurveAdditionalInformation";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Masses;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updater_id;
        }

        public string GetUpdaterName()
        {
            return "CurveUpdater";
        }
    }
}
