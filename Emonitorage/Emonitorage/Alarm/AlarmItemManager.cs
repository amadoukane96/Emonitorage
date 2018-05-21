using System;
using System.Collections.Generic;
using System.Text;

namespace Emonitorage.shared
{
    class AlarmItemManager
    {
        static AlarmItemManager()
        {
        }

        public static AlarmItem GetAlarm(int id)
        {
            return AlarmItemRepositoryADO.GetAlarm(id);
        }

        public static IList<AlarmItem> GetAlarms()
        {
            IList<AlarmItem> alarmsInverted = new List<AlarmItem>(AlarmItemRepositoryADO.GetAlarms());
            IList<AlarmItem> alarms = new List<AlarmItem>();
            if (alarmsInverted.Count > 0)
            {

                for (int i = alarmsInverted.Count - 1; i >= 0; i--)
                {
                    alarms.Add(alarmsInverted[i]);
                }
            }
            return alarms;
        }


        public static int ChangeStatus(int id, int nStatus)
        {
            return AlarmItemRepositoryADO.ChangeStatus(id, nStatus);
        }

        public static int AddAlarm(AlarmItem item)
        {
            return AlarmItemRepositoryADO.AddAlarm(item);
        }

        public static int DeleteAlarm(int id)
        {
            return AlarmItemRepositoryADO.DeleteAlarm(id);
        }
        public static int DeleteAlarms()
        {
            return AlarmItemRepositoryADO.DeleteAlarms();
        }

        public static int DeleteTable()
        {
            return AlarmItemRepositoryADO.DeleteTable();
        }
    }
}
