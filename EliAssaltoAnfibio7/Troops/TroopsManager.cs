//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// classe manager per la creazione truppe ed il controllo degli stati 
// detiene la lista truppe e le operazioni su di essi
// classe singleton
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EliAssaltoAnfibio
{
	public class TroopsManager : AbstractManager
	{
		public static  TroopsManager instance= null;

		public List<Soldier> TroopList{set;get;} // definizione della lista di elicotteri ( pubblica )
		public StatusManager Status;// definizioni dello stato del manager 
		public int TotalTroops{set;get;}// truppe totali
		public ActionManager Action; //definizioni delle azioni svolte
		public int SingleSoldierW {set;get;} // indica il peso del soldato singolo
		public InfoWindow WinI; // finestra informativa
		public Texture2D Troopstexture; // truppe shape

		// contruttore
		public TroopsManager (int Element, InfoWindow winI):base("Manager_per_Truppe")
		{
			this.WinI = winI;
			this.TotalTroops=Element; // inizializzo gli elementi

		}


		// istanza singleton è prevista l'esistenza di un'unica finestra
		public static TroopsManager Instance(int Element, InfoWindow winI)
		{
			if (instance == null) instance= new TroopsManager( Element ,  winI );
			return instance; // ritorno il costruttore	
		}


		// crea la lista fatta di N elementi 
		public override void MakeList(int element)		
		{	
			this.TroopList =new List<Soldier>(element);	
		}

		// inserisci un elemento nella lista
		public override void InsertElement (Object troop)
			
		{
			this.TroopList.Add ((Soldier) troop);
		}
			
		//rimuovi un elemento dalla lista REMOVEAT INDEX
		public override  void RemoveElement (int i)
			
		{
			this.TroopList.RemoveAt (i);
		}


		// il meteodo derve per spostare un elemento di truppa su un elicottero
		// ogni elicottero deve quindi possedere la propria lista di soldati
		// ogni elicottero deve possedere la propria lista cargo
		public  void MoveElement (List<Soldier> list1src, List<Soldier> list2dst, Soldier troop)
		{
			list2dst.Add (troop);// aggiungo l'elicottero alla seconda lista
			list1src.Remove(troop); // sottraggo l'elicottero alla lista iniziale
		}


		// controllo stato manager override
		// EMPTY non ci sono piu' truppe
		// CanAll ci sono ancora truppe da sbarcare
		public override void CheckState ()// controllo stato lista manager
		{

			if (this.TroopList.Count== 0) { // se le truppe in hangar sono 0
					this.Status = StatusManager.Empty;// lo stato è vuoto
					this.Action = ActionManager.ReadyToReceive;// l'hangar è pronto a ricevere
				}

			if (this.TroopList.Count > 0) {// se le truppe in hangar sono maggiori di 0
					this.Status = StatusManager.CanAll;// lo stato è promiscuo
					this.Action = ActionManager.ReadyToSendAndReceive; // è possibile qualcunque azione di invio o ricezione sul ponte
				}
		}

		// LIST DISPOSER MODULES
		// - controllo il numero di spot occupati  
		// - calcolo lo spazio totate sugli eli
		// - distribuisco le truppe equamente sugli elicotteri
		public void EliSpotTroopDistribution (SpotManager SpotM , TroopsManager TroopsM , InfoWindow winI)

		{
			int totWEli=SpotM.TotalWEliSpotForTroopS(); // calcolo il peso totale disponibile dagli elicotteri sugli spot
			int totWTroops = TroopsM.TroopList.Count*TroopsM.SingleSoldierW; // calcolo il peso totale delle truppe da imbarcare
			int w_Embarcable=0;// peso imbarcabile corrispondente al minore tra il peso totale disponibile ed il peso totale truppe
			int troopsE=0;// truppe imbarcabili
			int i=0; // variabile indice

			if (totWTroops<totWEli) w_Embarcable=totWTroops; else w_Embarcable=totWEli; // se il peso totale dell truppe da imbarcare è
			// minore rispetto al peso totale disponibile sugli elicotteri
			// allora le truppe imbarcabili saranno il minimo tra i due valori
			troopsE=w_Embarcable/TroopsM.SingleSoldierW;//trasformo il peso imbarcabile in uomini da imbarcare
			winI.InsertSomeText("TROOPS DISPOSER: ponte di volo occupato da: "+SpotM.TotalElionDeck()
				 + " Elicotteri sul ponte pronti per l'imbarco di "+troopsE +" truppe");

			//trasferisco ogno uomo su ogni elicottero disponibile con una distribuzione alternata
			//su tutti gli elicotteri disponibili, la distribuzione di reitera fino a  a quando tutti gli uomini 
			// sono stati spostati sugli elicotteri.
			if (troopsE>0) // se esistono truppe da imbarcare
				while ( i<troopsE) // continua fino a quando l'indice è inferiore alle truppe 
				{
					foreach (Spot spot in SpotM.Spotlist) // per ogni spot 
					{	// se spot occupato da un elicottero in moto e pale aperte e l'elicottero puo' ospitare le truppe, allora effettuo il carico
						if (spot != null && spot.Eli != null 
							&& !spot.Eli.IsBlocked  // eli non bloccato
							&&!spot.Eli.IsFull  // eli non pieno di cargo e truppe
							&& !spot.Eli.isTroopsFull // eli non pieno di truppe
							&& spot.Eli.IsRunning  // eli in moto
							&& spot.Eli.IsBladeSpread  // eli a pale a perte
							&& (TroopsM.TroopList.Count>0) // eli con truppe a bordo
							&& (spot.Eli.WTroopsLeftOnBoard >= TroopsM.SingleSoldierW)) {// truppe accettabili superano il peso del singolo


							// scambio di lista,  simulo che la truppa imbarca sull'elicottero sistemato sul ponte, la lista verra poi passata alla landing zone 
							// per permettere lo sbarco della truppa

							TroopsM.TroopList [TroopsM.TroopList.Count - 1].IsEli = true; // imposto la truppa in elicottero
							spot.Eli.IstroopsOnBoard = true; // elicottero ha truppe a bordo
							try{
							// sposto la truppa dalla lista truppe a bordo
							TroopsM.MoveElement (TroopsM.TroopList, spot.Eli.EliSoldierList, TroopsM.TroopList [(TroopsM.TroopList.Count) - 1]);
							}
							catch{
								System.Console.WriteLine ("ERRORE SPOSTAMENTO TRUPPE");
							}																												// alla lista truppe sull'elicottero

							spot.Eli.IsBoarding = true; // l'elicottero sta imbarcando truppe
							// diminuisco il peso totale disponibile sull'eli
							spot.Eli.WTroopsLeftOnBoard = (spot.Eli.WTroopsLeftOnBoard - TroopsM.SingleSoldierW);
							if (spot.Eli.WTroopsLeftOnBoard < TroopsM.SingleSoldierW)
								spot.Eli.isTroopsFull = true; // modifico la flag di full truppe
							if (spot.Eli.isTroopsFull && spot.Eli.IsCargoFull)
								spot.Eli.IsFull = true;; // modifico la flag di full cargo e truppe
						
							i++; // VAR INDICE,indica il numero di truppe spostate
						}}}// fine metodo

			// inserisco attesa elicotteri imbarco truppe se la variaibile is boarding is TRUE
			foreach (Spot spot in SpotM.Spotlist) { // per ogni spot 

				if (spot.Eli != null && spot.Eli.IsBoarding) // gestione tempi attesa
					spot.Eli.EliBlock (StaticSimulator.timeToEmbarcTroops, 8); // blocco l'eli e resetto la variabile isBoarding to false
				}
		} // fine metodo

		// controlla se l'elicottero puo' ospitare il numero di truppe indicato 
		// restituisce vero se l'operazione è fattibile falso se invece l'operazione non è fattibile
		public   bool EliTroopsCheck (int troopsN, Elicottero eli , TroopsManager TroopsM)
		{
			int k; // var indice
			int totW = 0;//peso totale delle truppe

			if (TroopsM.TroopList.Count >= troopsN) {// controllo se ho le truppe che servono
				for (k=0; k< troopsN; k++)
					totW = totW + TroopsM.SingleSoldierW;// controllo il peso totale necessario
				if (totW < eli.WTroopsLeftOnBoard)	return true; // risultato
			}return false;
		} // fine metodo


	}// fine classe
}// finr name space