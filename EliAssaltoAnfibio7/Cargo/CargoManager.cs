
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// classe manager per la creazione del cargo ed il controllo degli stati 
// detiene la lista cargo e le operazioni su di essi
// classe singleton
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


using System;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{
	public class CargoManager : AbstractManager
	{

		public static CargoManager instance = null;
		private int _numCargo;// indica il numero di record iniziali
		public List<Cargo> CargoList; // definizione della lista di elicotteri ( pubblica )
		public StatusManager Status;// definizioni dello stato del manager 
		public ActionManager Action; //definizioni delle azioni svolte
		public const int CargoLoadTiming = 5; // tempo per operazioni di cargo load
		public const int CargoTimeTodisembark = 4; // tempo richiesto per disimbarcare il cargo
		public InfoWindow WinI;


		public CargoManager(int numCargo, InfoWindow winI) : base("Manager_per_Cargo")
		{
			this.WinI = winI;
			this._numCargo=numCargo;
			this.Status=StatusManager.Empty;  // setta lo stato come vuoto 
			this.Action=ActionManager.Wait; // le azioni sono in attesa di informazioni aggiuntive

		}

		// istanza singleton è prevista l'esistenza di un'unica finestra
		public static CargoManager Instance(int numCargo, InfoWindow winI)
		{
			if (instance == null) instance= new CargoManager( numCargo ,  winI );
			return instance; // ritorno il costruttore	
		}

		// definizione dei metodi comuni alle classi in OVERRIDING
		
		public override void MakeList(int i ) 
		{
			this.CargoList = new List<Cargo>(i);	
		}

		// METODI INSERIMENTO - RIMOZIONE LISTA
		// inserisci un elemento i nella lista
		public override void InsertElement(Object i)
		{
				this.CargoList.Add ((Cargo)i);
		}

		
		//rimuovi un elemento dalla lista REMOVEAT INDEX
		public override  void RemoveElement(int i)
		{
			this.CargoList.RemoveAt (i);
		}


		// inserisci elemento nella lista
		public  void AddElementTolist (List<Cargo> lstobj, Cargo obj)
		{

			lstobj.Add (obj);

		}
		// elimina elemento dalla lista
		public  void RemoveElementTolist (List<Cargo> lstobj, Cargo obj)
		{
			lstobj.Remove (obj);

		}


		public  void InsertElementCargo (int cargoRec ,string cargoType, int cargoW , int cargoP ) 
		{

		     	Cargo cargoN = new Cargo( cargoRec ,cargoType, cargoW , cargoP ); // creazione del nuovo record spot
				CargoList.Insert ( (cargoRec), cargoN);
		}
	
		// movimentazione del cargo ( sorgente , destinazione , cargo)
		public  void MoveElement (List<Cargo> list1src, List<Cargo> list2dst, Cargo cargo)
		{
			list2dst.Add (cargo);// aggiungo l'elicottero alla seconda lista
			list1src.Remove(cargo); // sottraggo l'elicottero alla lista iniziale 
		}
		// FINE METODI INSERIMENTO RIMOZIONE



		// ----------------------------------------------------------------------------------------------
		// list disposer methods
		// distribuzione del cargo sull'elicottero che lo puo' ospitare
		// il metodo serve per distribuire il cargo a bordo degli elicotteri
		// che possono effettuare il trasporto
		// blocca temporalmente l'elicottero che effettua il trasferimento di cargo
		public void CargoDistribution (InfoWindow winI , EliManager EliM , TroopsManager TroopM)
		{
			// variaibli d'impiego locale
			int i = 0; // index
			int maxCargoNum = this.CargoList.Count; // quanti pacchi di cargo devo distribuire
			Cargo cargo = null; 
			Elicottero eli = null;

			if (this.CargoList != null && this.CargoList.Count!=0) {// controlla che la lista cargo contenga elementi


				// è stato usato un indice per l'impossibilità di operare sulla lista dinamicamente con un foreach
				// cancellando i dati e leggendoli insieme.
				while ((i < this.CargoList.Count)) {// continua fino a quando l'indice raggiunge il valore massimo di elementi
					cargo = this.CargoList [i];// cargo da assegnare



					if (!cargo.IsEliC   // il cargo vede essere caricabile su uno degli elicotteri
						&& cargo.IsFittable)  // controlla se il cargo non è ancora stato assegnato
					{  
						eli = EliM.EliAbleToCargo (cargo, TroopM);// definisce l'elicottero in grado di ospitare il cargo
						//l'elicottero per ospitare il cargo non deve essere necessariamente sul ponte di volo


						// controlla che l'elicottero assegnato esista e se esistono le condizioni per poter accettare il cargo
						if (eli != null && !eli.IsCargoFull // elicottero non vede essere cargo full
							&& !eli.IsFlying // non deve essere in volo
							&& !eli.IsBlocked // no deve essere time blocked
							&& eli.IsEfficient  // deve essere efficiente
							&& eli.isRequiredForOP)  // deve essere richiesto per l'operazione
						{

							eli.IsBoardingCargo = true; // eli inizia ad imbarcare il cargo

							eli.WCargoLeftOnBoard = (eli.WCargoLeftOnBoard - cargo.ChecktotalCargoWeight());// sottraggo il peso del cargo al carico utile dell'eli
							// tale peso comprende le truppe che fanno parte del cargo

							this.MoveElement (this.CargoList, eli.EliCargoList, cargo); // effettua lo spostamento necessario setto la flag del cargo a bordo

							this.CheckThisEliCargoFull (eli); // controlla se esistono altri cargo che possono essere 
							//inseriti altrimenti CARGO FULL set flag

							cargo.IsEliC = true; // la flag del cargo viene settata in elicottero
							cargo.Eli = eli;// assegno al cargo l'elicottero su cui viaggia

							eli.EliBlock (CargoManager.CargoLoadTiming,5 ); // blocco l'elicottero N ticks per operazioni di cargo
							// al termine dell'attesa IsCargoOnBoard variable e' TRUE

							winI.InsertSomeText ("CARGO DISPOSER: è stato assegnato: " + cargo.CargoString +
								" , restano ancora " + this.CargoList.Count + " elementi CARGO da imbarcare");	
							i++; // passo al rec ord successivo
						}
						else i++;// incremento il contatore del cargo, cambio cargo

					}
				}// fine while
			} else
				winI.InsertSomeText ("TROOPS DISPOSER: nessuna lista di CARGO è stata definita");

		}// NotFiniteNumberException metodo distribuzione cargo

		// il metodo controlla che esista ancora un cargo che possa essere inserto in elicottero
		// il metodo va riproposto dopo ogni inserimento di cargo
		// nonchè dopo ogni scarico
		public void CheckEli_CargoFull (EliManager EliM)
		{
			foreach (Elicottero eli in EliM.ElicotteriList) 
			{

				eli.IsCargoFull = true;

				if (eli.isRequiredForOP && eli.IsEfficient ) // se l'elicottero è efficiente e richiesto per l'op
					foreach (Cargo cargo in this.CargoList)  // controlla che esista ancora un cargo utile
					{
						if ( (cargo.ChecktotalCargoWeight() < eli.WCargoLeftOnBoard ) && !cargo.IsEliC )
							eli.IsCargoFull = false;
					}
			}
			this.CheckState (); // aggiorno lo stato
		}

		// controllo il singolo Eli per verificare se è possibile impiegarlo per altri carghi
		public void CheckThisEliCargoFull (Elicottero eli)
		{
			eli.IsCargoFull = true;

			foreach (Cargo cargo in this.CargoList) 
			{
				if ( (cargo.ChecktotalCargoWeight() < eli.WCargoLeftOnBoard ) && !cargo.IsEliC )
					eli.IsCargoFull = false;
			}

		}


		// controllo che ogni cargo sia trasportabile da almeno 1 elicottero
		// altrimenti il cargo è unfittable
		public void CheckCargoIsFittable (EliManager EliM)
		{
			foreach (Cargo cargo in this.CargoList) // per ogni cargo
			{
				foreach (Elicottero eli in EliM.ElicotteriList) {  // per ogni elicottero

					// nota le truppe sono inserite come CARGO e quindi fanno parte della pesata totale del cargo
					// la classe cargo è infatti composta da una lista di truppe assegnate aventi come peso 
					// il peso basico del soldato inserio per i soldati
					if (eli.WCargoLeftOnBoard > cargo.ChecktotalCargoWeight ()) 
					{
						cargo.IsFittable = true; // switch cargo flag
						return;
					}
				}
				cargo.IsFittable = false;
			}
		}


		// stato lista elicotteri
		public override void CheckState ()
		{
			int cargoNotFill = this.CargoList.FindAll (cargo => cargo.IsFittable == false).Count; // conta il numero di cargo che non puo' essere imbarcato
			int cargoONboard = this.CargoList.FindAll (cargo => cargo.IsEliC == true).Count; // conta il numero di cargo gia' imbarcato
			int totalCargo = this.CargoList.Count;

			// se ho N carghi di cui K sono gia' a bordo e I sono inutlizzabili
			// se N==0 l'hangar è vuoto di cargo
			if ((totalCargo-cargoONboard-cargoNotFill)== 0) { // se il cargo in hangar è 0
					this.Status = StatusManager.Empty;// lo stato è vuoto
					this.Action = ActionManager.ReadyToReceive;// l'hangar è pronto a ricevere
				}
			// se N è maggiore di 0 l'hangar ha ancora del Cargo da movimentare
			if ((totalCargo-cargoONboard-cargoNotFill)> 0) {// se il cargo in hangar è maggiore di 0
					this.Status = StatusManager.CanAll;// lo stato è promiscuo, il restante cargo deve ancora essere distribuito
					this.Action = ActionManager.ReadyToSendAndReceive; // è possibile qualcunque azione di invio o ricezione sul ponte
				}
				
		}

		// eliste almeno un cargo in grado di trasportare le truppe sull'elicottero
		// restituisce vero se esiste almeno un cargo che è possibile caricare sull'elicottero
		public bool CheckCargoLoadOnEli (Elicottero eli, int singleSoldW)
		{
			foreach (Cargo cargo in this.CargoList)
				if ((cargo.CargoW <= eli.WCargoLeftOnBoard ) && ( cargo.CargoP*singleSoldW <= eli.WTroopsLeftOnBoard))
				{
					return true ;
				}
				return false;
		}

	}// fine classe
}// fine name space

