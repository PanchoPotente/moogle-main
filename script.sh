#1/bin/bash
while true; do

read -p "
Lista de comandos:
run: ejecutar proyecto
report: compilar y generar el pdf del proyecto latex relativo al informe
show_report: ejecutar un programa para visualizar el informe (normal lo ejecuta por defecto custom permite elegir comando de ejecucion)
clean: eliminar todos los ficheros auxiliares que no forman parte del repositorio
exit: salir del script
Inserte un comando: " C
echo $C
case $C in
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
rm 121-Lázaro-Javier-Aragón-García.fls
rm 121-Lázaro-Javier-Aragón-García.synctex.gx
cd..
;;
"show_report")
read -p "normal or custom " S 
echo $S
case $S in 
"normal")
cd ./informe
if[ -o 121-Lázaro-Javier-Aragón-García.pdf ]; then
start 121-Lázaro-Javier-Aragón-García.pdf     
else
pdflatex 121-Lázaro-Javier-Aragón-García.tex
start 121-Lázaro-Javier-Aragón-García.pdf
fi
;;
"custom")
cd ./informe
if[ -e 121-Lázaro-Javier-Aragón-García.pdf ]; then
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
    echo "Comando Invalido"
    ;;
    esac
    ;;
    "exit")
    echo "Saliendo... "
    break
    ;;
    *)
    echo "Comando Invalido"
    ;;
    esac
    done