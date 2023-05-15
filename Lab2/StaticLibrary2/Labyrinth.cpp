// ReSharper disable CppClangTidyConcurrencyMtUnsafe
#include "Labyrinth.h"
#include <algorithm>
#include <iomanip>
#include <stack>

#include <iostream>

#define PASS_SIZE 5

#define WAY (-3)
#define WALL (-1)
#define PASS (-2)
#define UNVISITED (-2)
#define TOP 0
#define RIGHT 1
#define BOTTOM 2
#define LEFT 3

FLabyrinth::FLabyrinth(const size_t SizeX, const size_t SizeY) : SizeX(SizeX), SizeY(SizeY)
{
    GenerateNewLabyrinth();
}

void FLabyrinth::GenerateNewLabyrinth()
{
    Field = std::vector<std::vector<int>>(SizeX, std::vector<int>(SizeY, WALL));
    const size_t NumOfCrossroads = (SizeX - 1) * (SizeY - 1) / 4;
    size_t NumOfVisitedCrossroads = 1;

    size_t X = 1, Y = 1;
    while (NumOfCrossroads > NumOfVisitedCrossroads)
    {
        Field[X][Y] = PASS;
        while (!IsDeadEnd(X, Y))
        {
            const int Course = rand() % 4;
            CreatePass(X, Y, Course);
            NumOfVisitedCrossroads++;
        }
        do
        {
            X = 2 * (rand() % ((SizeX - 1) / 2)) + 1;
            Y = 2 * (rand() % ((SizeY - 1) / 2)) + 1;
        }
        while (Field[X][Y] != PASS);
    }
}

void FLabyrinth::PrintLabyrinth() const
{
    for (size_t Y = 0; Y < SizeY; ++Y)
    {
        for (size_t X = 0; X < SizeX; ++X)
        {
            if (Field[X][Y] == PASS)
                std::cout << ' ' << std::setw(PASS_SIZE);
            else if (Field[X][Y] == WALL)
                std::cout << 'M' << std::setw(PASS_SIZE);
            else if (Field[X][Y] == WAY)
            {
                std::cout << '-' << std::setw(PASS_SIZE);
            }
            else
                std::cout << Field[X][Y] << std::setw(PASS_SIZE);
        }
        std::cout << '\n';
    }
}

bool FLabyrinth::IsDeadEnd(const size_t X, const size_t Y) const
{
    return IsLeftEnd(X, Y) && IsRightEnd(X, Y) && IsTopEnd(X, Y) && IsBottomEnd(X, Y);
}

bool FLabyrinth::IsBottomEnd(const size_t X, const size_t Y) const
{
    return (Y == SizeY - 2) || (Field[X][Y + 2] == PASS);
}

bool FLabyrinth::IsLeftEnd(const size_t X, const size_t Y) const
{
    return (X == 1) || (Field[X - 2][Y] == PASS);
}

bool FLabyrinth::IsRightEnd(const size_t X, const size_t Y) const
{
    return (X == SizeX - 2) || (Field[X + 2][Y] == PASS);
}

bool FLabyrinth::IsTopEnd(const size_t X, const size_t Y) const
{
    return (Y == 1) || (Field[X][Y - 2] == PASS);
}

void FLabyrinth::CreatePass(size_t& X, size_t& Y, size_t Course)
{
    bool PassCreated = false;
    while (!PassCreated)
    {
        switch (Course)
        {
        case TOP:
            if (!IsTopEnd(X, Y))
            {
                Field[X][Y - 1] = PASS;
                Field[X][Y - 2] = PASS;
                Y -= 2;
                PassCreated = true;
            }
            break;

        case RIGHT:
            if (!IsRightEnd(X, Y))
            {
                Field[X + 1][Y] = PASS;
                Field[X + 2][Y] = PASS;
                X += 2;
                PassCreated = true;
            }
            break;

        case BOTTOM:
            if (!IsBottomEnd(X, Y))
            {
                Field[X][Y + 1] = PASS;
                Field[X][Y + 2] = PASS;
                Y += 2;
                PassCreated = true;
            }

            break;

        case LEFT:
            if (!IsLeftEnd(X, Y))
            {
                Field[X - 1][Y] = PASS;
                Field[X - 2][Y] = PASS;
                X -= 2;
                PassCreated = true;
            }

            break;

        default:
            std::cout << "Wrong Way";
        }
        Course = (Course + 1) % 4;
    }
}

