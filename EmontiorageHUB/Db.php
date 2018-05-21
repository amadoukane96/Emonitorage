<?php 	
class DB{
	
	private $serve="LAPTOP-SM4DEAV6\SQLEXPRESS";//Nom du serveur BDD
	private $db = array('Database'=>'EmonitorAge',"UID"=>"win", "PWD"=>"ay");//Nom bdd, identifiant utilisateurs sql server

	//fonction pour récupérer les services en chaine de caractère de format"service,service,service"
	public function getServices($profil)
	{
		$con = sqlsrv_connect($this->serve, $this->db);
		if($con){
			$p2=array($profil);
			$q2="select [ID_Service] from [RT_Profil_Service] where ID_Profils = (?);";	
			$stmt = sqlsrv_query($con,$q2,$p2, array( "Scrollable" => 'static' ));
			if ($stmt==false || !sqlsrv_has_rows($stmt)){
			}
			else{
				$services="";
				$count = sqlsrv_num_rows($stmt);
				for ($i = 0; $i < $count; $i++) {
					if($i!=$count-1){
						$row = sqlsrv_fetch_array( $stmt, SQLSRV_FETCH_ASSOC);
    					$services=$services.$row['ID_Service'].',';
					}
					else{
						$row = sqlsrv_fetch_array( $stmt, SQLSRV_FETCH_ASSOC);
    					$services=$services.$row['ID_Service'];
					}
    				
				}
				
				return $services; 
			}
		}
		return "";
	}

	public function firstLogin($uid,$pwd){
		$con = sqlsrv_connect($this->serve,$this->db);
		if($con)
		{
			$q="select ID as IdSS, ID_Profils as Profil, First_Name as FirstName, Last_Name as LastName from [EmonitorAge].[dbo].[Users] where Login_User=(?) and Pwd_User=(?)";
			$p=array($uid,$pwd);
			$stmt = sqlsrv_query( $con,$q,$p);

			if( $stmt==false || !sqlsrv_has_rows($stmt)) 
			{	http_response_code(400);
     			return "";
			}
			else
			{	
				//$services = $this->getServices(2);
				//echo $services;
				http_response_code(200);
				
				while ($row = sqlsrv_fetch_array( $stmt, SQLSRV_FETCH_ASSOC))
				{	
					$row['ID']=0;
					$row['IsLogged']=1;
					header('Content-Type: application/json');
					header('Services:'.$this->getServices($row['Profil']));
					return $row;
				}
      			
			}

		}
		else{
			http_response_code(400);
		}
	}

	
	public function login($uid,$pwd){
		$con = sqlsrv_connect($this->serve,$this->db);
		if($con){
			$q="select * from Users where  Login_User=(?) and Pwd_User=(?)";
			$p=array($uid,$pwd);
			$stmt = sqlsrv_query( $con,$q,$p);
			if($stmt==false || !sqlsrv_has_rows($stmt)) {
     			http_response_code(400);
			}
			else{
				http_response_code(200);
			}
		}
		else{
			http_response_code(400);
			echo "Erreur lors de la connexion à la base de données";
		}
	}

	public function isAlarme($profil){
		$con = sqlsrv_connect($this->serve,$this->db);
		if($con){
			$q="select D.Room_Name as Chambre,A.Dt as DtDebut, A.Status_Bis as Status,PAM.Display, R.ID_Service as Service, E.First_Name as NomOccupant, U.First_Name as NomPersonnelAidant, U.Last_Name as PrenomPersonnelAidant, A.ID as IDAlarm from Dimension_Room D 
			join Appels_Malades A on D.ID=A.ID_Room
			join RT_Room_Service R on A.ID_Room = R.ID_Room join Occupations O on O.ID_Room = A.ID_Room join Dimension_Occupant E on E.ID = O.ID_Occupant 
			join RT_Profil_Service P on P.ID_Service = R.ID_Service
			join Param_Appels_Malades PAM on PAM.ID = A.ID_Display
			 LEFT JOIN Users U 
			on A.ID_Intervenant = U.ID  
			where P.ID_Profils = (?)
			and ([A].Status =2 )
			;";
			$p=array($profil);
			$stmt = sqlsrv_query( $con,$q,$p);
			if( $stmt==false || !sqlsrv_has_rows($stmt)) {
				http_response_code(400);
     			return "";
			}
			else
			{	http_response_code(200);
				$result=array();
				$obj=array();
				
				while ($row = sqlsrv_fetch_array( $stmt, SQLSRV_FETCH_ASSOC))
				{	

					$row['DtDebut']=$row['DtDebut']->format('H:i');
					$result[]=$row;
				}
      			return $result;
			}		
		}
		else{
			http_response_code(400);
		}
	}

	public function changeStatus($idAlarme, $idIntervenant){
		$con = sqlsrv_connect($this->serve,$this->db);
		if($con){
			$p=array($idAlarme);
			$q="update [Appels_Malades] set [Status_Bis] = 1 where [ID] = (?) and [Status_Bis] = 0";			
			$stmt = sqlsrv_query($con,$q,$p);
			if( $stmt==false) 
			{	
				return false;
			}
			else{
				$p2=array($idIntervenant,$idAlarme);
				$q2="update [Appels_Malades] set [ID_Intervenant] = (?) where [ID] = (?)";			
				$stmt2 = sqlsrv_query($con,$q2,$p2);
				return true;
			}

			
		}
		else
		{
			http_response_code(400);
			return false;
		}
	}
	

}


?>