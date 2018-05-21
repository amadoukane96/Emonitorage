using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Messaging;
using Android.Media;

using Emonitorage.shared;
//mmmm
using Firebase.Iid;
using WindowsAzure.Messaging;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.Net.Http;

namespace EAndroid.Resources
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public const bool notifyNewSupport = true; //Définit si une notification est envoyée pour informer d'une nouvelle prise en charge
        public MyFirebaseMessagingService()
        {
        }
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                SendNotificationAsync(message);
            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                SendNotificationAsync(message);

            }
        }

        async Task SendNotificationAsync(RemoteMessage message)
        {
            string notificationType = message.Data["type"];
            string idAlarm = message.Data["idAlarm"];
            string idRoom = message.Data["idRoom"];
            string numRoom = message.Data["numRoom"];
            string nomOccupant = message.Data["occupant"];
            string triggerTime = message.Data["triggerTime"];
            //Personnalisation du message de la notification
            string notifTitle = "Nouvel appel en chambre " + numRoom + " !";
            string notifMessage = "Appel effectué à " + triggerTime;

            //S'il s'agit d'une notification de type nouvelle alarme
            if (notificationType.Equals("newAlarm"))
            {
                //Ajout de l'alarme dans la base de données locale
                AlarmItem alarm = new AlarmItem();
                alarm.IDAlarm = int.Parse(idAlarm);
                alarm.Status = false;
                alarm.Chambre = numRoom;
                alarm.NomOccupant = nomOccupant;
                alarm.DtDebut = triggerTime;

                AlarmItemManager.AddAlarm(alarm);

                Console.WriteLine(AlarmItemManager.GetAlarm(4).Chambre);
            }

            
            
            //Affichage de la notification que nous venons de configurer
            var intent = new Intent(this, typeof(Accueil));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            Notification.BigTextStyle textStyle = new Notification.BigTextStyle();

            //...
            textStyle.BigText(notifMessage);


            var notificationBuilder = new Notification.Builder(this)
                                                      .SetContentTitle(notifTitle)
                                                      .SetStyle(textStyle)
                                                      .SetContentText(notifMessage)
                                                      .SetPriority((int)Android.App.NotificationPriority.High)
                                                      .SetDefaults(NotificationDefaults.Vibrate)
                                                      .SetSmallIcon(Resource.Drawable.patient2)
                                                      .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                                                      .SetAutoCancel(true)
                                                      .SetContentIntent(pendingIntent)
                                                      .SetVisibility(NotificationVisibility.Public);



            if (notificationType.Equals("newAlarm") || (notificationType.Equals("support") && notifyNewSupport)) {
                var notificationManager = NotificationManager.FromContext(this);
                notificationManager.Notify(int.Parse(idRoom), notificationBuilder.Build());

            }
            


            



        }
    }
}
