//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// classe manager per la creazione spot ed il controllo degli stati 
// detiene la lista spot e le operazioni su di essi
// classe singleton
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


using System;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{
	public class SpotManager: AbstractManager
	{
		public static SpotManager instance = null; // classe singleton
		public int TotSpot {set;get;} // numero totale di spot
		public List<Spot> Spotlist; // definizione della lista di elicotteri ( pubblica )
		public StatusManager Stato;// definizioni dello stato del manager 
		public ActionManager Action; //definizioni delle azioni svolte
		public InfoWindow WinI; // finestra informativa
		public const int timeNeedToEngage = 10; //  minuti per la messa in moto dell'elicottero
		public const int timeNeedToMoveToDeck = 15; // 15 minuti per muove l'elicottero sul ponte dall'hangar

		public SpotManager (int SpotNum, InfoWindow winI): base("Manager_per_Spot")
		{
			this.WinI = InfoWindow.Instance();
			this.TotSpot = SpotNum;
			this.Stato = StatusManager.Empty;  // setta lo stato come vuoto 
			this.Action = ActionManager.Wait; // le azioni sono in attesa di informazioni aggiuntive
		
		}


		//istanza singleton è prevista l'esistenza di un'unica finestra
		public static SpotManager Instance(int SpotNum, InfoWindow winI)
		{
			if (instance == null) instance= new SpotManager( SpotNum ,  winI );
			return instance; // ritorno il costruttore	
		}


		// override crea la lista di spot per la gestione
		public override void MakeList (int elementi)
		{	
			this.Spotlist = new List<Spot> (elementi);	
		}
		
		// inserisci un elemento nella lista
		public override void InsertElement (object spot)
		{
			this.Spotlist.Add ((Spot)spot);
		}

		//rimuovi un elemento dalla lista REMOVEAT INDEX
		public override  void RemoveElement (int i)
		{
			this.Spotlist.RemoveAt (i);
		}


		// inserisci lo spot nell'elenco degli spot
		public  void InsertElementSpot (int spotPos, bool spotRunnable, bool cat1D, bool cat2D, bool cat3D, bool cat1N, bool cat2N, bool cat3N)
		{
			// creazione del nuovo elicottero con valori inizializzati
			Spot spotN = new Spot (spotPos, spotRunnable, cat1D, cat2D, cat3D, cat1N, cat2N, cat3N); // creazione del nuovo record spot
			// inserisco l'elemento creato nella lista
			Spotlist.Insert ((spotPos), spotN);
		}

		// restituisce il primo spot efficiente in grado di ospitare una determinata categoria di elicottero
		// se non ci sono spot efficienti allora viene restituito il valore nullo
		public Spot SpotToHostaCat (int catEli, Spot.day_night _dn)
		{

			if (this.Spotlist.Count > 0) { // controllo se il numero di spot presenti è maggiore di 0
				foreach (Spot _spot in this.Spotlist) {
					if (_spot.SpotRunnable 
						&& (!_spot.SpotOccupied) 
						&& !_spot.IsReservd) { // controllo se lo spot trovat è efficiente e non è gia occupato

						if (_dn == Spot.day_night.day) { // controllo le cateogire accettate di giorno
							if (((catEli == 1) && (_spot.DayCanAccept.Cat1 == true)) 
								|| ((catEli == 2) && (_spot.DayCanAccept.Cat2 == true)) 
								|| ((catEli == 3) && (_spot.DayCanAccept.Cat3 == true)))
								return _spot;
						} else {// qua la categoria deve essere per forza notturna
							if (((catEli == 1) && (_spot.NightCanAccept.Cat1 == true)) 
								|| ((catEli == 2) && (_spot.NightCanAccept.Cat2 == true)) 
								|| ((catEli == 3) && (_spot.NightCanAccept.Cat3 == true)))
								return _spot;
						}
					} 	
				}
			} 
			return null;
		}// fine metodo SpotToHostaCat
	

		// resittuisce il numero di spot attualmente efficienti
		public int  NumSpotEfficient ()
		{
			int n = 0;
			foreach (Spot _spot in this.Spotlist)
				if (_spot.SpotRunnable == true)
					n++;
			return n;
		}

		// assegno un elicottero allo spot ed effettua il blocco
		public bool SpotEliAssign (Elicottero eli, Spot spot)
		{

			if (spot != null 
				&& eli != null 
				&& !spot.IsReservd 
				&& !eli.IsBlocked) {	// spot ed eli devono essere diversi da null

				spot.IsReservd = true;// spot riservato
				spot.eliWhoReservd = eli; // elicottero riservante 
				eli.Spotrequested = spot;// assegna lo spot all'elicottero
				eli.hasSpotReserved = true; // l'elicottero riserva un posto

				eli.EliBlock(SpotManager.timeNeedToMoveToDeck, 3); // elicottero bloccato per attività di movimentazione per 15 primi
				// al termine del blocco eli.PosEli = true indica che l'elicottero ha assunto la posizione sul ponte
				return true;// assegnazione avvenuta con successo
			}
			else return false;// assegnazione non avvenuta 
		}

		// restituisce il peso totale disponibile per imbarcare le truppe
		// calcolato con gli elciotteri attualmente presenti sullo spot
		public int TotalWEliSpotForTroopS ()
		{
			int totalW = 0;

			foreach (Spot spot in this.Spotlist) {
				// controllo che lo spot sia occupato da un elicottero efficiente
				// che abbia una configurazione di pale aperte 
				// che sia con i motori in moto
				// che non sia in uno stato di pieno e non possa piu' imbarcare altro
				if (spot.SpotOccupied 
					&& !spot.Eli.IsBlocked
					&& spot.Eli.IsBladeSpread 
					&& spot.Eli.IsRunning
					 && !spot.Eli.IsFull)

					totalW = totalW + spot.Eli.WTroopsLeftOnBoard;
			}
			return totalW;

		}


		// fornisce il numero di elicotteri attualmente presenti sul ponte
		public int TotalElionDeck ()
		{
			int n = 0;// indicatore numero di elicotteri 

			foreach (Spot spot in this.Spotlist) {
				if (spot.SpotOccupied && spot.Eli.IsEfficient)
					n++; // incrementa se lo spot e occupato da un eli efficiente
			}
			return n;

		}

		// tutti gli elicotteri presenti sul ponte mettono in moto i motori
		// la messa in moto è propedeutica per l'apertura delle pale
		public void AllEliSpottedRunning ()
		{
			// metto in moto gli elicotteri sul ponte che
			foreach (Spot spot in this.Spotlist)
				if (spot.Eli!=null 
					&& spot.Eli.PosEli  //sono sul ponte 
					&& spot.SpotOccupied // lo spot è occupato
					&& !spot.Eli.IsBlocked // eli non bloccato
					&& !spot.Eli.IsRunning // eli non running
					&& !spot.Eli.IsBladeSpread   // eli non a pale paerte
					&& !spot.Eli.IsFlying // eli non volante
				) 
				{ // controlla che l'elicottero sullo spot sia efficiente e non blocked status, che non sia già in moto
					// e che sia required per l'operazione.
					spot.Eli.EliBlock (SpotManager.timeNeedToEngage, 1); // metto l'elicottero in blocco per la messa in moto
																// il thread cambierà la variabile running alla fine dell'esecuzione

				}
		}


		//  gli elicotteri presenti sul ponte aprono le pale
		public void AllEliBladeSpreading ()
		{
			foreach (Spot spot in this.Spotlist)
				// controlla che l'elicottero non sia in stato di blocco, che sia efficiente e che le pale non siano gia aperte
				if ( spot.Eli!=null && !spot.Eli.IsBlocked &&  !spot.Eli.IsBladeSpread && spot.Eli.IsRunning && !spot.Eli.IsFlying ) 
				{
					spot.Eli.EliBlock (StaticSimulator.BladeSpreadBlockingTime, 4); // blocco l'eli segnale 10

				}
		}


		// gli elicotteri decollano ed assumono la velocità di crociera indicata
		public void AllEliTakingOff(float speed)
		{
			foreach (Spot spot in this.Spotlist)
				// controllo che lo spot abbia un elicottero funzionanete a pale aperte e a motore acceso
				if (spot.Eli!=null  
					&& !spot.Eli.IsBlocked 
					&& spot.Eli.isREADYstatus
					&& !spot.Eli.IsFlying) 
				{
					// setto le flags
					spot.Eli.isApproaching = false; // setto l'elicottero non in apporch alla dest
					spot.Eli.IsFlying=true;// viene settata la flag di elicottero in volo 
					spot.eliWhoReservd = null; // eli che ha effettuato la riserva
					spot.SpotOccupied = false; // lo spost non è piu' occupato
					spot.IsReservd = false; // fine riserva
					spot.Eli.SpotAssigned = null; // fine spot assegnato
					spot.Eli.hasSpotReserved = false; // l'eli ha lo spot riservato
					spot.Eli.isOnDestination = false; // resetto la var di destinazione
					spot.Eli.isREADYstatus = false; // reimposta lo stato di pronto al decollo su falso in quanto è deocllato
					spot.Eli.DirToGo = Elicottero.Direction.hp; // l'elicottero assume destinazione HP
					spot.Eli.EliSpeed = speed; // impost la velocità dell'elicottero a 1

					// salvo le informazioni prima di liberare lo spot
					WinI.InsertEvent ( spot.Eli.current_time , " T/O SHIP ", spot.Eli.IdEli, spot.Eli.NumEli);
					WinI.InsertSomeText( " ELI " +spot.Eli.IdEli + " DECOLLA time: "+ spot.Eli.current_time.ToString() );
					//----------------------------------------------------
					spot.Eli = null; // libera l'elicottero elicottero assegnato spot
				}
		}

		// indica il numero di spot liberi e funzionali
		public int TotalSpotFree ()
		{
			int n = 0;										// indicatore numero di elicotteri 
			foreach (Spot spot in this.Spotlist) {
				if (!spot.SpotOccupied && spot.SpotRunnable)
					n++; }
			return n;										// restituisce il numero di spot

		}// fine metodo

		// restituisce gli spot inefficienti
		public int TotalSpotInefficient ()
		{
			int n = 0;										 

			foreach (Spot spot in this.Spotlist) {
				if (!spot.SpotRunnable)
					n++; }
			return n;										
		} // fine metodo


		// restituisce il numero di elicotteri attualmente bloccati temporalmente sul ponte
		// gli elicotteri devono essere pronti per l'imbarco truppe
		public int TotalEli_NOT_Blocked_on_Deck ()
		{
			int n = 0;										 

			foreach (Spot spot in this.Spotlist) {
				if (spot.Eli!=null 
					&& !spot.Eli.IsBlocked // se l'elicottero non è bloccato
					&& !spot.Eli.hasSpotReserved // se l'eli non ha spot riservato
					&& spot.Eli.IsRunning // eli running
					&& spot.Eli.IsBladeSpread  // eli blade spread
					&& spot.Eli.isRequiredForOP) // eli required
					n++; }
			return n;										
		} // fine metodo

		// serve per calcolare quanti elicotteri ho pronti sul ponte per il decollo
		public void EliReadyStatus()
		{
			foreach (Spot spot in this.Spotlist)
				if (spot.Eli != null && !spot.Eli.IsBlocked
				    && spot.Eli.IsBladeSpread && spot.Eli.IsRunning
				    && !spot.Eli.IsFlying// se l'elicottero è pronto
					&& !spot.Eli.isHotRef// eli non hot ref
					&& ((spot.Eli.EliSoldierList.Count > 0) // eli con truppe
						|| (spot.Eli.EliCargoList.Count > 0)) // oppure cargo
				    && spot.Eli.Go_goback// elicottero in direzione LZ
				    && !spot.Eli.LowFuel// controllo che non l'elicottero non abbia lowfuel prima di decollare
				) // ed ha truppe o cargo
				{
					spot.Eli.isREADYstatus = true;// stato di pronti al decollo viene assunto solo se l'elicottero è
				}
		}// fine metodo

		// metodo di supporto serve a controllare che gli elicotteri sul ponte 
		// non siano in uno stato di fuel prima della partenza che richiedere rifornimento
		// se lo sono devo effettuare rifornimento
		public void CheckHOTREFLowFuelState (int numEli)
		{
			foreach (Spot spot in this.Spotlist)
				if (spot.Eli != null 
					&& spot.Eli.PosEli
					&& spot.SpotOccupied 
					&& spot.Eli.EliSoldierList.Count==0 // le truppe devono essere 0 a bordo per l'hot ref
					&& !spot.Eli.IsBlocked // elicottero non deve essere bloccato
				    && spot.Eli.IsBladeSpread// elicottrero a pale apertr
					//	&& (spot.Eli.LowFuel )
					&& (spot.Eli.Fuel<spot.Eli.Fuelrequired(numEli))
				    && spot.Eli.IsRunning 
					&& spot.Eli.isRequiredForOP// l'elicottero è running 
				    && !spot.Eli.IsFlying)
				
				 { // l'elicottero non deve volare

					spot.Eli.isHotRef = true;// eli hot ref status
					spot.Eli.EliBlock (6, 7); // blocco per 6 minuti per carburante 
					// al temrine del blocco l'elicottero è rifornito 
					// e la flag FuelWarning viene resettata dal fuel_thread
				}
		}//fine metodo

		//controllo lo status degli spot
		// metodo di controllo stato per spot
		// FULL = ponte pieno
		// EMPTY = ponte vuoto
		// CanAll = ponte con elicotteri ma che puo' ancora ospitare
		public override void CheckState ()
		{
			int spotListCount = this.Spotlist.Count; // spot totali di qualunque tipo
			int ineff = this.TotalSpotInefficient (); // Spot inefficienti
			int used = Spotlist.FindAll (x => x.SpotOccupied == true).Count;// List count spot occupati
			int reserved = Spotlist.FindAll (x => x.SpotOccupied==false && x.IsReservd==true ).Count; // list count spot riservati
			int totaSpotFree=spotListCount-ineff-used-reserved;

			if (used==0 && reserved==0 && (spotListCount-ineff)>0) { // se gli spot liberi corrispondono a tutti gli spot disponibili allora
					this.Stato = StatusManager.Empty;				// lo stato di ponte è vuoto, non ci sono elicotteri sul ponte
					this.Action = ActionManager.ReadyToReceive;		// il ponte è pronto a ricevere elicotteri
				}
			if ((totaSpotFree > 0) &&  (used>0 || reserved>0)) {		// se eli in hangar non sono tutto ma piu' di 0
					this.Stato = StatusManager.CanAll;		// lo stato è promiscuo il ponte non è nè vuoto nè pieno
					this.Action = ActionManager.ReadyToSendAndReceive; 						// è possibile qualcunque azione di invio o ricezione sul ponte
				}
			if (totaSpotFree == 0) {				//tutti gli spot sono occupati
					this.Stato = StatusManager.Full;		// lo stato risulta pieno
				this.Action = ActionManager.ReadyToSend;	// l'hangar puo' solo inviare elicotteri
				}
		}// fine metodo
	}// fine classe
}// fine name space