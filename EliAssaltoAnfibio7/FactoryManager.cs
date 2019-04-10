//++++++++++++++++++++++++++++++++++++++++++++++
// classe provvisoria si tenta di creare un class 
// maker per le differenti tipologie
// di manager degli elicotteri
//++++++++++++++++++++++++++++++++++++++++++++++

using System;

namespace EliAssaltoAnfibio
{
	// classe statica del factory manager, serve per la creazione 
	// di un oggetto di tipo manager necessario.

	static public class FactoryManager
	{
		// le classi statiche non hanno un costruttore
		
		// con la variabile elementi viene indicato il numero iniziale di elementi che dovra' vestire il manager.
		public static AbstractManager Make(int id, int elementi, InfoWindow winI)
		{
			
			switch(id)
			{
			case 0: default: return EliManager.Instance(elementi, winI);// manager per elicotteri
				
			case 1: return TroopsManager.Instance(elementi, winI);// manager per truppe
				
			case 2: return SpotManager.Instance(elementi, winI);// manager per spot
			
			case 3: return CargoManager.Instance(elementi, winI);//manager per cargo
			
			}	
		}
	} // fine classe
} // fine name space