int FLabyrinth::LDFS(const size_t MaxDepth)
{
    FNodeLDFS CurrNode{};
    const FNodeLDFS LookedNode{SizeX - 2, SizeY - 2, 0};
    std::stack<FNodeLDFS> NodeStack;
    NodeStack.push(FNodeLDFS{1, 1, 1});

    while (!NodeStack.empty() && Field[LookedNode.X][LookedNode.Y] == UNVISITED)
    {
        CurrNode = NodeStack.top();
        const bool bIsLimitOversize = CurrNode.Depth >= MaxDepth;
        NodeStack.pop();
        if (IsPass(CurrNode.X - 1, CurrNode.Y) && !bIsLimitOversize)
        {
            NodeStack.push(FNodeLDFS{CurrNode.X - 1, CurrNode.Y, CurrNode.Depth + 1});
            Info.GeneratedNodes++;
        }
        if (IsPass(CurrNode.X + 1, CurrNode.Y) && !bIsLimitOversize)
        {
            NodeStack.push(FNodeLDFS{CurrNode.X + 1, CurrNode.Y, CurrNode.Depth + 1});
            Info.GeneratedNodes++;
        }
        if (IsPass(CurrNode.X, CurrNode.Y - 1) && !bIsLimitOversize)
        {
            NodeStack.push(FNodeLDFS{CurrNode.X, CurrNode.Y - 1, CurrNode.Depth + 1});
            Info.GeneratedNodes++;
        }
        if (IsPass(CurrNode.X, CurrNode.Y + 1) && !bIsLimitOversize)
        {
            NodeStack.push(FNodeLDFS{CurrNode.X, CurrNode.Y + 1, CurrNode.Depth + 1});
            Info.GeneratedNodes++;
        }
        Info.SavedNodes = std::max(Info.SavedNodes, NodeStack.size());
        Field[CurrNode.X][CurrNode.Y] = ++Info.Iteration;
    }

    // if (Field[LookedNode.X][LookedNode.Y] != UNVISITED)
    //     FindWay();

    if (CurrNode.Depth >= MaxDepth)
        return 2;
    return Field[LookedNode.X][LookedNode.Y] == UNVISITED;
}

void FLabyrinth::FindWay()
{
    size_t X = SizeX - 2, Y = SizeY - 2;
    while (X != 1 || Y != 1)
    {
        if (Field[X - 1][Y] < Field[X][Y] && Field[X - 1][Y] > 0)
            Field[X--][Y] = WAY;
        if (Field[X + 1][Y] < Field[X][Y] && Field[X + 1][Y] > 0)
            Field[X++][Y] = WAY;
        if (Field[X][Y - 1] < Field[X][Y] && Field[X][Y - 1] > 0)
            Field[X][Y--] = WAY;
        if (Field[X][Y + 1] < Field[X][Y] && Field[X][Y + 1] > 0)
            Field[X][Y++] = WAY;
    }
    Field[X][Y] = WAY;
}

bool FLabyrinth::IsPass(const size_t X, const size_t Y) const
{
    return Field[X][Y] == PASS;
}

double FLabyrinth::CountEvklidDistance(size_t X, size_t Y) const
{
    return sqrt(pow(SizeX - X, 2) + pow(SizeY - Y, 2));
}

bool FLabyrinth::Compare(const FNodeRBFS& A, const FNodeRBFS& B)
{
    return A.Value < B.Value;
}

bool FLabyrinth::RBFS()
{
    std::vector<FNodeRBFS> SavedNodes;
    const FNodeRBFS LookedNode{SizeX - 2, SizeY - 2, 0, 0};
    SavedNodes.push_back(FNodeRBFS{1, 1, 1, CountEvklidDistance(1, 1)});
    while (!SavedNodes.empty() && Field[LookedNode.X][LookedNode.Y] == UNVISITED)
    {
        auto CurrNodePos = std::min_element(SavedNodes.begin(), SavedNodes.end(), Compare);
        const FNodeRBFS CurrNode = *CurrNodePos;
        SavedNodes.erase(CurrNodePos);
        if (IsPass(CurrNode.X - 1, CurrNode.Y))
        {
            SavedNodes.push_back(FNodeRBFS{
                CurrNode.X - 1, CurrNode.Y, CurrNode.Depth + 1, CountEvklidDistance(CurrNode.X - 1, CurrNode.Y)
            });
            ++Info.GeneratedNodes;
        }
        if (IsPass(CurrNode.X + 1, CurrNode.Y))
        {
            SavedNodes.push_back(FNodeRBFS{
                CurrNode.X + 1, CurrNode.Y, CurrNode.Depth + 1, CountEvklidDistance(CurrNode.X + 1, CurrNode.Y)
            });
            ++Info.GeneratedNodes;
        }
        if (IsPass(CurrNode.X, CurrNode.Y - 1))
        {
            SavedNodes.push_back(FNodeRBFS{
                CurrNode.X, CurrNode.Y - 1, CurrNode.Depth + 1, CountEvklidDistance(CurrNode.X, CurrNode.Y - 1)
            });
            ++Info.GeneratedNodes;
        }
        if (IsPass(CurrNode.X, CurrNode.Y + 1))
        {
            SavedNodes.push_back(FNodeRBFS{
                CurrNode.X, CurrNode.Y + 1, CurrNode.Depth + 1, CountEvklidDistance(CurrNode.X, CurrNode.Y + 1)
            });
            ++Info.GeneratedNodes;
        }
        Info.SavedNodes = std::max(Info.SavedNodes, SavedNodes.size());
        Field[CurrNode.X][CurrNode.Y] = ++Info.Iteration;
    }
    // if (Field[LookedNode.X][LookedNode.Y] != UNVISITED)
    //     FindWay();
    return Field[LookedNode.X][LookedNode.Y] != UNVISITED;
}

FInfo FLabyrinth::GetInfo() const
{
    return Info;
}
