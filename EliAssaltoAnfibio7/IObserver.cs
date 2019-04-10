//++++++++++++++++++++++++++++++++++++++++++++++++++++
// interfaccia Observer 
// definita per il design patter Observer
// serve un timer di sincronizzazione per le attivita'
//++++++++++++++++++++++++++++++++++++++++++++++++++++
using System;

namespace EliAssaltoAnfibio
{
	public interface IObserver
	{
		void Update(Subject subj);
	}

} // fine name space

