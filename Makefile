all:
#    gmcs -out:Checkmark.dll -target:library -r:/usr/lib/tomboy/Tomboy.exe -pkg:gtk-sharp-2.0 -resource:Checkmark.addin.xml -pkg:tomboy-addins -r:Mono.Posix Checkmark.cs
#    gmcs -debug -out:Checkmark.dll -target:library -r:/usr/lib/tomboy/Tomboy.exe -pkg:gtk-sharp-2.0 -resource:Checkmark.addin.xml -pkg:tomboy-addins -r:Mono.Posix Checkmark.cs
	gmcs -out:Checkmark.dll -target:library -r:/usr/lib/tomboy/Tomboy.exe -pkg:gtk-sharp-2.0 -resource:Checkmark.addin.xml Checkmark.cs

clean:
	rm Checkmark.dll

