<?php
require_once 'Db.php';
require_once 'MyNotifications.php';
if ($_SERVER['REQUEST_METHOD'] == "POST" && isset($_POST["URL"])){
	$b = new DB();

	if ( $_POST['URL'] == "login" ){
			
			$a=$b->login($_POST["UID"],$_POST["PWD"]);
	}

	elseif($_POST['URL']=="firstlogin"){
			$a=$b->firstLogin($_POST["UID"],$_POST["PWD"]);
			print_r(json_encode($a));
	}		  

	elseif ( $_POST['URL'] == "changestatus" ){
			$a=$b->changeStatus($_POST["ID_Alarme"],$_POST["ID_Intervenant"]);

			if($a){
				http_response_code(200);
			}
			else{
				http_response_code(400);
			}
	}

	elseif($_POST['URL']=="newAlarm") {
		
	   	//handleNewAlarm($_POST['idAlarm'], $_POST['nameRoom'], $_POST['occupantName'], $_POST['triggerTime'], $_POST['service']);

	   	handleNewAlarm($_POST['idAlarm'], $_POST['idRoom'], $_POST['nameRoom'], "gogo", "1");
    }
	
	elseif($_POST['URL']=="refresh"){
			$a=$b->isAlarme($_POST["Profil"]);
			header('Content-Type: application/json');
			print_r(json_encode($a));
	}
	else{
		http_response_code(400);
	}
	

} 

else{
	http_response_code(300);
}

//handleNewAlarm("3", "20", "439", "Sami", "3");


?>




