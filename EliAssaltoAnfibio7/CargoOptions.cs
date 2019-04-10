// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// CARGO OPTIONS
// la classe si occupa dell'inserimento dei dati cargo
// all'interno della lista definita nel cargo manager
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


using System;
using Gtk;
using System.Windows.Forms;

namespace EliAssaltoAnfibio
{
	public partial class CargoOptions : Gtk.Window
	{
		private static  CargoOptions instance= null;
		private InfoWindow WinI;//link alla finestra informazioni
		private int cargoTot=0; // cargo totale
		private string _cargoSTR=null; // nome del cargo
		private int _cargoW=0; // peso totale del cargo
		private int _cargoP=0; // personale necessario al trasporto del cargo
		public int cargoRec=0; // numero di record del CARGO visualizzato o inserito
		public CargoManager CargoM;// definizione per la creazione del cargomanager
		static private MainWindow MainWin = MainWindow.Instance();// identifica la finestra principale in modo che possa essere ritrovata alla chiusura di questa


		protected CargoOptions (int _cargo,  InfoWindow _win) :  base(Gtk.WindowType.Toplevel)
		{

			this.WinI=_win; // gli passo la finestra informazioni
			this.cargoTot=_cargo;
			WinI.InsertSomeText("OPZIONI CARGO: --> EFFETTUATA APERTURA FINESTRA OPZIONI CARGO <--");
			CargoM = (CargoManager) FactoryManager.Make(3,this.cargoTot, this.WinI); // viene creato lo Spot Manager tramite factory method
			CargoM.MakeList(cargoTot); // crea la  lista iniziale di un numero di spot impostato
			this.Build ();
			this.label1.Text= "Numero di Record cargo #  "+ (this.cargoRec+1) +" Record inseriti: "+ CargoM.CargoList.Count + " Record disponibili: "+this.cargoTot;
			this.InitValue();
		}
	
		// istanza singleton è prevista l'esistenza di un'unica finestra
		public static CargoOptions Instance(int TotCargo , InfoWindow winI)
		{
			if (instance == null) instance= new CargoOptions( TotCargo ,  winI );
			return instance; // ritorno il costruttore	
		}

		// ShowWin la finestra corrente e nasconde la principlae 
		public void ShowWin()
		{	
			MainWin.Sensitive=false;
			this.Visible=true;;
			this.label1.Text= "Numero di Record cargo #  "+ (this.cargoRec+1) +" Record inseriti: "+ CargoM.CargoList.Count + " Record disponibili: "+this.cargoTot;

		}

		// ShowWin la finestra corrente e nasconde la principale DELETE EVENT ( richiamato dalla chiusura con la X )
		protected void OnDeleteEvent (object sender, DeleteEventArgs e)
		{	

			if ((cargoRec >= CargoM.CargoList.Count) && ( CargoM.CargoList.Count>0)) // effettuo il check di out bound
				cargoRec--;
			if (CargoM.CargoList.Count == 0)
				cargoRec = 0;
			this.Hide (); // nascondo la finestra
			MainWin.Sensitive=true; // resetto la sensibilità della finestra principale
			MainWin.CheckGreenFlags ();// check green flags
			e.RetVal=true;
		}		
		
		// Metodo di inizializzazione valori cargo a 0
		private void InitValue ()
		{
			this.label1.Text= "Numero di Record cargo #  "+ (this.cargoRec+1) +" Record inseriti: "+ CargoM.CargoList.Count + " Record disponibili: "+this.cargoTot;
			this.entry1.Text="";
			this.hscale1.Value=0;
			this.hscale2.Value=0;
			this._cargoSTR=" DA INSERIRE ";
			this._cargoW=0;
			this._cargoP=0;
		}

		// update record
		private void UpdateRec (int i)
		{
			this._cargoSTR= this.entry1.Text;
			CargoM.CargoList[i].CargoString = this._cargoSTR ;

			this._cargoW = (int)this.hscale1.Value;
			CargoM.CargoList[i].CargoW =this._cargoW ;

			this._cargoP = (int)this.hscale2.Value;
			CargoM.CargoList[i].CargoP =this._cargoP ;

			WinI.InsertSomeText(" OPZIONI CARGO: record # "+(i+1)+ " AGGIORNATO..");
		}
		
