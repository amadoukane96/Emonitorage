<?php
require("NotificationHub.php");

function handleNewAlarm($idAlarm, $idRoom, $numRoom, $occupant, $services) {
	$today = date('d')."-".date('m')."-".date('Y');
    $testTimestamp = time(); 
    date_default_timezone_set('Europe/Paris'); 
        $testLocaltime = localtime($testTimestamp,true); 
        $heure = $testLocaltime['tm_hour']; 
    $minute = $testLocaltime['tm_min'];
    if(strlen($minute) == 1) {
        $minute = "0".$minute;
    }
    $time = $heure.":".$minute;
    
    echo $time;
    echo "\n";
    $data = '{"data":{"type":"newAlarm", "idAlarm":"'.$idAlarm.'", "idRoom":"'.$idRoom.'", "numRoom":"'.$numRoom.'", "occupant":"'.$occupant.'", "triggerTime":"'.$time.'"}}';
    
    sendNotificationAndroid($data, $services); //Send notifications to Android smartphones
    
}

function handleNewSupport($idAlarm, $numRoom, $occupant, $services, $aidantName, $localIdAlarm) {
    $testTimestamp = time(); 
    date_default_timezone_set('Europe/Paris'); 
        $testLocaltime = localtime($testTimestamp,true); 
        $heure = $testLocaltime['tm_hour']; 
    $minute = $testLocaltime['tm_min']; 
    $time = $heure.":".$minute;
    $data = '{"data":{"type":"support", "idAlarm":"'.$idAlarm.'", "numRoom":"'.$numRoom.'", "occupant":"'.$occupant.'",  "supportTime":"'.$time.'", "aidantName":"'.$aidantName.'", "localIdAlarm":"'.$localIdAlarm.'"}}';
    
    sendNotificationAndroid($data, $services); //Send notifications to Android smartphones
    
}

function sendNotificationAndroid($message, $service) {
    $hub = new NotificationHub("Endpoint=sb://emonitoragespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=gc4o3majAzzwiHZMb7PChPM1nUSHjuMhJlGF6NMwt54=", "Emonitorage"); 
    
    $notification = new Notification("gcm", $message);
    $hub->sendNotification($notification, $service);
    echo "Android";
}
    
function sendNotificationiOS($room, $callDate, $service) {
    $hub = new NotificationHub("Endpoint=sb://emonitoragespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=gc4o3majAzzwiHZMb7PChPM1nUSHjuMhJlGF6NMwt54=", "Emonitorage"); 
    $alert = '{"data":{"msg":"Chambre : '.$room.' à '.$callDate.'"}}';
    $notification = new Notification("apple", $alert);
    $hub->sendNotification($notification, $service);
    echo ", iOS";
}

function register($idUser, $services) {

    $data = '{"data":{"type":"register", "idUser":"'.$idUser.'", "services":"'.$services.'"}}';

    $hub = new NotificationHub("Endpoint=sb://emonitoragespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=gc4o3majAzzwiHZMb7PChPM1nUSHjuMhJlGF6NMwt54=", "Emonitorage"); 
    
    $notification = new Notification("gcm", $message);
    $hub->sendNotification($notification, "");
    echo "Android";
}
?>