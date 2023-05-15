#include <iostream>
#include <time.h>
#include "Sorter.h"
#include "ArrayGenerator.h"

using namespace std;

int main()
{
    srand(time(NULL));
    char c;
    cout << "Enter 1 to generate file \nEnter 2 to sort file \nEnter 3 to stop program \n";
    
    bool isWorking = true;
    while (isWorking)
    {
        cin >> c;
        switch (c)
        {
        case '1':
            cout << "Enter size of input file:\n";
            int size;
            cin >> size;
            ArrayGenerator gen;
            gen.GenerateArray("Input.txt", size);
            cout << "File generated\n";
            break;
        case '2':
            {
                Sorter sorter;
                sorter.SetInputFile("Input.txt");
                sorter.SortFile();
                cout << "File sorted\n";
                break;
            }
        case '3':
            isWorking = false;
            break;
        default:
            cout << "Wrong input\n";
            break;
        }
    }
    return 0;
}
