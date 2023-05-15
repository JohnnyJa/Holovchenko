#include "ArrayGenerator.h"
#include <fstream>

void ArrayGenerator::GenerateArray(const char* fileName, long long size)
{
    std::ofstream file(fileName, std::ios::trunc);
    for (long long i = 0; i < size; i++)
    {
        file << ' ' << rand() % size + 1;
    }
    file.close();
}
