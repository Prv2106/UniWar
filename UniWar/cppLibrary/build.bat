:: Compila uniwar_lib.cpp in un file oggetto 
g++ -m64 -c uniwar_lib.cpp

:: Per prima cosa invochiamo il compilatore GNU per C++
:: con -c diciamo al compilatore di compilare il file sorgente ma senza eseguire il linking, 
:: risultando in un file oggetto (.o) che contiene il codice compilato ma non completo.
:: -DBUILD_MY_DLL serve a definire una macro chiamata BUILD_MY_DLL, 
:: questa macro viene utilizzata per determinare se il codice debba essere compilato in modalità esportazione di DLL o in modalità di importazione.
g++ -m64 -c -DBUILD_MY_DLL functions_lib.cpp


:: -shared indica al compilatore di creare una libreria condivisa (DLL in questo caso), invece di un codice eseguibile.
:: -o functions_lib.dll specifica il nome del file di output, che in questo caso è functions_lib.dll.
:: functions_lib.o è il file oggetto che è stato compilato nel primo comando
:: -Wl,--out-implib,libfunctions_lib.a passa un'opzione al linker.
:: -Wl, indica che le opzioni che seguono devono essere passate al linker.
:: --out-implib,libfunctions_lib.a crea una libreria statica di importazione (libfunctions_lib.a) che può essere utilizzata da altri programmi per fare il linking con la DLL creata.
:: in pratica, questa libreria di importazione è un interfaccia per collegare il codice al programma in modo che possa chiamare la DLL.
g++ -m64 -shared -o functions_lib.dll functions_lib.o uniwar_lib.o -Wl,--out-implib,libfunctions_lib.a




