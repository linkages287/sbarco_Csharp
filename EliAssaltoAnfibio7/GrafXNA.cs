//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// CLASSE DI GESTIONE GRAFICA
// SI OCCUPA DEL LOOP DI DISEGNO DEGLI SPRITE 
// IN MODO CHE POSSANO ESSERE VISUALIZZATI A SCHERMO
// ELICOTTERI CARGO TRUPPE LZ , BACKGROUND E INFORMAZIONI
// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EliAssaltoAnfibio
{
	public class GrafXNA: Microsoft.Xna.Framework.Game
	{
		
		// spazio caratteri info eli
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		//-----------------------------------------------------
		Texture2D background;
		Vector2 backgroudPOS;
		//	Texture2D Troopstexture; // truppe shape
		Texture2D RectangleTexture; // rettangolo shape
		Texture2D bar; // barra visualizzazione finale
		Texture2D CargoTexture; // cargo shape
		Texture2D lineTexture; // linea
		//-----------------------------------------------------
		private SpriteFont font;
		private SpriteFont font2;

		int i = 0; // var indice eli sprite

		float elapsedTime; // elapsed timer 1
		float elapsedTime1; // elapsed timer 2
		float elapsedtimeRotationEli; // eli rotation frame

		SimMover SimMovL;

		// velocità di movimento rotazionale
		public GrafXNA (SimMover simM) // costruttore
		{
			graphics = new GraphicsDeviceManager (this); // costruzione del manager grafico

			Window.AllowUserResizing = true;			//costruisci con allow resizing viene resettata nell'inizializzazione
			Window.BeginScreenDeviceChange (true);

			this.SimMovL = simM; // contiene tutte le informazioni di gestione

			Content.RootDirectory = "Content";		   // directory contente i multimedia
		}
		// METODO DI INIZIALIZZAZIONE: viene utilizzato per inizializzare i valori e gli oggetti grafici
		protected override void Initialize () // metodo eseguito all'inizio del programma
										 // serve per inizializzare le variabili
		{
			//inizializzazione variabili grafiche per deifinizione shcermo
			graphics.PreferredBackBufferWidth = 1024;   // impostazione schermo
			graphics.PreferredBackBufferHeight = 768;   // impostazione schermo
			GraphicsDevice.PresentationParameters.BackBufferWidth = 1024; //IMPOSTAZIONI GRAFICHE
			GraphicsDevice.PresentationParameters.BackBufferHeight = 768; // IMPOSTAZIONI GRAFICHE

			spriteBatch = new SpriteBatch (GraphicsDevice); // nuovo oggetto spritebatch per la visualizzazione
			backgroudPOS = new Vector2 (0f, 0f); // posizione iniziale del background img
			// inizliazzazione posiz LZ
			SimMovL.EliM.LandingZoneL.LZvector = new Vector2 (SimMovL.EliM.LandingZoneL.LZposition.X, SimMovL.EliM.LandingZoneL.LZposition.Y);

			int disposerContent = (StaticSimulator.shipY1Y2spacing) / SimMovL.EliM.initEliNum; // calcolo degli interspazi sul ponte
			// per la disposizione grafica degli elicotteri
			// poisiziono elicotteri sul ponte
			foreach (Elicottero eli in SimMovL.EliM.ElicotteriList) {
				eli.rotation = StaticSimulator.SetRotationAngle (StaticSimulator.InitialRotationAsset); // prende gradi 

				eli.elivector2d = new Vector2 (eli.elivector2d.X + StaticSimulator.shipborderLandX , 
					eli.elivector2d.Y + StaticSimulator.shipborderLandYdown- 20- (eli.NumEli) * disposerContent);
			}

			Window.AllowUserResizing = true; // l'untente pu' dimesionare la finestra
			graphics.IsFullScreen = true; 	 // simulazione windowed	
			graphics.ApplyChanges ();		// applico i cambiamenti alla grafica
			base.Initialize ();
		}
		// METODO DI CONTENT: viene utilizzato per caricare le informazioni gfrafiche
		// immagini, suono, filmati...
		protected override void LoadContent () // solo grafica
		{
			// All’interno del metodo LoadContent() ci sono
			//tutte le chiamate ai file media da caricare. 
			background = Content.Load<Texture2D> ("terra4"); // sfondo
			SimMovL.EliM.EliTexture[0] = Content.Load<Texture2D> ("Helicopter80_1");//elicottero pale aperte
			SimMovL.EliM.EliTexture[1] = Content.Load<Texture2D> ("Helicopter80_2");//elicottero pale aperte
			SimMovL.EliM.EliTexture[2] = Content.Load<Texture2D> ("Helicopter80_3");//elicottero pale aperte
			SimMovL.EliM.EliTexture[3] = Content.Load<Texture2D> ("Helicopter80_4");//elicottero pale aperte
			SimMovL.EliM.EliTexture[4] = Content.Load<Texture2D> ("Helicopter80_5");//elicottero pale aperte
			SimMovL.EliM.EliTexture[5] = Content.Load<Texture2D> ("Helicopter80x");//elicottero pale aperte
			SimMovL.EliM.EliTexture[6] = Content.Load<Texture2D> ("HeliFolded"); // elicottero pale piegate
			SimMovL.EliM.CrashedEli = Content.Load<Texture2D> ("skull"); // crash eli
			bar = Content.Load<Texture2D> ("barj"); // barra visualizzazione finale
			RectangleTexture = Content.Load<Texture2D> ("green-rectangle-hi");// rettangolo
			SimMovL.EliM.LandingZoneL.LZtexture = Content.Load<Texture2D> ("LZ1"); // landing zone
			SimMovL.TroopM.Troopstexture = Content.Load<Texture2D> ("troops.png"); // truppe	
			font = Content.Load<SpriteFont> ("Sprite2"); // carico il font
			font2 = Content.Load<SpriteFont> ("Sprite5");//
			CargoTexture = Content.Load<Texture2D> ("pallet60");
			lineTexture = Content.Load<Texture2D> ("line");
		}
		// Quando si deve liberare la memoria di determinati
		//oggetti, va utilizzato questo metodo che viene esegui-
		//to nel momento in cui la classe che lo contiene viene
		//chiusa.
		protected override void UnloadContent ()
		{
			Content.Unload ();
		}



		#region [UPDATE LOGIC LOOP]
		//-------------------------------------------UPDATE LOGIC--------------------------------------------

		// logica di update per la grafica dle simulatore
		// LA PARTE DI UPDATE LOGICO NON GRAFICO VIENE DELEGATA ALLA CLASSE SimMover
		protected override void Update (GameTime gameTime)
		{
			if (!SimMovL.EndProgramState)
			{
				i++;	if (i == 5) i = 0; // sprite COUNTER , VARIA lo sprite degli elicotteri in volo per dare l'effetto pale rotanti

				SimMovL.LoopforUpdate (elapsedTime1); // LOOP UPDATER - e' il cuore della gestione logica 
										 // viene effettuato un update logico di truppe cargo elicotteri e posizioni
										//vengono determinate le posizioni e cosa fare
										// la gestione è lasciata al simumover, la classe di logica di movimento e simulazione
				base.Update (gameTime);
			}
		}

		//------------------------------------------------------------------------------------------------
		#endregion [FINE UPDATE LOGIC]


		// DRAWING-----------------------------------------------------------------------------------------
		// metodo di disegno contiene le immagini da disegnare
		// l'ordine è importante in quanto definisce la visuale layerizzata
		protected override void Draw (GameTime gameTime) // loop di disegno
		{
			GraphicsDevice.Clear (Color.TransparentBlack);
			// logica di update per la grafica dle simulatore
						elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // elapsed per speed 
						elapsedTime1 =(float)gameTime.ElapsedGameTime.TotalSeconds; // lapsed fot eli
						elapsedtimeRotationEli =(float)gameTime.ElapsedGameTime.TotalSeconds; 

			#region [INIZIO DRAWING LOOP]

			spriteBatch.Begin (); //inizio spritebatch
			// null indica valori di default


			// disegna LZ
			if(!SimMovL.EndProgramState)
			{
				//disegna sfondo
					spriteBatch.Draw (background, GraphicsDevice.Viewport.Bounds, Color.White);

				spriteBatch.Draw (SimMovL.EliM.LandingZoneL.LZtexture,SimMovL.EliM.LandingZoneL.LZvector, 
									null, Color.White, 0f, new Vector2 (SimMovL.EliM.LandingZoneL.LZtexture.Width / 2, 
									SimMovL.EliM.LandingZoneL.LZtexture.Height / 2), 
									SimMovL.EliM.LandingZoneL.SpriteEffectMath (), SpriteEffects.None, 0f); 

				// L'ORDINE DI DISEGNO CONTA
				this.DrawGlobalInfo (); // disegna info GENERALI
				this.drawSold_Cargo (); // disegna info OPZIONALI ELI
				this.drawEli (); // disegna elicotteri

			}
				else  this.WriteScreenEndLoopInfo (); // se la simulazione è terminata mostra i risultati
		
				this.checkKey (); // keyboard posso regolare alcuni parametri
				spriteBatch.End (); // termine spritebatch
		#endregion
			base.Draw (gameTime);
				
		}
		//----------------------------------------------------------------------------------------------------------

	
		#region [CHECK CONTROL KEY SECTION] 
		// vivne usate per modificare alcuni parametri all'interno della simulazione
		// regolo alcuni parametri tramite la tastiera
		void checkKey ()
		{

			if (Keyboard.GetState ().IsKeyDown (Keys.PageUp)) {
				if (SimMovL.Speed < 160) SimMovL.Speed++;

			}
			if (Keyboard.GetState ().IsKeyDown (Keys.PageDown))
			if (SimMovL.Speed > 60) SimMovL.Speed--;


			// premi escape per uscire dalla scermata grafica
			if (Keyboard.GetState ().IsKeyDown (Keys.Escape)) 
			{
				SimMovL.EliM.MainTime.Stop(); // stop timer
				Gtk.Application.Quit ();
				Exit ();
			}
		}
		#endregion
		
		// DISEGNA GLI ELICOTTERI
		void drawEli ()
		{
		


			// draw crashed eli
			if (SimMovL.EliM.CrashList.Count>0)
				foreach (Elicottero eli in SimMovL.EliM.CrashList)
					spriteBatch.Draw ( SimMovL.EliM.CrashedEli, eli.elivector2d, null, Color.White, -MathHelper.PiOver2 , 
						new Vector2 (SimMovL.EliM.CrashedEli.Width / 2, SimMovL.EliM.CrashedEli.Height / 2),
						0.6f, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video

			// DISEGNO ELICOTTERI
			foreach (Elicottero eli in SimMovL.EliM.ElicotteriList) {
			
				if (eli.PosEli != false) {// se poseli == false helo in hangar NON VISIBILE
					//eli pale paerte
					if (eli.PosEli && !eli.IsFlying && eli.isRequiredForOP &&eli.IsBladeSpread) { // posizione sul ponte eli non vola
						spriteBatch.Draw (SimMovL.EliM.EliTexture[5], eli.elivector2d, null, Color.White, eli.rotation, 
							new Vector2 (SimMovL.EliM.EliTexture[5].Width / 2, SimMovL.EliM.EliTexture[5].Height / 2), 
							0.7f, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video
					}
					// eli pale piegate
					if (eli.PosEli && !eli.IsFlying && eli.isRequiredForOP &&!eli.IsBladeSpread) { // posizione sul ponte eli non vola
						spriteBatch.Draw (SimMovL.EliM.EliTexture[6], eli.elivector2d, null, Color.White, eli.rotation, 
							new Vector2 (SimMovL.EliM.EliTexture[6].Width / 2, SimMovL.EliM.EliTexture[6].Height / 2), 
							0.7f, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video
					}
					// se l'elicottero vola livellato e non è in holding
					if (eli.IsFlying && !eli.IsHolding && !eli.isOnDestination && !eli.isLZ) { 

						// eli in volo
						spriteBatch.Draw (SimMovL.EliM.EliTexture[i], eli.elivector2d, null, Color.White, eli.rotation, 
							new Vector2 (SimMovL.EliM.EliTexture[i].Width / 2,SimMovL.EliM.EliTexture[i].Height / 2),
							 eli.quota, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video
					}

					// eli IS holding
					if (eli.IsHolding) { // il vettore di rotazione permette la rotazione sul centro dell'immagine
						spriteBatch.Draw (SimMovL.EliM.EliTexture[i], eli.elivector2d, null, Color.White, eli.rotation, 
							new Vector2 (0, 100), eli.quota, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video
					}

					// elicottero sulla LZ
					if (eli.isLZ) {
						spriteBatch.Draw (SimMovL.EliM.EliTexture[i], eli.elivector2d, null, Color.White, eli.rotation, 
							new Vector2 (SimMovL.EliM.EliTexture[i].Width / 2, SimMovL.EliM.EliTexture[i].Height / 2),
							 0.5f, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video
					}
				} // termine if NON VISUALIZZA NULLA SE NON CI SONO ELICOTTERI SUL PONTE
				this.DrawEliInfo (eli);
			}
		}

		// DISEGNO CARGO SU LZ
		void drawSold_Cargo ()
		{

			// mostra il cargo sulla LZ
			foreach (Cargo cargo in SimMovL.EliM.LandingZoneL.LZCargo) {
				if (cargo.isLand)
					spriteBatch.Draw (CargoTexture, cargo.CargoVector, null, Color.White, 0f, 
						new Vector2 (CargoTexture.Width / 2, CargoTexture.Height / 2),
						 0.5f, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video

			}

			// mostra le truppe sulla LZ
			foreach (Soldier troop in SimMovL.EliM.LandingZoneL.LZSoldierList) {
				if (troop.IsLand)
					spriteBatch.Draw (SimMovL.TroopM.Troopstexture, troop.vectoreTroop, null, Color.White, 0f, 
						new Vector2 (SimMovL.TroopM.Troopstexture.Width / 2, SimMovL.TroopM.Troopstexture.Height / 2), 
						0.35f, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video
			}
		}
		// ritorna vero se il cargo esiste ed è 0 count
		// il metodo serve per i dati su mission failed
		private bool CheckCargoBoolZero()
		{
			if (SimMovL.CargoM != null && SimMovL.CargoM.CargoList != null) 
			{
				if (SimMovL.CargoM.CargoList.Count == 0) return true;
				else return false;
			} 
				else  return true;
				}
		
		// DRAW END LOOP DATA INFORMATIONS
		// IL METODO SERVE PER SCRIVERE E FORMATTARE I DATI VISIBILI A FINE SIMULAZIONE
		// I DATI VENGONO PRELEVATI DA UNA LISTA DI STRUTTURE IN CUI SONO STATI SALVATI
		// I DATI FONDAMENTALI DELLA SIMULAZIONE.
		void WriteScreenEndLoopInfo ()
		{
		
			int totInfo = SimMovL.WinI.infoTimeList.Count - 2; // numero totale di elementi info per eli senza start e fine
			int deY = 768 / (SimMovL.EliM.initEliNum+1); // delta Y per rappresentazione

			int timeD = ((int)SimMovL.WinI.infoTimeList.Find (x => x.Elirec == -2).timer.TotalSeconds / 20)+1; // blocchi di 20 minuti
			int deX = 900 / timeD; // lunghezza in px di un blocco da 20 min 
			float minPix = (float)deX / 20; // lunghezza in pix di 1 min

			if (SimMovL.TroopM.TroopList.Count == 0 && CheckCargoBoolZero ()) {
				 // se il numero di truppe da trasportare ==0 e il cargo è == 0
				// simulazione terminata con successo altrimenti simulazione fallita

				int y = 0; // indicatore per posizione
				int y1 = 0; // indicatore per posizione
				for (int v = 1; v <= timeD; v++) { // disegno le barre verticali che designano blocchi da 20 minuti
					spriteBatch.Draw (lineTexture, new Viewport (new Rectangle (v * deX +30 , 0, 3, 768)).Bounds, Color.White);
					spriteBatch.DrawString (font, "T: " + 20 * v, new Vector2 (v * deX+30, 10), Color.LightGreen);
				}

				for (int w = 0; w < SimMovL.EliM.initEliNum; w++)
				 {  // disegno i rettangoli orizzonatali

					spriteBatch.Draw (bar, new Viewport (new Rectangle (0, deY * (w+1) - 85+30, 1024, 60)).Bounds, Color.White);
					spriteBatch.DrawString (font,"ELI#"+ SimMovL.WinI.infoTimeList.Find(x=>x.Elirec==w).Elirec ,
						new Vector2 (10,deY *(w+1) - 65+ 30), Color.Red);
					// disegno le informazioni necessarie in ogni barra orizzonatale
					foreach (InfoWindow.timeInfoStruct timeStr in  ( SimMovL.WinI.infoTimeList.FindAll(x=>x.Elirec==w))) {
						if (y >= 24)y = 0; else y=y+8; // correzione pos scritta
						if (y1 >2) y1 = 0; else y1=y1+1; // correzione pos scritta
						spriteBatch.DrawString (font2, timeStr.timer.TotalSeconds.ToString(), 
							new Vector2 ((float)timeStr.timer.TotalSeconds*minPix + 30, deY *(w+1)-10-y1*5), 
							Color.Blue,0f, new Vector2(0,0),0.8f,SpriteEffects.None,0f); // draw timers

						spriteBatch.DrawString (font2, timeStr.info, 
							new Vector2 ((float)timeStr.timer.TotalSeconds*minPix + 30, deY * (w+1) - 80+y+25), 
							Color.Red,0f, new Vector2(0,0),0.8f,SpriteEffects.None,0f);// draw info
					}
				}
			
			} // end if
			// altrimenti la simulazione è FALLITA
			else spriteBatch.DrawString (font, " SIMULAZIONE FALLITA ", new Vector2 (50, 30), Color.GhostWhite);
	}
		
		// SCRITTURA INFORMAZIONI GLOBALI IN ALTO A DESTRA
		void DrawGlobalInfo ()
		{
			spriteBatch.Draw (RectangleTexture, new Vector2(71,104), null, Color.White, MathHelper.PiOver2, 
				new Vector2 (RectangleTexture.Width / 2, RectangleTexture.Height / 2),
				 0.38f, SpriteEffects.None, 0f);//mostra l'immagine degli elicotteri su video
			spriteBatch.DrawString (font, "MINUTI TRASCORSI: "+(int)this.SimMovL.EliM.MainTime.GetTime ().TotalSeconds, 
				new Vector2 (10, 10), Color.Black);
			spriteBatch.DrawString (font, "SPOT STATUS: " + SimMovL.SpotM.Stato.ToString (),
				 new Vector2 (10, 30), Color.Black);
			spriteBatch.DrawString (font, "TROOPS STATUS: " + SimMovL.TroopM.Status.ToString (), 
				new Vector2 (10, 50), Color.Black);
			spriteBatch.DrawString (font, "HANGAR STATUS: " + SimMovL.EliM.Stato.ToString (), 
				new Vector2 (10, 70), Color.Black);
			spriteBatch.DrawString (font, "TRUPPE RESTANTI: " + SimMovL.TroopM.TroopList.Count, 
				new Vector2 (10, 90), Color.Black);
			spriteBatch.DrawString (font, "TRUPPE SU LZ: " + SimMovL.EliM.LandingZoneL.LZSoldierList.Count, 
				new Vector2 (10, 110), Color.Black);
			spriteBatch.DrawString (font, "FORMATION SPEED: " + SimMovL.Speed, 
				new Vector2 (10, 130), Color.Black);
			spriteBatch.DrawString (font, "HOLDING: " + SimMovL.EliM.HoldingP.EliHolding.Count, 
				new Vector2 (10, 150), Color.Black);
			if (SimMovL.CargoM != null)
				spriteBatch.DrawString (font, "CARGO SU LZ: " + SimMovL.EliM.LandingZoneL.LZCargo.Count,
					 new Vector2 (10, 170), Color.Black);
			else spriteBatch.DrawString (font, "CARGO SU LZ: null", new Vector2 (10, 170), Color.Black);
		
			if (SimMovL.CargoM != null)
				spriteBatch.DrawString (font, "CARGO RESTATE: " + SimMovL.CargoM.CargoList.Count,
					 new Vector2 (10, 190), Color.Black);
			else spriteBatch.DrawString (font, "CARGO RESTANTE: null", new Vector2 (10, 190), Color.Black);

			// disegna le scritte per gli elicotteri crashati
			if (SimMovL.EliM.CrashList.Count > 0) {
				spriteBatch.DrawString (font, "ELI CADUTI: " + SimMovL.EliM.CrashList.Count, new Vector2 (150, 10), Color.Red);
				// uomini morti e cargo perso scrittura dati
				int i = 0; int k = 0;
				foreach (Elicottero eli in SimMovL.EliM.CrashList) {
					i = i + eli.EliSoldierList.Count;
					k = k + eli.EliCargoList.Count;
				}
				spriteBatch.DrawString (font, "UOMINI MORTI: " +i , new Vector2 (150, 30), Color.Red);
				spriteBatch.DrawString (font, "CARGO PERSO: " +k , new Vector2 (150, 50), Color.Red);
			}
		}
		
		// DRAW ELI INFORMATIONS -- PREMERE IL TASTO CORRISPONDENTE AL RECORD DELL'ELICOTTERO PER VISUALIZZA LE INFORMAZIONI

		void DrawEliInfo (Elicottero eli)
		{
			if (eli.LowFuel) 
				spriteBatch.DrawString (font2, "LOW FUEL" , new Vector2 (eli.elivector2d.X -30, eli.elivector2d.Y), Color.Blue);
			if (eli.isHotRef)
				spriteBatch.DrawString (font2, "REFUELING", new Vector2 (eli.elivector2d.X +30, eli.elivector2d.Y), Color.Blue);
			if (eli.IsBoarding)
				spriteBatch.DrawString (font2, "BOARDING TRUPPE", new Vector2 (eli.elivector2d.X-15, eli.elivector2d.Y+30), Color.Blue);
			if (eli.IsBoardingCargo)
				spriteBatch.DrawString (font2, "BOARDING CARGO", new Vector2 (eli.elivector2d.X-15, eli.elivector2d.Y+30), Color.Blue);
			if (eli.hasSpotReserved && !eli.PosEli)
				spriteBatch.DrawString (font2, eli.IdEli+" IN MOVIMENTAZIONE", new Vector2 (eli.elivector2d.X-30, eli.elivector2d.Y-15), Color.Blue);
			if (!eli.IsRunning && eli.PosEli)
				spriteBatch.DrawString (font2, eli.IdEli+" PRE_START CHECKS", new Vector2 (eli.elivector2d.X-30, eli.elivector2d.Y+15), Color.Blue);
			if (eli.IsRunning && !eli.IsBladeSpread)
				spriteBatch.DrawString (font2, eli.IdEli+" BLADE SPREADING", new Vector2 (eli.elivector2d.X-30, eli.elivector2d.Y+15), Color.Blue);

			int keypress=10;
		
			Keys[] keyArray = (Keyboard.GetState().GetPressedKeys()); // pressione dei tasti numpad 1 - 9 per visualizzazione info elicottero
			if (keyArray.Length > 0)
			 { 
				if (keyArray [0] >= Keys.NumPad1 && keyArray [0] <= Keys.NumPad9) {
					switch (keyArray [0]) {
					case Keys.NumPad1:
						keypress = 1; break;
					case Keys.NumPad2:
						keypress = 2;break;

					case Keys.NumPad3:
						keypress = 3;break;
					case Keys.NumPad4:
						keypress = 4;break;
					case Keys.NumPad5:
						keypress = 5;break;
					case Keys.NumPad6:
						keypress = 6;break;
					case Keys.NumPad7:
						keypress = 7;break;
					case Keys.NumPad8:
						keypress = 8;break;	
					case Keys.NumPad9:
						keypress = 9;break;	
					}
				}
			}

			if (eli.NumEli==keypress-1 )
			{ 
				spriteBatch.DrawString (font2, eli.IdEli, new Vector2 (eli.elivector2d.X -10, eli.elivector2d.Y), Color.Blue); // nome eli

				spriteBatch.DrawString (font2, "RUNNING: " + eli.IsRunning.ToString (), new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y - StaticSimulator.spacing), Color.Red);
				spriteBatch.DrawString (font2, "BLADE SPREAD: " + eli.IsBladeSpread.ToString (), new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y - StaticSimulator.spacing * 2), Color.Red);
				spriteBatch.DrawString (font2, "READY: " + eli.isREADYstatus.ToString (), new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y -  StaticSimulator.spacing * 3), Color.Red);
				spriteBatch.DrawString (font2, "REQUIRED: " + eli.isRequiredForOP, new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y -  StaticSimulator.spacing * 4), Color.Red);
				spriteBatch.DrawString (font2, "FUEL: " + eli.Fuel, new Vector2 
					(eli.elivector2d.X +20, eli.elivector2d.Y -  StaticSimulator.spacing * 5), Color.Red);

				spriteBatch.DrawString (font2, "FLY: " + eli.IsFlying, new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y -  StaticSimulator.spacing), Color.Red);
				spriteBatch.DrawString (font2, "BLOCKED: " + eli.IsBlocked, new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y - StaticSimulator.spacing * 2), Color.Red);
				spriteBatch.DrawString (font2, "IS HOLDING: " + eli.IsHolding, new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y -  StaticSimulator.spacing* 3), Color.Red);
				spriteBatch.DrawString (font2, "TRUPPE : " + eli.EliSoldierList.Count, new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y - StaticSimulator.spacing*4), Color.Red);
				if (SimMovL.CargoM != null)
				spriteBatch.DrawString (font2, "CARGO : " + eli.EliCargoList.Count, new Vector2 
						(eli.elivector2d.X -110, eli.elivector2d.Y -  StaticSimulator.spacing * 5), Color.Red);


				spriteBatch.DrawString (font2, "DIREZIONE: " + eli.DirToGo.ToString (), new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y +  StaticSimulator.spacing), Color.Red);
				spriteBatch.DrawString (font2, "LOW FUEL: " + eli.LowFuel, new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y +  StaticSimulator.spacing * 2), Color.Red);
				spriteBatch.DrawString (font2, "ELI FULL: " + eli.IsFull, new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y +  StaticSimulator.spacing * 3), Color.Red);
				spriteBatch.DrawString (font2, "ANGLE toFLY: " + eli.AngleToFly, new Vector2
					 (eli.elivector2d.X - 110, eli.elivector2d.Y +  StaticSimulator.spacing * 4), Color.Red);
				spriteBatch.DrawString (font2, "IS SPEED: " + eli.EliSpeed, new Vector2 
					(eli.elivector2d.X - 110, eli.elivector2d.Y +  StaticSimulator.spacing * 5), Color.Red);


				spriteBatch.DrawString (font2, "FREE W CARGO: " + eli.WCargoLeftOnBoard, new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y +  StaticSimulator.spacing), Color.Red);
				spriteBatch.DrawString (font2, "FREE W TROOPS: " + eli.WTroopsLeftOnBoard, new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y +  StaticSimulator.spacing * 2), Color.Red);
				spriteBatch.DrawString (font2, "ANGOLO ATTUALE: " + eli.rotation, new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y +  StaticSimulator.spacing * 3), Color.Red);
				spriteBatch.DrawString (font2, "DISTANZA AL PUNTO: " + eli.DIstanceToPnT, new Vector2 
					(eli.elivector2d.X + 20, eli.elivector2d.Y +  StaticSimulator.spacing * 4), Color.Red);
			}
		} // termine metodo draw info
	}
// termina classe XNA
}
// termina name space
