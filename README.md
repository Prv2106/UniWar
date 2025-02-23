## GUIDA SU COME PROVARE IL PROGRAMMA
Per eseguire il gioco (solo su Windows) è necessario:
- Installa .NET SDK v8 con i workload MAUI (dotnet workload install maui).
- Scaricare la directory del progetto (clone del repository in una cartella locale).
- Aprire la cartella in VSCode / Visual Studio o altri IDE / editor.
- Spostarsi nella cartella "cppLibrary" ed eseguire il file build.bat per compilare la libreia in C++ (N.B: è necessario avere installato un compilatore g++).
- Avviare l'applicazione da qui.
- Adesso sarà possibile testare la modalità di gioco offline. Se si vuole provare anche quella online, bisogna eseguire il server.
Per eseguire il server python è necessario:
-  installare docker
-  nel terminale in corrispondenza della root del progetto uniwar bisognerà poi eseguire il seguente comando: **docker compose up**
