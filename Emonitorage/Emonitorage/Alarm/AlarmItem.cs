using System;
using System.Collections.Generic;
using System.Text;

namespace Emonitorage.shared
{
    public class AlarmItem
    {
        public AlarmItem() { }
        public string Chambre { get; set; }
        public int IDAlarm { get; set; }
        public int ID { get; set; }
        public bool Status { get; set; }
        public string NomOccupant { get; set; }
        public string NomPersonnelAidant { get; set; }
        public string PrenomPersonnelAidant { get; set; }
        public string Display { get; set; }
        public int Service { get; set; }
        public string DtDebut { get; set; }
    }
}
