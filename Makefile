
all: 
#	gmcs -debug -out:Checkmark.dll -target:library -r:/usr/lib/tomboy/Tomboy.exe -pkg:gtk-sharp-2.0 -pkg:tomboy-addins -r:Mono.Posix Checkmark.cs -resource:Checkmark.addin.xml 
	gmcs -debug -out:Checkmark.dll -target:library -pkg:gtk-sharp-2.0 -pkg:tomboy-addins -r:Mono.Posix Checkmark.cs -resource:Checkmark.addin.xml 

