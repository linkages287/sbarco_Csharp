// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//  Vengono raccolti tutti i valori inseriti
// viene effettuata l'inizializzazione del sistema
// e controllati i dati inseriti nel loro insieme.
// Una volta inseriti singolarmente i dati di ogni elicottero
// spot, truppa, cargo deve essere verificata anche che 
// l'interazione tra tutte le informazioni inserite funzioni
// Una volta effettuato il controllo iniziale il simulatore
// può partire caricando le classi che gestiscono la parte grafica
// e la parte logica.
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Threading;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{
	public class Simulator
	{
		public Spot.day_night DNOps { set; get; }


		// gestione delle liste
		public TroopsManager TroopM { set; get; }

		public EliManager EliM { set; get; }

		public SpotManager SpotM { set; get; }

		public CargoManager CargoM { set; get; }

		public InfoWindow WinI { set; get; }

		public MainWindow MainWindow = MainWindow.Instance ();

		public int Distance { set; get;} // distanza LZ - ship
		//------------------------------------------------
		// il simulatore è composto da un gestore grafico 
		// non chè da un supporto logico di simulazione
		GrafXNA simulG;// supporto grafico

		SimMover simMoverLogic;// supporto logico
		//------------------------------------------------

		public bool assignEnded;// se TRUE indica l'assegnazione iniziale degli spot dedicati

		// al simulatore vengono passate tutte le informazioni necessarie alla simulazione degli eventi
		public Simulator (int distance, Spot.day_night _dnOps, InfoWindow _winI, TroopsManager _troopM, EliManager _eliM, SpotManager _spotM, CargoManager _cargoM)
		{
			// passo al simulatore TUTTE le informazioni necessarie 
			this.DNOps = _dnOps; // operazione day night
			this.TroopM = _troopM; // manager truppe  e lista truppe
			this.EliM = _eliM; // manager eli e lista eli
			this.SpotM = _spotM; // magaer spot e lista spot
			this.CargoM = _cargoM; // manager cargo e lista cargo
			this.WinI = _winI; // information window
			this.Distance = distance; // pass il paramentro distanz
			this.WinI.InsertSomeText ("SIMULATOR: simulatore creato. Effettuo l'inizializzazione....");

			// creazione della logica di gesitione e della grafica di funzionamento
			simMoverLogic = new SimMover (this.DNOps, this.WinI, this.TroopM, this.EliM, this.SpotM, this.CargoM);
			simulG = new GrafXNA (this.simMoverLogic); // costruzione del supporto grafico di riferimento 
		
		}

		// disposizione di elicotteri e truppe
		public void InitDeckSpotAssign ()
		{
			//--------------------------------------------------------------
			// fase inizializzazione GENERALE e controllo dei dati inseriti
			//--------------------------------------------------------------

			WinI.infoTimeList.Add (new InfoWindow.timeInfoStruct(EliM.MainTime.GetTime()," - INIZIO MOVIMENTAZIONI PONTE- ", "", -1));
			this.WinI.InsertSomeText (" - SIMULATORE: INIZIO MOVIMENTAZIONI SUL PONTE");

			// crea la LZ e L'HP come membro di elimanager
			this.EliM.MakeHoldingPoint (3);// creazione dell'HP con la distanza standard di 3 miglia dalla NAVE
			this.EliM.MakeLZ (this.Distance); // viene creata la LZ con la distanza dalla nave
			//---------------------------------------------------------------------

			this.EliM.initEliNum = this.EliM.ElicotteriList.Count; // salvo il numero iniziale di elicotteri poiche' la variabile tendera a mutare

			this.EliM.CheckSpotDataConsistency (SpotM,DNOps); // controllo la consistenza dei dati sugli spot se esistono degli elicotteri
			// che non sono in gradi di ospitare gli spot è necessario eliminare gli elicotteri che non possono transitare sul ponte di volo
			// prima dell'inizio della simulazione

			this.EliM.CheckEliDataFuelConsistency (this.Distance);// controllo il carburante a bordo degli elicotteri per poter effettuare la missione

			this.EliM.TroopsW_Data_correction (TroopM.SingleSoldierW); // corregge i pesi degli elicotteri diventando multipli


			this.EliM.Check_Eli_Usability (WinI, TroopM, CargoM); // controllo l'effettiva necessità iniziale degli elicotteri inseriti
																// elimino gli elicotteri che non servono alla missione


			EliM.EliAutoSpotAssign (SpotM, WinI, DNOps); // assegnazione AUTOMATICA degli spot-possibile riduzione degli elicotteri 
														// se gli elicotteri definiti sul ponte superano il numero degli spot assegnabili

			if (CargoM != null) // se il cargo è stato inserito
			{
				CargoM.CheckCargoIsFittable (EliM); // controllo che il cargo sia imbarcabile su almeno 1 elicottero
				// se i dati del cargo non sono coerenti con i pesi trasportabili dall'elicottero il cargo viene taggato non trasportabile
			}

			EliM.CheckState (); // eli manager check state
			TroopM.CheckState (); // troops manager check state
			SpotM.CheckState (); // spot manager check state
			simMoverLogic.AssignEli (); // assegna le destinazioni iniziali
			// INIZIO SIMULAZIONE
			this.Start (); // passa il controllo a start()
			// INIZIO SIMULAZIONE
		}

		public void Start ()
		{
			// timer start
			WinI.InsertSomeText ("SIMULATORE: ciclo inserimento e disposizione iniziale ultimato");	
			WinI.InsertSomeText ("SIMULATORE: INIZIO SIMULAZIONE, starting X timer\"");	
			//SIMULATION WAITING SEQUNCE
			this.EliM.MainTime.Start ();// start main time											
			simulG.Run (); // grafica del simulatore gestito tramite XNA
		}

}// fine classe
}// fine name space
