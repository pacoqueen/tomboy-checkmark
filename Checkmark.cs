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

namespace Tomboy.Checkmark{
    public class CheckmarkAddin:NoteAddin{

        Gtk.MenuItem itemInsert;
        Gtk.MenuItem itemMarked;
        Gtk.MenuItem itemXMarked;
        Gtk.MenuItem itemTick;
        Gtk.MenuItem itemBallot;
        Gtk.MenuItem itemToggle;

        static string[] UNCHECK_PATTERNS = {"[ ]", "[]"};
        static string[] CHECK_PATTERNS = {"[v]", "[V]"};
        static string[] XMARK_PATTERNS = {"[x]", "[X]"};

        static string CHECK_UNMARKED = "☐";
        static string CHECK_MARKED = "☑";
        static string CHECK_XMARKED = "☒";
        static string CHECK_TICK = "✓";
        static string CHECK_BALLOTX = "✗";
        
        static string[] UPATTERNS = {CHECK_MARKED, CHECK_XMARKED, CHECK_TICK, CHECK_BALLOTX, CHECK_UNMARKED};

        public override void Initialize(){
            itemInsert = new Gtk.MenuItem(Catalog.GetString("Insert checkbox"));
            itemInsert.Activated += OnMenuItemActivatedUnmarked;
            itemInsert.Show();
            AddPluginMenuItem(itemInsert);
            itemMarked = new Gtk.MenuItem(Catalog.GetString("Insert marked checkbox"));
            itemMarked.Activated += OnMenuItemActivatedMarked;
            itemMarked.Show();
            AddPluginMenuItem(itemMarked);
            itemXMarked = new Gtk.MenuItem(Catalog.GetString("Insert X marked checkbox"));
            itemXMarked.Activated += OnMenuItemActivatedXMarked;
            itemXMarked.Show();
            AddPluginMenuItem(itemXMarked);
            itemTick = new Gtk.MenuItem(Catalog.GetString("Insert tick"));
            itemTick.Activated += OnMenuItemActivatedTick;
            itemTick.Show();
            AddPluginMenuItem(itemTick);
            itemBallot = new Gtk.MenuItem(Catalog.GetString("Insert X ballot"));
            itemBallot.Activated += OnMenuItemActivatedBallotX;
            itemBallot.Show();
            AddPluginMenuItem(itemBallot);
            itemToggle = new Gtk.MenuItem(Catalog.GetString("Toggle checkmark"));
            itemToggle.Activated += OnMenuItemActivatedToggle;
            AddPluginMenuItem(itemToggle);
            Gtk.AccelGroup accel_group = new Gtk.AccelGroup();
            Window.AddAccelGroup(accel_group);
            itemToggle.AddAccelerator("activate", accel_group,
                                      new AccelKey(Gdk.Key.m,
                                                   Gdk.ModifierType.ControlMask,
                                                   AccelFlags.Visible));
            itemToggle.Show();
        }
    
        public override void Shutdown(){
            itemInsert.Activated -= OnMenuItemActivatedUnmarked;
            itemMarked.Activated -= OnMenuItemActivatedMarked;
            itemXMarked.Activated -= OnMenuItemActivatedXMarked;
            itemTick.Activated -= OnMenuItemActivatedTick;
            itemBallot.Activated -= OnMenuItemActivatedBallotX;
            itemToggle.Activated -= OnMenuItemActivatedToggle;
        }
    
        public override void OnNoteOpened(){
            MakeSubs(); // Text inserted with plugin deactivated is now updated.
            Buffer.InsertText += OnInsertText;
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
                    pos = es;   // Search is started after «s» in next iteration
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
            Gtk.TextIter cursor = buffer.GetIterAtMark(buffer.InsertMark);
            buffer.InsertWithTagsByName(ref cursor, CHECK_UNMARKED, "checkbox");
        }

