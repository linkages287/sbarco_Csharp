// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// la classe definisce le carateteristiche della landing zone
// la LZ puo' contenere truppe, cargo , elicotteri. 
// le liste vengono create ed inizializzate a 0
// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EliAssaltoAnfibio
{

	public class LandingZone
	{

		public int ShipDistance{set;get;} // indica la distanza dall'unità
		public int HPDistance { set; get;} // indica la distanza dall'holding point
		public List<Soldier> LZSoldierList{set;get;} // definizione di una lista soldati di contenimento
		public List<Cargo> LZCargo { set; get; } // definizione di una lista cargo di contenimento
		public List<Elicottero> LZeliList { set; get;} // elicotteri sulla LZ
		public Point LZposition{ set; get;} // posizione LZ
		public Texture2D LZtexture; // texture LZ
		public Vector2 LZvector;  // vettore LZ
		private float effect=0.22f; // effetto per flikering

		public LandingZone (int dist)
		{
			this.ShipDistance = dist; // viene inizializzata la distanza LZ-Ship
			// la landing zone viene inizializzata in una posizione grafica permissiva
			LZposition = new Point (StaticSimulator.LZposX, StaticSimulator.LZposY); 
			LZSoldierList = new List<Soldier>(0); // la landing è inizializzata a 0 elementi soldato
			LZCargo = new List<Cargo> (0); // la landing zone è inizializzata a 0 elementi cargo
			LZeliList = new List<Elicottero> (0); // crea un alista vuota di elicotteri
		}
	

		// effetto flikering LZ
		public float SpriteEffectMath(){
		
			if (effect > 0.24f) effect = 0.22f;
				else effect= effect+0.01f;
		
			return effect;
		}

	} // fine classe
} // fine namespace

