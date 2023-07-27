#!/bin/bash
while true; do

read -p "
Lista de comandos:

run: ejecutar proyecto

slides: compilar y generar el pdf del proyecto latex relativo a la presentación

report: compilar y generar el pdf del proyecto latex relativo al informe

show_report: ejecutar un programa para visualizar el informe (normal lo ejecuta por defecto custom permite elegir comando de ejecucion)

show_slides: ejecutar un programa que permita visualizar la presentación, si el fichero en pdf no ha sido generado debe generarse. Esta opción puede recibir puede recibir como parámetro el comando de la herramienta de visualización que se quiera utilizar, aunque debe tener una por defecto

clean: eliminar todos los ficheros auxiliares que no forman parte del repositorio

exit: salir del script

Inserte un comando: " C
echo $C

case $C in 
"slides")
  cd ./presentacion
  pdflatex presentacion.tex
  cd ..
  ;;
  "report")
  cd ./informe
  pdflatex 121-Lázaro-Javier-Aragón-García.tex
  cd ..
  ;;
  "run")
dotnet watch run --project MoogleServer
  ;;
  "clean")
 cd ./informe
 rm 121-Lázaro-Javier-Aragón-García.aux
 rm 121-Lázaro-Javier-Aragón-García.log
 rm 121-Lázaro-Javier-Aragón-García.pdf
 rm 121-Lázaro-Javier-Aragón-García.fdb_latexmk
 rm 121-Lázaro-Javier-Aragón-García.fls
 rm 121-Lázaro-Javier-Aragón-García.synctex.gz
 cd ..
 cd ./presentacion
 rm e.log
 rm x.log
 rm presentacion.aux
 rm presentacion.fdb_latexmk
 rm presentacion.fls
 rm presentacion.synctex.gz
 rm presentacion.log
 rm presentacion.out
 rm presentacion.nav
 rm presentacion.snm
 rm presentacion.toc
 rm presentacion.pdf
 cd ..
  ;;
  "show_slides")
 read -p "normal or custom: " S
echo $S

case $S in
"normal")
cd ./presentacion
if [ -e presentacion.pdf ]; then
start presentacion.pdf
else
pdflatex presentacion.tex
start presentacion.pdf
  fi
  ;;
"custom")
cd ./presentacion
if [ -e presentacion.pdf ]; then
read -p "ingrese su comando de ejecucion de pdf " F
echo $F
$F presentacion.pdf
else
pdflatex presentacion.tex
read -p "ingrese su comando de ejecucion de pdf " F
echo $F
$F presentacion.pdf
fi
;;
*)
echo "Comando invalido"
;;
esac
;;
"show_report")
read -p "normal or custom: " S
echo $S

case $S in
"normal")
cd ./informe
if [ -e 121-Lázaro-Javier-Aragón-García.pdf ]; then
start 121-Lázaro-Javier-Aragón-García.pdf
else
pdflatex 121-Lázaro-Javier-Aragón-García.tex
start 121-Lázaro-Javier-Aragón-García.pdf
fi
;;
"custom")
cd ./informe
if [ -e 121-Lázaro-Javier-Aragón-García.pdf ]; then
read -p "ingrese su comando de ejecucion de pdf " F
echo $F
$F 121-Lázaro-Javier-Aragón-García.pdf
else
pdflatex 121-Lázaro-Javier-Aragón-García.tex
read -p "ingrese su comando de ejecucion de pdf " F
echo $F
$F 121-Lázaro-Javier-Aragón-García.pdf
fi
;;
*)
echo "Comando invalido"
;;
esac
;;
"exit")
echo "Saliendo ... "
break
             
;; 
*)
echo "Comando invalido"
;;
        
esac
done