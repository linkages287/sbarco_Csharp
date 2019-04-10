// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// CLOCK TIMER
// sistema di gestione del timer per la sincronizzazione delle 
// attività. ogni elicottero deve essere dotato di un timer
// opportunamente fasato su un master ( PATTERN OBSERVER)
// che permette la sincronizzazione degli eventi.
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Threading;

namespace EliAssaltoAnfibio
{
	public class ClockTimer : Subject // LA CLASSE EREDITA DA SUBJECT
	{
			
			
			private static ReaderWriterLockSlim wr1 = new ReaderWriterLockSlim();
			private TimeSpan currentTime = TimeSpan.Zero; // default value 00:00:00
			private volatile bool started = false, stopped = true;
			public void SetTime(TimeSpan newTime) { currentTime = newTime; }
			public TimeSpan GetTime() 	{ 	

			try {
				wr1.EnterReadLock();
				return currentTime;
			}
				finally{
						wr1.ExitReadLock();
				}}

			// lista di eventi - REGISTRAZIONE TIMER
		public void Start ()
		{
			if (!started) {
				started = true;
				new Thread (Run).Start (); // carica il thread del metodo run
			}
		}
			
			public void Stop() { stopped = true; }

			private void Run() {
				stopped = false;
					while (!stopped) {
					Thread.Sleep(1000); // pausa 1 secondo
								// posso modificare il moltiplicatore di sleep per variare 
								// la velocità di simulazione dell'intero sistema SPERIMENTALE
						Tick();
				}
			started = false; stopped = true;
			}
	
			private void Tick()
			{
			wr1.EnterWriteLock ();
			currentTime += new TimeSpan(0, 0, 1); // increment: 1 sec
			wr1.ExitWriteLock ();
			Notify(); //METODO DELLA CLASSE SUBJECT: permette l'update di tutti i timer degli elicotteri
			}
	}}

