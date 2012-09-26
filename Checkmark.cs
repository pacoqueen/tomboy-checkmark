//  
//  Checkmark.cs
//  
//  Author:
//       Francisco José Rodríguez Bogado <frbogado@novaweb.es>
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

namespace Tomboy.Todo{
    public class Todo:NoteAddin{

        Gtk.MenuItem item;

        /*
        static string[] TODO_PATTERNS = {
            "FIXME",
            "TODO",
            "XXX"
        };
        */

        public override void Initialize(){
            item = new Gtk.MenuItem(Catalog.GetString("Insert checkbox"));
            item.Activated += OnMenuItemActivated;
            item.Show();
            AddPluginMenuItem(item);
            /*
            foreach (string s in TODO_PATTERNS) {
                if (Note.TagTable.Lookup(s) == null) {
                    TextTag tag = new TextTag(s);
                    tag.Foreground = "#0080f0";
                    tag.Weight = Pango.Weight.Bold;
                    tag.Underline = Pango.Underline.Single;
                    Note.TagTable.Add(tag);
                }
            }
            */
        }
    
        public override void Shutdown(){
            item.Activated -= OnMenuItemActivated;
        }
    
        public override void OnNoteOpened(){
            /* Buffer.InsertText += OnInsertText;
            Buffer.DeleteRange += OnDeleteRange;
            HighlightNote(); */
        }

        void OnMenuItemActivated(object sender, EventArgs args){
            string text = "☐";

            // Logger.Log("Activated 'Insert checkbox' menu item.");

            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark (buffer.InsertMark);
            buffer.InsertWithTagsByName (ref cursor, text, "checkbox");
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
