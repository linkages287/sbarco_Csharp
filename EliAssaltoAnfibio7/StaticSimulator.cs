// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// classe di gestione statica del simulatore
// contiene le costanti statiche di posizione degli oggetti 
// sullo schermo, e contiene i metodi per il calcolo del volo
// degli elicotteri 
// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{
	public static class StaticSimulator
	{
		// costanti utilizzate nella simulazione
		public const int LZpixelDistance = 700;// distanza in pixel ship lz
		public const float angleMov = (MathHelper.Pi / 360f)*2; // all'interno dei 2 gradi
		public const int CargoLoadTiming = 5; // tempo per operazioni di cargo load
		public const int CargoTimeTodisembark = 2; // tempo richiesto per disimbarcare il cargo
		public const int BladeSpreadBlockingTime=2; // tempo di blocco per l'apertura pale
		public const int timeNeedToEngage = 10; //  minuti per la messa in moto dell'elicottero
		public const int timeNeedToMoveToDeck = 15; // 15 minuti per muove l'elicottero sul ponte dall'hangar
		public const int timeToEmbarcTroops = 3; //3 minuti per l'imbarco truppe
		public const int timeToDisembarkTroops = 2;// 2 minuti scarico truppe
		public const int spacing = 12; // spaziatura caratteri
		public const int shipborderLandX = 865;// X di appontaggio degli elicotteri sulla nave 865
		public const int shipborderLandYup = 210;// min Y di appontaggio
		public const int shipborderLandYdown = 590;//max Y di appontaggio
		public const int shipY1Y2spacing = 350; // max - min Y di appontaggio
		public const int InitialRotationAsset = 240; // ni gradi la posizione iniziale degli elicotteri
		public const int HPposX = 750;// posizione X holding point
		public const int HPposY = 430;// posizione Y holding point
		public const int LZposX = 155;// posizione X LZ point
		public const int LZposY = 390;// posizione Y LZ point
		public const int CargoVectorPosX = 25; // posizione relativa X cargo vector
		public const int CargoVectorPosY = -20; // posizione relativa Y cargo vector
		public const int TroopsVectorPosX = 25; // posizione relativa X truppe vector
		public const int TroopsVectorPosY = -20; // posizione relativa Y truppe vector

		// gruppo costanti utilizzate nella simulazione
		//-----------------------------------------------------------------------
		

		#region [FLY MATH]
		// calcolo della velcità degli elicotti, movimento in pixel in base 
		// alla distanza dalla LZ
		public static float SpeedinPixMin (float speed , int distance)
		{	
			float pixVal = 0;

			try{
				pixVal =	(StaticSimulator.LZpixelDistance /distance); // ogni viglio corrisponde a X pixel calcolati
			}
			catch 
			{
				System.Console.WriteLine ("ERRORE CALCOLO DISTANZA"); // division by 0?
			}
			// il simulatore temporizza ogni MINUTO nel mondo reale come se fosse un secondo sul simulatore

				float speedVal = (speed / 60); // quante miglia faccio in un minuto
				return speedVal * pixVal; // pixel che devono essere proporzionalmente percorsi in 1 secondo

		}

		// generatore di interi randomici fino al max inserito
		public static int RandomIntGenerator (int max )
		{
			Random rnd = new Random ();
			int i= rnd.Next(max);
			return i;
		}
		
		//calcola la distanza tra due punti
		public static int DistancePtoP (float x1pos, float y1pos, float x2pos, float y2pos)
		{
			float Xdelta, Ydelta;
			Xdelta = Math.Abs (x1pos - x2pos);
			Ydelta = Math.Abs (y1pos - y2pos);

			return (int)Math.Sqrt (Xdelta * Xdelta + Ydelta * Ydelta);

		}
		// calcola l'angolo per la nuova posizione
		public static float AngleFlyTo (float xpos, float ypos, Elicottero eli)
		{
			float Xdelta, Ydelta, val;

			Xdelta = xpos - eli.elivector2d.X;
			Ydelta = ypos - eli.elivector2d.Y;
			// calcolo l'angolo tra l'elicottero ed il punto di arrivo

			val = ((float)(Math.Atan ((Ydelta) / (Xdelta))));

			//	if (Xdelta > 0 && Ydelta > 0)
			//	val = val;

			if (Xdelta < 0 && Ydelta <= 0)
				val = val + MathHelper.Pi;

			if (Xdelta < 0 && Ydelta > 0)
				val = val + MathHelper.Pi;

			//----------------------------
			//val = val + MathHelper.Pi;

			if (val < 0)
				val = val + MathHelper.TwoPi;

			if (val > MathHelper.TwoPi)
				val = val - MathHelper.TwoPi;

			return val;
		}
		//-----------------------------------------------------------------------
		// setto la correzione dell'angolo del vettore eli
		// con imput la direzione voluta
		public static float AngleCorrection (Elicottero eli, float directionAngle)
		{
			// rinormalizza gli angoli all'interno del range 0/2pi
			if (eli.rotation >= MathHelper.TwoPi)
				eli.rotation = eli.rotation - MathHelper.TwoPi;

			if (directionAngle >= MathHelper.TwoPi)
				directionAngle = directionAngle - MathHelper.TwoPi;

			if (eli.rotation < 0) eli.rotation = eli.rotation + MathHelper.TwoPi; // trasla di +2pi
			if (directionAngle < 0) directionAngle = directionAngle + MathHelper.TwoPi; //trasla di +2pi


			float delta = eli.rotation - directionAngle; // delta angle

			// normalizza il delta per valori 0 / 2pi
			if (delta < 0)
				delta = delta + MathHelper.TwoPi;
			if (delta > MathHelper.TwoPi)
				delta = delta - MathHelper.TwoPi;


			float dirPlusPi = eli.rotation + MathHelper.Pi;
			if (dirPlusPi < 0)
				dirPlusPi = dirPlusPi + MathHelper.TwoPi;
			if (dirPlusPi > MathHelper.TwoPi)
				dirPlusPi = dirPlusPi - MathHelper.TwoPi;


			if (delta <= MathHelper.Pi && delta > angleMov) {
				eli.rotation = eli.rotation - angleMov;
				return eli.rotation ;
			}

			if (delta > MathHelper.Pi && delta > angleMov) {
				eli.rotation  = eli.rotation  + angleMov;
				return eli.rotation ;

			}

			return eli.rotation;
		}
		//-----------------------------------------------------------------------
		// trasforma il valore da radianti a gradi
		public static float SetRotationAngle (float f )
		{
			return  MathHelper.ToRadians (f);
		}

		// effettua il calcolo per muovere l'elicottero
		// calcolo il vettore , lo normalizzo e gli assegno una velocità
		public static void MoveEli (float speed, Elicottero eli, float x, float y, float elapsed)
		{	
			eli.AngleToFly = AngleFlyTo (x, y, eli);
			Vector2 direction = new Vector2 ((float)Math.Cos (AngleCorrection (eli,eli.AngleToFly)), 
			(float)Math.Sin (AngleCorrection (eli, eli.AngleToFly))); // calcolo del vettore di movimento
			direction.Normalize (); // normalizzazione del vettore , 1 unità

			eli.elivector2d +=  direction * SpeedinPixMin (eli.EliSpeed, eli.DistancetoRun) * elapsed ;
		}
		//-----------------------------------------------------------------------
		#endregion
	} // fine classe
} // fine name space

