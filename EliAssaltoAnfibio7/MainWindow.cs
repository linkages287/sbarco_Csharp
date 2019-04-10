//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// La classe di occupa del controllo gli eventi e delle attivazioni 
// dei bottoni della schermata principale.
// La classe non è ripetibile quindi viene trattata come singleton
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using Gtk;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EliAssaltoAnfibio
{
	public partial class MainWindow: Gtk.Window
	{
		private static MainWindow instance = null;
		public Spot.day_night DNOperation = Spot.day_night.day;
		// setto le operazioni a day di default
		public int Elicopters { set; get; }
		// indica il numero di elicotteri partecipanti
		public int Truppe { set; get; }
		// indica le truppe partecipanti
		public int Distanza { set; get; }
		// indica la distanza dall'obiettivo
		public int AltroCargo { set; get; }
		// indica se esiste dell'altro cargo
		public int NumeroSpot { set; get; }
		// indica il numero di spot disponibili
		//all'apertura delle finestre opzioni viene creato anche il Manager di ogni classe che poi verra' passato
		//come parametro allo Start della simulazione, quindi mi serve una flag per indicare l'apertura di ogni finestra
		//e quindi la definizione dei dati necessari per l'inizio della simulazione.
		private bool _eliFlagWindow = false;
		// se vero indica l'apertura della finestra EliOptions la prima volta
		private bool _troopsFlagWindow = false;
		// se vero indica l'apertura della finestra TruppeOptions per la prima volta
		private bool _spotFlagWindow = false;
		// indica l'apertura della finestra opzioni spot per la prima volta
		private bool _cargoFlagWindow = false;
		// indica l'apertura della finestra cargo
		public InfoWindow InformationWin = InfoWindow.Instance ();
		// viene creata la schermata di informazioni generali.
		static public EliOptions Elioptions = null;
		// finestra opzioni spot elicottero
		static public TruppeOptions TroopsOpt = null;
		// finestra opzioni truppe
		static public SpotOptions SpotOpt = null;
		// finestra opzioni spot
		static public CargoOptions CargoOpt = null;
		// finestra opzioni cargo
		public Simulator SimulatorM;
		// COSTRUTTORE DELLA CLASSE
		protected  MainWindow () : base (Gtk.WindowType.Toplevel) // costruttore singleton richiamato da istance
		{
			// singleton il costruttore è richiamato dall'istanza
			this.Build ();// disegna la grafica del menu
			this.Elicopters = 0;
			this.Truppe = 0;
			this.Distanza = 0;
			this.AltroCargo = 0;
			this.NumeroSpot = 0;
			this.InformationWin.Show ();// finestra informazioni visibile
			// inizializzo i manager a null

			
		}
		// creazione di un metodo pubblico di accesso al costruttore SINGLETON
		public static MainWindow Instance ()
		{
			if (instance == null)
				instance = new MainWindow ();
			return instance;

		}
		// gestione degli eventi
		// uscita per pressione tasto X di uscita dal programma
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			InformationWin.Destroy ();
			if (Elioptions != null) {
				Elioptions.EliM.MainTime.Stop (); // stop timer 
				Gtk.Application.Quit ();
			}


			Gtk.Application.Quit ();// quit application
		}
		// MENU REGION
		// MENU quit, uscita per selezione quit
		protected void OnQuitAction1Activated (object sender, EventArgs e)
		{
			InformationWin.Destroy ();
			if (Elioptions != null) {
				Elioptions.EliM.MainTime.Stop (); // stop timer 
				Gtk.Application.Quit ();
			}
			Gtk.Application.Quit ();// quit application

		}
		// MENU RESET BUTTON, resetting dell'applicazione.
		protected void OnResetAction1Activated (object sender, EventArgs e)
		{
			if (Elioptions != null)
				Elioptions.EliM.MainTime.Stop (); // stop timer 
			System.Windows.Forms.Application.Restart ();
			Gtk.Application.Quit ();
			
		}
		// MENU visualizza information window ON - rende la schermata d'informazioni visibile
		protected void OnOnActionToggled (object sender, EventArgs e)
		{
			InformationWin.Show ();
		}
		// menu nascondi information window - rende la schermata informazioni invisibile
		protected void OnOffActionToggled (object sender, EventArgs e)
		{
			InformationWin.Hide ();
		}
		//menu - opzioni simulazione - operazioni diurne
		protected void OnGiornoAction2Activated (object sender, EventArgs e)
		{
			DNOperation = Spot.day_night.day; // setto lo sbarco diurno
			InformationWin.InsertSomeText ("MAIN WINDOW: selezionata operazione DIURNA");


		}
		// menu - opzioni simulazione - operazioni notturne
		protected void OnNotteActionActivated (object sender, EventArgs e)
		{
			DNOperation = Spot.day_night.night; // setto lo sbarco notturne
			InformationWin.InsertSomeText ("MAIN WINDOW: selezionata operazione NOTTURNA");

		}
		// bottone: ACCETTA NUMERO ELICOTTERI
		protected void OnButton1Released (object sender, EventArgs e)
		{
			if (this.Elicopters == 0) {
				this.Elicopters = (int)hscale1.Value;
				label5.Text = "ELICOTTERI -> " + this.Elicopters + " UNITA'";
				InformationWin.InsertSomeText ("MAIN WINDOW: Effettuato inserimento del NUMERO di elicotteri: "+this.Elicopters);
			} else {
				InformationWin.InsertSomeText ("MAIN WINDOW: numero di elicotteri già inserito");
				hscale1.Value =	this.Elicopters;
			}

		}
		// bottone: ACCETTA NUMERO TRUPPE
		protected void OnButton2Released (object sender, EventArgs e)
		{
			if (this.Truppe == 0) {
				this.Truppe = (int)hscale2.Value; // assegna il valore alla variabile truppe
				label6.Text = "TRUPPE -> " + this.Truppe + " UNITA'";
				InformationWin.InsertSomeText ("MAIN WINDOW: Effettuato inserimento del NUMERO di truppe: "+this.Truppe);

			} else {
				InformationWin.InsertSomeText ("MAIN WINDOW: numero di truppe già inserito");
				hscale1.Value =	this.Elicopters;
			}	
		}
		// bottone: ACCETTA ALTRO CARGO
		protected void OnButton3Released (object sender, EventArgs e)
		{
			if (this.AltroCargo == 0) {
				this.AltroCargo = (int)hscale3.Value;// assegna il valore alla variabile del cargo
				label7.Text = "ALTRO CARGO -> " + this.AltroCargo + " UNITA'";
				InformationWin.InsertSomeText ("MAIN WINDOW: Effettuato inserimento di ALTRO CARGO: "+this.AltroCargo);
				this.label7.ModifyFg (StateType.Normal, new Gdk.Color (120, 120, 1));
			} else {
				InformationWin.InsertSomeText ("MAIN WINDOW: numero cargo già inserito");
				hscale1.Value =	this.Elicopters;
			}
		}
		// bottone: ACCETTA DISTANZA
		protected void OnButton4Released (object sender, EventArgs e)
		{
			if (this.Distanza == 0) {
			
				this.Distanza = (int)hscale4.Value;// assegna il valore alla variabile della distanza
				label8.Text = "DISTANZA -> " + this.Distanza + " MILES";
				InformationWin.InsertSomeText ("MAIN WINDOW: inserita la distanza della landing zone dall'unità navale: "+this.Distanza+" NM");
				this.label8.ModifyFg (StateType.Normal, new Gdk.Color (1, 120, 1)); // DISTANZA GREEN FLAG 
			} else {
				InformationWin.InsertSomeText ("MAIN WINDOW: distanza già inserita");
				hscale1.Value =	this.Elicopters;
		
			}
		}
		// bottone: ACCETTA NUMERO SPOT
		protected void OnButton12Released (object sender, EventArgs e)
		{

			if (this.NumeroSpot == 0) {

				this.NumeroSpot = (int)hscale5.Value;
				label12.Text = "SPOT -> " + this.NumeroSpot + " UTILIZZABILI";
				InformationWin.InsertSomeText ("MAIN WINDOW: Effettuato inserimento del NUMERO di SPOT: "+this.NumeroSpot);
	
			} else {
				InformationWin.InsertSomeText ("MAIN WINDOW: numero di spot già inserito");
				hscale1.Value =	this.Elicopters;
			
			}
		}
		// bottone: OPZIONE  ELICOTTERI - determina l'accesso all'inserimento dati per ogni singolo elicotteri
		public void OnButton5Released (object sender, EventArgs e)
		{
			if (this.Elicopters == 0) { //controllo se esistono elicotteri da inserire
				InformationWin.InsertSomeText ("MAIN WINDOW: WARNING !!Nessun elicottero inserito...");
			} else {
				// creazione della finestra inserimento dati elicotteri
				// alla classe grafica viene passato il numero di record per la creazione della lista 
				if (this._eliFlagWindow == false) { // permette di effettuare la selezione solo 1 volta
					Elioptions = EliOptions.Instance (this.Elicopters, this.InformationWin); // richiama l'istanza singleton
					this._eliFlagWindow = true; // imposta la flag di apertura finestra a VERO
				
					Elioptions.ShowWin ();// mostra la finestra opzioni per gli elicotteri


				
				} else
					Elioptions.ShowWin ();// mostra la finestra opzioni per gli elicotteri

				if (Elioptions.EliM.ElicotteriList != null && Elioptions.EliM.ElicotteriList.Count > 0)
					Elioptions.ShowRecord (Elioptions.Eli_Record);
			}
		}
		// BOTTONE: visualizzo informazioni sugli ELICOTTERI
		protected void OnButton10Released (object sender, EventArgs e)
		{

			if (Elioptions == null)
				InformationWin.InsertSomeText ("MAIN WINDOW: Nessun elicottero è stato creato al momento...");
			else {
				// stampa il nume eli di ogni elicottero presente nella lista elicotteri
				InformationWin.InsertSomeText ("MAIN WINDOW: E' stata inserita la seguente LISTA elicotteri: ");
				foreach (Elicottero eli in Elioptions.EliM.ElicotteriList) {
					InformationWin.InsertSomeText (eli.EliInfo ()); // scrittura informazioni per ogni elicottero3
				}
			}
		}
		// BOTTONE: opzione gestione truppe
		protected void OnButton6Released (object sender, EventArgs e)
		{
			if (this.Truppe == 0) {

				InformationWin.InsertSomeText ("MAIN WINDOW: WARNING !! INSERIRE UN NUMERO DI TRUPPE MAGGIORE DI 0");
			} else {
				// creazione della finestra inserimento dati Truppe
				TroopsOpt = TruppeOptions.Instance (this.Truppe, InformationWin); // richiama l'istanza singleton per la finestra truppe
				InformationWin.InsertSomeText ("MAIN WINDOW: VISUALIZZUATA SCHERMATA OPZIONI TRUPPE");
				this._troopsFlagWindow = true; // schermata segnalata aperta
				this.Sensitive = false;
				TroopsOpt.ShowWin (); //viene visualizzata la schermata elicotteri e nascosta la schermata principale

			}

		}
		// bottone: opzioni gestione SPOT per l'iserimento delle informazioni sugli spot
		protected void OnButton11Released (object sender, EventArgs e)
		{
			if (this.NumeroSpot == 0) {

				InformationWin.InsertSomeText ("MAIN WINDOW: WARNING !! INSERIRE UN NUMERO DI SPOT MAGGIORE DI 0");
			} else {
				// creazione della finestra inserimento del cago
				if (this._spotFlagWindow == false) {

					// creazione della finestra inserimento dati Spot 	
					SpotOpt = SpotOptions.Instance (this.NumeroSpot, InformationWin); // richiama l'istanza singleton
					this._spotFlagWindow = true; // schermata segnalata aperta

					SpotOpt.ShowWin (); //viene visualizzata la schermata elicotteri e nascosta la schermata principale

				} else
					SpotOpt.ShowWin ();
				if (SpotOpt.SpotM.Spotlist != null && SpotOpt.SpotM.Spotlist.Count > 0)
					SpotOpt.ShowRecord (SpotOpt._spotRec); // se la finestra gia' esiste mostra il record 
			}
		}
		// termine metodo opzioni SPOT
		// BOTTONE: opzioni altro cargo per l'inserimento delle infomazioni
		protected void OnButton7Released (object sender, EventArgs e)
		{

			if (this.AltroCargo == 0) { 
				InformationWin.InsertSomeText ("MAIN WINDOW: WARNING !! INSERIRE UN NUMERO DI ELEMENTI CARGO MAGGIORE DI 0");
			} else {
				if (_cargoFlagWindow == false) {
					CargoOpt = CargoOptions.Instance (this.AltroCargo, this.InformationWin); // richiama l'istanza singleton
					this._cargoFlagWindow = true; // schermata segnalata aperta
					this.label7.ModifyFg (StateType.Normal, new Gdk.Color (1, 120, 1));
					CargoOpt.ShowWin ();
				} else
					CargoOpt.ShowWin (); //viene visualizzata la schermata elicotteri e nascosta la schermata principale

				if (CargoOpt.CargoM.CargoList != null && CargoOpt.CargoM.CargoList.Count > 0)
					CargoOpt.ShowRecord (CargoOpt.cargoRec);
		
			

			}

		}
		//BOTTONE : visualizza l'elenco  lista ALTRO CARGO
		protected void OnButton14Released (object sender, EventArgs e)
		{
			if (CargoOpt == null)
				InformationWin.InsertSomeText ("MAIN WINDOW: nessun cargo definito al momento...");
			else {
				if (CargoOpt.CargoM.CargoList.Count == 0)
					InformationWin.InsertSomeText ("MAIN WINDOW: Nessun ALTRO CARGO è stato creato al momento...");
				else {
					InformationWin.InsertSomeText ("MAIN WINDOW: E' stata inserita la seguente LISTA Altro Cargo: ");
					foreach (Cargo carg in CargoOpt.CargoM.CargoList) {
						InformationWin.InsertSomeText (carg.CargoInfo ());// scrivi l'informazioni per ogni cargo
					}
				}
			}
		}
		//BOTTONE: visualizza l'elenco degli SPOT disponibili
		protected void OnButton16Released (object sender, EventArgs e)
		{
			if (SpotOpt == null)
				InformationWin.InsertSomeText ("MAIN WINDOW: nessuno spot è stato definito al momento...");
			else {
				if (SpotOpt.SpotM.Spotlist.Count == 0) // controllo se la spot list ha almeno un elemento
				InformationWin.InsertSomeText ("MAIN WINDOW: Nessuna LISTA SPOT è stato creata al momento...");
				else {
					InformationWin.InsertSomeText ("MAIN WINDOW: E' stata inserita la seguente LISTA SPOT: ");
					foreach (Spot spotL in SpotOpt.SpotM.Spotlist) {
						InformationWin.InsertSomeText (spotL.SpotInfo ());
					}
				}
			}
		}
		//BOTTONE: start - simulazione
		protected void OnButton9Released (object sender, EventArgs e)
		{

			if ((this.Distanza > 0)// la distanza e' maggiore di 0
			    && _eliFlagWindow
			    && !(Elioptions.EliM.ElicotteriList.Count == 0)// sono stati inseriti degli elicotteri
			    && _spotFlagWindow
			    && !(SpotOpt.SpotM.Spotlist.Count == 0)// sonno stati inseriti degli spot
			    && _troopsFlagWindow
			    && !(TroopsOpt.TroopM.TroopList.Count == 0)) { // controllo che siano state aperte le finestre per l'inserimento dati
				Elioptions.EliM.LZdistance = this.Distanza; // viene passata la distanza della nave al manager

				// verificare per il cargo che sia esistente <<<<< il cargo puo' anche non essere un parametro necessario alla simulazione
				// i parametri fondamentali sono elicotteri spot , truppe e distanza LZ
				if (CargoOpt == null)
					SimulatorM = new Simulator (this.Distanza, this.DNOperation, this.InformationWin, TroopsOpt.TroopM, Elioptions.EliM, SpotOpt.SpotM, null);
				else
					SimulatorM = new Simulator (this.Distanza, this.DNOperation, this.InformationWin, TroopsOpt.TroopM, Elioptions.EliM, SpotOpt.SpotM, CargoOpt.CargoM);
	
				SimulatorM.InitDeckSpotAssign (); // spot assegnazione

			} else
				InformationWin.InsertSomeText ("MAIN WINDOW: NON E' POSSIBILE INIZIALIZZARE IL SIMULATORE. INSERIRE I DATI MANCANTI...");
		}
		// attivazione dello start simulation attraverso il menu principale
		protected void OnStartAction1Activated (object sender, EventArgs e)
		{

			this.OnButton9Released (sender, e);
		}

		public void CheckGreenFlags ()
		{
			//elicotteri GREEN FLAG
			if (Elioptions != null && Elioptions.EliM != null && Elioptions.EliM.ElicotteriList != null && Elioptions.EliM.ElicotteriList.Count != 0)
				this.label5.ModifyFg (StateType.Normal, new Gdk.Color (1, 120, 1));

			// truppe GREEN FLAG
			if (TroopsOpt != null && TroopsOpt.TroopM != null && TroopsOpt.TroopM.TroopList != null && TroopsOpt.TroopM.TroopList.Count != 0)
				this.label6.ModifyFg (StateType.Normal, new Gdk.Color (1, 120, 1));

			// spot GREEN FLAG
			if (SpotOpt != null && SpotOpt.SpotM != null && SpotOpt.SpotM.Spotlist != null && SpotOpt.SpotM.Spotlist.Count != 0)
				this.label12.ModifyFg (StateType.Normal, new Gdk.Color (1, 120, 1));

			if (CargoOpt != null && CargoOpt.CargoM != null && CargoOpt.CargoM.CargoList.Count != 0)
				this.label7.ModifyFg (StateType.Normal, new Gdk.Color (1, 120, 1));

		}
	}
	// fine classe
}
 // fine namespace