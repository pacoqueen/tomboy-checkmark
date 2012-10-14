#!/bin/bash

#gmcs -debug -out:Checkmark.dll -target:library -pkg:tomboy-addins -r:Mono.Posix Checkmark.cs -resource:Checkmark.addin.xml $@

if [ ! -f /usr/lib/pkgconfig/gtk-sharp.pc ]; then 
    sudo ln -s /usr/lib/pkgconfig/gtk-sharp-2.0.pc /usr/lib/pkgconfig/gtk-sharp.pc
fi

gmcs -debug -out:Checkmark.dll -target:library -pkg:tomboy-addins -r:Mono.Posix Checkmark.cs -resource:Checkmark.addin.xml -pkg:gtk-sharp $@

