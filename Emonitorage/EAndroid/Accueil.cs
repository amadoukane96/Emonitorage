using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Emonitorage.shared;

namespace EAndroid
{
    [Activity(Label = "EMonitorÂge", MainLauncher = false)]
    public class Accueil : Activity
    {
        AlarmItemListAdapter alarmListAdapter;
        AlarmServerManager server = new AlarmServerManager();
        IList<AlarmItem> alarms;
        IList<AlarmItem> alarmsInverted;
        ListView alarmListView;
        Button refreshButton;
        AlarmItem alarm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // on set notre layout
            SetContentView(Resource.Layout.Accueil);

            alarmListView = FindViewById<ListView>(Resource.Id.listView);
            refreshButton = FindViewById<Button>(Resource.Id.refreshButton);

            refreshButton.Click += new EventHandler(async (sender, e) =>
            {
                User u = UserItemManager.GetUser("0");
                
                alarmsInverted = await AlarmServerManager.getAlarms(u.Profil);
                alarms = AlarmItemManager.GetAlarms();
                
                alarmListAdapter = new AlarmItemListAdapter(this, alarms);
                alarmListView.Adapter = alarmListAdapter;

            });
            /*
            alarm = new AlarmItem();
            alarm.IDAlarm = 1;
            alarm.Status = true;
            alarm.Chambre = "Chambre 1";
            alarm.NomPersonnelAidant = "Amadou";
            alarm.NomOccupant = "Diaby";
            alarm.DtDebut = "23:05";
            AlarmItemManager.AddAlarm(alarm);
            */
          

        }

        protected override void OnResume()
        {
            base.OnResume();
            
            alarmsInverted = AlarmItemManager.GetAlarms();
            alarms = new List<AlarmItem>();
            if (alarmsInverted.Count > 0)
            {

                for (int i = alarmsInverted.Count - 1; i >= 0; i--)
                {
                    alarms.Add(alarmsInverted[i]);
                }
            }
            // create our adapter
            alarmListAdapter = new AlarmItemListAdapter(this, alarms);
            alarmListView.Adapter = alarmListAdapter;
        }

        public void RefreshView(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            //alarmsInverted = AlarmItemManager.GetAlarms();
            alarms = new List<AlarmItem>();
            alarmListAdapter = new AlarmItemListAdapter(this, alarms);
            alarmListView.Adapter = alarmListAdapter;
        }
    }
}