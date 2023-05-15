#pragma once
#include <fstream>
#include <string>
/**
 * Sort array in file
 */
class Sorter
{
private:
    std::string inputFileName;
    std::fstream inputFile, tempFile1, tempFile2;
    
public:
    void SetInputFile(const char* fileName);
    void SortFile();
    bool FormTempFiles();
    void MergeTempFiles();
};
