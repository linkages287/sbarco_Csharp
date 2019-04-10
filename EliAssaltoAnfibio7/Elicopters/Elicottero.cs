// +++++++++++++++++++++++++++++++++++++++++++++++++++++
// CLASSE FISICA DI GESTIONE ELICOTTERO 
// CONTIENE TUTTI GLI ATTRIBUTI NECESSARI 
// ALLA DEFINIZIONE DI UN ELICOTTERO
// EREDITA DA IOBSERVER IN QUANTO OGNI ELICOTTERO
// E' DOTATO DI TIMER E NECESSITA UNA SINCRONIZZAZIONE
// PER IL CALCOLO DEL CARBURANTE E PER GESTIRE LE PAUSE
//++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace EliAssaltoAnfibio
{
	public class Elicottero : IObserver
	{
		//---internal varibiales---------------------------
		public ClockTimer timer;  // timer variabili si uso generico e MASTER TIMER
		public int NumEli {set; get;}// indica il numero di sequenza assegnato all'elicottero ( e' un indice )
		public string IdEli {set; get;}// nome o matricola che identifica l'elicottero in volo ES. King-01 
		public int CatEli {set; get;} // classe di elicottero 1=pesante 2=leggero 3=altro
		public float Fuel {set; get;} // indica il livello di carburante dell'elicottero
		public float FuelC {set;get;}// indica il consumo di carburante orario
		public int WTroopsLeftOnBoard {set; get;} // indica il numero di truppe imbarcate sull'elicottero
		public int WCargoLeftOnBoard {set;get;} // indica altri componenti di cargo general purpouse
		public int MaxTOLoad {set;get;} // max Take Off load indica il peso massimo al decollo del velivolo
		public int OffLoadWeight {set;get;}// indica il peso del velivolo scarico di carburante, truppe o altro carico
		public int TotalWeightCalculated {set;get;} // indica il peso del velivolo allo stato attuale
		public float EliSpeed {set; get;}// velocità del velivolo
		public int WaitingTime { set; get;} // waiting time per il blocco
		public int DistancetoRun { set;get;} // distanza obiettivo LZ in NM
		public int DIstanceToPnT { set; get;} // distanza obiettivo
		public float AngleToFly { set; get;} // angolo di volo per destinazione
		public float quota {set; get;} // quota iniziale , serve per mostrare graficamente la dimensione del texture 
		// variando il parametro è possibile dare l'impressione ottica  di una variazione di quota
		private TimeSpan time;
		private int blockReason;
		public Spot Spotrequested{ set; get;}
		public LandingZone LZ { set; get;}
		// --------------------------------------------------

		// status flags---------------------------------------
		public bool IsRunning {set;get;} // indica se i motori sono accesi
		public bool IsRefueling {set;get;}// indica lo stato di refueling ( richiede 10 minuti )
		public bool IsBladeSpread {set;get;}// l'elicottero è a pale aperte o chiuse
		public bool IsEfficient {set;get;} // indica lo stato di efficienza del velivolo 1 efficiente 0 non efficiente
		public bool PosEli{set;get;} // indica se eli in hangar(false) o se eli sul ponte(true) 
		public bool IsFlying {set;get;} // 0 se eli non in volo - 1 se eli in volo
		public bool IsHolding { set; get;} // flag indicante lo stato di holding dell'elicopttero
		public bool isLZ { set; get;} // elicottero sulla LZ
		public bool LowFuel {set;get;} // flag che indica se l'elicottero sta esaurendo il carburante
		public bool EliCrash {set;get;}// flag che indica se l'elicottero ha terminato il carburante ed è caduto
		public bool IsBlocked {set;get;} // indica lo stato di blocco momentaneo dell'elicottero ( la variabile è
		public bool IstroopsOnBoard { set; get;} // l'elicottero ha le truppe a bordo	
		public bool IsCargoOnBoard { set; get;} // l'elicttero ha del cargo a bordo							o
		public bool isOnDestination { set; get;} // l'elicottero sta occupando un a LZ
		public bool isREADYstatus { set; get;} // l'elicottero è sul ponte, truppe a bordo , carico a bordo pronto per partire
		public bool isRequiredForOP { set; get;} // elicottero richiesto per l'operazione
		public bool Go_goback { set; get;} // vai verso la lanzig zone (true ) torna dalla lanzing zone (false)
		public bool IsFull { set; get;} // indica che non è piu' possibile caricare altro materiale
		public bool IsCargoFull { set; get;} // cargo full
		public bool isTroopsFull { set; get;} // truppe full
		public bool isApproaching { set; get;} // approach to destination
		public bool isHotRef { set; get;} // flag di hotref
		public bool hasSpotReserved { set; get;} // ha riservato lo spot
		public bool IsBoarding { set; get;} // se l'elicottero sta caricando truppe
		public bool IsBoardingCargo { set; get; } // boarding del cargo
		// end status flags------------------------------------

		//  settaggi per il movimento nello spazio grafico-----
		public Vector2 elivector2d { set; get;} // vettore pos e movimento

		public Point destination { set; get;}
		public float rotation {set;get;}	// indica la rotazione del vettore rispetto ad un punto misurata in radianti
		public Texture2D EliTexture{set; get;} // eli texture TODO cambia a seconda dell'elicottero
	
		public enum Direction: int {lz,hp,ship};// enumerazione giorno e notte
		public Direction DirToGo { set; get;}// direzione di movimento
		Random random = new Random(); // RANDOM GENERATOR utile per variabili casuali
		//-----------------------------------------------------

		//------------------------------------------------------


		// liste di gestione ogni elicottero puo' essere carico con truppe e/o cargo
		public List<Soldier> EliSoldierList{set;get;} // lista soldati a bordo dell'elicottero
		public List<Cargo> EliCargoList{set;get;} //lista del cargo presente a bordo dell'elicottero - il cargo è formato da blocchi di materiale 
										// o armamento che hanno un peso ( ogni cargo necessita di persone a bordo per il controllo e lo scarico - carico)

		public Spot SpotAssigned{set;get;} // indica lo spot di assegnazione attribuito all'elicottero 
										// il valore è nullo se l'elicottero non è sullo spot.



		public TimeSpan current_time; // rappresenta l'update del timer corrente


		// costruttore della classe elicottero
		public  Elicottero (ClockTimer timerCon, int numEli, string Id, int Cat, float Fuel ,  int Troops , int Cargo , int MaxToLoad , int OffLoadWG, bool Running, bool BladeSprd , bool IsEff, bool InitialPos, bool IsFly , float FuelC) 
		{
			// passo le variabili inizializzate all'oggetto elicottero

			this.NumEli=numEli;
			this.IdEli= Id;
			this.CatEli=Cat;
			this.Fuel=Fuel;
			this.WTroopsLeftOnBoard=Troops;
			this.WCargoLeftOnBoard=Cargo;
			this.MaxTOLoad=MaxToLoad;
			this.OffLoadWeight=OffLoadWG;
			this.IsRunning=Running; 
			this.IsRefueling=false; // l'elicottero non parte mai in fase di refueling 
			this.IsBladeSpread=BladeSprd;
			this.IsEfficient=IsEff; 
			this.IsFlying=IsFly;
			this.PosEli=InitialPos; // posizione iniziale Hangar o Ponte
			this.FuelC=FuelC;// consumo di carburante all'ora
			this.SpotAssigned=null; // assegnazione spot inizializzata a null
			this.LowFuel=false;// setto il lowfuel state a falso, alla messa in moto viene ricontrollato.
			this.EliCrash=false;// setta la flagh di crash a falso
			this.IsBlocked = false; // l'elicottero non parte bloccato
			this.isREADYstatus = false; // l'elicottero inizializzato non pronto
			this.IsHolding = false; // l'elicottero è inizializzato non in holding
			this.isLZ = false; // elicottero sulla LZ
			this.hasSpotReserved = false;
			this.rotation = 0; // rotazione iniziale
			this.quota = 0.6f; // quota iniziale
			this.isRequiredForOP = true;// elicottero richiesto per l'operazione di default
			this.IstroopsOnBoard = false; // truppe a bordo
			this.IsCargoOnBoard = false; // l'elicottero parte senza cargo
			this.Go_goback = true; // setto l'andata
			this.IsFull = false; // elicottero parte vuoto
			this.IsCargoFull = false; // cargo full
			this.isTroopsFull =false;  // truppe full
			this.isApproaching = false; // approach to destinaiton
			this.isHotRef = false; // flas hot ref su falso
			this.IsBoardingCargo = false; // sett iniziale imbarco cargo falso
			this.EliSoldierList = new List<Soldier>(0); // ogni elicottero detiene la sua lista di soldati
			this.EliCargoList = new List<Cargo>(0);// ogni elicottero detiene la lista del proprio cargo
			this.DirToGo = Direction.ship; // inizalizzato per direzione holding point
			this.IsBoarding = false; // l'elcittero è iizializzato a caricamento truppe falso


			// il peso totale è dato dal peso dell'elicottero scarico + il peso del carburante + il peso delle truppe + il peso del cargo
			this.TotalWeightCalculated=this.ToTWGCalculus();

			//timer attachment: ogni elicottero e' dotato di un timer
			// serve per sincronizzare le operazioni
			this.timer = timerCon; // definizione del timer
			this.timer.Attach(this); // attachment del timer secondo il pattern Observer
								// l'elicottero entra a far parte della lista di observers
								// il notify() del timer provvederà all'update dei clock di tutti gli observer
		
		
			this.elivector2d = new Vector2 (0f,0f); // creazione del vettore di movimento verrà usato dal framework grafico monogame
													// indica la posizione attuale del velivolo sullo schermo
			this.destination = new Point (0,0); // destinazione dell'elicottero espressa con una struttura point X e Y


			this.EliSpeed = 0f; // viene settato il parametro velocita' iniziale dell'eli


		}

		//OBSERVER PATTERN
		public void Update (Subject subj) // observe pattern, la variabile curren_time viene
										// ridefinita ogni volta che si effettua l'update del timer
		{

			if (subj != null) 
			{
				// UPDATE RICHIAMATO OGNI TICK
				this.current_time=timer.GetTime(); // copia il timer nella variabile timeSpan current_time 

			}

		}


		// metodo di generazione di quota randomico
		// l'elicottero simula una piccola variazione di quota
		// generata graficamente con la variazione dello zoom dello sprite
		public void  RandomQuoteGenrator ()
		{
			if (random.Next (2) == 0) {
				if (quota < 0.8f)
					this.quota = this.quota + 0.005f;
			} else if (quota > 0.55f)
				this.quota = quota - 0.005f;
		}

	

		// setta la flag in blocco per il tempo necessario all'operazione
		public void EliBlock (int ticks, int k)
		{


			bool passFlag = false;


			if (!this.IsBlocked && k!=-1) { // se l'elicottero non è bloccato resetto il timer

				time = new TimeSpan (0, 0, ticks); // BLOCCO PER UN NUMERO DI TICKS
				time = this.timer.GetTime() + time; // tempo massimo di blocco
				this.blockReason = k; // salvo la motivazione di blocco NON POSSO RESETTARE LE MOTIVAZIONI DI 
										// BLOCCO SE L'ELICOTTERO NON E' SBLOCCATO
				this.IsBlocked = true;
				}

			if (time <= timer.GetTime() && this.IsBlocked ) 
				{
				passFlag = true; // se supero il tempo necessario la variabile passa diventa true
				}
				
			if (passFlag) // la passflag sblocchera l'accesso alle casistiche
				{
				// definizione dei blocchi temporali con il segnale che le ha generate
			
					
				if (this.blockReason == 1)  this.IsRunning = true; // messa in moto

				if (this.blockReason  == 2) this.IsBladeSpread = true; // pale aperte

				if (this.blockReason  == 3) // blocco per riposizionamento
					{
						this.PosEli = true;// posizione sullo spot
						this.SpotAssigned = this.Spotrequested;
						this.SpotAssigned.SpotOccupied = true; // spot occupato
						this.SpotAssigned.Eli = this; // assegna lo spot all'elicottero
						this.SpotAssigned.IsReservd = false; // lo spot è occupato non è piu' riservato
						this.hasSpotReserved = false;
						this.Spotrequested = null;
					}

				// SEGNALE 4 ATTESA PER APERTURA PALE
				if (this.blockReason  == 4) this.IsBladeSpread = true;

				// SEGNALE 5 ATTESA PER INSERIMENTO CARGO
				if (this.blockReason == 5) {
					this.IsBoardingCargo = false; // termine del boarding cargo
					this.IsCargoOnBoard = true; // cargo a bordo flag
				}
				// SEGNALE 6 ATTESA PER SCARICO DEL CARGO
				if (this.blockReason == 6) 
				{
					this.IsCargoOnBoard = false; // cargo unloading time
					foreach (Cargo cargo in this.EliCargoList) {	

						cargo.IsEliC = false; // il cargo e' a terra
						cargo.Eli = null;
						cargo.isLand = true;// setto la flag per cargo a terra
						// vettore cargo a terra
						cargo.CargoVector = this.elivector2d + (new Vector2 (StaticSimulator.CargoVectorPosX, StaticSimulator.CargoVectorPosY));
						this.WCargoLeftOnBoard = this.WCargoLeftOnBoard + cargo.ChecktotalCargoWeight ();  // aumento il peso totale destinato al cargo	
						this.LZ.LZCargo.Add(cargo);// aggiungo elementi alla lista cargo presente sulla LZ

					}
					this.EliCargoList.Clear();//
					this.IsCargoFull = false;
					this.IsCargoOnBoard = false;

				}
				
				// SEGNALE 7 ATTESA PER HOT REF
				// due volte la distanza di andata e ritorno dalla LZ + 20 minuti di carburante
				if (this.blockReason  == 7) 
				{
				
					this.Fuel = this.Fuelrequired (MainWindow.Elioptions.EliM.ElicotteriList.Count);
					this.isHotRef = false; // resetto la variabile hot ref e ripristino il
											// normale afflusso dicarburante
				}
					
				// SEGNALE 8 ATTESA PER IMBARCO TRUPPE
				if (this.blockReason == 8) this.IsBoarding = false; // CARICO TRUPPE - alla fine resetto la variabile il boarding a falso

				// SEGNALE 9 ATTESA PER SCARICO TRUPPE
				if (this.blockReason == 9) 
				{
					foreach (Soldier troop in this.EliSoldierList) 
					{
						troop.IsEli = false; // le truppe sono a terra
						troop.IsLand = true;// setto le flags per eli a terra
						// vettore truppa a terra
						troop.vectoreTroop = this.elivector2d + (new Vector2 
							(StaticSimulator.TroopsVectorPosX + StaticSimulator.RandomIntGenerator (10), 
							StaticSimulator.TroopsVectorPosY + StaticSimulator.RandomIntGenerator (10)));

						this.LZ.LZSoldierList.Add(troop); // aggiungo elementi alla lista della LZ
						this.WTroopsLeftOnBoard = this.WTroopsLeftOnBoard + troop.Weight; // aumento il peso totale destinato alle truppe 

					}

					this.EliSoldierList.Clear (); // libera la lista sull' elicottero
					this.IstroopsOnBoard = false; // SCARICO TRUPPE
					 this.isTroopsFull = false; // l'elicottero essendo scarico resetta anche la variabile FULL
				}


				// FINE BLOCCO PER TUTTI I SEGNALI
				this.IsBlocked = false; // sblocco la variabile blocco
			}
		} // termine time 
		


		// calcolo peso truppe a bordo
		private int totTroopOnBoard ()
		{
			if (this.EliSoldierList != null && this.EliSoldierList.Count != 0)
				return (this.EliSoldierList.Count * this.EliSoldierList [0].Weight);
			else return 0;
		}


		// calcolo peso cargo a bordo 
		private int totCargoOnBoard ()
		{
			int part = 0;
			if (this.EliCargoList != null && this.EliCargoList.Count != 0) {
				foreach (Cargo cargo in this.EliCargoList) {
					part = part + cargo.ChecktotalCargoWeight ();
				}
				return part;
				}
			else return 0;
		}


		//calcolo il peso totale dell'elicottero a pieno carico con pieno di truppe e pieno di cargo

		public int ToTWMaxFull ()
		{
			return this.OffLoadWeight + this.WTroopsLeftOnBoard+this.WCargoLeftOnBoard+ (int)this.Fuel;		
		}




		// calcolo del peso totale del velivolo con truppe e carburante a bordo
		public int ToTWGCalculus ()
		{
			return this.OffLoadWeight+this.totTroopOnBoard()+this.totCargoOnBoard ()+ (int)this.Fuel;		
		}

		// calcolo del consumo


		// scrivi le informazioni disponibili sull elicottero corrente
		public string EliInfo ()
		{
			return "Record_eli: "+ this.NumEli +" ID: "+this.IdEli+" PosEli:"+ this.PosEli+ " Cat: "+ this.CatEli+
				" Fuel: "+this.Fuel+ " Spazio truppe: "+this.WTroopsLeftOnBoard+" Spazio Cargo: "+this.WCargoLeftOnBoard
				+" MaxT-O weight: "+this.MaxTOLoad+" OffLoad Weight: "+this.OffLoadWeight+" Consumo: "+this.FuelC;
		}
	
		// CALCOLA IL CARBURANTE NECESSARIO
		// 2 volte la distanza andata e ritorno per raggiungere la LZ
		// calcolata alla minima velocità
		// viene sommato il tempo di attesa ipotetico in holding pari a 1h di volo
		// se il numero di elicotteri aumenta i tempi di attesa potrebbero aumentare 
		// quindi viene sommato un fattore correttivo
		public float Fuelrequired (int _toteliNum)
		{
			int fuelKcorrection = 0;
			if (_toteliNum > 6)
				fuelKcorrection = 100 * (_toteliNum - 6);
			return (((((2 * this.DistancetoRun) / 70) * this.FuelC) + (this.FuelC))+fuelKcorrection);
		}
	}// end class
}// end namespace

