
// This file has been generated by the GUI designer. Do not modify.
namespace EliAssaltoAnfibio
{
	public partial class CargoOptions
	{
		private global::Gtk.VBox vbox1;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Label label1;
		private global::Gtk.HSeparator hseparator1;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Label label4;
		private global::Gtk.Entry entry1;
		private global::Gtk.HSeparator hseparator5;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Label label2;
		private global::Gtk.HScale hscale1;
		private global::Gtk.VBox vbox2;
		private global::Gtk.HSeparator hseparator2;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Label label3;
		private global::Gtk.HScale hscale2;
		private global::Gtk.HSeparator hseparator3;
		private global::Gtk.VBox vbox4;
		private global::Gtk.HBox hbox4;
		private global::Gtk.HBox hbox7;
		private global::Gtk.Button button1;
		private global::Gtk.Button button2;
		private global::Gtk.HBox hbox5;
		private global::Gtk.Button button3;
		private global::Gtk.HBox hbox6;
		private global::Gtk.Button button4;
		private global::Gtk.Button button6;
		private global::Gtk.HSeparator hseparator4;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget EliAssaltoAnfibio.CargoOptions
			this.TooltipMarkup = "INSERISCI IL NOME DEL CARGO DA TRASPORTARE";
			this.Name = "EliAssaltoAnfibio.CargoOptions";
			this.Title = "CargoOptions";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child EliAssaltoAnfibio.CargoOptions.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = "Record #";
			this.vbox3.Add (this.label1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.label1]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.vbox3.Add (this.hseparator1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hseparator1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Homogeneous = true;
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = "MATERIALE TRASPORTATO";
			this.hbox3.Add (this.label4);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label4]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.entry1 = new global::Gtk.Entry ();
			this.entry1.CanFocus = true;
			this.entry1.Name = "entry1";
			this.entry1.IsEditable = true;
			this.entry1.InvisibleChar = '•';
			this.hbox3.Add (this.entry1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.entry1]));
			w4.Position = 1;
			this.vbox3.Add (this.hbox3);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox3]));
			w5.Position = 2;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hseparator5 = new global::Gtk.HSeparator ();
			this.hseparator5.Name = "hseparator5";
			this.vbox3.Add (this.hseparator5);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hseparator5]));
			w6.Position = 3;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Homogeneous = true;
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.TooltipMarkup = "INSERISCI IL PESO DEL SOLO MATERIALE DA TRASPORTARE";
			this.label2.Name = "label2";
			this.label2.Xpad = 23;
			this.label2.LabelProp = "PESO COMPLESSIVO MATERIALE";
			this.hbox1.Add (this.label2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label2]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.hscale1 = new global::Gtk.HScale (null);
			this.hscale1.CanFocus = true;
			this.hscale1.Name = "hscale1";
			this.hscale1.Adjustment.Upper = 5000D;
			this.hscale1.Adjustment.PageIncrement = 10D;
			this.hscale1.Adjustment.StepIncrement = 1D;
			this.hscale1.DrawValue = true;
			this.hscale1.Digits = 0;
			this.hscale1.ValuePos = ((global::Gtk.PositionType)(2));
			this.hbox1.Add (this.hscale1);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.hscale1]));
			w8.Position = 1;
			this.vbox3.Add (this.hbox1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
			w9.Position = 4;
			w9.Expand = false;
			w9.Fill = false;
			this.vbox1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.vbox3]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.vbox2.Add (this.hseparator2);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hseparator2]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Homogeneous = true;
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.TooltipMarkup = "INSERISCI IL NUMERO DI PERSONE NECESSARIE AL TRASPORTO";
			this.label3.Name = "label3";
			this.label3.Xpad = 51;
			this.label3.LabelProp = "PERSONALE NECESSARIO AL TRASPORTO";
			this.hbox2.Add (this.label3);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.label3]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.hscale2 = new global::Gtk.HScale (null);
			this.hscale2.CanFocus = true;
			this.hscale2.Name = "hscale2";
			this.hscale2.Adjustment.Upper = 50D;
			this.hscale2.Adjustment.PageIncrement = 10D;
			this.hscale2.Adjustment.StepIncrement = 1D;
			this.hscale2.DrawValue = true;
			this.hscale2.Digits = 0;
			this.hscale2.ValuePos = ((global::Gtk.PositionType)(2));
			this.hbox2.Add (this.hscale2);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.hscale2]));
			w13.Position = 1;
			this.vbox2.Add (this.hbox2);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox2]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hseparator3 = new global::Gtk.HSeparator ();
			this.hseparator3.Name = "hseparator3";
			this.vbox2.Add (this.hseparator3);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hseparator3]));
			w15.Position = 2;
			w15.Expand = false;
			w15.Fill = false;
			this.vbox1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.vbox2]));
			w16.Position = 1;
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.HeightRequest = 47;
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			// Container child hbox4.Gtk.Box+BoxChild
			this.hbox7 = new global::Gtk.HBox ();
			this.hbox7.Name = "hbox7";
			this.hbox7.Spacing = 6;
			// Container child hbox7.Gtk.Box+BoxChild
			this.button1 = new global::Gtk.Button ();
			this.button1.CanFocus = true;
			this.button1.Name = "button1";
			this.button1.UseUnderline = true;
			this.button1.Label = "RECORD PRECEDENTE";
			global::Gtk.Image w17 = new global::Gtk.Image ();
			w17.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-back", global::Gtk.IconSize.Menu);
			this.button1.Image = w17;
			this.hbox7.Add (this.button1);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.button1]));
			w18.Position = 0;
			w18.Expand = false;
			w18.Fill = false;
			// Container child hbox7.Gtk.Box+BoxChild
			this.button2 = new global::Gtk.Button ();
			this.button2.CanFocus = true;
			this.button2.Name = "button2";
			this.button2.UseUnderline = true;
			this.button2.Label = "ACCETTA DATI - TORNA AL MENU PRINCIPALE";
			this.hbox7.Add (this.button2);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.button2]));
			w19.Position = 1;
			w19.Expand = false;
			w19.Fill = false;
			this.hbox4.Add (this.hbox7);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.hbox7]));
			w20.Position = 0;
			w20.Expand = false;
			w20.Fill = false;
			// Container child hbox4.Gtk.Box+BoxChild
			this.hbox5 = new global::Gtk.HBox ();
			this.hbox5.Name = "hbox5";
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.button3 = new global::Gtk.Button ();
			this.button3.CanFocus = true;
			this.button3.Name = "button3";
			this.button3.UseUnderline = true;
			this.button3.Label = "SALVA RECORD E RESETTA";
			this.hbox5.Add (this.button3);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.button3]));
			w21.Position = 0;
			w21.Expand = false;
			w21.Fill = false;
			this.hbox4.Add (this.hbox5);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.hbox5]));
			w22.Position = 1;
			w22.Expand = false;
			w22.Fill = false;
			// Container child hbox4.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox ();
			this.hbox6.Name = "hbox6";
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.button4 = new global::Gtk.Button ();
			this.button4.CanFocus = true;
			this.button4.Name = "button4";
			this.button4.UseUnderline = true;
			this.button4.Label = "ELIMINA RECORD";
			this.hbox6.Add (this.button4);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.button4]));
			w23.Position = 0;
			w23.Expand = false;
			w23.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.button6 = new global::Gtk.Button ();
			this.button6.CanFocus = true;
			this.button6.Name = "button6";
			this.button6.UseUnderline = true;
			this.button6.Label = "RECORD SUCCESSIVO";
			global::Gtk.Image w24 = new global::Gtk.Image ();
			w24.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-forward", global::Gtk.IconSize.Menu);
			this.button6.Image = w24;
			this.hbox6.Add (this.button6);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.button6]));
			w25.Position = 1;
			w25.Expand = false;
			w25.Fill = false;
			this.hbox4.Add (this.hbox6);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.hbox6]));
			w26.Position = 2;
			w26.Expand = false;
			w26.Fill = false;
			this.vbox4.Add (this.hbox4);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox4]));
			w27.Position = 0;
			w27.Expand = false;
			w27.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hseparator4 = new global::Gtk.HSeparator ();
			this.hseparator4.Name = "hseparator4";
			this.vbox4.Add (this.hseparator4);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hseparator4]));
			w28.Position = 1;
			w28.Expand = false;
			w28.Fill = false;
			this.vbox1.Add (this.vbox4);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.vbox4]));
			w29.Position = 2;
			w29.Expand = false;
			w29.Fill = false;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 957;
			this.DefaultHeight = 281;
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.button1.Released += new global::System.EventHandler (this.OnButton1Released);
			this.button2.Released += new global::System.EventHandler (this.OnButton2Released);
			this.button3.Released += new global::System.EventHandler (this.OnButton3Released);
			this.button4.Released += new global::System.EventHandler (this.OnButton4Released);
			this.button6.Released += new global::System.EventHandler (this.OnButton6Released);
		}
	}
}
