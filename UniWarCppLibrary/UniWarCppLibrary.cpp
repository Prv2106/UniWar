#include "pch.h"
#include "UniWarCppLibrary.h" // Assicurati che l�intestazione sia inclusa


extern "C" __declspec(dllexport) const char* GetCppMessage()
{
    return "Saluti da C++!";
}

int Multiply(int a, int b) {
    return a * b;
}
