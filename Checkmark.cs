using System;
using Mono.Unix;
using Gtk;
using Tomboy;

namespace Tomboy.Checkmark{
    public class CheckmarkAddin: NoteAddin{
        Gtk.MenuItem item;

        public override void Initialize(){
            item = new Gtk.MenuItem(Catalog.GetString("Insert checkmark"));
            item.Show();
            AddPluginMenuItem(item);
        }
        
        public override void Shutdown(){
            item.Activated -= OnMenuItemActivated;
        }
        
        public override void OnNoteOpened(){}
        
        void OnMenuItemActivated(object sender, EventArgs args){
            // Logger.Log("Activated 'Insert checkmark' menu item");
            
            string text = "☐";
            // string text = "☑";
            
            NoteBuffer buffer = Note.Buffer;
            Gtk.TextIter cursor = buffer.GetIterAtMark(buffer.InsertMark);
            buffer.InsertWithTagsByName(ref cursor, text, "checkmark");
        }
    }
}
