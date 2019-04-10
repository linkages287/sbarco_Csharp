// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// classe spot : descrive gli attributi di ogni spot.
// Ogni spot puo' avere caratteristiche che lo rendono 
// gestibile di giorno e notte da ogno classe di elicotteri
// Un elicottero può appontare su uno spot di giorno o di notte
// in relazione alla classe di appartenenza leggero o pesante o altro 
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


using System;

namespace EliAssaltoAnfibio
{
	public class Spot
	{
		public enum day_night: int {day, night};// enumerazione giorno e notte

		// struttura per definire l'accettabilità diurna/notturna di un una categoria eli
		// Non tutte le categorie di elicottero sono impiegabili di giorno e di notte 
		// su un determinato Spot. E' possiible ad esempio che una categoria di elicottero leggero
		// possa appontare di giorno e di notte mentre una CAT pesante possa appontare solo di notte.
		public struct DNCatAccept

		{
			public day_night DN {get;set;} // accettazione day night
			public bool Cat1 {get; set;} // accettazione per la cat 1
			public bool Cat2 { get; set;} // accettazione per la cat 2
			public bool Cat3 {get; set;} // accettazione per la cat 3

		public  DNCatAccept(  day_night DN , bool cat1 , bool cat2, bool cat3): this()
		{
				this.DN = DN;
				this.Cat1=cat1;
				this.Cat2=cat2;
				this.Cat3=cat3;
			
		}}

		public DNCatAccept DayCanAccept = new DNCatAccept(day_night.day,false,false,false); // struttura operazioni diurne
		public DNCatAccept NightCanAccept = new DNCatAccept(day_night.night,false,false,false);// struttura operazioni notturne



		public int SpotPos{set;get;} // la posizione viene contrassegnata da prora verso poppa dal numero 1 al numero X
							  // ad esempio lo spot1 indica lo spot piu' a prora.
							  // viene usato come numero di indice per l'assegnazione

		// status flags ----
		public bool SpotOccupied {set;get;} // indica se lo spot è ingombro o libero da elicotteri
		public bool SpotRunnable{set;get;} // indica lo stato di efficienza dello spot
		public bool IsReservd { set; get;} // indica lo stato di prenotato dello spot
		// end status 
		public Elicottero eliWhoReservd { set; get;} // indica l'elicottero che ha effettuato la prenotazione
		public Elicottero Eli {set;get;} // quando non nullo indica l'elicottero presente sullo spot

	

		// costruttore della classe con l'inserimento dei dati
		public Spot (int spotPos, bool spotRunnable, bool cat1D, bool cat2D , bool cat3D , bool cat1N , bool cat2N , bool cat3N )
		{
			this.SpotPos = spotPos; // viene assegnato un numero di pos allo spot
			this.SpotRunnable = spotRunnable; // lo spot è creato funzionante
			this.SpotOccupied=false; // lo spot non viene creato occupato
			this.IsReservd = false; // lo spot non nasce riservato ad alcun elicottero
			this.DayCanAccept.Cat1=cat1D; // classe 
			this.DayCanAccept.Cat2=cat2D;
			this.DayCanAccept.Cat3=cat3D;
			this.NightCanAccept.Cat1=cat1N;
			this.NightCanAccept.Cat2=cat2N;
			this.NightCanAccept.Cat3=cat3N;

			// elicotteri 
			this.Eli = null; // nessun elicottero sul ponte
			this.eliWhoReservd = null; // nessun elicottero riservante lo spot
		}
	
		// stringa informazioni per la classe spot
		public string SpotInfo ()
		{
			string _idEli="null"; // impostazione iniziale stringa eli nulla
			if (Eli!=null) _idEli=Eli.IdEli; // se esiste un elicottero sullo spot indica quale
		return "Record_Spot: "+ SpotPos+ " Efficienza: "+SpotRunnable+" Spot occupato: "+SpotOccupied +" Da Eli: "+_idEli+ " GIORNO Cat1: "+DayCanAccept.Cat1 + " GIORNO Cat2: "+DayCanAccept.Cat2 + " GIORNO Cat3: "+ DayCanAccept.Cat3 +" NOTTE Cat1: " + NightCanAccept.Cat1 + " NOTTE Cat2: "+ NightCanAccept.Cat2 + " NOTTE Cat3: "+ NightCanAccept.Cat3;
		}
	} // fine classe
} // fine namespace

