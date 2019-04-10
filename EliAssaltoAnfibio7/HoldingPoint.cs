// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// indica il punto di attesa in volo per gli elicotteri
// il punto di attesa è una rotta circolare in cui far attendere 
// gli elicotteri in attesa che si verifichino alcuni eventi
// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
using System;
using System.Collections.Generic;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
namespace EliAssaltoAnfibio
{
	public class HoldingPoint
	{

		int radius {set;get;} // raggio di evoluzione dell'holding point 
		int Distanza {set;get;} // indica la distanza dell'holding point dalla nave
		public List<Elicottero> EliHolding{set;get;}// l'holding zone contiene gli elicotteri
		public Vector2 HPvector; // la lz ha un vettore

		public HoldingPoint (int dist) // HP è inizializzato con una distanza dalla nave

		{
			this.Distanza = dist; // viene passala la distanza dal costruttore
			EliHolding = new List<Elicottero>(0); // viene creata la lista di elicotteri preseti e settata a 0
			HPvector = new Vector2 (StaticSimulator.HPposX, StaticSimulator.HPposY); // nuovo vettore per l'Holding point
		}
	} // fine classe
}// fine namespace