        void OnMenuItemActivatedMarked(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark(buffer.InsertMark);
            buffer.InsertWithTagsByName(ref cursor, CHECK_MARKED, "marked_checkbox");
        }

        void OnMenuItemActivatedXMarked(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark(buffer.InsertMark);
            buffer.InsertWithTagsByName(ref cursor, CHECK_XMARKED, "xmarked_checkbox");
        }

        void OnMenuItemActivatedTick(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark(buffer.InsertMark);
            buffer.InsertWithTagsByName(ref cursor, CHECK_TICK, "tick");
        }

        void OnMenuItemActivatedBallotX(object sender, EventArgs args){
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark(buffer.InsertMark);
            buffer.InsertWithTagsByName(ref cursor, CHECK_BALLOTX, "ballot_x");
        }

        void OnMenuItemActivatedToggle(object sender, EventArgs args){
            // Moves along the line backward and forward until reaches begin and end of
            // line or a mark. Then exit or it toggles.
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextMark insert_mark = buffer.InsertMark;
            Gtk.TextIter backFinder = buffer.GetIterAtMark(insert_mark);
            Gtk.TextIter frontFinder = buffer.GetIterAtMark(insert_mark);
            Gtk.TextIter BOL = buffer.GetIterAtMark(insert_mark);
            Gtk.TextIter EOL = buffer.GetIterAtMark(insert_mark);
            
            while (! BOL.StartsLine()) BOL.BackwardChar();
            BOL.BackwardChar();   // I need JUST one char before start of line.
            while (! EOL.EndsLine()) EOL.ForwardChar();
            do{
                if (CurrentCharIsMark(backFinder)){
                    ToggleMark(backFinder);
                    break;
                }else{
                    if (! backFinder.Equal(BOL))
                        backFinder.BackwardChar();
                }
                if (CurrentCharIsMark(frontFinder)){
                    ToggleMark(frontFinder);
                    break;
                }else{
                    if (! frontFinder.Equal(EOL))
                        frontFinder.ForwardChar();
                }
            }while (! (backFinder.Equal(BOL) && frontFinder.Equal(EOL)));
        }

        private bool CurrentCharIsMark(Gtk.TextIter iter){
            /*
             * Returns true when char under iter is any of mark predefined symbols.
             */
            Gtk.TextIter siguiente = new Gtk.TextIter();
            siguiente = iter;
            siguiente.ForwardChar();
            bool res = false;
            string car = iter.GetText(siguiente);
            foreach (string s in UPATTERNS) {
                if (car == s){
                    res = true;
                    break;
                }
            }
            return res;
        }

        private void ToggleMark(Gtk.TextIter iter){
            /*
             * If char under iter is a marked symbol, changes for an unmarked symbol and
             * viceversa.
             */
            string uchar;
            Gtk.TextBuffer b;
            Gtk.TextIter iterNext;
            string charToggled = "";
            bool found = true;
                       
            b = iter.Buffer;
            iterNext = iter;
            iterNext.ForwardChar();
            uchar = iter.GetText(iterNext);
            
            if (uchar == CHECK_UNMARKED /* ☐ */ ){
                charToggled = CHECK_MARKED;
            }else if (uchar == CHECK_MARKED /* ☑ */ ){
                charToggled = CHECK_UNMARKED;
            }else if (uchar == CHECK_XMARKED /* ☒ */ ){
                charToggled = CHECK_UNMARKED;
            }else if (uchar == CHECK_TICK /* ✓ */ ){
                charToggled = CHECK_UNMARKED;
            }else if (uchar == CHECK_BALLOTX /* ✗ */ ){
                charToggled = CHECK_UNMARKED;
            }else{
                found = false;
            }
            if (found){
                b.Delete(ref iter, ref iterNext);
                b.Insert(ref iter, charToggled);
            }
        }

        private void OnInsertText(object sender, Gtk.InsertTextArgs args){
            MakeSubs();
        }
    }
}
