using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Widget;
using Emonitorage.shared;

using System.Net;
using System.IO;
using System.Text;
using Android.Graphics;

namespace EAndroid
{
    class AlarmItemListAdapter : BaseAdapter<AlarmItem>
    {
        Activity context = null;
        AlarmItemListAdapter alarmListAdapter;
        ListView alarmListView;
        IList<AlarmItem> alarms = new List<AlarmItem>();
        Button listeButton;

        public AlarmItemListAdapter(Activity context, IList<AlarmItem> alarms) : base()
        {
            this.context = context;
            this.alarms = alarms;
        }

        public override AlarmItem this[int position]
        {
            get { return alarms[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return alarms.Count; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            // Get our object for position
            var alarm = alarms[position];

            // TODO: use this code to populate the row, and remove the above view
            Android.Views.View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Resource.Layout.CustomRowView, null);

            listeButton = view.FindViewById<Button>(Resource.Id.PriseEnChargeButton);
            String etat;
            if (alarm.Status == false)
            {
                etat = "Non pris en charge";

            }
            else
            {
                listeButton.Enabled = false;
                listeButton.SetBackgroundColor(Color.Gray);
                etat = "Pris en charge par " + alarm.NomPersonnelAidant;

            }
            view.FindViewById<TextView>(Resource.Id.DateTime).Text = alarm.DtDebut;
            view.FindViewById<TextView>(Resource.Id.AlarmDescription).Text = "Alarme : Appel de la chambre " + alarm.Chambre + 
                " (" + alarm.NomOccupant + ")"+"\n Type:"+alarm.Display;
            view.FindViewById<TextView>(Resource.Id.Status).Text = "Etat : " + etat;
            view.FindViewById<Button>(Resource.Id.PriseEnChargeButton).Tag = position;

            listeButton.Click += async (sender,e) => {
                var  user = UserItemManager.GetUser("0");
                var Cstatus = await AlarmServerManager.changeStatusAsync(alarms[Convert.ToInt32(((Button)sender).Tag)].IDAlarm, user.IdSS);
                if (Cstatus)
                {

                    Button clickedButton = (Button)sender;
                    alarm = alarms[Convert.ToInt32(clickedButton.Tag)];
                    alarm.NomPersonnelAidant = user.Lastname;
                    alarm.PrenomPersonnelAidant = user.Firstname;
                    //int r = 0;                   
                    //r = AlarmItemManager.AddAlarm(alarm);
                    alarms = AlarmItemManager.GetAlarms();
                    alarmListView = context.FindViewById<ListView>(Resource.Id.listView);
                    alarmListAdapter = new AlarmItemListAdapter(context, alarms);
                    alarmListView.Adapter = alarmListAdapter;
                }
                else
                {
                    User u = UserItemManager.GetUser("0");
                    AlarmItemManager.DeleteAlarms();
                    var alarmsInverted = await AlarmServerManager.getAlarms(u.Profil);
                    alarms = AlarmItemManager.GetAlarms();
                    alarmListView = context.FindViewById<ListView>(Resource.Id.listView);
                    alarmListAdapter = new AlarmItemListAdapter(context, alarms);
                    alarmListView.Adapter = alarmListAdapter;
                }
            }
            ;
            return view;
        }

        public void PrendreEnCharge(object sender, EventArgs e)
        {
            int r = 0;
            Button clickedButton = (Button)sender;
            r = AlarmItemManager.ChangeStatus(alarms[Convert.ToInt32(clickedButton.Tag)].IDAlarm, UserItemManager.GetUser("0").IdSS);
            alarms = AlarmItemManager.GetAlarms();
            alarmListView = context.FindViewById<ListView>(Resource.Id.listView);
            alarmListAdapter = new AlarmItemListAdapter(context, alarms);
            alarmListView.Adapter = alarmListAdapter;
            /*
            var request = (HttpWebRequest)WebRequest.Create("http://192.168.1.69:88/ptrans/ptrans.php");
            var postData = "newSupport=hello";
            postData += "&idAlarm=23";
            postData += "&numRoom=65";
            postData += "&occupantName=Marie";
            postData += "&services=1";
            postData += "&aidantName=default";
   
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseString);*/
        }
    }
}