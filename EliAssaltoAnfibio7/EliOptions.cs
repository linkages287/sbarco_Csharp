//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// CLASSE DI CONTROLLO PER L'INSERIMENTO DATI ELICOTTERI
// SI OCCUPA DELL'INSERIMENTO E DEL CONTROLLO DELLE INFORMAZIONI
// SUGLI ELICOTTERI DA IMPIEGARE
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Windows.Forms;
using Gtk;
using System.Drawing;

namespace EliAssaltoAnfibio
{
	public partial class EliOptions : Gtk.Window
	{
		private static EliOptions instance = null;
		// istanza singleton
		private static MainWindow MainWin = MainWindow.Instance ();
		// identifica la finestra principale in modo che possa essere ritrovata alla chiusura di questa
		public int Eli_Record = 0;
		// indica il record trattato, È IL NUMERO DI SEQUENZA DELL'ELICOTTERO
		private int _totEliC = 0;
		// indica il numero voluto di elicotteri inizile
		public EliManager EliM;
		// definizione dell'eli manager
		// VARIABILI DA INSERIRE NEL RECORD ELICOTTERO
		//-----------------------------------------------
		private string _IdEli = "DA ASSEGNARE";
		// nome o matricola che identifica l'elicottero in volo ES. King-01
		private int _catEli = 0;
		// classe di elicottero 1=pesante 2=leggero 3=altro
		private float _fuel = 0;
		// indica il livello di carburante dell'elicottero
		private int _WTroopsLeftOnBoard = 0;
		// indica il numero di truppe imbarcate sull'elicottero
		private int _WCargoLeftOnBoard = 0;
		// indica altri componenti di cargo general purpouse
		private int _maxTOLoad = 0;
		// max Take Off load indica il peso massimo al decollo del velivolo
		private int _offLoadWeight = 0;
		// indica il peso del velivolo scarico di carburante, truppe o altro carico
		private float _fuelConsumption = 0;
		// indica la quantita' oraria di carburante consumato
		public bool _isRunning = false;
		// indica se i motori sono accesi
		public bool _bladeSpread = false;
		// l'elicottero è a pale aperte o chiuse
		public bool _isEfficient = false;
		// indica lo stato di efficienza del velivolo 1 efficiente 0 non efficiente
		public bool _posEli = false;
		// 0 se eli in hangar - 1 se eli sul ponte
		public bool _isFlying = false;
		// 0 se eli non in volo - 1 se eli in volo
		//-------------------------------------------------
		public InfoWindow win = null;
		// finestra informazioni
		// viene definita la lista per la generazione degli elicotteri
		// da aggiungere dinamicamente all'elenco
		// COSTRUTTORE alla classe viene passato il numero di record iniziali
		protected EliOptions (int totEli, InfoWindow _win) : base (Gtk.WindowType.Toplevel)
		{
			this.Build (); // disegna fisicamente la finestra
			this._totEliC = totEli;// passo la variabile con il numero di elicotteri 
			this.win = _win; // finestra informazioni
			this.EliM = (EliManager)FactoryManager.Make (0, totEli, win); // il manager crea l'elimanager
			EliM.MakeList (0); // viene definita l'istanza della lista di elementi
			this.InitValue (); // azzera valori dopo la creazione

		}
		// istance per la classe singleton - viene creata una sola istanza della cWinlasse
		public static EliOptions Instance (int TotEli, InfoWindow winI)
		{
			if (instance == null) { // se l'instanza non esiste la crea 
				instance = new EliOptions (TotEli, winI);
				return instance;
			} else {  // riprende dal rec 0 

				return instance; // ritorno il costruttore	
			}
		}
		//---- METODI DI SUPPORTO------------------------------------------------------------------------
		// INIZIALIZZA i campi prima interni dell'inserimento di un nuovo record
		private void InitValue ()
		{
			_IdEli = "NUOVO RECORD DA ASSEGNARE";
			this.label4.Text = this._IdEli;
			this.entry1.Text = _IdEli;
			_catEli = 0;
			this.label5.Text = "INSERIRE VALORE";
			this.hscale1.Value = 0;
			_fuel = 0;
			this.label6.Text = "INSERIRE VALORE";
			this.hscale2.Value = 0;
			_WTroopsLeftOnBoard = 0;
			this.label8.Text = "INSERIRE VALORE";
			this.hscale4.Value = 0;
			_WCargoLeftOnBoard = 0;
			this.label3.Text = "INSERIRE VALORE";
			this.hscale5.Value = 0;
			_maxTOLoad = 0;
			this.label11.Text = "INSERIRE VALORE";
			this.hscale6.Value = 0;
			_offLoadWeight = 0;
			this.label9.Text = "INSERIRE VALORE";
			this.hscale7.Value = 0;
			_fuelConsumption = 0;
			this.label10.Text = "INSERIRE VALORE";
			this.hscale3.Value = 0; 
			_isFlying = false;
			this.button17.Label = "EFFICIENTE";
			_isEfficient = true;
			this.button16.Label = "SPENTO";
			_isRunning = false; 
			this.button10.Label = "IN HANGAR";
			_posEli = false; 
			this.button15.Label = "PALE CHIUSE";
			_bladeSpread = false; 
			_isFlying = false;
			this.label1.Text = "Record # " + (Eli_Record + 1) + " Record inseriti # " 
			                   + EliM.ElicotteriList.Count + " su: " + this._totEliC;
		}
		// MOSTRA il record I sull'interfaccia grafica
		public void ShowRecord (int i)
		{
			this.entry1.Text = EliM.ElicotteriList [i].IdEli; // id eli
			this._IdEli=EliM.ElicotteriList [i].IdEli; // id eli
			this.label4.Text = "VALORE INSERITO: " + this.entry1.Text;
			this.hscale1.Value = EliM.ElicotteriList [i].CatEli; // cat eli
			this._catEli=EliM.ElicotteriList [i].CatEli; // cat eli
			switch ((int)this.hscale1.Value) {
			case 1:
				this.label5.Text = "ELICOTTERO TIPO LEGGERO";
				break;
			case 2:
				this.label5.Text = "ELICOTTERO TIPO PESANTE";
				break;
			case 3:
				this.label5.Text = "ALTRO MODELLO";
				break;
			}
			this.hscale2.Value = EliM.ElicotteriList [i].Fuel; // carburante
			this._fuel= EliM.ElicotteriList [i].Fuel;
			this.label6.Text = "ELICOTTERO RIFORNITO: " + this.hscale2.Value + "KG DI CARBURANTE";
			this.hscale4.Value = EliM.ElicotteriList [i].WTroopsLeftOnBoard;// truppe
			this._WTroopsLeftOnBoard=EliM.ElicotteriList [i].WTroopsLeftOnBoard;// truppe
			this.label8.Text = "PESO TRUPPE IMBARCABILI: " + this.hscale4.Value + " KG";
			this.hscale5.Value = EliM.ElicotteriList [i].WCargoLeftOnBoard; // peso cargo
			this._WCargoLeftOnBoard=EliM.ElicotteriList [i].WCargoLeftOnBoard; // peso cargo
			this.label3.Text = "PESO DEL CARGO IMBARCABILE: " + this.hscale5.Value + " KG";
			this.hscale6.Value = EliM.ElicotteriList [i].MaxTOLoad;// MAX TO
			this._maxTOLoad= EliM.ElicotteriList [i].MaxTOLoad;// MAX TO
			this.label11.Text = " PESO MASSIMO AL DECOLLO: " + this.hscale6.Value + " KG";
			this.hscale7.Value = EliM.ElicotteriList [i].OffLoadWeight;// off load W
			this._offLoadWeight= EliM.ElicotteriList [i].OffLoadWeight;// off load W
			this.label9.Text = " PESO BASICO ELICOTTERO SCARICO: " + this.hscale7.Value;
			this.hscale3.Value = EliM.ElicotteriList [i].FuelC;// consumi di carburante
			this._fuelConsumption=EliM.ElicotteriList [i].FuelC;// consumi di carburante
			this.label10.Text = " CONSUMO ELICOTTERO: " + this.hscale3.Value;
			if (EliM.ElicotteriList [i].PosEli) {
				this.button10.Label = "SUL PONTE";
				this._posEli = true;       
			} else {
				this.button10.Label = "IN HANGAR";
				this._posEli = false;
			}

			if (EliM.ElicotteriList [i].IsBladeSpread) {
				this.button15.Label = "PALE APERTE"; 
				this._bladeSpread = true;      	
			} else {
				this.button15.Label = "PALE CHIUSE";
				this._bladeSpread = false;
			}

			if (EliM.ElicotteriList [i].IsRunning) {
				this.button16.Label = "ACCESO";
				this._isRunning = true;       	
			} else {
				this.button16.Label = "SPENTO";
				this._isRunning = false;
			}

			if (!EliM.ElicotteriList [i].IsEfficient) {
				this.button17.Label = "INEFFICIENTE";
			this._isEfficient = false;      
			} else {
				this.button17.Label = "EFFICIENTE";
				this._isEfficient = true;
			}

			this.label1.Text = "Record # " + (Eli_Record + 1) + " Record inseriti # " + EliM.ElicotteriList.Count + " su: " + this._totEliC;
		}
		// effettua l'update del record corrente sostituiendo i nuovi valori rilevati a quelli vecchi in memoria
		private void UpdateRec (int i)
		{
			this._IdEli = this.entry1.Text;
			EliM.ElicotteriList [i].IdEli = this.entry1.Text; // id eli
			this._catEli = (int)this.hscale1.Value;
			EliM.ElicotteriList [i].CatEli = (int)this.hscale1.Value; // cat eli
			this._fuel = (int)this.hscale2.Value;
			EliM.ElicotteriList [i].Fuel = (int)this.hscale2.Value; // carburante
			this._WTroopsLeftOnBoard=(int)this.hscale4.Value;
			EliM.ElicotteriList [i].WTroopsLeftOnBoard = (int)this.hscale4.Value;// truppe
			this._WCargoLeftOnBoard=(int)this.hscale5.Value;
			EliM.ElicotteriList [i].WCargoLeftOnBoard = (int)this.hscale5.Value; // peso cargo
			this._maxTOLoad=(int)this.hscale6.Value;
			EliM.ElicotteriList [i].MaxTOLoad = (int)this.hscale6.Value;// MAX TO
			this._offLoadWeight = (int)this.hscale7.Value;
			EliM.ElicotteriList [i].OffLoadWeight = (int)this.hscale7.Value;// off load W
			this._fuelConsumption=(int)this.hscale3.Value;
			EliM.ElicotteriList [i].FuelC = (int)this.hscale3.Value;// consumi di carburante

			if (this.button10.Label == "IN HANGAR") {
				this._posEli = false;
				EliM.ElicotteriList [i].PosEli = false;
			} else {
				this._posEli = true;
				EliM.ElicotteriList [i].PosEli = true; // posizione eli hangar o ponte
			}

			if (this.button15.Label == "PALE APERTE")
			 {
				this._bladeSpread = true;
				EliM.ElicotteriList [i].IsBladeSpread = true;
			} else 
			{
				this._bladeSpread = false;
				EliM.ElicotteriList [i].IsBladeSpread = false;// pale aperte o chiuse
			}

			if (this.button16.Label == "ACCESO") {
				this._isRunning = true;
				EliM.ElicotteriList [i].IsRunning = true;
			} else 
			{
				this._isRunning = false;
				EliM.ElicotteriList [i].IsRunning = false;// accesso spento
			}

			if (this.button17.Label == "EFFICIENTE") 
			{
				this._isEfficient = true;
				EliM.ElicotteriList [i].IsEfficient = true;
			} 
			else 
			{
				this._isEfficient = false;
				EliM.ElicotteriList [i].IsEfficient = false; // is efficient - inefficiente
			}

			win.InsertSomeText (" OPZIONI ELICOTTERO: record # " + (i + 1) + " AGGIORNATO..");
		} // termine update record


