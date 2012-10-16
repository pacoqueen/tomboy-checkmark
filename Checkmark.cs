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

        static string[] UNCHECK_PATTERNS = {
            "[ ]",
            "[]"
        };

        static string[] CHECK_PATTERNS = {
            "[x]",
            "[X]",
            "[v]",
            "[V]"
        };

        public override void Initialize(){
            item = new Gtk.MenuItem(Catalog.GetString("Insert checkbox"));
            item.Activated += OnMenuItemActivatedUnmarked;
            item.Show();
            AddPluginMenuItem(item);
            item = new Gtk.MenuItem(Catalog.GetString("Insert marked checkbox"));
            item.Activated += OnMenuItemActivatedMarked;
            item.Show();
            AddPluginMenuItem(item);
        }
    
        public override void Shutdown(){
            item.Activated -= OnMenuItemActivatedMarked;
            item.Activated -= OnMenuItemActivatedUnmarked;
        }
    
        public override void OnNoteOpened(){
            MakeSubs(); // Text inserted with plugin deactivated changes now.
            Buffer.InsertText += OnInsertText;
            /* Buffer.DeleteRange += OnDeleteRange;
            HighlightNote(); */
        }

        public void MakeSubs(){
            string text_marked = "☑";
            string text_unmarked = "☐";
            NoteBuffer buffer = Note.Buffer;
            foreach (string s in CHECK_PATTERNS) {
                Reemplazar(buffer, s, text_marked);
            }
            foreach (string s in UNCHECK_PATTERNS) {
                Reemplazar(buffer, s, text_unmarked);
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
            string text_unmarked = "☐";

            // Logger.Log("Activated 'Insert checkbox' menu item.");

            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, text_unmarked, "checkbox");
        }

        void OnMenuItemActivatedMarked(object sender, EventArgs args){
            string text_marked = "☑";

            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, text_marked, "marked_checkbox");
        }

        private void OnInsertText(object sender, Gtk.InsertTextArgs args){
            MakeSubs();
        }

        /*
        private void OnDeleteRange (object sender, Gtk.DeleteRangeArgs args){
            HighlightRegion(args.Start, args.End);
        }
    
        private void HighlightNote(){
            HighlightRegion(Buffer.StartIter, Buffer.EndIter);
        }
    
        private void HighlightRegion(Gtk.TextIter start, Gtk.TextIter end){
            if (!start.StartsLine())
                start.BackwardLine();
            if (!end.EndsLine())
                end.ForwardLine();
    
            foreach (string pattern in TODO_PATTERNS) {
                HighlightRegion(pattern, start, end);
            }
        }
    
        private void HighlightRegion(string pattern, Gtk.TextIter start, Gtk.TextIter end){
            Buffer.RemoveTag(pattern, start, end);
            Gtk.TextIter region_start = start;
            while (start.ForwardSearch(pattern + ":", TextSearchFlags.TextOnly, out region_start, out start, end)){
                Gtk.TextIter region_end = start;
                //region_end.ForwardSentenceEnd();
                Buffer.ApplyTag(pattern, region_start, region_end);
            }
        }
        */
    }
}
