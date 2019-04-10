//**************************************************************
// classe manager per la creazione elicotteri ed il controllo degli stati 
// detiene la lista elicotteri e le operazioni su di essi
// classe singleton
//***************************************************************

using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace EliAssaltoAnfibio
{
	public class EliManager : AbstractManager
	{
		public static  EliManager instance= null;
		public ClockTimer MainTime = new ClockTimer (); // definizione del main timer per gli elicotteri
		public int TotEli{ set; get;}	// indica il numero di elicotteri da creare
		public int LZdistance{ set; get;} // è la distanza della LZ dalla nave
		public List<Elicottero> ElicotteriList{ set; get;} // definizione della lista di elicotteri
		public HoldingPoint HoldingP{ set; get;}	// holding point per elicotteri
		public LandingZone LandingZoneL{ set; get;}// landing zone	
		public StatusManager Stato{ set; get;}// variabili di stato
		public List<Elicottero> SupportEliList = new List<Elicottero> (0); // lista supporto
		public List<Elicottero> CrashList = new List<Elicottero> (0); //eli crash list
		public ActionManager Action{ set; get;}// definizioni dello stato del manager
		public InfoWindow WinI;
		public int initEliNum { set;get;}
		//implementare la lista di elicotteri e di vettori-----
		// si è preferito implementare la lista di frame per la visualizzazione dell'eli
		// con le pale in rotazione nella classe manager per eli
		public Texture2D[] EliTexture = new Texture2D[7]; // frames eli volo
		public Texture2D CrashedEli; // frame crash

		// eli list disposer specs
		// costruttore della classe, Eredita il tipo di manager dalla classe base
		public EliManager (int numeroEli, InfoWindow winI) : base ("Manager_per_Elicottero")
		{	
			this.WinI = winI; // finestra informazioni
			this.TotEli = numeroEli; // acuisisce il numero di elicotteri iniziali da gestire
			this.Stato = StatusManager.Empty;  // setta lo stato come vuoto in quanto non ci sono ancora elicotteri inseriti
			this.Action = ActionManager.Wait; // le azioni sono in attesa di informazioni aggiuntive
			this.MainTime.SetTime (new TimeSpan (0, 0, 0));// setting del timer iniziale a 0
		}

		// istanza singleton è prevista l'esistenza di un'unica finestra
		public static EliManager Instance(int numeroEli, InfoWindow winI)
		{
			if (instance == null) instance= new EliManager( numeroEli ,  winI );
			return instance; // ritorno il costruttore	
		}

		// crea la lista con un numero di elementi specifico
		// la lista in relata' resta vuota, ma viene assegnato lo spazio
		// iniziale per poter ospitare un numero di oggetti voluto
		// il manager puo' gestire differenti tipi di liste a seconda dell'override del metodo
		public override void MakeList (int elementi)
		{	
			this.ElicotteriList = new List<Elicottero> (elementi);	
		}

		// inserisci un elemento nella lista, viene usata per inseire nella lista un elicotteo
		// che è già stato creato
		public override void InsertElement (object eli)
		{
			this.ElicotteriList.Add ((Elicottero)eli);
		}

		//rimuovi un elemento dalla lista REMOVEAT INDEX
		public override  void RemoveElement (int i)
		{
			this.ElicotteriList.RemoveAt (i);
		}

		// aggiungi l'elicottero alla lista di holding
		public void AddEliHp (HoldingPoint hp, Elicottero eli)
		{

			hp.EliHolding.Add (eli);
		}
		//rimuovi l'elicottero dalla lista degli hoding
		public void RemoveEli (HoldingPoint hp, Elicottero eli)
		{

			hp.EliHolding.Remove (eli);

		}
		// aggiungi l'elicottero alla lista
		public void AddEliList (List<Elicottero> obj, Elicottero eli)
		{

			obj.Add (eli);
		}

		//metodo di creazione della landing zone
		public  void MakeLZ (int dist)
		{	
			LandingZoneL = new LandingZone (dist); // viene creata la landing zone
		}
		// viene creato l'holding point quando necessario e dove necessario
		public void MakeHoldingPoint (int dist) // crea l'hoding point alla distanza definita
		{

			HoldingP = new HoldingPoint (dist);
		}

		// determina l'inserimento di un nuovo elicottero nella lista
		public  void InsertElementEli (ClockTimer time, int numEli, string id, int cat, float fuel, int troops, int cargo, 
			int maxToLoad, int offLoadWg, bool running, bool bladeSprd, bool isEff, bool initialPos, bool isFly, float fuelC)
		{
			// creazione del nuovo elicottero con valori inizializzati
			Elicottero eli = new Elicottero (time, numEli, id, cat, fuel, troops, cargo, 
				maxToLoad, offLoadWg, running, bladeSprd, isEff, initialPos, isFly, fuelC); // creazione del nuovo elicottero con valori inizializzati
			// inserisco l'elemento creato nella lista
			ElicotteriList.Insert ((numEli), eli);
		}
		// controlla gli elicotteri bloccati e non volanti
		// controlla quanti elicotteri attualmente bloccati sono in hangar
		// l'elicottero inoltre deve essere efficiente e richiesto per l'operazione
		public int CheckEliBlockedNotFlying ()
		{
			int n = 0;
			foreach (Elicottero eli in this.ElicotteriList) {
				if (eli.IsBlocked && !eli.PosEli && eli.IsEfficient && eli.isRequiredForOP)
					n++;
			}
			return n;
		}

		// restituisce il numero di elicotteri pianificati o presenti sul ponte di volo
		public int EliOnDEck ()
		{
			int n = 0;
			foreach (Elicottero eli in this.ElicotteriList) {
				if (eli.PosEli)
					n++;
			}
			return n;
		}
		// restituisce il numero di elicotteri efficienti ed impiegabili per l'operazione
		// gli elicotteri sono la forza effettiva
		public int EliForce ()
		{
			int n = 0;
			foreach (Elicottero eli in this.ElicotteriList) {
				if (eli.IsEfficient 
					&& eli.isRequiredForOP  // se gli eli sono required
					&& eli.Go_goback  // vanno in direzione lz
					&& !(eli.DirToGo==Elicottero.Direction.lz)) // hanno la destinazione LZ
					n++; // incremento il contatore
			}
			return n;
		}
		// controllo gli elicotteri bloccato e richiesti per l'operazione attualmente in hangar
		public int CheckBlockedHangar ()
		{
			int n = 0;

			foreach (Elicottero eli in this.ElicotteriList) 
			{
				if (eli.IsBlocked // se l'eli è bloccato
					&& !eli.PosEli  // l'eli è in hangar
					&& eli.hasSpotReserved) // ed ha uno spot riservato
					n++; // incremento il contatore
			}

			return n;
		}

		// restituisce il numero di elicotteri in hangar che possono essere spostati
		// gli elicotteri devono essere efficienti non bloccati e richiesti per l'attività
		public int EliInHangar ()
		{


			int n = 0;
			foreach (Elicottero eli in this.ElicotteriList) {
				if (!eli.PosEli  // posizione in hangar
					&& !eli.IsFlying // l'eli non vola
					&& !eli.hasSpotReserved // l'eli ha uno spot riservato
					&& eli.SpotAssigned == null // non  
					)
					n++; // posizione in hangar posEli=false
			}
			return n;
		}


		// restituisce l'elicottero efficiente in grado di contenere il cargo specificato
		// con le truppe necessarie
		// viene effettuato anche il controllo sugli operatori necessari per la movimentazione
		// del cargo stesso, Viene impiegato prima l'elicottero con il maggior peso dsponibile
		public Elicottero  EliAbleToCargo (Cargo cargo, TroopsManager troopM)
		{
			int minTWeight = 0;// peso totale minimo tra cargo e truppe a bordo trovato su elicotteri disponibili

			Elicottero eliMinW = null;// elicottero efficiente che ha il maggior carico disponibile

			foreach (Elicottero eli in this.ElicotteriList) { // per ogni elicottero in lista

				// controllo che lo spot sia occupato da un elicottero efficiente, e che abbia lo spazio per cargo e truppe richieste
				if ((eli.isRequiredForOP && eli.Go_goback && !eli.IsBlocked && eli.IsEfficient && !eli.IsCargoFull && !eli.IsFlying
				    && eli.WCargoLeftOnBoard >= cargo.ChecktotalCargoWeight ()))

					// controllo che l'elicottero sia quello ceh offre la maggior disponibilità di cargo
				if ((eli.WCargoLeftOnBoard) > minTWeight) { 	
					minTWeight = eli.WCargoLeftOnBoard;
					eliMinW = eli;	// assegno l'elicottero
				}
			}
			return eliMinW; // ritorno l'elicottero trovato se non lo trova rotorna null
		}

		// LIST DISPOSER METODI - metodi di movimentazione degli elementi in relazione agli altri manager
		#region list disposer
		// sposta gli elicotteri sugli spot se questi possono contenerli
		public  void MoveEliOnSpot (SpotManager SpotM ,Spot.day_night DN, InfoWindow WinI)
		{ 	
			bool elidispoSuccess; // flag disposizione avvenuta con successo
			// controllo se esistono elicotteri che possono essere ospitati dagli spot per le caratteristiche
			// che hanno elicottero e spot
			foreach (Elicottero eli in this.ElicotteriList) {
				if (!eli.IsBlocked // controllo se l'elico non è bloccato ed è in hangar
					&& !eli.PosEli // eli è in hangar
					&& !eli.hasSpotReserved // eli non ha riservato uno spot
				) { 

					Spot _spot = SpotM.SpotToHostaCat (eli.CatEli, DN);//ritorno il primo spot in grado di ospitare l'elicottero 

					if (_spot != null) { // se lo spot è stato trovato 
						WinI.InsertSomeText ("Spot trovato per assegnazione: " + _spot.SpotPos); // scrittura info

						// viene se l'elicottero è efficiente, la posizione è in hangar 
						//e lo spot esiste e non è riservato ad altri elicotteri
						// inoltre possono essere movimentati solo due elicotteri alla volta 

							if (elidispoSuccess = SpotM.SpotEliAssign (eli, _spot)) {// prova ad assegnare l'elicottero
							WinI.InsertSomeText ("DISPOSER: movimentato elicottero : " 
							+ eli.IdEli + " su spot: " + eli.Spotrequested.SpotPos);
							}
						} else
							WinI.InsertSomeText ("DISPOSER: movimentazione elicottero " + eli.IdEli + " non effettuabile");
				} //termine IF
			} // termin foreach elicottero ciclo
		}

		//----------------------------------------------------------------------------------
		//-- controllo eli in hangar e moviementazione sul ponte ------------------//
		// è necessario controllare se gli eli assegnati sugli spot sono superiori al 
		// numero di spot effettivamente disponibile, in tal caso gli eli devono essere riassegnati

		public int ElisSpotAutoHangarChangeNumber (SpotManager SpotM, InfoWindow WinI)
		{

			int n = this.EliOnDEck (); // numero di elicotteri pianificati sul ponte
			int k = SpotM.NumSpotEfficient (); // numero di spot efficienti presente
			int diff = (n - k); // qundo positivo indica il numero di elicotteri da ridurre

			int z = 0; // variabile indice
			int i = 0; // variabile indice
			// se il numero di elicotteri è superiore al nuemro di spot efficienti 
			// allora serve ridurre il numero di elicottero sullo spot
			if (diff > 0) { // se la diff è positiva il numero di eli assegnati al ponte supera il numero degli spot disponibili
			WinI.InsertSomeText (" SIMULATORE: auto correzione del numero di elicotteri sul ponte. Decremento di " + diff + " unità");

				// correzione del valore di posizione sui primi i elicotteri trovati 
				while (i < diff) {
					if (this.ElicotteriList [z].PosEli == true) {
						this.ElicotteriList [z].PosEli = false; // cambio a false
						i++; // valore trovato incremento indice
					}
					z++; // incremento l'indice
				}
				return k; // restituisce il numero di elicotteri ga gestire manualmente
			} 
			return n; // restituisce il numero di elicotteri da gestire manualmente
		}
		//----------------------------------------------------------------------------------

		public void CheckSpotDataConsistency(SpotManager spotm, Spot.day_night _dn)
		{
			foreach (Elicottero eli in this.ElicotteriList)
				if (spotm.SpotToHostaCat (eli.CatEli, _dn) == null)
				 {
					eli.IsEfficient = false;
					eli.isRequiredForOP = false;
					WinI.InsertSomeText ("SIMULATOR: rimosso elicottero " + eli.IdEli + " ,nessuno spot accessibile");
				}
		}

		//----------------------------------------------------------------------------------
		// assegnazione  degli elicotteri assegnati agli spot 
		// è necessario assegnare agli spot gli elicotteri che di default sono stati posizionati sugli spot
		public void EliAutoSpotAssign (SpotManager SpotM, InfoWindow winI, Spot.day_night DN)
		{
			// cambia automanticamente il numero di elicotteri assegnati sul ponte
			// la funzione assicura che il nuemero di eli è inferiore o uguale agli spot disponibili 
			// e restituisce il numero di elicotteri da gestire  per l'assegnazione automatica degli spot
			int manualAssign = this.ElisSpotAutoHangarChangeNumber (SpotM, winI); 

			// una volta determinato il numero di elicotteri da assegnare procedo con l'assegnazione
			if (manualAssign > 0) { // se il numero di assegnazioni è maggiore di 0 creo la finestra per le assegnazioni

				foreach (Elicottero eli in this.ElicotteriList) {
					if (eli.PosEli) { // se l'elicottero è posizionato sul ponte di default cerco di assegnarlo ad uno spot
						Spot spot = SpotM.SpotToHostaCat (eli.CatEli, DN);
						if (spot != null) {
							winI.InsertSomeText ("SIMULATOR: Spot trovato per assegnazione AUTOMATICA: " + spot.SpotPos);
							eli.DirToGo = Elicottero.Direction.hp; // direzione assegnata all'eli sul ponte holding point
							eli.SpotAssigned = spot; // spot assegnato
							eli.PosEli = true;   // posizione elicottero sul ponte
							spot.IsReservd = true; // lo spot viene riservato
							spot.eliWhoReservd = eli; // l'elicottero che ha richiesto il reserve
							spot.Eli = eli; // assegno l'elicottero
							spot.SpotOccupied = true; // lo spot e' occupato
						}
					}
				}
			}
		} // fine metodo
		//----------------------------------------------------------------------------------

		// metodo di supporto alla classe serve per attuare l'inizializzazione delle macchine,
		// controllo se gli elicotteri proposti sono tutti necessari per l'operazione
		// se ad esempio ho 3 elicotteriche possono trasportarte 10 truppe per un totate di 
		// 30 persone mentre ho solo 15 persone disponibili gli elciotteri necessari saranno
		// solo 2. il terzo quindi risulterà non impiegabile
		// tuttavia va controllato anche che l'elicottero risulti non necessario anche per
		// il trasporto del cargo
		public void Check_Eli_Usability (InfoWindow WinI,TroopsManager TroopM, CargoManager CargoM)
		{

			WinI.InsertSomeText ("SIMULATOR : controllo effettiva NECESSITA' degli elicotteri inseriti per la missione");
			int TroopTotW =	TroopM.TroopList.Count * TroopM.SingleSoldierW; // peso totale delle truppe
			int EliTotW = this.ElicotteriList.Sum (x => x.WTroopsLeftOnBoard); // peso totale disponibile sugli elicotteri
			int diffT_E = EliTotW- TroopTotW; // calcolo la differenza tra il peso totale trsportabile e il peso totale da trasportare
			List<Cargo> SupportCargoList = new List<Cargo> (); // lista cargo di supporto
			Elicottero eliMinW = this.ElicotteriList.Find (y => y.WTroopsLeftOnBoard == (this.ElicotteriList.Min (x => x.WTroopsLeftOnBoard))); 
			// elicottero con peso minimo disponibile nella lista elicotteri

			// l'algoritmo elimina tutti gli elicotteri che offrono meno peso per il trasporto 
			// dopo che viene definita la quantità di peso totale necessaria
			while (diffT_E > 0 && diffT_E> eliMinW.WTroopsLeftOnBoard) { 

				bool found = false; // bool di supporto al metodo
				int supportCounter = 0; // bool var di supporto

				if (SupportCargoList.Count > 0)
					SupportCargoList.Clear (); // reset list

				if (diffT_E > eliMinW.WTroopsLeftOnBoard) { //se la differenza tra il peso totale delle truppe
					// trasportabile ed il peso totale disponibile sugli elicotteri 
					// è maggiore del peso disponibile sull'elicottero con peso trasportabile minimo allora ho trovato un 
					// elicottero candidato

					this.SupportEliList.Add (eliMinW); // aggiungo l'elciottero alla lista 
													//di supporto come candidato all'eliminazione

					// devo ora controllare che l'elicottero sia anche indispensabile per l'attività di trasporto cargo
					if (CargoM != null && CargoM.CargoList != null) {

						// estrapolo elenco cargo che puo' ospitare l'elicottero candidato
						foreach (Cargo cargo in CargoM.CargoList.FindAll(x => (x.ChecktotalCargoWeight() <= eliMinW.WCargoLeftOnBoard))) {
							SupportCargoList.Add (cargo); // inserisco i valori trovati all'interno della lista di supporto
						}

						// DEVO CONTROLLARE
						// 1 CHE NON POSSA TRASPORTARE NESSUN CARGO
						// 2 CHE QUALORA POSSA TRASPORTARE DEL CARGO QUESTO NON SIA TRASPORTABILE DA ALTRI ELICOTTERI
						// SE UNA O L'ALTRA CONDIZIONE SONO VERE L'ELICOTTERO PUO' ESSERE ELIMINATO DALL'ELENCO
						if (SupportCargoList.Count > 0) { // controllo che non possa trasportare nessun cargo

							// conrtollo tutti i cargo presenti nell'elenco in quanto devono essere tutti 
							// trasportabili da altri elicotteri
							foreach (Cargo cargo in SupportCargoList) {
								found = false; // resetto la variabile bool di supporto

								// per ogni altro elicotteo NON MINIMO PESO effettuo il confronto con il cargo
								foreach (Elicottero eli in this.ElicotteriList.FindAll(x=> x != eliMinW)) {
									if (eli.WCargoLeftOnBoard >= cargo.ChecktotalCargoWeight ()) { // se esiste un elicottero che puo' imbarcare il cargo
										//incremento il contatore, anche gli altri cargo nell'elenco devono essere inclusi nella condizione
										found = true;
									}

								} // end loop su eli 

								// se ho trovato almeno un elemento in grado di accettare il cargo la var bool found è settata su vero
								// quindi posso incrementare il counter
								if (found)
									supportCounter++; // se lo trova il cargo nella lista cargo di supporto 
								//che poteva trasportare l'eliminW ora lo puo' trasportare qualun altro

							} // end loop su cargo 

							// se il numero degli elicotteri che possono ospitare il cargo eguaglia il numero del cargo da ospitare
							//l'elicottero non è necessario
							if (supportCounter == SupportCargoList.Count) {

								// posso eliminare l'elicottero dall'elenco elicotteri
								this.ElicotteriList.Remove (eliMinW);
								WinI.InsertSomeText ("SIMULATOR : rimosso elicottero: "+ eliMinW.IdEli + " l'eli risulta non necessario");
							} else
								// altrimenti devo rimuoverlo dall'elenco dei candidati
								this.SupportEliList.Remove (eliMinW);
						}// end if cargo count >0 ELICOTTERO INUTILE PER L'OPERAZIONE ELIMINARE ELICOTTERO DALL'ELENCO

						else {
							// elicottero inutile per l'operazione
							this.ElicotteriList.Remove (eliMinW);
							// l'elicottero viene cosi lasciato nell'elenco 
							// degli elicotteri di supporto non utilizzabili
							WinI.InsertSomeText ("SIMULATOR : rimosso elicottero: "+ eliMinW.IdEli + " l'eli risulta non necessario");
						}

					} // fine if CARGO NULL INESISTENTE

					else {
						// elicottero inutile per l'operazione
						this.ElicotteriList.Remove (eliMinW);
						WinI.InsertSomeText ("SIMULATOR : rimosso elicottero: "+ eliMinW.IdEli + " l'eli risulta non necessario");
					}
				} // fine if differenza l'elicottero è utile per l'operazione

				// update delle variabili
				EliTotW = this.ElicotteriList.Sum (x => x.WTroopsLeftOnBoard); // peso totale disponibile sugli elicotteri
				diffT_E = EliTotW- TroopTotW; // calcolo la differenza tra il peso totale trsportabile e il peso totale da trasportare
				eliMinW = this.ElicotteriList.Find (y => y.WTroopsLeftOnBoard == 
					(this.ElicotteriList.Min (x => x.WTroopsLeftOnBoard))); //determino il nuovo min
			}// fine while
		} // fine metodo Check_Eli_Usability

		// effettuo il controllo carburante per ogni elicottero 
		// IList carburante deve essere necessario alla missione
		// altrimenti devo aumentare il carburante e ridurre peso per le truppe
		public void CheckEliDataFuelConsistency(int dist)
		{
			float totalCarbRequired; // il carburante richiesto
			foreach (Elicottero eli in this.ElicotteriList) {
				// controllo il carburante necessario alla missione
				// il valore viene calcolato con una velocità media di 100 nodi
				// e viene sommato al risultato 20 minuti di volo.

				eli.DistancetoRun = dist; // resetto la run distance dell'elicottero
				totalCarbRequired =  eli.Fuelrequired(this.initEliNum); //calcolo il carburante necessario all'operazione
				totalCarbRequired = totalCarbRequired + 50; // assegno 50 Kg in piu' per lo start iniziale
				if (eli.Fuel < totalCarbRequired) { // se il carburante all'interno dell'elicottero NON
					// è sufficiente devo incrementasre il carburante levando peso alle truppe e al cargo

					float diffF = totalCarbRequired - eli.Fuel;// differenza di carburante che devo ancora inserire
					float freeW = eli.MaxTOLoad-eli.ToTWMaxFull (); // peso libero PESO MASSIMO AL DECOLLO - MESO A PIENO CARICO IMPOSTO UTENTE
					//  devo prima verificare che il carburante possa essere inferito in elicottero senza 
					// diminuzione del carico di truppe o cargo
					if (freeW >= diffF) {
						float fuelN = eli.Fuel + diffF; // correggo il fuel prendendolo tutto dal peso libero non occupato a bordo
						eli.Fuel = (fuelN);
					} else {

						float fuelK = eli.Fuel + freeW;// correggo il fuel prendnolo in parte dal peso libero non occupato a bordo 
						eli.Fuel= (fuelK); // nuovo fuel = vecchio fuel + la differenza di peso libero ancora imbarcabile
						diffF = diffF - freeW; // differenza in peso di carburante+ ancora da imbarcaree
					
					if (diffF <= (eli.WTroopsLeftOnBoard + eli.WCargoLeftOnBoard)) // se posso inserire il resto del carburante al posto delle truppe
						{														  // applico la correzione
						// elimino il peso dalle truppe e dal cargo proporzionalmente

						float p1 = ((float)(eli.WTroopsLeftOnBoard) /(float) (eli.WCargoLeftOnBoard + eli.WTroopsLeftOnBoard)); // percentuale peso truppe
									
						float p2 = ((float)(eli.WCargoLeftOnBoard) /(float) (eli.WCargoLeftOnBoard + eli.WTroopsLeftOnBoard)); // percentuale cargo

						// sottraggo peso all'eli in percentuale
						eli.WTroopsLeftOnBoard = eli.WTroopsLeftOnBoard - (int) (p1 * (int)diffF);
						eli.WCargoLeftOnBoard = eli.WCargoLeftOnBoard - (int)(p2 * (int)diffF);

						float fuelZ = eli.Fuel + diffF;
						eli.Fuel= (fuelZ); // rieffettuo la distribuzione dei pesi di carburante
					} else 
					{
						// la missione non è effettuabile per l'elicottero
						// setto le flag mettere l'elicottero fuori uso e lasciarlo in hangar
						eli.IsEfficient = false;
						eli.isRequiredForOP = false;
						eli.PosEli = false;
						eli.IsRunning = false;
						eli.IsBladeSpread = false;
					}}
				}
			}
		} // end method CheckEliDataFuelConsistency


		// correggo i pesi degli elicotteri per evitare spezzoni
		// se il peso di una singola truppa è 80 kg ma il mio elicottero puo' trasportare 90 K di truppe
		// in realtà il peso trasportabile è sempre 80 l'eli necessita di una correzione dei multipli
		public void TroopsW_Data_correction (int singleW)
		{
			foreach (Elicottero eli in this.ElicotteriList) 
			{
				int W;

				W = eli.WTroopsLeftOnBoard / singleW; // numero di soldati 
													  // l'arrotondamento è sempre all'intero minore
				eli.WTroopsLeftOnBoard = W * singleW;	                        

			}
		}  // fine metodo TroopsW_Data_correction


		// controllo DEGLI STATI  lista elicotteri
		// controllo LO STATO DELL'HANGAR
		// CHE PUO' ESSERE PIENO, VUOTO O IN UNO STATO INTERMEDIO
		public override void CheckState ()
		{		

			// rimuovo gli leementi inefficienti o non richiesti
			ElicotteriList.RemoveAll (x => x.IsEfficient == false); // rimuovo gli elementi inefficienti
			ElicotteriList.RemoveAll (x => x.isRequiredForOP == false); // rimuovo gli elementi non required

			int eliHangar = this.EliInHangar (); // elicotteri flaggati sul ponte richiesti per op efficienti
			// che non hanno riservato uno spot, che non sia voltante, che non abbia uno spot assegnato o riservato


			int eliHasSpotorFly = this.ElicotteriList.FindAll (x =>( x.PosEli==true || x.IsFlying==true || x.hasSpotReserved==true)).Count;
			int toteli = this.ElicotteriList.Count;
			int eliHangarUsable = toteli - eliHasSpotorFly;


			if (eliHangar == 0) {	// se gli elicotteri in hangar sono 0
				this.Stato = StatusManager.Empty;	// lo stato è vuoto, l'hangar non contiene elicotteri efficienti
				this.Action = ActionManager.ReadyToReceive; // l'hangar è pronto a ricevere
			}

			if ((eliHangar> 0) && (eliHasSpotorFly==0)) {//tutti gli eli sono in hangar se il numero di quelli che vola o è sullo spot è 0
				this.Stato = StatusManager.Full;		// lo stato risulta pieno
				this.Action = ActionManager.ReadyToSend;// l'hangar puo' solo inviare elicotteri
			}

			if ((eliHangar > 0 ) && (eliHasSpotorFly>0)) {// se eli in hangar non sono tutto ma piu' di 0
				this.Stato = StatusManager.CanAll;	// lo stato è promiscuo, l'hangar non è pieno o vuoto
				this.Action = ActionManager.ReadyToSendAndReceive; // è possibile qualcunque azione di invio o ricezione sul ponte
			}

		}
		#endregion
	} //-------------------------------------------------- fine classe
}
//---------------------------------------------------- fine namespace

