// ++++++++++++++++++++++++++++++++++++++++++++++++
// pattern observer per il timer degli elicotteri
// contiene gli attach e detach per l'osservatore
//+++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{

	public abstract class Subject
	{
		private List<IObserver> observers = new List<IObserver>();
		public void Attach(IObserver obs) { observers.Add(obs); }
		public void Detach(IObserver obs) { observers.Remove(obs); }
		public void Notify()
			{
			foreach (IObserver obs in observers) // viene essettuato l'udate di ogni elemento 
												// nella lista di Observers
			obs.Update(this);

		}
	}
}

