// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// ABSTRACT MANAGER
// nel abstract manager viene definita una classe astratta dalla quale 
// derivare le altre tipologie di classe.
// nella fattispecie adottiamo differenti tipologie di manager
// in funzione degli oggetti che devono essere controllati.
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

using System;
using System.Collections.Generic;

namespace EliAssaltoAnfibio
{
	public abstract class AbstractManager
	{


		
		protected string TypeManager; // indica la tipologia di Manager che verra' implementato
		
		// stato FULL - la lista è piena e non puo' ospitare altri elementi 
		// stato EMPTY - la lista è vuota ma non è detto che possa accettare altri elementi
		// stato CanAll - puo' fare tutto - caricare o scaricare
		public enum StatusManager: int
		{
			Full,
			Empty,
			CanAll
		} // stato lista piena , vuota , lista con alcuni dati
		
		// LO STATUS E' STATO PREVISTO PER SUCCESSIVE IMPLEMENTAZIONI MA MAI USATO
		// ALL'INTERNO DEL PROGRAMMA DETERMINA LO STATO DEL MANAGER DI GESTIONE
		// stato ReadyToSend - pronto per inviare elementi ad un altro manager
		// stato ReadyToReceive - pronto per ricevere elementi da un altro manager
		// stato ReadyToSendAndReceive - pronto per inivare o ricevere
		// stato wait - stato attesa
		public enum ActionManager: int
		{
			ReadyToSend ,
			ReadyToReceive ,
			ReadyToSendAndReceive,
			Wait
		} // lo stato di azione del manager
		
		public AbstractManager (string tmanager)
		{ 

			TypeManager = tmanager;
		}


		// definizione dei metodi comuni alle classi 
		
		public abstract void MakeList (int i) ;
		
		// inserisci un elemento nella lista
		public abstract void InsertElement (Object i);
		
		
		//rimuovi un elemento dalla lista REMOVEAT INDEX
		public abstract  void RemoveElement (int i);



		//controlla lo stato della lista
		public abstract void CheckState ();
	}
}

