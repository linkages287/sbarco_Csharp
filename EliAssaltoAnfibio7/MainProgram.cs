//+++++++++++++++++++++++++++++++++++++++
// punto iniziale di start del programma
//++++++++++++++++++++++++++++++++++++++

using System;
using Gtk;

namespace EliAssaltoAnfibio
{
	class MainClass
	{

		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = MainWindow.Instance();

			win.Show ();
			win.InformationWin.InsertSomeText( "-- PROGRAMMA PARTITO --");
			Application.Run ();
		}
	}
}
