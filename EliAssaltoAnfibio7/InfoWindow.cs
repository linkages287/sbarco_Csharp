//+++++++++++++++++++++++++++++++++++++++++++++++++++++++
// definizione di una finestra per le informazioni sullo 
// stato di avanzamento del programma
// lo scopo della finestra è quello di visualizzare 
// le indicazione e le informazioni
// durante la prosecuzione del programma.
// le informaizoni vengono salvate in un file che funge 
// da logger in modo da poter esaminare i dati
// al termine della simulazione
// parte delle informazioni rilevanti viene inoltre salvato
// all'interno di una struttura e servono per la visualizzazione
// finale delle informazioni rilevanti a schermo
// CLASSE SINGLETON
//++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;
using Gtk;
using System.Windows.Forms;
using System.IO;

namespace EliAssaltoAnfibio
{

	public partial class InfoWindow : Gtk.Window
	{

		private static InfoWindow instance = null;// istanza singleton

		public static InfoWindow Instance()
		{

			if (instance == null) { // se l'instanza non esiste la crea 
				instance = new InfoWindow ();
				return instance;
			} 
			else 
			{  // riprende dal rec 0
				return instance; // ritorno il costruttore	
			}
		}



		public InfoWindow () :  base(Gtk.WindowType.Toplevel)
		{
			using (StreamWriter filew = new StreamWriter (fileName)) { filew.WriteLine(" - INIZIO LOG IN DATA : "+DateTime.Now.ToString());};
			infoTimeList = new List<timeInfoStruct>(0);

			this.Build ();
		}
		
		// struttura di contenimento tempi rilevanti, evento rilevante
		public struct timeInfoStruct

		{
			public TimeSpan timer; // tempo di accadimento
			public string info;	 // cosa accade
			public string EliName; // nome elicottero 
			public int Elirec; // record eli che ha effettuato l'inserimento

			// costruttore
			public timeInfoStruct (TimeSpan time, string infoS, string eli, int rec): this()
			{
				this.timer = new TimeSpan();
				timer=time;
				this.info = infoS;
				this.Elirec=rec;
				this.EliName=eli;
			}
		}

		public  List<timeInfoStruct> infoTimeList; 

		// salvo le informazioni nel file - uso come nome file la data e l'ora del salvataggio
		private static string fileName = string.Format("LogEliAssault_{0:hh-mm-ss}.txt", DateTime.Now);
		// file di testo


		// COSTRUTTORE classe
		public  void InsertSomeText(string s1)
		{
			string date_time = string.Format(" {0:hh-mm} ", DateTime.Now); // assegno nome file
			var iter = this.textview1.Buffer.StartIter;
			this.textview1.Buffer.Insert(ref iter, date_time +"-> "+ s1+"\n");
			this.AppendFIle ( date_time +"-> "+ s1+"\n");
		}


		// inserisci l'informazione nel file
		public  void AppendFIle (string text)
		{
			// scrivi l'informazione
			using (StreamWriter filew = new StreamWriter(fileName, true))
			{
				filew.WriteLine(text);
			}
		}

		// inserimento dell'evento nella struttura timeInfoStruct
		public void InsertEvent(TimeSpan timer, string str, string eliname, int rec)
		{
			this.infoTimeList.Add (new timeInfoStruct (timer, str,eliname, rec));
		}


		// chiusura finestra da X
		protected void OnDeleteEvent (object sender, DeleteEventArgs e)
		{	
			this.InsertSomeText(" WARNING! Puoi nascondere la finestra informazioni dal menu principale");
			e.RetVal=true;
		}	
		
	} // fine classe
} // fine namespace


