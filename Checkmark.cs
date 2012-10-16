//  
//  Checkmark.cs
//  
//  Author:
//       Francisco José Rodríguez Bogado <bogado@qinn.es>
// 
//  Copyright (c) 2012 Francisco José Rodríguez Bogado
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Gtk;
using Tomboy;
using Mono.Unix;
using System.Collections.Generic;

/* TODO: Al pinchar con el ratón en ☐, cambiar a marcado y viceversa. Dar
   opción también para tachar el texto a continuación hasta el siguiente salto
   de línea.
*/

namespace Tomboy.Checkmark{
    public class CheckmarkAddin:NoteAddin{

        Gtk.MenuItem item;

        static string[] UNCHECK_PATTERNS = {"[ ]", "[]"};
        static string[] CHECK_PATTERNS = {"[v]", "[V]"};
        static string[] XMARK_PATTERNS = {"[x]", "[X]"};

        static string CHECK_UNMARKED = "☐";
        static string CHECK_MARKED = "☑";
        static string CHECK_XMARKED = "☒";
        static string CHECK_TICK = "✓";
        static string CHECK_BALLOTX = "✗";

        public override void Initialize(){
            item = new Gtk.MenuItem(Catalog.GetString("Insert checkbox"));
            item.Activated += OnMenuItemActivatedUnmarked;
            item.Show();
            AddPluginMenuItem(item);
            item = new Gtk.MenuItem(Catalog.GetString("Insert marked checkbox"));
            item.Activated += OnMenuItemActivatedMarked;
            item.Show();
            AddPluginMenuItem(item);
            item = new Gtk.MenuItem(Catalog.GetString("Insert X marked checkbox"));
            item.Activated += OnMenuItemActivatedXMarked;
            item.Show();
            AddPluginMenuItem(item);
            item = new Gtk.MenuItem(Catalog.GetString("Insert tick"));
            item.Activated += OnMenuItemActivatedTick;
            item.Show();
            AddPluginMenuItem(item);
            item = new Gtk.MenuItem(Catalog.GetString("Insert X ballot"));
            item.Activated += OnMenuItemActivatedBallotX;
            item.Show();
            AddPluginMenuItem(item);
        }
    
        public override void Shutdown(){
            item.Activated -= OnMenuItemActivatedMarked;
            item.Activated -= OnMenuItemActivatedXMarked;
            item.Activated -= OnMenuItemActivatedUnmarked;
            item.Activated -= OnMenuItemActivatedTick;
            item.Activated -= OnMenuItemActivatedBallotX;
        }
    
        public override void OnNoteOpened(){
            MakeSubs(); // Text inserted with plugin deactivated changes now.
            Buffer.InsertText += OnInsertText;
			Note.ButtonPressEvent += ButtonPressed;
        }

        public void MakeSubs(){
            NoteBuffer buffer = Note.Buffer;
            foreach (string s in CHECK_PATTERNS) {
                Reemplazar(buffer, s, CHECK_MARKED);
            }
            foreach (string s in UNCHECK_PATTERNS) {
                Reemplazar(buffer, s, CHECK_UNMARKED);
            }
            foreach (string s in XMARK_PATTERNS) {
                Reemplazar(buffer, s, CHECK_XMARKED);
            }
        }

        void Reemplazar(NoteBuffer b, string s, string r){
            /*
             * Changes every occurrences of string s for r in buffer b.
             * Why three passess? Because TextIter left invalidated every time I modify
             * content of a note. I use marks to postprocessing deletions and insertions.
             * Despite that, still invalidation warnings happens.
             */
            bool found;
            Gtk.TextIter w;     // Where to insert "r".
            List<Gtk.TextMark> inserciones = new List<Gtk.TextMark>();
                                // Marks to insert.
            Gtk.TextIter ss;    // Start of slice containing the occurrence
            Gtk.TextIter es;    // End of slice
            List<Gtk.TextMark> comienzos = new List<Gtk.TextMark>();
            List<Gtk.TextMark> finales = new List<Gtk.TextMark>();
            int i = 0;
            int j;
            Gtk.TextIter pos;

            // First pass. Finding...
            pos = b.StartIter;
            do{
                found = pos.ForwardSearch(s, Gtk.TextSearchFlags.TextOnly,
                                          out ss, out es, b.EndIter);
                if (found){
                    inserciones.Add(b.CreateMark("check" + i.ToString(), ss, false));
                    comienzos.Add(b.CreateMark("comienzo" + i.ToString(), ss, false));
                    finales.Add(b.CreateMark("final" + i.ToString(), es, false));
                    i++;
                    pos = es;   // Search is started after "s" in next iteration
                    // (Tomboy:7048): Gtk-WARNING **: Invalid text buffer iterator...
                    //b.Delete(ref ss, ref es);
                    //w = b.GetIterAtMark(m);
                    //b.Insert(ref w, r);
                }
            }while (found);
            // Second pass. Replacing...
            for (j = 0; j < i; j++){
                ss = b.GetIterAtMark(comienzos[j]);
                es = b.GetIterAtMark(finales[j]);
                b.Delete(ref ss, ref es);
            }
            // Third pass. Inserting...
            for (j = 0; j < i; j++){
                w = b.GetIterAtMark(inserciones[j]);
                b.Insert(ref w, r);
            }
        }

        void OnMenuItemActivatedUnmarked(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, CHECK_UNMARKED, "checkbox");
        }

        void OnMenuItemActivatedMarked(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, CHECK_MARKED, "marked_checkbox");
        }

        void OnMenuItemActivatedXMarked(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, CHECK_XMARKED, "xmarked_checkbox");
        }

        void OnMenuItemActivatedTick(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, CHECK_TICK, "tick");
        }

        void OnMenuItemActivatedBallotX(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, CHECK_BALLOTX, "ballot_x");
        }

        private void OnInsertText(object sender, Gtk.InsertTextArgs args){
            MakeSubs();
        }
		
		private void ButtonPressed(object sender, Gtk.ButtonPressEventArgs args){
			Logger.Info("SOY YO");
		}
    }
}