		// BOTTONE: accetta dati - torna al menu principale
		protected void OnButton2Released (object sender, EventArgs e)
		{
			if ((cargoRec >= CargoM.CargoList.Count) && (CargoM.CargoList.Count > 0)) // effettuo il check di out bound
				cargoRec--;
			if (CargoM.CargoList.Count == 0)
				cargoRec = 0;

			this.Hide (); // nascondo la finestra CARGO

			MainWin.Sensitive=true; // resetto la sensibilità della finestra principale
			MainWin.CheckGreenFlags ();// check green flags della finestra principale

		}		

		// BOTTONE: salva record e resetta
		protected void OnButton3Released (object sender, EventArgs e)
		{

			this._cargoSTR = this.entry1.Text; // valore stringa cargo NOME cargo
			this._cargoW = (int)this.hscale1.Value; // peso cargo
			this._cargoP = (int)this.hscale2.Value; // personale per cargo
			if (CargoM.CargoList.Count <= this.cargoTot) { // se il numero di record cargo è dentro il range
															// effettua inserimento o update

				if (checkDataConsistency ()) { // CHECK CONSISTENZA DATI// controllo prima la consistenza dei dati inseriti

					//-------------------------------------------------------------------
					if (CargoM.CargoList.Count <= cargoTot // se il numero di record inseriti è minore del totale inseribile
						&& cargoRec < cargoTot  // se il record che dobbiamo inserire è minore del totale inseribile
						&& cargoRec >= CargoM.CargoList.Count) // se il record che dobbiamo inserire è maggiore del totale inserito
					{ // SE IL RECORD NON ESISTE: INSERIMENTO NUOVO RECORD
						try
						{ // provo a inserire il dato cargo
						CargoM.InsertElementCargo (cargoRec, this._cargoSTR, this._cargoW, this._cargoP); // inserisci il nuovo elemento all'interno della lista cargo
						}
						catch 
						{
							System.Console.WriteLine ("ERRORE NELL'INSERIMENTO DEL RECORD CARGO"); // ERRORE INSERIMENTO
						}
						// visualizzo e salvo le informazioni sul record------------------------------------------
						WinI.InsertSomeText ("OPZIONI CARGO: inserto nuovo record#__" + (this.cargoRec + 1) + " Elementi totali presenti: " + CargoM.CargoList.Count);
						WinI.InsertSomeText (CargoM.CargoList [cargoRec].CargoInfo ()); // visualizzo le info nella finestra informazioni

						// controllo se ultimo record e chiudo oppure se ci sono altri record allora inizializzo
						if ((cargoRec == CargoM.CargoList.Count - 1) && (CargoM.CargoList.Count == cargoTot)) {	
							this.Hide (); // nascondo la finestra
							MainWin.Sensitive = true; // resetto la sensibilità della finestra principale
							MainWin.CheckGreenFlags ();// check green flags
						} else 
							// se non è l'ultimo inizializzo i valori per il prossimo inserimento
						{
							this.cargoRec++;// incrementa il record
							this.InitValue ();// reinizializza valori a 0 per il nuovo inserimento
						}
					
					}// fine inserimento nuovo record

					else // controllo se si tratta di un UPDATE
						if (CargoM.CargoList.Count <= cargoTot // se il numero di valori inseriti è minore al totale inseribile
							&& cargoRec < cargoTot // se il numero di record da trattare è inferiore al totale
							&& cargoRec < CargoM.CargoList.Count // se il numero di record da trattare è inferiore al totale inserito
						) 
						{ 
							// SE IL RECORD ESISTE E' UN UPDATE 
							//effettuo l'update del record
							this.UpdateRec (cargoRec);
						} 
					} // FINE CHECK CONSISTENZA DATI
				}// ed if NUMERO ECCESSIVO DI DATI
			}  // end fine metodo

		
		// BOTTONE: elimina record corrente
		protected void OnButton4Released (object sender, EventArgs e)
		{
			if (CargoM.CargoList.Count > 0) { // esegui se il conteggio dati inseriti è positivo
				bool flag = false;
				if (this.cargoRec < CargoM.CargoList.Count) { // se il record attuale è minore del massimo di record inseirti
					try{
					CargoM.RemoveElement (cargoRec); // elimina il record
					}
					catch{
						System.Console.WriteLine ("ERRORE DI RIMOZIONE RECORD CARGO");
					}
					// se dopo l'eliminazione il record è 0
					if ((CargoM.CargoList.Count) == 0) { // se dopo la rimozione non ci sono piu' record
						cargoRec = 0; // setta il rec a 0
						this.InitValue ();// inizializza valori
						flag = true; // flag di elemento eliminato
					} else {
						if ((CargoM.CargoList.Count == cargoRec)) { // se il mio record è l'ultimo della lista
							cargoRec = CargoM.CargoList.Count - 1;// setto il mio record ultimo
						} 
						try{
						this.ShowRecord (cargoRec);
						} catch {
							System.Console.WriteLine ("ERRORE DI VISUALIZZAZIONE RECORD");
						}
						flag = true; // imposto la flag di rimozione se è riuscita
					}
					if (flag)
						WinI.InsertSomeText ("OPZIONI ELICOTTERO: rimosso record # " + (cargoRec + 1));
					else // altrimenti
						this.InitValue ();  //inizializzo i valori a 0
				}
			}
		}	// termine elimina record	

		
		// BOTTONE: record precedente
		protected void OnButton1Released (object sender, EventArgs e)
		{
			if (CargoM.CargoList.Count > 0) // controlla se esistono dati da leggere
			{
				if (this.cargoRec > 0) 
				{ 
					this.cargoRec--; // decrementa il numero di record per la visualizzazione
					this.ShowRecord(cargoRec); // ShowWin il record
				} 
				if (this.cargoRec==0)
				{
					WinI.InsertSomeText ("OPZIONI CARGO: raggiunto record iniziale.");
					try{
						this.ShowRecord(cargoRec); // visualizzo il record precedente
					} catch {
						System.Console.WriteLine ("ERRORE DI VISUALIZZAZIONE RECORD");
					}
				}
			} 
		} // fine metodo record precedente