		// ShowWin la finestra corrente e nasconde la MAIN WINDOW in modo da evitare inserimenti errati di dati
		public void ShowWin ()
		{		
			MainWin.Sensitive = false;
			this.Visible = true;
		}
		// effettuo il controllo dei dati inseriti per ogni elicottero
		// i pesi devono essere congruenti con il sistema
		private bool checkDataConsistency ()
		{
			// peso totale
			float allUpW = _fuel + _WTroopsLeftOnBoard + _WCargoLeftOnBoard + _offLoadWeight;

			if ((allUpW > _maxTOLoad) || (_fuel == 0) || ((_WTroopsLeftOnBoard + _WCargoLeftOnBoard) == 0)) { // se viene superato il peso massimo al decollo le pesate devono essere corrette
		
				// crea il messaggio
				MessageBox.Show ("Sono stati inseriti dei dati incoerenti", "WARNING MESSAGE", MessageBoxButtons.OK);
			
				return false; // ritorna la flag dei dati errati

			}
			return true; // ritorno la flag dei dati corretti

		}
		//--------------------------------------------------------------------------------------------------------------
		// CONTROLS AREA---------------------------------------------------------------------------------------
		// VENGONO GESTITI I CONTROLLI DELL'UI
		// bottone chiusura applicazione
		protected void OnDeleteEvent (object sender, DeleteEventArgs e)
		{	
			if ((Eli_Record >= EliM.ElicotteriList.Count) && (EliM.ElicotteriList.Count > 0))// effettuo il check di out bound
				Eli_Record--;

			if (EliM.ElicotteriList.Count == 0)
				Eli_Record = 0;

			this.Hide (); // nascondo la finestra

			MainWin.Sensitive = true; // resetto la sensibilità della finestra principale
			MainWin.CheckGreenFlags ();// check green flags
			e.RetVal = true;
		}
		// BOTTONE ACCETTA DATI E TORNA AL MENU PRINCIPALE
		protected void OnButton9Released (object sender, EventArgs e)
		{

			if ((Eli_Record >= EliM.ElicotteriList.Count) && (EliM.ElicotteriList.Count > 0))// effettuo il check di out bound
				Eli_Record--;

			if (EliM.ElicotteriList.Count == 0)
				Eli_Record = 0;


			this.Hide (); // nascondo la finestra

			MainWin.Sensitive = true; // resetto la sensibilità della finestra principale
			MainWin.CheckGreenFlags ();// check green flags
		}
		// ACCETTAZIONE dell'identificativo dell'elicottero
		protected void OnButton1Released (object sender, EventArgs e)
		{
			_IdEli = this.entry1.Text;
			this.label4.Text = "VALORE INSERITO: " + this._IdEli;

		}
		// accettazione categoria elicottero
		protected void OnButton2Released (object sender, EventArgs e)
		{

			_catEli = (int)this.hscale1.Value;

			switch (_catEli) {
			case 1:
				this.label5.Text = "ELICOTTERO TIPO LEGGERO";
				break;
			case 2:
				this.label5.Text = "ELICOTTERO TIPO PESANTE";
				break;
			case 3:
				this.label5.Text = "ALTRO MODELLO";
				break;
			}
		}
		// accettazione peso carburante
		protected void OnButton3Released (object sender, EventArgs e)
		{
			_fuel = (int)this.hscale2.Value;
			this.label6.Text = "ELICOTTERO RIFORNITO: " + _fuel + "KG DI CARBURANTE";
		}
		// accettazione PESO TRUPPE
		protected void OnButton5Released (object sender, EventArgs e)
		{

			_WTroopsLeftOnBoard = (int)this.hscale4.Value;
			this.label8.Text = "PESO TRUPPE IMBARCABILI: " + _WTroopsLeftOnBoard + " KG";

		}
		// accettazione del PESO DEL CARGO
		protected void OnButton13Released (object sender, EventArgs e)
		{
			_WCargoLeftOnBoard = (int)this.hscale5.Value;
			this.label3.Text = "PESO DEL CARGO IMBARCABILE: " + _WCargoLeftOnBoard + " KG";

		}
		// accettazione del Valore di MAX TAKE OFF
		protected void OnButton14Released (object sender, EventArgs e)
		{
			_maxTOLoad = (int)this.hscale6.Value;
			this.label11.Text = " PESO MASSIMO AL DECOLLO: " + _maxTOLoad + " KG";
		
		}
		// accettazione del Valore di PESO dell'elicottero scarico
		protected void OnButton6Released (object sender, EventArgs e)
		{
			_offLoadWeight = (int)this.hscale7.Value;
			this.label9.Text = " PESO BASICO ELICOTTERO SCARICO: " + _offLoadWeight;
		}

