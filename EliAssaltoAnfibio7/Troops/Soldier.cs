// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// classe soldato : descrive gli attributi di ogni soldato
// ogni singolo soldato che deve essere caricato 
// a bordo dell'elicottero per effettuare lo sbarco
// la missione è completata quando truppe e cargo sono sulla LZ
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EliAssaltoAnfibio
{
	public class Soldier
	{

		public int Weight{ set; get;} // peso dell'unità
		public bool IsShip{ set; get;} // truppa a bordo nave, di default la truppa parte a bordo 
		public Vector2 vectoreTroop { set; get;} // indica la posizione dell'ellemento truppa a terra


		public bool IsLand{ set; get;}  // truppa a terra
		public bool IsEli{ set; get;} // truppa in elicottero
		public int EliNum{ set; get;}  // indica il record # del numero di elicottero a cui viene assegnata la struppa
	
		// contrutto della classe soldato
		public Soldier (int weight)
		{
			this.IsShip = true; // i soldati partono sulla nave
			this.Weight= weight; // peso del singolo soldato
			this.IsLand = false;
			this.IsEli = true;
			this.vectoreTroop = new Vector2 (0, 0);
		
		}
	
	} // fine classe
} // fine namespace

