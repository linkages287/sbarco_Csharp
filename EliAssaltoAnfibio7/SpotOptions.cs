//++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// FINESTRA VISUALIZZAZIONE OPZIONI SULLO SPOT
// PERMETTE L'INSERIMENTO DEI VALORI RICHIESTI PER OGNI SPOT 
// I VALORI SONO DEFINITI IN BASE ALL'EFFICIENZA DELLO SPOT
// ED ALLA CLASSE DI UTILIZZO DELLO SPOT QUALI:
// LEGGERA - PESANTE - ALTRO.
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++
using System;
using Gtk;
using System.Windows.Forms;

namespace EliAssaltoAnfibio
{
	public partial class SpotOptions : Gtk.Window
	{

		private static SpotOptions instance = null;
	    static public MainWindow MainWin = MainWindow.Instance();// identifica la finestra principale in modo che 
																//possa essere ritrovata alla chiusura di questa

		public SpotManager SpotM; // definizione lo spot manager
		public int _spotRec=0; // numero di record per gli spot
		private bool _isEfficient=true;
		private int _totSpot=0;

		public InfoWindow WinI;
		private bool cat1D=false;
		private bool cat2D=false;
		private bool cat3D=false;
		private bool cat1N=false;
		private bool cat2N=false;
		private bool cat3N=false;


		// COSTRUTTORE DELLA CLASSE SPOT OPTION 
		protected SpotOptions (int spot , InfoWindow _win) :  base(Gtk.WindowType.Toplevel)
		{
			this.WinI = InfoWindow.Instance (); // gli passo la finestra informazioni
			_totSpot=spot;
			this._spotRec = 0;
			Build ();
			WinI.InsertSomeText("OPZIONI SPOT: EFFETTUATA APERTURA FINESTRA OPZIONI SPOT");
			SpotM = (SpotManager) FactoryManager.Make(2,spot, WinI); // viene creato lo Spot Manager tramite factory method
			SpotM.MakeList(spot); // crea la  lista iniziale di un numero di spot impostato
			this.label1.Text=("Numero di Record SPOT #  "+ this._spotRec);
			this.InitValue();
			this.label1.Text = "Record # " + _spotRec + " Record inseriti: "+SpotM.Spotlist.Count+ 
			                   " Record disponibli: "+ this._totSpot;
		}

		public static SpotOptions Instance(int TotSpot, InfoWindow winI)
		
		{	
			// crea il costruttore se non esiste
			if (instance == null) instance= new SpotOptions(TotSpot, winI); 
			return instance; // ritorno il costruttore	
		}		


		private void InitValue ()
		{
			// Inizializza variabili e checkbox per un nuovo inserimento
			_isEfficient=true;
 			cat1D=false;
		 	cat2D=false;
		 	cat3D=false;
			cat1N=false;
		 	cat2N=false;
		 	cat3N=false;
			this.checkbutton7.Active = false;
			this.checkbutton1.Active =false;
			this.checkbutton2.Active=false;
			this.checkbutton3.Active=false;
			this.checkbutton4.Active=false;
			this.checkbutton5.Active=false;
			this.checkbutton6.Active=false;
			this.label1.Text = "Record # " + (_spotRec+1) + " Record inseriti: "+SpotM.Spotlist.Count+ 
			                   " Record disponibli: "+ this._totSpot;
		}

		public void ShowRecord (int i)
		{
			this.checkbutton7.Active =SpotM.Spotlist[i].SpotRunnable ;
			this.checkbutton1.Active =SpotM.Spotlist[i].DayCanAccept.Cat1;
			this.checkbutton2.Active=SpotM.Spotlist[i].DayCanAccept.Cat2 ;
			this.checkbutton3.Active=SpotM.Spotlist[i].DayCanAccept.Cat3 ;
			this.checkbutton4.Active=SpotM.Spotlist[i].NightCanAccept.Cat1;
			this.checkbutton5.Active=SpotM.Spotlist[i].NightCanAccept.Cat2 ;
			this.checkbutton6.Active=SpotM.Spotlist[i].NightCanAccept.Cat3;
			this.label1.Text = "Record # " + (_spotRec+1) + " Record inseriti: "+
			                   SpotM.Spotlist.Count+ " Record disponibli: "+ this._totSpot;
		}


