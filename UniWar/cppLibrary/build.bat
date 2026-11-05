:: Compile uniwar_lib.cpp into an object file.
g++ -m64 -c uniwar_lib.cpp

:: Invoke the GNU C++ compiler.
:: -c compiles the source file without linking, producing an object file (.o)
:: that contains compiled but unresolved code.
:: -DBUILD_MY_DLL defines the BUILD_MY_DLL macro, which selects DLL export
:: mode instead of import mode.
g++ -m64 -c -DBUILD_MY_DLL functions_lib.cpp


:: -shared instructs the compiler to build a shared library (DLL) instead of
:: an executable.
:: -o functions_lib.dll sets the output file name.
:: functions_lib.o is the object file produced by the previous command.
:: -Wl,--out-implib,libfunctions_lib.a forwards an option to the linker.
:: -Wl, marks the following options as linker options.
:: --out-implib,libfunctions_lib.a creates a static import library that other
:: programs can use to link against the generated DLL.
g++ -m64 -shared -o functions_lib.dll functions_lib.o uniwar_lib.o -Wl,--out-implib,libfunctions_lib.a