//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// CLASSE DI CONTROLLO PER L'INSERIMENTO DATI TRUPPE
// SI OCCUPA DELL'INSERIMENTO E DEL CONTROLLO DELLE INFORMAZIONI
// SULLE TRUPPE DA IMPIEGARE
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using Gtk;
using System.Windows.Forms;

namespace EliAssaltoAnfibio
{
	public partial class TruppeOptions : Gtk.Window // CLASSE parziale eredita da gtk.windows
	{

		private static TruppeOptions instance = null; // singleton
		static public MainWindow MainWin = MainWindow.Instance();// identifica la finestra principale in
																// modo che possa essere ritrovata alla chiusura di questa
		public TroopsManager TroopM;  // crea il manager delle truppe
		private int _troop=0;	//numero truppe
		public InfoWindow winI; // finestra informazioni
		// al costruttore viene passato il numero di truppe, la lista è ancora vuota
		// e viene allocato lo spazio in memoria necessario
		protected TruppeOptions (int Troop, InfoWindow _winI) : base(Gtk.WindowType.Toplevel)
		{
			this.winI=_winI;
			this._troop = Troop;
			this.Build ();
			// viene creato il manager delle truppe
			// con il factory method
			// l'opzione 1 indica che si tratta di un troops manager
			TroopM = (TroopsManager) FactoryManager.Make(1,Troop, winI); // creazione manager
			this.winI.InsertSomeText("TROOPS MANAGER: effettuata creazione manager per truppe");
			// viene definita l'istanza del numero di elementi richiesti
			TroopM.MakeList (Troop); // creazione della lista truppe
		
		}

		public static TruppeOptions Instance(int TotEli, InfoWindow WinI) // singleton
		{
			if (instance == null)
			instance= new TruppeOptions(TotEli, WinI);
			return instance; // ritorno il costruttore	
		}

		// metodo per mostrare la finestra truppe
		public void ShowWin()
		{	
			this.Visible=true; // finestra visibile
		}
	
		// metodo di chiusura finestra truppe tramite X
	 	protected void OnDeleteEvent (object sender, DeleteEventArgs e)
		{	
			this.Visible=false; // finestra invisibile
			MainWin.Sensitive=true; // la main window puo' essere clikkata
			MainWin.CheckGreenFlags ();// check green flags
			e.RetVal=true; // ret value true evita il destroy  delle informazioni
		}		

		// BOTTONE: viene accettato il peso delle truppe
		// viene creata la lista inserendo in essa le truppe richieste.
		// La lista verra poi gestita in modo da assegnare truppe ed elicotteri
		protected void OnButton1Released (object sender, EventArgs e)
		{
			if (TroopM.TroopList.Count == 0) { // se la lista è vuota  creala
				//inserimento soldati nella lista
				int i;
				for (i=0; i<_troop; i++) 
				{ 
					Soldier _soldier = new Soldier ((int)this.hscale1.Value);
					try{
						TroopM.InsertElement (_soldier);	// inserisco il dato truppa
					}
					catch {
						System.Console.WriteLine ("ERRORE DI INSERIMENTO RECORD");
					}
				}
				TroopM.SingleSoldierW=(int)this.hscale1.Value; // salvo il peso del soldato singolo
				winI.InsertSomeText("OPZIONI TRUPPE: effettuato l'inserimento di "+TroopM.TroopList.Count+ " soldati del peso singolo di: "+ TroopM.SingleSoldierW+"kg");
		} 

			else 
			{
				winI.InsertSomeText("OPZIONI TRUPPE: lista truppe già creata, sostituisco con i nuovi dati");
			
				foreach (Soldier sold in TroopM.TroopList)
				sold.Weight =(int)this.hscale1.Value;// varia il perso di ogni soldato nella lista
				winI.InsertSomeText("OPZIONI TRUPPE: nuovo peso inserito: "+ this.hscale1.Value);
			}

			this.Hide();
			MainWin.Sensitive=true;
			MainWin.CheckGreenFlags ();// check green flags
			}

	} // fine classe
} // FINE name space