		private void UpdateRec (int i)
		{

			SpotM.Spotlist[i].SpotRunnable = this.checkbutton7.Active ;
			SpotM.Spotlist[i].DayCanAccept.Cat1 =this.checkbutton1.Active;
			SpotM.Spotlist[i].DayCanAccept.Cat2 = this.checkbutton2.Active;
			SpotM.Spotlist[i].DayCanAccept.Cat3 =	this.checkbutton3.Active;
			SpotM.Spotlist[i].NightCanAccept.Cat1 = this.checkbutton4.Active;
			SpotM.Spotlist[i].NightCanAccept.Cat2 =	this.checkbutton5.Active;
			SpotM.Spotlist[i].NightCanAccept.Cat3 =this.checkbutton6.Active;
			WinI.InsertSomeText(" OPZIONI SPOT: record # "+(i+1)+ " AGGIORNATO..");
		}

		
		// Mostra la finestra secondaria e nascondi il main
		public void ShowWin()
		{	
			MainWin.Sensitive=false;
			this.Visible=true;
		}
 		
		// tasto di chiusura finestra
		protected void OnDeleteEvent (object sender, DeleteEventArgs e)
		{	
			if ((_spotRec >= SpotM.Spotlist.Count) && (SpotM.Spotlist.Count>0))// effettuo il check di out bound
				_spotRec--;

			if (SpotM.Spotlist.Count==0) _spotRec=0;

			this.Hide (); // nascondo la finestra

			MainWin.Sensitive=true; // resetto la sensibilità della finestra principale
			MainWin.CheckGreenFlags ();// check green flags
			e.RetVal=true;
		}	


		// bottone: accetta dati e torna al menu principale
		protected void OnButton4Released (object sender, EventArgs e)
		{
			if ((_spotRec >= SpotM.Spotlist.Count) && (SpotM.Spotlist.Count>0)) // effettuo il check di out bound
				_spotRec--;

			if (SpotM.Spotlist.Count==0) _spotRec=0;

			this.Hide (); // nascondo la finestra

			MainWin.Sensitive=true; // resetto la sensibilità della finestra principale
			MainWin.CheckGreenFlags ();// check green flags
		}		

		// bottone : record precedente
		protected void OnButton2Released (object sender, EventArgs e)
		{
			if (SpotM.Spotlist.Count > 0) // controlla se esistono dati da leggere
			{
				if (this._spotRec > 0) 
				{ 
					this._spotRec--; // decrementa il numero di record per la visualizzazione
					this.ShowRecord(_spotRec); // ShowWin il record
				} 
				if (this._spotRec==0)
				{
					WinI.InsertSomeText ("OPZIONI ELICOTTERO: raggiunto record iniziale.");
					this.ShowRecord(_spotRec);
				}

			} if (SpotM.Spotlist.Count == 0)
				_spotRec = 0;
		}	

		// bottone : record successivo
		protected void OnButton3Released (object sender, EventArgs e)
		{
			// effettuo i controlli
			if ((SpotM.Spotlist.Count > 0) // record presenti
				&& (_spotRec<SpotM.Spotlist.Count) // il numero di record minore del massimo recordinseriti
				&& (_spotRec<(_totSpot))) //il numero di record minore del massimo di rec inseribili
			{
				if (this._spotRec == (SpotM.Spotlist.Count - 1) && (SpotM.Spotlist.Count==_totSpot)) {  // raggiunto ultimo record

					WinI.InsertSomeText ("OPZIONI ELICOTTERO: raggiunto record finale");		

				} else if (_spotRec==SpotM.Spotlist.Count-1 
							&& SpotM.Spotlist.Count < _totSpot) 
				{// controlla se esistono dati da leggere
					this._spotRec++;
					this.InitValue ();// se non esistono mostra dati vuoti
				}
				else{
					this._spotRec++; // incrementa il record
					this.ShowRecord (_spotRec); // mostra il record
				}
			}
		} // fine metodo


