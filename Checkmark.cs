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
            "[X]"
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
            /* Buffer.InsertText += OnInsertText;
            Buffer.DeleteRange += OnDeleteRange;
            HighlightNote(); */
            MakeSubs();
        }

        public void MakeSubs(){
            string text_marked = "☑";
            string text_unmarked = "☐";
            NoteBuffer buffer = Note.Buffer;
            string contenido_nota = buffer.Text;
            foreach (string s in CHECK_PATTERNS) {
                contenido_nota = contenido_nota.Replace(s, text_marked);
            }
            foreach (string s in UNCHECK_PATTERNS) {
                contenido_nota = contenido_nota.Replace(s, text_unmarked);
            }
            // FIXME: Esto no respeta formatos ni nada... :(
            //buffer.Text = contenido_nota;
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

        /*
        private void OnInsertText(object sender, Gtk.InsertTextArgs args){
            HighlightRegion(args.Pos, args.Pos);
        }
    
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