		protected void OnButton4Released (object sender, EventArgs e)
		{
			_fuelConsumption = (int)this.hscale3.Value;
			this.label10.Text = " CONSUMO ELICOTTERO: " + _fuelConsumption;
		}
		//BUTTON: PONTE HANGAR --- vengono applicate le restrizioni al caso
		protected void OnButton10Released (object sender, EventArgs e)
		{
			if (this._posEli == false) {
				_posEli = true;// switch nella posizione sul ponte
				this.button10.Label = "SUL PONTE";

				// constrain sul ponte deve essere efficiente
				this._isEfficient = true;
				this.button17.Label = "EFFICIENTE";    


			} else {
				this._posEli = false; // switch nella posizione in hangar
				this.button10.Label = "IN HANGAR";

				// constrain in hangar - necessariamente a pale chiuse
				_bladeSpread = false; // switch nella posizione in hangar
				this.button15.Label = "PALE CHIUSE";    

				// constrin in hangar - necessariamente spento
				_isRunning = false; // switch nella posizione in hangar
				this.button16.Label = "SPENTO";

			}
		}
		// BUTTON: pale CHIUSE O pale APERTE
		protected void OnButton15Released (object sender, EventArgs e)
		{
			if (this._bladeSpread == false) {
				this._bladeSpread = true;
				this.button15.Label = "PALE APERTE";

				// pale aperte necessita di elicottero sul ponte
				this._posEli = true;
				this.button10.Label = "SUL PONTE";
			
				// con le pale aperte deve essere efficiente
				_isEfficient = true;
				this.button17.Label = "EFFICIENTE";    

				// a pale aperte deve essere acceso
				_isRunning = true;
				this.button16.Label = "ACCESO"; 
			
			} else {
				_bladeSpread = false; // switch nella posizione in hangar
				this.button15.Label = ("PALE CHIUSE");
			}
		}
		// BUTTON: indica se l'elicottero ha i motori accesi oppure spenti
		protected void OnButton16Released (object sender, EventArgs e)
		{
			if (!_isRunning) {
				_isRunning = true;
				this.button16.Label = "ACCESO"; 

				// constrain
				//se è acceso deve stare per forza sul ponte
				_posEli = true;
				this.button10.Label = "SUL PONTE";

				// se è acceso deve essere efficiente
				_isEfficient = true;
				this.button17.Label = "EFFICIENTE";    
			
			} else {
				_isRunning = false; // switch nella posizione in hangar
				this.button16.Label = "SPENTO";
			
				// constrain elicottero spento -- pale chiuse
				_bladeSpread = false; 
				this.button15.Label = "PALE CHIUSE";    

			}
		}
		// indicatore di elicottero efficiente o inefficiente
		protected void OnButton17Released (object sender, EventArgs e)
		{
			if (_isEfficient) {
				_isEfficient = false;
				this.button17.Label = "INEFFICIENTE";  

				// constrain se inefficiente deve essere a pale chiuse
				_bladeSpread = false; // switch nella posizione in hangar
				this.button15.Label = "PALE CHIUSE";    
			
				//constrain se inefficiente deve essere in hangar
				_posEli = false; 
				this.button10.Label = "IN HANGAR";

				// contrain se inefficiente deve essere spento
				_isRunning = false; 
				this.button16.Label = "SPENTO";    
			} else {
				_isEfficient = true;
				this.button17.Label = "EFFICIENTE";
			}
		}
		//BOTTONE: SALVA RECORD E RESETTA
		protected void OnButton11Released (object sender, EventArgs e)
		{
			if (EliM.ElicotteriList.Count <= this._totEliC) { // se il numero di elicotteri è eccessivo												

				if (checkDataConsistency ()) { // CHECK CONSISTENZA DATI// controllo prima la consistenza dei dati inseriti

					//--------------------------------------------------------------------------------------------------------------
					if (EliM.ElicotteriList.Count <= this._totEliC // se il numero di record inseriti è minore del totale inseribile
						&& Eli_Record < _totEliC  // se il record che dobbiamo inserire è minore del totale inseribile
						&& Eli_Record >= EliM.ElicotteriList.Count) // se il record che dobbiamo inserire è maggiore del totale inserito
					{ // SE IL RECORD NON ESISTE: INSERIMENTO NUOVO RECORD------------------------------------------------------------
						try
						{ // provo a inserire il dato cargo
							EliM.InsertElementEli (EliM.MainTime, Eli_Record, _IdEli, _catEli, _fuel, 
								_WTroopsLeftOnBoard, _WCargoLeftOnBoard, _maxTOLoad, _offLoadWeight,
								_isRunning, _bladeSpread, _isEfficient, _posEli, _isFlying, _fuelConsumption);
						}
						catch 
						{
							System.Console.WriteLine ("ERRORE NELL'INSERIMENTO DEL RECORD ELICOTTERO"); // ERRORE INSERIMENTO
						}
						// visualizzo e salvo le informazioni sul record------------------------------------------
						win.InsertSomeText ("OPZIONI ELICOTTERO: inserto nuovo record#__" + (this.Eli_Record + 1) + " Elementi totali presenti: " + EliM.ElicotteriList.Count);
						win.InsertSomeText (EliM.ElicotteriList [Eli_Record].EliInfo ()); // visualizzo le info nella finestra informazioni

						// controllo se ultimo record e chiudo oppure se ci sono altri record allora inizializzo
						if ((Eli_Record == EliM.ElicotteriList.Count - 1) && (EliM.ElicotteriList.Count == _totEliC)) {	
							this.Hide (); // nascondo la finestra
							MainWin.Sensitive = true; // resetto la sensibilità della finestra principale
							MainWin.CheckGreenFlags ();// check green flags
						} else 
							// se non è l'ultimo inizializzo i valori per il prossimo inserimento
						{
							this.Eli_Record++;// incrementa il record
							this.InitValue ();// reinizializza valori a 0 per il nuovo inserimento
						}

					}// fine inserimento nuovo record

					else // controllo se si tratta di un UPDATE
						if (EliM.ElicotteriList.Count <= _totEliC // se il numero di valori inseriti è minore al totale inseribile
							&& Eli_Record < _totEliC // se il numero di record da trattare è inferiore al totale
							&& Eli_Record < EliM.ElicotteriList.Count // se il numero di record da trattare è inferiore al totale inserito
						) { 
							// SE IL RECORD ESISTE E' UN UPDATE 
							//effettuo l'update del record
							this.UpdateRec (Eli_Record);
						} 

				} // FINE CHECK CONSISTENZA DATI

			}// ed if NUMERO ECCESSIVO DI DATI
		}
		// end fine metodo
		// BOTTONE: elimina record
		protected void OnButton12Released (object sender, EventArgs e)
		{
			if (EliM.ElicotteriList.Count > 0) {


				bool flag = false;
				if (this.Eli_Record < EliM.ElicotteriList.Count) {
					EliM.RemoveElement (Eli_Record); // elimina il record
					// se dopo l'eliminazione il record è 0
					if ((EliM.ElicotteriList.Count) == 0) { // se dopo la rimozione non ci sono piu' record
						Eli_Record = 0; // setta il rec a 0
						this.InitValue ();// inizializza valori
						flag = true; // flag di elemento eliminato
					} else {
						if ((EliM.ElicotteriList.Count == Eli_Record)) { // se il mio record è l'ultimo della lista
							Eli_Record = EliM.ElicotteriList.Count - 1;
						} 

						this.ShowRecord (Eli_Record); // mostra il record
						flag = true;
					}
					if (flag)
						win.InsertSomeText ("OPZIONI ELICOTTERO: rimosso record # " + (Eli_Record + 1));
					else
						this.InitValue (); 
				}
			}
		}
		// termine elimina record
		//-------------------------------------------------------------------------------
		// BOTTONE: visualizza record precedente
		protected void OnButton8Released (object sender, EventArgs e)
		{
			
			if (EliM.ElicotteriList.Count > 0) { // controlla se esistono dati da leggere
				if (this.Eli_Record > 0) { 
					this.Eli_Record--; // decrementa il numero di record per la visualizzazione
					this.ShowRecord (Eli_Record); // ShowWin il record
				} 
				if (this.Eli_Record == 0) {
					win.InsertSomeText ("OPZIONI ELICOTTERO: raggiunto record iniziale.");
					this.ShowRecord (Eli_Record);
				}
			} 
		}
		// fine metodo record precedente

		//BOTTONE: record successivo
		protected void OnButton7Released (object sender, EventArgs e)
		{
			// effettuo i controlli
			if ((EliM.ElicotteriList.Count > 0)// record presenti
			    && (Eli_Record < EliM.ElicotteriList.Count)// il numero di record minore del massimo recordinseriti
			    && (Eli_Record < (_totEliC))) { //il numero di record minore del massimo di rec inseribili

				if (this.Eli_Record == (EliM.ElicotteriList.Count - 1) && (EliM.ElicotteriList.Count == _totEliC)) {  // raggiunto ultimo record

					win.InsertSomeText ("OPZIONI ELICOTTERO: raggiunto record finale");		

				} else if (Eli_Record==EliM.ElicotteriList.Count-1 
							&& EliM.ElicotteriList.Count < _totEliC) 
				{// controlla se esistono dati da leggere
					this.Eli_Record++;
					this.InitValue ();
				} else {
					this.Eli_Record++; // incrementa il record
					try {
					this.ShowRecord (Eli_Record); // mostra il record
					} catch{
						System.Console.WriteLine ("ERRORE DI VISUALIZZAZIONE RECORD");
					}
				}
			}
		} // termine bottone record successivo
	} // fine classe
} // fine namespace

