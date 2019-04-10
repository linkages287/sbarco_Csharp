//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// la classe contiene la logica di funzionamento del programma
// la gestione dei movimenti degli elicotteri 
// la gestione dei movimenti delle truppe
// la gestione dei movimenti del cargo 
// vengono effettuate all'interno della classe
// la gestione logica delle informazioni
// verrà poi utilizzata dalla classe di visualizzazione
// delle informazioni
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{
	public class SimMover
	{
		

		public Spot.day_night DNOps { set; get; } // spot day night struct
		// gestione delle liste
		public TroopsManager TroopM { set; get; } // gestore truppe

		public EliManager EliM { set; get; } // gestore elicotteri

		public SpotManager SpotM { set; get; } // gestore spot

		public CargoManager CargoM { set; get; } // gestore cargo

		public InfoWindow WinI { set; get; } // finestra informazioni

		public float Speed{ set; get;} // set iniziale della velocità pari a 100 nodi

		public bool EndProgramState { set; get;}// flag di fine simulazione
		
		// costruttore della classe
		public SimMover (Spot.day_night _dnOps, InfoWindow _winI, TroopsManager _troopM, EliManager _eliM, SpotManager _spotM, CargoManager _cargoM)
		{
			this.DNOps = _dnOps; // passo la struttua giorno notte
			this.WinI = _winI; // passo la finestra informazioni
			this.TroopM = _troopM; // passo il manager truppe
			this.EliM = _eliM; // passo il manager elicotteri
			this.SpotM = _spotM; // passo il manager spot
			this.CargoM = _cargoM; // passo il manager cargo
			this.Speed = 100; // set iniziale della velocità pari a 100 knots
			this.EndProgramState = false; // inizializzo l'end state a falso
		}
		//---------------------------------------------------------------
		// METODI DI SUPPORTO  ALLA GESTIONE DELLA CLASSE
		// controlla che tutti gli elicotteri disponibili siano in volo
		// cambia la destinazione dell'elicottero
		public void ChangeEli (Elicottero eli, float x, float y)
		{

			eli.destination = new Point ((int)x, (int)y);
			//	eli.destination.X = (int)x;
			//eli.destination.Y = (int)y;
		}

		// -----------------------------------------------------------------
		// assegna una destinazione in coordinate all'elicottero
		// viene trasformata la destinazione enum in coordinate
		public void AssignEli ()
		{
			foreach (Elicottero eli in EliM.ElicotteriList) {
				// direzione holding point assegno le coordinate HP
				if (!eli.isOnDestination && eli.DirToGo == Elicottero.Direction.hp) {
					ChangeEli (eli, EliM.HoldingP.HPvector.X, EliM.HoldingP.HPvector.Y);
				}

				// direzione landing zone assegno le coordinate LZ
				if (eli.DirToGo == Elicottero.Direction.lz) {
					ChangeEli (eli, (EliM.LandingZoneL.LZposition.X - (eli.NumEli) * 5), (EliM.LandingZoneL.LZposition.Y - (eli.NumEli) * 5));
					eli.LZ = EliM.LandingZoneL; // assegno la LZ

				}

				// direzione nave assegna le coordinate nave agli elicotteri
				if (eli.DirToGo == Elicottero.Direction.ship) {
					ChangeEli (eli, StaticSimulator.shipborderLandX, 
						(StaticSimulator.shipborderLandYdown-20 - 
						((eli.NumEli) * ((StaticSimulator.shipborderLandYdown - StaticSimulator.shipborderLandYup) / EliM.initEliNum))));
				}
			}
		}
		// restituisce vero se l'elicottero sullo spot puo' caricare un cargo qualunque ancora da prelevare
		// restituisce falso altrimenti
		private bool checkCargo (Spot spot)
		{

			if ((CargoM != null) && (CargoM.CargoList.Count > 0) && // se il cargo non è nullo 
			    (CargoM.CheckCargoLoadOnEli (spot.Eli, TroopM.SingleSoldierW))) // ed esiste un cargo che possa andare sull'elicottero
				return true; // ritorna vero
			return false; // altrimenti ritorna falso
			
		}
		// ----------------------------------------------------------------------
		// restituisce vero se l'elicottero sullo spot puo' caricare ancora delle truppe rimaste
		private bool checkTroops (Spot spot)
		{
			if ((TroopM.TroopList.Count == 0) 
				|| ((TroopM.TroopList.Count > 0) 
				&& (spot.Eli.WTroopsLeftOnBoard < TroopM.SingleSoldierW)))
				return false;
			return true;
		}
		//------------------------------------------------------------------------------------
		// ho necessità di controllare che gli elicotteri sul ponte siano carichi di soldati o truppe
		// se un elicottero è scarico non h atruppe o cargo a bordo e l'hangar è vuoto
		// l'elicottero va dichiarato non impiegabile e rimesso in hangar
		public void CheckEliRequired ()
		{
			foreach (Spot spot in SpotM.Spotlist) {
				// la condizione si verifica quando l'elicottero è scarico 
				// e non esistono truppe o cargo che possono essere caricate
				if (spot.Eli != null && (spot.Eli.EliSoldierList.Count == 0)  // se l'elicottero è scarico di truppe
					&&(spot.Eli.EliCargoList.Count == 0)  // l'elicottero è scarico di cargo
					&&!checkTroops (spot) // non ci sono piu' truppe da caricare
					&& !this.checkCargo (spot) // non ce' piu' cargo da caricare
					&& !spot.Eli.IsFlying // e l'elicottero non è in volo ma è atterrato
							
				) { // non ci sono truppe e cargo che possono essere prelevate
					// se la condzione si verifica all'ora l'elicottero non è piu' necessario all'attività
					// e pertanto viene rimesso in hangar
					spot.Eli.isRequiredForOP = false; // elicottero non richiesto
					spot.Eli.PosEli = false; // eli posizionato in hangar
					spot.Eli.IsEfficient = false; // eli impostato non efficiente
					spot.Eli.hasSpotReserved = false; //  eli senza uno spot
					spot.Eli.SpotAssigned = null; // eli senza uno spot
					spot.Eli.IsBladeSpread = false; // eli pale chiuse
					spot.Eli.IsFlying = false; // eli non vola
					spot.Eli.IsRunning = false; // eli not running
					spot.Eli.isApproaching = false; // eli not apprch
					spot.SpotOccupied = false;// eli non spot
					spot.IsReservd = false; // eli non spot
					spot.eliWhoReservd = null; // eli non sopot
					spot.Eli = null; // eli non spot
				}	
			}
		} // fine classe
		//------------------------------------------------------------------------
	
		// metodo per il disimbarco delle truppe sulla LZ
		public void LZTroopsandCargo_Disembark_changeDST ()
		{
			foreach (Elicottero eli in EliM.LandingZoneL.LZeliList) {
			
				if (eli.isLZ  // Se l'elicottero è sulla LZ
					&& !eli.IsBlocked // non è bloccato
					// ha cargo o truppe a bordo
					&& (eli.EliSoldierList.Count>0 || eli.EliCargoList.Count>0)
					) {

					// scarico le truppe e modifico le variaibli
					if (eli.IstroopsOnBoard 
						&& !eli.IsBlocked) // se l'elicottero ha la flag di truppe a bordo le scarico

					eli.EliBlock (StaticSimulator.timeToDisembarkTroops, 9); // blocco elicottero 2 minuti per scarico truppe
					// qui si setta elitroopsisonboard == false FLAG , viene inoltre cancellata la lista truppe da bordo eli


					// scarico il cargo e modifico le variaibili ( la cargo list essite ed è 0 se non cè cargo)
					if (!eli.IstroopsOnBoard 
						&& eli.IsCargoOnBoard 
						&& !eli.IsBlocked) // se l'elicottero ha scaricato le truppe
										// ma ancora non ha scaricato il cargo , allora deve scaricare anche il cargo

						// il blocco degli eli per scaricare il cargo avviene in modo incrementale per il numero di cargo presente a bordo
						// ogni cargo ha in media una necessita' di scarico di 3 minuti
						eli.EliBlock (CargoManager.CargoTimeTodisembark*eli.EliCargoList.Count, 6); 
								
					if (CargoM!=null)	CargoM.CheckThisEliCargoFull (eli); // controllo l'eli 
					}

					if (CargoM != null) CargoM.CheckEli_CargoFull (EliM);// il metodo controlla che esista ancora un cargo 
																		//che possa essere inserto in elicottero
					 													
					// se l'elicottero non è bloccato
					// non ha cargo a bordo 
					// non ha truppe a bordo
					// procedi all'assegnazione del rientro
					if (eli.isLZ
						&&!eli.IsBlocked 
						&& !eli.IsCargoOnBoard 
						&& !eli.IstroopsOnBoard) 
				{ 
						//---------------------CARGO E PERSONALE SCARICATO 
						//-------------------- ASSEGNAZIONE SPOT PER IL RIENTRO
						eli.Go_goback = false; // setta l'eli per il rientro
						eli.IsFull = false; // resetto la variabile di carico completo

						// controllo l'esistenza di uno spot in grado di ospitare l'elicottero al rientro dalla LZ
						Spot spot = SpotM.SpotToHostaCat (eli.CatEli, DNOps);

						if (spot != null) 
						{ // se lo spot non è nullo procedo verso ship
							eli.DirToGo = Elicottero.Direction.ship; // assegno direzione Ship
							SpotM.Spotlist.Find(x => x==spot).IsReservd = true; // spot riservato per l'elicottero
							eli.SpotAssigned = spot; // spot assegnato all'elicottero
							SpotM.Spotlist.Find(x=> x==spot).eliWhoReservd = eli; // l'elicottero che ha richiesto il reserve
							SpotM.CheckState (); // assegnazione dello spot cambio di stato del manager
						} else
							eli.DirToGo = Elicottero.Direction.hp; // se il ponte è occupato vado 
						// resto in atesa nell'holding point


						//--------------------------------------------
						// reset variabili di stato eli per il rientro
						eli.isApproaching = false;
						eli.EliSpeed = this.Speed;// resetto la velocità dell'eli
						eli.isOnDestination = false; // resetto la destinazione
						eli.isLZ = false; // resetto la flag LZ
						this.AssignEli (); // assegno il vettore della nuova destinazione

						WinI.InsertSomeText ("LANDING ZONE : elicottero : " + eli.IdEli + " lascia LZ time: " + eli.current_time.ToString ());
						WinI.InsertEvent (eli.current_time, "T/O LZ", eli.IdEli, eli.NumEli);
					}

			} // termine scansione elioctteri sulla LZ
			// cancellare l'eli dalla LZ 
			if (EliM.LandingZoneL.LZeliList.Count>0)
				EliM.LandingZoneL.LZeliList.RemoveAll (x => x.isLZ==false);// rimuovi gli elicotteri dalla LZ che sono stati rilasciati




		}// termine metodo
		//--------------------------------------------------------------------

		// metodo di controllo elicotteri in holding
		// vengono controllati tutti gli elicotteri in holding sia in direzione per sbarco truppe
		// sia in direzione per rientro a bordo
		// al termine del controllo o viene mantenuto l'holding attesa
		// oppure viene assegnata una nuova direzione di movimento
		public void HPCheckAndChange ()
		{
			//int eliHolding;
			int eliForces;
			//controlla lo stato della lista degli elicotteri in holdin

			int dirHoldGO = EliM.HoldingP.EliHolding.FindAll (eli => eli.Go_goback == true).Count; // conta gli elicotteri all'interno della LZ dir to GO 
			int dirHoldBACK = EliM.HoldingP.EliHolding.FindAll (eli => eli.Go_goback == false).Count; // eli con direzione nave in attesa 


			//eliHolding = EliM.HoldingP.EliHolding.Count- dirHoldGO; // numero  elicotteri in holding attualmente
			eliForces = EliM.EliForce (); // tutte le forze elicottero utilizzabili
			// per l'assalto mi serve che ci siano tutti gli elicotteri impiegabili in una sola ondata

			if (dirHoldGO == eliForces) { // se gli elicotteri sono tutti presenti allora inizia l'assalto
				foreach (Elicottero eli in EliM.ElicotteriList) {
					if (eli.IsHolding && eli.Go_goback) { // se l'eliocottero è in holding e ha direzione ANDATA
						eli.DirToGo = Elicottero.Direction.lz; // gli elicotteri assumono tutti la direzione della LZ
						eli.isApproaching = false; // resetta l'approaching
						eli.EliSpeed = this.Speed; // NECESSARIA?
						eli.isOnDestination = false; // l'elicottero non è piu' a destinazione
						eli.IsHolding = false; // l'elicottero non è piu' in holding
						EliM.RemoveEli (EliM.HoldingP, eli); // rimuovi gli elicotteri dall'HP
						WinI.InsertSomeText ("HOLDING POINT : elicottero : " + eli.IdEli + " lascia l'HP time: "+ eli.current_time.ToString());
						WinI.InsertEvent ( eli.current_time, "OUT HP ",eli.IdEli,eli.NumEli);
					}
				}
			}

			
			// controllo che ci sia uno spot libero per l'elicottero in holding e con direzione Nave 
			// se lo spot esiste lo assegnao all'elicottero
			if (dirHoldBACK > 0) {
				foreach (Elicottero eli in EliM.ElicotteriList) {
					if (eli.IsHolding && !eli.Go_goback) {
						Spot spot = SpotM.SpotToHostaCat (eli.CatEli, DNOps);
						if (spot != null) { // se lo spot non è nullo procedo verso ship
							eli.isOnDestination = false; // resett flag
							eli.isApproaching = false; // resetta l'approaching
							eli.SpotAssigned = spot; // assegnazione spot
							eli.IsHolding = false;// reset flag
							eli.DirToGo = Elicottero.Direction.ship; // assegno nuova direzione
							eli.EliSpeed = Speed;
							spot.IsReservd = true; // lo spot è riservato
							spot.eliWhoReservd = eli; // l'elicottero che ha richiesto il reserve
							EliM.RemoveEli (EliM.HoldingP, eli); // rimuovi gli elicotteri dall'HP
							WinI.InsertSomeText ("HOLDING POINT : elicottero : " 
							+ eli.IdEli + " lascia l'HP time: "+ eli.current_time.ToString());
							WinI.InsertEvent ( eli.current_time, "OUT HP",eli.IdEli, eli.NumEli);
						}
					}
				}
			}
			this.AssignEli (); // assegna la nuova destinazione vettoriale in base alla LZ
			SpotM.CheckState (); // ricontrollo stato ponte
		}


		// FUEL DATA , BLOKING QUOTE GENERATORS PER ELICOTTERI
		// LA FUNZIONE VIENE RICHIAMATA DAL LOOP PRINCIPALE
		// ---------------------------------------------------------------------------------

		// controllo del carburante: per ogni elicottero 
		// viene effettuato un update del carburante
		public void FuelandBlockChekEliloop (float elapsedtime)
		{
			foreach (Elicottero eli in EliM.ElicotteriList) {
				if (eli.IsRunning) {
					if (eli.Fuel > 0) {
						if (!eli.isHotRef)
							eli.Fuel = eli.Fuel - ((eli.FuelC / 60) * elapsedtime); // sottraggo il consumo del carburante di 1 minuto simulato

					} // decremento il carburante al tick del secondo
					//----------------------------------------------

					if (eli.Fuel < (eli.FuelC / 3)) {
						eli.LowFuel = true; //se mancano 20 minuti di carburante dichiaro LOW FUEL STATE
					} else {	
						eli.LowFuel = false; // la variaible puo' RICAMBIARE dopo il rifornimento a caldo
					}
					//----------------------------------------------
					if (eli.Fuel <= 0 && !eli.EliCrash) { // setta le variaibl 
						eli.IsRunning = false; // se il carburante finisce ferma i motori
						if (eli.IsFlying == true) {
							eli.EliCrash = true;// se l'elicottero era in moto setta il crash
							eli.IsFlying = false;
							eli.IsEfficient = false; // eli inefficiente
							eli.isRequiredForOP = false; // eli non piu' richiesto

							if (eli.IsHolding) {
								EliM.HoldingP.EliHolding.Remove (eli);
								eli.IsHolding = false;
							}


							if (eli.SpotAssigned != null) {
								eli.Spotrequested = null; // spot richiesto dall'eli
								// resetto lo spot assegnao all'elicottero abbattuto 
								int k = eli.SpotAssigned.SpotPos;
								SpotM.Spotlist.Find(x => x == eli.SpotAssigned).Eli = null;
								SpotM.Spotlist.Find(x => x == eli.SpotAssigned).eliWhoReservd = null;
								SpotM.Spotlist.Find(x => x == eli.SpotAssigned).IsReservd = false;
								SpotM.Spotlist.Find(x => x == eli.SpotAssigned).SpotOccupied = false;
								eli.SpotAssigned = null; // resetto lo spot assegnato
								SpotM.CheckState (); // ricontrollo lo stato del ponte
							}
		
								EliM.CrashList.Add (eli); // eli crashed
								WinI.InsertSomeText ("SIMULATOR: crash eli (0 fuel): " + eli.IdEli 
								+ " truppe perse: " + eli.EliSoldierList.Count
								+ " cargo perso: " + eli.EliCargoList.Count); 
								WinI.InsertEvent (eli.current_time, "ELI CRASH", eli.IdEli, eli.NumEli);
							}	
					} 
					//---------------------------------------------------------

					eli.TotalWeightCalculated = eli.ToTWGCalculus ();// update del peso totale 

					//---------------------------------------------------------
				}
				eli.RandomQuoteGenrator (); // generatore di quota randomica in volo
				eli.EliBlock (0, -1); // EFFETTUO UN CONTROLLO SUL BLOCCO ELICOTTERI

			} // termine for each
		} // termine FuelandBlockChekEliloop----------------------------------------

		// ritorna vero se il cargo esiste ed è 0 count
		private bool CheckCargoBoolZero()
		{
			if (this.CargoM != null && this.CargoM.CargoList != null) 
			{
				if (this.CargoM.CargoList.Count == 0) return true;
				else return false;
			} 
			else  return true;
		} // termine CheckCargoBoolZero-----------------------------------------------------------------

		//----------------------------------------------------------------------------------------------
		#region [LOGICA DI FUNZIONAMENTO DEL PROGRAMMA ]
		// LOOP LOGICO PRINCIPALE, QUI SI SVOLGE LA LOGICA DI FUNZIONAMENTO DEL PROGRAMMA //
		//----------------------------------------------------------------------------------------------
		// looping delle attività di controllo effettuate dalla logica del programma
		// le informazioni vengono passate al manager grafico per la visualizzazione
		// AL TERMINE DELL'AGGIORNAMENTO LOGICO VIENE RICHIAMATO IL LOOP 
		// CHE SI OCCUPA DELL'AGGIORNAMENTO DELLA GRAFICA E DELLA VISUALIZZAZIONE
		public void LoopforUpdate (float elapsed)
		{

			this.FuelandBlockChekEliloop (elapsed); // CONTROLLO E AGGIORNO FUEL A BORDO ELI

			// ----- inizio il loop con aggiorno gli STATI dei manager
			EliM.CheckState ();
			SpotM.CheckState ();
			TroopM.CheckState ();
			//------termine controllo stati

			// CARGO-------------------------------------------------------------------------
			// controllo se esiste del cargo da prelevare
			// il cargo non deve essere inserito necessariamente negli elicotteri sul ponte
			// puo' essere inserito anche se gli elicotteri sono in hangar
			if (CargoM != null) {

				CargoM.CheckState ();
				if (CargoM.Status != AbstractManager.StatusManager.Empty) {
					// carica il cargo se disponibile sugli elicotteri disponibili
					if (this.CargoM != null)
						CargoM.CargoDistribution (WinI, EliM, TroopM); // distribuisci prima il cargo delle truppe,
				}	  													// il cargo potrebbe non essere presente

				this.CheckEliRequired (); // se gli elicotteri non sono required vengono rimessi in hangar
				CargoM.CheckState (); // aggiorno lo status del cargo
				TroopM.CheckState (); // aggiorno lo statu delle truppe

			}
			//-------------------------------------------------------------------------------------


			// SPOSTO ELI SUL PONTE ------------------------------------------------------------------
			// controllo se l'hangar ha ancora elicotteri da movimentare
			// se l'hangar è vuoto non ho possibilità di movimentare altri elicotteri
			// se il ponte è pieno non ho possibilità di movimentare gli elicotteri
			// inoltre se esistono piu' di due elicotteri bloccati non ne posso movimentare altri,
			// il ponte dispone solo di due elevatori per elicottero 


			if (((EliM.Stato != AbstractManager.StatusManager.Empty) &&
			    (SpotM.Stato != AbstractManager.StatusManager.Full)) /*&& (EliM.CheckBlockedHangar()<=2)*/) {
				// sposta gli elicotteri sul ponte se ci sono spost liberi
				EliM.MoveEliOnSpot (SpotM, DNOps, WinI); // muovi, se possibile, gli elicotteri dall'hangar sugli spot
				EliM.CheckState (); // check elim state
				SpotM.CheckState (); // check spot state
			}


			//-------------------------------------------------------------------------------------------
			//  RUNNING AND BLADE SPREADING
			// controllo la presenza di elicotteri sul ponte
			if (SpotM.Stato != AbstractManager.StatusManager.Empty) {
				// controlla che gli elicotteri sul ponte siano funzionali per l'attività altrimenti li rimette in hangar
				this.CheckEliRequired ();
				// metti in moto gli elicotteri sul ponte
				if ((EliM.CheckBlockedHangar () == 0)) { // se non ci sono piu' elicotteri in attesa in hangar
						
					// posso metter in moto tutti gli elicotteri sul ponte
					SpotM.AllEliSpottedRunning ();// elicotteri sul ponte mettono in moto INIZIO FUEL CHECK

					// apri le pale degli elicotteri sul ponte
					SpotM.AllEliBladeSpreading ();// elicotteri sul ponte aprono le pale pronti per l'imbarco

					// controllo lo STATO del carburante degli elicotteri sul ponte PER LA MISSIONE
					// se gli elciotteri hanno necessità di carburante devo effettuare il rifornimento
					// avvinado un ritardo al decollo di 6 primi
					SpotM.CheckHOTREFLowFuelState (EliM.initEliNum);
				}
			}

			//---------------------------------------------------------------------------------------------

			// SPOSTO LE TRUPPE A BORDO -------------------------------------------------------------------
			// controllo che ci siano truppe ancora non assegnate, che il ponte non sia vuoto e che gli elicotteri presenti non siano bloccati
			// le truppe devono essere assegnate agli elicotteri sul ponte running e a pale aperte

			if ((TroopM.Status == AbstractManager.StatusManager.CanAll)// se il manager ha delle truppe
			     && (SpotM.Stato != AbstractManager.StatusManager.Empty)// se il ponte ha degli elicotteri
			     && (EliM.ElicotteriList.FindAll (x => (x.IsRunning// il numero di elicotteri running
			     && !x.IsFlying// ma che non volano
			     && !x.IsBlocked// elicottero non bloccato
					&& !x.isTroopsFull)).Count > 0)) //  hanno spazio per le truppe
					// procedo con l'imbarco delle truppe se le condizioni sono corrette
				{
				//carica le truppe a bordo degli elicotteri disponibili sul ponte
				TroopM.EliSpotTroopDistribution (SpotM, TroopM, WinI);// muovi le truppe sul sugli spot dopo l'apertura

				TroopM.CheckState (); // aggiorno lo stato delle truppe dopo l'imbarco
				this.CheckEliRequired (); // se gli elicotteri non sono required vengono rimessi in hangare successivamente eliminati
				}

			//-----------------------------------------------------------------------------------------------

			// HOT REF, READY STATUS, TAKE OFF PER TUTTI GLI ELICOTTERI SUL PONTE-------------------------
			// controllo che esistano elicotteri sul ponte per il decollo
			if (SpotM.Stato != AbstractManager.StatusManager.Empty) {

				// se gli elicotteri sul ponte non bloccati assumono lo stato di pronti al decollo
				SpotM.EliReadyStatus (); // gli elicotteri in volo non entrano in questo loop
				// viene settata la velocità di crociera per gli elicotteri decollati

				this.SpotM.AllEliTakingOff (this.Speed); // decolla ed assume destinazione HP
				// gli eicotteri in volo non entrano in questo loop
				// assegna una destinazione agli elicotteri (HP , LZ , DECK )

				//RICONTROLLO GLI STATI -----
				EliM.CheckState (); // controlla lo stato hangat
				SpotM.CheckState (); // controlla lo stato ponte
				this.AssignEli (); // assegna una destinazione vettoriale
				//---------------------------
			}

			//----------------------------------------------------------------
			// se gli elicotteri disponibi sono tutti in HP lascia  HP
			// e assegna LZ come nuova destinazione
			if (EliM.HoldingP.EliHolding.Count > 0) 
			{
				// controlla che tutti gli elicotteri disponibili siano in HP
				this.HPCheckAndChange ();
			}

			// se l'elicottero è sulla LZ sbarca le truppe 
			if (EliM.LandingZoneL.LZeliList.Count > 0) {
				this.LZTroopsandCargo_Disembark_changeDST (); //TRUPPE E CARGO RILASCIO

				TroopM.CheckState (); // aggiorno lo statu delle truppe
				EliM.CheckState ();
			} // controllo gli spot assegnabili

			if (EliM.LandingZoneL.LZeliList.Count > 0)
				EliM.LandingZoneL.LZeliList.RemoveAll (x => x.isLZ == false); // rimuovo dalla LZ l'elicottero ridecollato


			// se il numero di elicotteri lista è 0
			// in quanto se non utili sono stati eliminati
			// allora fine simulazione raggiunto
			//controllo la fine della simulazione
			if (EliM.ElicotteriList.Count == 0) {
				this.EndProgramState = true;
				// loggo il tempo di termine operazioni
				WinI.InsertSomeText ("TERMINE OPERAZIONI " + EliM.MainTime.GetTime ().ToString ());
				WinI.InsertEvent (EliM.MainTime.GetTime (), "TERMINE OPERAZIONI ", "", -2);
				// loggo il tempo di termine operazioni

				// MOTIVAZIONI PROGRAM FAIL
				if (this.TroopM.TroopList.Count == 0 && CheckCargoBoolZero ())  // se il numero di truppe da trasportare ==0 e il cargo è == 0
 {					// simulazione terminata con successo altrimenti simulazione fallita
					WinI.InsertSomeText ("SIMULAZIONE TERMINATA CON SUCCESSO");
					WinI.InsertEvent (EliM.MainTime.GetTime (), "SIMULAZIONE TERMINATA CON SUCCESSO ", "", -3);

				} else {
				
					WinI.InsertSomeText ("SIMULAZIONE FALLITA");
					WinI.InsertEvent (EliM.MainTime.GetTime (), "SIMULAZIONE FALLITA ", "", -3);
				}
					// stoppa i thread ------------------------
					EliM.MainTime.Stop ();
					//-----------------------------------------
				}

				this.EliMovingLogic (elapsed); // logica movimento eli
			}
		

		#endregion [TERMINA IL MAIN LOOP ]
		//---------------------------------------------------------------------

		//---------------------------------------------------------------------
		#region [ MATEMATICA DEI MOVIMENTI GRAFICI]

			public void EliMovingLogic (float elaspedT)
		{

			//----------------------------------------------------------------------------------------------------
			// per ogni elicottero controlla la destinazione e permetti la rotazione per raggiungere la destinazione
			foreach (Elicottero eli in EliM.ElicotteriList) {

				eli.DIstanceToPnT=(StaticSimulator.DistancePtoP ((float)eli.destination.X, (float)eli.destination.Y, eli.elivector2d.X, eli.elivector2d.Y));// distanza obiettivo

				// muove gli elicotteri alla destinazione
				if (!eli.IsBlocked && eli.IsFlying && !eli.isOnDestination) {
					if (!eli.isApproaching)
						eli.EliSpeed = this.Speed;
					StaticSimulator.MoveEli (eli.EliSpeed, eli, (float)eli.destination.X, (float)eli.destination.Y, elaspedT);
				}

				eli.DIstanceToPnT=(StaticSimulator.DistancePtoP ((float)eli.destination.X, (float)eli.destination.Y, eli.elivector2d.X, eli.elivector2d.Y));
				// controllo la distanza dal destinazione
				if ((eli.DirToGo == Elicottero.Direction.hp)// la direzione dell'eli è lhp
					&& (eli.DIstanceToPnT < 30)// la distanza è inferiore a 30px
				    && !eli.isOnDestination) { // e l'elicottero non è ancora flaggato a destinazione// se è arrivato a destinazione
					eli.isApproaching = true; // approach to destination smette di prendere la velocità dalla formazione e la riduce per l'atterraggio
					eli.isOnDestination = true; // sotto i 20 px l'elicotter è a destinazione nell'HP
					eli.IsHolding = true; // flag holding!!
					WinI.InsertEvent ( eli.current_time, "IN HP",eli.IdEli, eli.NumEli);
					WinI.InsertSomeText( " ELI " + eli.IdEli + " ENTRA HP: "+ eli.current_time.ToString());
					eli.EliSpeed = 0f; // setto la velocità
					EliM.AddEliHp (EliM.HoldingP, eli); // aggiungi l'elicottero alla holding lista
				}


				// ELICOTTERI IN HOLDING MANTENGONO UNA ROTTTA CIRCOLARE
				//--------------------------------------------------------------------------------------
				if (eli.IsHolding && eli.isOnDestination) {  // se l'elicottero è in holdign ruota attorno ad un punto fisso
					eli.rotation = eli.rotation + 0.9F*elaspedT; // viene impostata la velocità di rotazione
				}


				//------------------------------------------------------------------------------------------------
				// LANDING zone acquisita
				if ((eli.DirToGo == Elicottero.Direction.lz) && (eli.DIstanceToPnT < 25)) {
					eli.isApproaching = true;
					if (eli.EliSpeed > 0) {
						eli.EliSpeed = eli.EliSpeed - 5f; // decremento della velocità
						eli.quota = eli.quota - 0.01f;
					}
						else { // se l'elicottero ha raggiungo velcotà 0
						if (!eli.isOnDestination) { // se la variabile non è settata a destinazione
							eli.isOnDestination = true; // setta la destinazione come raggiunta
							//	eli.Go_goback = false; // eli assume direzione rientro
							eli.isLZ = true; // flag LZ
							WinI.InsertEvent ( eli.current_time, "LAND LZ" ,eli.IdEli, eli.NumEli);
							WinI.InsertSomeText( " ELI " + eli.IdEli + " SULLA LZ time: "+ eli.current_time.ToString());
							EliM.AddEliList (EliM.LandingZoneL.LZeliList, eli); // aggiungo l'elicottero alla lista

							// INSERIRE SBARCO TRUPPE E CARGO
							// QUANDO TERMINATO CAMBIARE DESTINAZIONE TO SHIP
						}					  
					}
				}


				// DIREZIONE NAVE l'elicottero rientra dalla LZ PUO': RIFORNIRE, TERMINARE L'OPERAZIONE, CARICARE CARGO TRUPPE E DECOLALRE
				//----------------------------------------------------------------------------------------------------
				if (!eli.isApproaching && (eli.DirToGo == Elicottero.Direction.ship)
				    && eli.IsFlying && !eli.Go_goback
					&& eli.DIstanceToPnT < 25) {

					eli.isApproaching = true; // elicottero approaching
				}
				// ship acquisita diminuzione di speed
				if (eli.isApproaching && (eli.DirToGo == Elicottero.Direction.ship)
				    && eli.IsFlying && !eli.Go_goback
					&& eli.DIstanceToPnT < 20) {
					if (eli.EliSpeed > 0)
						eli.EliSpeed = eli.EliSpeed - 5f; // decremento della velocità
					else {
						eli.isOnDestination = true; // elicottero arrivato a destinazione
						eli.SpotAssigned = SpotM.Spotlist.Find (spot => spot.eliWhoReservd == eli);// assegno lo spot
						// salvo dati
						WinI.InsertSomeText ("ELICOTTERO APPONTA: " + eli.IdEli + " time: "+ eli.current_time.ToString());
						WinI.InsertEvent ( eli.current_time, "LAND SHIP",eli.IdEli, eli.NumEli);
						//--- salvo dati
						eli.SpotAssigned.SpotOccupied = true; // lo spot è occupato
						eli.SpotAssigned.Eli = eli; // as segno l'elicottero allo spot
						eli.PosEli = true; // pos sul ponte
						eli.Go_goback = true;
						eli.IsFlying = false; // elicottero a terra non piu' in volo
						eli.rotation = 230; // riporto l'angolo iniziale
						eli.isREADYstatus = false; // deve riacquisire il ready status perridecollare
						SpotM.CheckState (); // controllo lo stato del simulatore
						eli.EliSpeed = 0; // resetto la speed a 0
					}
				}
			}
		
		} // temine loop
		#endregion [ TERMINA LA LOGICA DEI MOVIMENTI ]
	}
	// fine classe
}
// fine namespace

