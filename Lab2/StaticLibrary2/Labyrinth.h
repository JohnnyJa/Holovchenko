#pragma once
#include <vector>

struct FInfo
{
    size_t Iteration = 0, GeneratedNodes = 0, SavedNodes = 0;
};

class FLabyrinth
{
private:
    std::vector<std::vector<int>> Field;
    const size_t SizeX, SizeY;
    FInfo Info;
    bool IsDeadEnd(size_t X, size_t Y) const;
    bool IsLeftEnd(size_t X, size_t Y) const;
    bool IsRightEnd(size_t X, size_t Y) const;
    bool IsTopEnd(size_t X, size_t Y) const;
    bool IsBottomEnd(size_t X, size_t Y) const;
    void CreatePass(size_t& X, size_t& Y, size_t Course);

    struct FNodeLDFS
    {
        size_t X, Y, Depth;
    };

    struct FNodeRBFS
    {
        size_t X, Y, Depth;
        double Value;
    };

    bool IsPass(size_t X, size_t Y) const;
    void FindWay();
    static bool Compare(const FNodeRBFS& A, const FNodeRBFS& B);
    double CountEvklidDistance(size_t X, size_t Y) const;
public:
    FLabyrinth(size_t SizeX, size_t SizeY);
    void GenerateNewLabyrinth();
    void PrintLabyrinth() const;
    int LDFS(size_t MaxDepth);
    bool RBFS();
    FInfo GetInfo() const;
};