		// BOTTONE: record successivo
		protected void OnButton6Released (object sender, EventArgs e)
		{
			// effettuo i controlli
			if ((CargoM.CargoList.Count > 0)// record presenti
			    && (cargoRec < CargoM.CargoList.Count)// il numero di record minore del massimo recordinseriti
				&& (cargoRec < (cargoTot))) //il numero di record minore del massimo di rec inseribili)
				 {// invece abbiamo ancora record
				if (this.cargoRec == (CargoM.CargoList.Count - 1) && (CargoM.CargoList.Count == cargoTot)) {  // raggiunto ultimo record

					WinI.InsertSomeText ("OPZIONI ELICOTTERO: raggiunto record finale");		

				} else if (cargoRec==CargoM.CargoList.Count-1 
							&& CargoM.CargoList.Count < cargoTot)
				 {// controlla se esistono dati da leggere
					this.cargoRec++; 
					this.InitValue ();
				} else {
					this.cargoRec++; // incrementa il record
					try{
						this.ShowRecord(cargoRec); // visualizzo il record precedente
					} catch {
						System.Console.WriteLine ("ERRORE DI VISUALIZZAZIONE RECORD");
					} } }

				}	// termine bottone record successivo

	   
		// mostra il record in input sull'interfaccia grafica 
		public void ShowRecord(int i)
		{
			this.entry1.Text=CargoM.CargoList[i].CargoString;
			this.hscale1.Value=CargoM.CargoList[i].CargoW;
			this.hscale2.Value=CargoM.CargoList[i].CargoP;
			this.label1.Text= "Numero di Record cargo #  "+ (this.cargoRec+1) +" Record inseriti: "+ CargoM.CargoList.Count + " Record disponibili: "+this.cargoTot;
		}
	


		// controllo consistenza dati 
		// non è possibile inserire cargo con peso 0
			private bool checkDataConsistency()
			{

			if (_cargoW==0) // se viene superato il peso massimo al decollo le pesate devono essere corrette
			{
				// crea il messaggio
				MessageBox.Show ("Sono stati inseriti dei dati incoerenti", "WARNING MESSAGE",MessageBoxButtons.OK);
				return false; // ritorna la flag dei dati errati
			}
			return true; // ritorno la flag dei dati corretti
			}
		
	} // fine classe
} // fine name space

