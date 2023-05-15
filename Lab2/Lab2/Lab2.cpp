#include  <La>
#include <iostream>
#include <time.h>

int main(int argc, char* argv[])
{
    // srand(time(NULL));

    char x;
    bool f = true;
    while (f)
    {
        FLabyrinth Labyrinth(21, 21);
        std::cin >> x;
        switch (x)
        {
        case '1':
            Labyrinth.LDFS(10);
            Labyrinth.PrintLabyrinth();
            break;
        case '2':
            Labyrinth.RBFS();
            Labyrinth.PrintLabyrinth();
            break;
        case '3':
            f = false;
            break;
        default:
            std::cout << "Wrong input\n";
        }
    }

    return 0;
}
