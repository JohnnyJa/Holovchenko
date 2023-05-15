#include "Sorter.h"
#include <iostream>

/**
 * \brief Set file that sorter should process
 * \param fileName name of file with unsorted array
 */
void Sorter::SetInputFile(const char* fileName)
{
    try
    {
        std::ifstream findFile(fileName);
        if (!findFile)
            throw("File doesn't exist.");
        else
            inputFileName = fileName;
        findFile.close();
    }
    catch (const char* msg)
    {
        std::cout << msg;
    }
}

/**
 * \brief Sort array in file
*/
void Sorter::SortFile()
{
    try
    {
        if (inputFileName.empty())
            throw("File hasn't been chosen.");
        while (FormTempFiles())
        {
            MergeTempFiles();
        }
    }
    catch (const char* msg)
    {
        std::cout << msg;
        return;
    }
}

/**
 * \brief Function form temporary files of native algorithm
 * \return False if it is only one temp file (array is already sorted)
 */
bool Sorter::FormTempFiles()
{
    long long currEl, nextEl;
    bool fileSwitcher = true;

    inputFile.open(inputFileName, std::ios::in);
    tempFile1.open("Temp1.dat", std::ios::out | std::ios::trunc);
    tempFile2.open("Temp2.dat", std::ios::out | std::ios::trunc);

    inputFile >> currEl;
    while (!inputFile.eof())
    {
        inputFile >> nextEl;

        if (fileSwitcher)
            tempFile1 << currEl << ' ';
        else
            tempFile2 << currEl << ' ';

        if (nextEl < currEl)
            fileSwitcher = !fileSwitcher;

        currEl = nextEl;
    }
    
    //finish tempFiles
    if (fileSwitcher)
        tempFile1 << currEl << ' '; 
    else
        tempFile2 << currEl << ' ';

    tempFile1.seekg(0, std::ios_base::end);
    tempFile2.seekg(0, std::ios_base::end);
    if (tempFile1.tellg() == 0 ||tempFile2.tellg() ==0)
    {
        return false;
    }
    //Add garbage series to the end of temp files. They make merge algo process last series to the end
    tempFile1 << nextEl - 1 << ' ' << nextEl - 1;
    
    tempFile2 << nextEl - 1 << ' ' << nextEl - 1;
    
    inputFile.close();
    tempFile1.close();
    tempFile2.close();

    return true;
}

/**
 * \brief Function merge formed temporary files with series
 */
void Sorter::MergeTempFiles()
{
    inputFile.open(inputFileName, std::ios::out | std::ios::trunc);
    tempFile1.open("Temp1.dat", std::ios::in);
    tempFile2.open("Temp2.dat", std::ios::in);

    long long currEl1, currEl2, nextEl1, nextEl2;
    
    tempFile1 >> currEl1 >> nextEl1;
    tempFile2 >> currEl2 >> nextEl2;

    while (!(tempFile1.eof() && tempFile2.eof()))
    {
        // Finish inputFile if one of the tempFile ended
        if (tempFile1.eof())
        {
            inputFile << ' ' << currEl2;
            currEl2 = nextEl2;
            tempFile2 >> nextEl2;
            continue;
        }
        
        if (tempFile2.eof())
        {
            inputFile << ' ' << currEl1;
            currEl1 = nextEl1;
            tempFile1 >> nextEl1;
            continue;
        }

        // Main logic
        if (currEl1 > nextEl1 && currEl2 > nextEl2)
        {
            if (currEl1 < currEl2)
            {
                inputFile << ' ' << currEl1 << ' ' << currEl2;
            }
            else
            {
                inputFile << ' ' << currEl2 << ' ' << currEl1;
            }
            currEl1 = nextEl1;
            tempFile1 >> nextEl1;
            currEl2 = nextEl2;
            tempFile2 >> nextEl2;
        }
        else
        if (currEl1 <= nextEl1 && currEl2 > nextEl2)
        {
            inputFile << ' ' << currEl1;
            currEl1 = nextEl1;
            tempFile1 >> nextEl1;
        }
        else
        if (currEl1 > nextEl1 && currEl2 <= nextEl2)
        {
            inputFile << ' ' << currEl2;
            currEl2 = nextEl2;
            tempFile2 >> nextEl2;
        }
        else
        if (currEl1 <= nextEl1 && currEl2 <= nextEl2)
        {
            if (currEl1 < currEl2)
            {
                inputFile << ' ' << currEl1;
                currEl1 = nextEl1;
                tempFile1 >> nextEl1;
            }
            else
            {
                inputFile << ' ' << currEl2;
                currEl2 = nextEl2;
                tempFile2 >> nextEl2;
            }
        }
    }
    
    inputFile.close();
    tempFile1.close();
    tempFile2.close();
}



