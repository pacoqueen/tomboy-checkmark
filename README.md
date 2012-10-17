Tomboy Checkmark Add-in
=======================

Support for easy writing of unicode checkmark symbols in tomboy notes. 

Changes [ ] and [x] for unicode chars ☐ and ☑. Supports writing of others unicode mark symbols via menu. It allows marking or unmarking using a keystroke (Ctrl+M).

BUILDING FROM SOURCE
--------------------

Just download `Checkmark.cs` and `Checkmark.addin.xml` and compile with

	gmcs -debug -out:Checkmark.dll -target:library -pkg:tomboy-addins -r:Mono.Posix Checkmark.cs -resource:Checkmark.addin.xml -pkg:gtk-sharp 

or download `compile.sh` and execute in the same dir where you downladed the files.

If you got a gtk-sharp error and you are sure that have already installed, try:

	sudo ln -s /usr/lib/pkgconfig/gtk-sharp-2.0.pc /usr/lib/pkgconfig/gtk-sharp.pc

INSTALL
-------

Once compiled, copy `Checkmark.dll` on default tomboy add-ins location:

	cp Checkmark.dll ~/.config/tomboy/addins

You can just download the [binary file from github](https://github.com/pacoqueen/tomboy-checkmark/blob/master/Checkmark.dll "Checkmark.dll") and copy it on the same directory if you prefer.

Restart tomboy and activate it in Preferences -> Add-ins.

DISCLAIMER
----------
English is not my native language and this is my first Mono/C# program. Be mercy. ;)