			// bottone:  salva record corrente
				protected void OnButton5Released (object sender, EventArgs e)
				{
					// elenco dati da salvare nella lista
					cat1D = this.checkbutton1.Active;
					cat2D = this.checkbutton2.Active;
					cat3D = this.checkbutton3.Active;
					cat1N = this.checkbutton4.Active;
					cat2N = this.checkbutton5.Active;
					cat3N = this.checkbutton6.Active;
					_isEfficient = this.checkbutton7.Active;
					//inserisco il nuovo spot nella lista e stampo l'elenco
				//-------------------------------------------------------------------
			if (SpotM.Spotlist.Count <= _totSpot // se il numero di record inseriti è minore del totale inseribile
				&& _spotRec < _totSpot  // se il record che dobbiamo inserire è minore del totale inseribile
				&& _spotRec >= SpotM.Spotlist.Count) // se il record che dobbiamo inserire è maggiore del totale inserito
				{ // SE IL RECORD NON ESISTE: INSERIMENTO NUOVO RECORD
					try
				{ // provo a inserire il dato spot
					SpotM.InsertElementSpot (_spotRec, _isEfficient, cat1D, cat2D, cat3D, cat1N, cat2N, cat3N);
					}
					catch 
					{
						System.Console.WriteLine ("ERRORE NELL'INSERIMENTO DEL RECORD CARGO"); // ERRORE INSERIMENTO
					}
					// visualizzo e salvo le informazioni sul record------------------------------------------
				WinI.InsertSomeText ("OPZIONI SPOT: inserto nuovo record#__" + (this._spotRec + 1) + " Elementi totali presenti: " + SpotM.Spotlist.Count);
				WinI.InsertSomeText (SpotM.Spotlist [_spotRec].SpotInfo ()); // visualizzo le info nella finestra informazioni

					// controllo se ultimo record e chiudo oppure se ci sono altri record allora inizializzo
				if ((_spotRec == SpotM.Spotlist.Count - 1) && (SpotM.Spotlist.Count == _totSpot)) {	
						this.Hide (); // nascondo la finestra
						MainWin.Sensitive = true; // resetto la sensibilità della finestra principale
						MainWin.CheckGreenFlags ();// check green flags
					} else 
						// se non è l'ultimo inizializzo i valori per il prossimo inserimento
					{
					this._spotRec++;// incrementa il record
						this.InitValue ();// reinizializza valori a 0 per il nuovo inserimento
					}

				}// fine inserimento nuovo record

				else // controllo se si tratta di un UPDATE
				if (SpotM.Spotlist.Count <= _totSpot // se il numero di valori inseriti è minore al totale inseribile
					&& _spotRec < _totSpot // se il numero di record da trattare è inferiore al totale
					&& _spotRec < SpotM.Spotlist.Count // se il numero di record da trattare è inferiore al totale inserito
					) { 
						// SE IL RECORD ESISTE E' UN UPDATE 
						//effettuo l'update del record
					this.UpdateRec (_spotRec);
					} 
				
		} // termine inserimento record o update
		
		//bottone: elimina record
		protected void OnButton6Released (object sender, EventArgs e)
		{
			if (SpotM.Spotlist.Count > 0) {

				bool flag = false;
				if (this._spotRec < SpotM.Spotlist.Count) {
					SpotM.RemoveElement (_spotRec); // elimina il record
					// se dopo l'eliminazione il record è 0
					if ((SpotM.Spotlist.Count) == 0) { // se dopo la rimozione non ci sono piu' record
						_spotRec = 0; // setta il rec a 0
						this.InitValue ();// inizializza valori
						flag = true; // flag di elemento eliminato
					} else {
						if ((SpotM.Spotlist.Count == _spotRec)) { // se il mio record è l'ultimo della lista
							_spotRec = SpotM.Spotlist.Count - 1;
						} 
						this.ShowRecord (_spotRec);
						flag = true;
					}
					if (flag)
						WinI.InsertSomeText ("OPZIONI SPOT: rimosso record # " + (_spotRec + 1));
					else
						this.InitValue (); 
				}
			}
		} // fine metodo
	} // fine classe
}// fine namespace
