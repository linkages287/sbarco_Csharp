// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// classe cargo : descrive gli attributi di ogni cargo
// ogni singolo cargo che deve essere caricato 
// a bordo dell'elicottero per effettuare lo sbarco
// la missione è completata quando truppe e cargo sono sulla LZ
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{
	public class Cargo
	{
		public int CargoRec {set;get;} // record cargo
		public int CargoW {set;get;} // cargo peso totale
		public int CargoP {set;get;}// personale necessario al trasporto 
		public string CargoString {set;get;}// descrizione del cargo
		public Vector2 CargoVector { set; get;} // vettore cargo
		public Elicottero Eli{set;get;} // al cargo puo' essere assegnato un elicottero
		public bool IsEliC {set;get;}// flag indicante che il cargo è a bordo
		public bool isLand { set; get;} // flag indicante che il cargo è a terra
		public bool IsFittable { set; get;}// flag indicante che un cargo non è adattabile
		public List<Soldier> ListCargoTroop { set; get;}
		private int SoldierW =0;

		// costruttore della classe Cargo
		public Cargo (int cargoRec ,string cargoType, int cargoW , int cargoP)


		{
			this.CargoRec=cargoRec; // record del cargo
			this.CargoString=cargoType; // stringa che definisce il cargo ( es armi , munizioni...)
			this.CargoW=cargoW; // pero del cargo SOLO MATERIALI
			this.CargoP=cargoP; // truppe ( le truppe hanno un peso derivante dal record inserimento per le truppe )
			this.IsEliC = false; // il cargo non parte in elicottero
			this.isLand = false; // il cargo non parte a terra
			this.IsFittable = true; // il cargo viene generato inseribile in elicottero
			CargoVector = new Vector2 (0, 0); // inizializzo un vettore vuoto
			ListCargoTroop = new List<Soldier> (0); // inizializzo una lista vuota
		}


		// stringa informazioni per il cargo
		public string CargoInfo ()
		{
			return "Record_cargo: "+ this.CargoRec + " Tipo Cargo: "+ this.CargoString + " Peso Cargo: "+ this.CargoW + " Utenti necessari: "+this.CargoP;
		}


		// calcola il peso totale del cargo e delle truppe necessarie al cargo
		public int ChecktotalCargoWeight ()
		{
			return  (this.CargoW + this.ListCargoTroop.Count * this.SoldierW);
		}
	
	} // fine classe
} // fine name space

