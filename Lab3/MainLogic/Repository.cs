using Entities;

namespace MainLogic;

public class Repository
{
    private string _pathIndex = "index.json";
    private string _pathMain = "main.json";
    private string _pathOversize = "oversize.json";
    private int _maxKey;
    private int _blockSize = 1024;
    private int _dataSize = 4;
    private int _indexSize = 2 * sizeof(int);
    private int _numOfBlocks;
    private int _keysInBlock;
    private int _comparisons;

    public void CreateDb(int maxKey = 1000)
    {
        _maxKey = maxKey;
        _keysInBlock = _blockSize / _indexSize;
        _numOfBlocks = GetNumOfBlocks();
        CreateIndexFile();
        CreateMainFile();
        CreateOversizeFile();
    }

    public void OpenDb(int maxKey = 1000)
    {
        _maxKey = maxKey;
        _keysInBlock = _blockSize / _indexSize;
        _numOfBlocks = GetNumOfBlocks();
    }

    private int GetNumOfBlocks()
    {
        return _maxKey / (_blockSize / _indexSize) + 1;
    }

    private void CreateIndexFile()
    {
        using BinaryWriter writer = new BinaryWriter(File.Create(_pathIndex));
        for (int i = 0; i < _blockSize * _numOfBlocks / _indexSize; i++)
        {
            writer.Write(-1);
            writer.Write(-1);
        }
    }

    private void CreateMainFile()
    {
        using FileStream fs = File.Create(_pathMain);
    }

    private void CreateOversizeFile()
    {
        using FileStream fs = File.Create(_pathOversize);
    }

    public void AddNewEntity(Entity entity)
    {
        AddEntityIndex(entity.Id);
        AddEntityToMain(entity);
    }

    private void AddEntityToMain(Entity entity)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(_pathMain, FileMode.Append));
        writer.Write(entity.Data);
    }

    private int GetLastPositionInMainFile()
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathMain, FileMode.Open));
        return (int)reader.BaseStream.Length;
    }

    private void AddEntityIndex(int key)
    {
        int block = FindBlockForKey(key);
        Index index = new Index()
        {
            Key = key,
            Position = GetLastPositionInMainFile()
        };
        if (block == -1)
        {
            AddEntityIndexInOversize(index);
            return;
        }

        int offset = FindOffsetForKey(block, key);
        if (HasOffsetOversizeBlock(offset, block))
        {
            AddEntityIndexInOffset(index, offset);
        }
        else
        {
            AddEntityIndexInOversize(index);
        }
    }

    private void AddEntityIndexInOffset(Index index, int offset)
    {
        List<Index> savedIndexes = GetSavedIndexes(offset);
        using BinaryWriter writer = new BinaryWriter(File.Open(_pathIndex, FileMode.Open));
        writer.Seek(offset, SeekOrigin.Begin);
        writer.Write(index.Key);
        writer.Write(index.Position);
        foreach (var savedIndex in savedIndexes)
        {
            writer.Write(savedIndex.Key);
            writer.Write(savedIndex.Position);
        }
    }

    private List<Index> GetSavedIndexes(int offset)
    {
        List<Index> savedIndex = new List<Index>();
        using BinaryReader reader = new BinaryReader(File.Open(_pathIndex, FileMode.Open));
        reader.BaseStream.Position = offset;
        Index readIndex = new Index()
        {
            Key = reader.ReadInt32(),
            Position = reader.ReadInt32()
        };
        while (readIndex.Key != -1)
        {
            savedIndex.Add(readIndex);
            readIndex = new Index()
            {
                Key = reader.ReadInt32(),
                Position = reader.ReadInt32()
            };
        }

        return savedIndex;
    }

    private void AddEntityIndexInOversize(Index index)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(_pathOversize, FileMode.Append));
        writer.Write(index.Key);
        writer.Write(index.Position);
    }

    private bool HasOffsetOversizeBlock(long offset, int block)
    {
        return offset < (block + 1) * _blockSize;
    }

    private int FindBlockForKey(int key)
    {
        for (int block = 0; block < _numOfBlocks; block++)
        {
            if (IsKeyInRightBlock(key, block))
            {
                return block;
            }
        }

        return -1;
    }

    private bool IsKeyInRightBlock(int key, int block)
    {
        return key >= block * _keysInBlock && key < (block + 1) * _keysInBlock;
    }

    private int FindOffsetForKey(int block, int key)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathIndex, FileMode.Open));
        int searchStart = block * _blockSize;
        int searchEnd = (block + 1) * _blockSize - _indexSize;

        reader.BaseStream.Position = searchStart;
        if (reader.ReadInt32() == -1)
        {
            return searchStart;
        }

        while (searchStart < searchEnd)
        {
            int searchCurr = (searchEnd + searchStart) / _indexSize / 2;
            reader.BaseStream.Position = searchCurr * _indexSize;
            int currKey = reader.ReadInt32();

            if (currKey == -1)
            {
                searchEnd = searchCurr * _indexSize;
                continue;
            }

            if (key < currKey)
            {
                searchEnd = searchCurr * _indexSize;
            }
            else
            {
                searchStart = (searchCurr + 1) * _indexSize;
            }
        }

        return searchEnd;
    }

    public Entity GetEntityByKey(int key)
    {
        _comparisons = 0;
        int block = FindBlockForKey(key);
        var position = block == -1 ? FindPositionByKeyInOversize(key) : FindPositionByKeyInBlock(key, block);
        return new Entity()
        {
            Id = key,
            Data = GetEntityDataFromPosition(position)
        };
    }

    private long FindPositionByKeyInBlock(int key, int block)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathIndex, FileMode.Open));
        int startPosition = block * _blockSize;
        int endPosition = (block + 1) * _blockSize - _indexSize;

        while (startPosition < endPosition)
        {
            _comparisons++;
            int currPosition = (startPosition + endPosition) / _indexSize / 2;
            reader.BaseStream.Position = currPosition * _indexSize;
            Index index = new Index()
            {
                Key = reader.ReadInt32(),
                Position = reader.ReadInt32()
            };
            if (index.Key == -1)
            {
                endPosition = currPosition * _indexSize;
                continue;
            }

            if (index.Key == key)
            {
                return index.Position;
            }

            if (key < index.Key)
            {
                endPosition = currPosition * _indexSize;
            }

            if (key > index.Key)
            {
                startPosition = currPosition * _indexSize;
            }
        }

        throw new Exception("Key doesn't exist");
    }

    private long FindPositionByKeyInOversize(int key)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathOversize, FileMode.Open));
        int startPosition = 0;
        int endPosition = (int)reader.BaseStream.Length;

        while (startPosition < endPosition)
        {
            _comparisons++;
            int currPosition = (startPosition + endPosition) / _indexSize / 2;
            reader.BaseStream.Position = currPosition;
            Index index = new Index()
            {
                Key = reader.ReadInt32(),
                Position = reader.ReadInt32()
            };

            if (index.Key == -1)
            {
                endPosition = currPosition * _indexSize;
                continue;
            }

            if (index.Key == key)
            {
                return index.Position;
            }

            if (key < index.Key)
            {
                endPosition = currPosition * _indexSize;
            }

            if (key > index.Key)
            {
                startPosition = currPosition * _indexSize;
            }
        }

        throw new Exception("Key doesn't exist");
    }

    private string GetEntityDataFromPosition(long position)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathMain, FileMode.Open));
        reader.BaseStream.Position = position;
        return reader.ReadString();
    }

    public void UpdateDataByKey(int key, string newData)
    {
        int block = FindBlockForKey(key);
        var position = block == -1 ? FindPositionByKeyInOversize(key) : FindPositionByKeyInBlock(key, block);
        UpdateDataAtPosition(position, newData);
    }

    private void UpdateDataAtPosition(long position, string newData)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(_pathMain, FileMode.Open));
        writer.Seek((int)position, SeekOrigin.Begin);
        writer.Write(newData);
    }

    public void RemoveDataByKey(int key)
    {
        int block = FindBlockForKey(key);
        if (block == -1)
        {
            RemoveEntityIndexInOversize(key);
            return;
        }

        int offset = FindOffsetForKeyToRemove(key, block);
        if (offset != -1)
        {
            RemoveEntityIndexInOffset(offset);
        }
        else
        {
            RemoveEntityIndexInOversize(key);
        }
    }

    private int FindOffsetForKeyToRemove(int key, int block)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathIndex, FileMode.Open));
        int searchStart = block * _blockSize;
        int searchEnd = (block + 1) * _blockSize - _indexSize;

        while (searchStart < searchEnd)
        {
            int searchCurr = (searchEnd + searchStart) / _indexSize / 2;
            reader.BaseStream.Position = searchCurr * _indexSize;
            int currKey = reader.ReadInt32();

            if (currKey == -1)
            {
                searchEnd = searchCurr * _indexSize;
                continue;
            }

            if (key == currKey)
            {
                return searchCurr * _indexSize;
            }

            if (key < currKey)
            {
                searchEnd = searchCurr * _indexSize;
            }
            else
            {
                searchStart = (searchCurr + 1) * _indexSize;
            }
        }

        return -1;
    }

    private void RemoveEntityIndexInOffset(int offset)
    {
        List<Index> savedIndexes = GetSavedIndexes(offset);
        if (savedIndexes.Count != 0)
        {
            savedIndexes.RemoveAt(0);  
        } 
        using BinaryWriter writer = new BinaryWriter(File.Open(_pathIndex, FileMode.Open));
        writer.Seek(offset, SeekOrigin.Begin);
        foreach (var savedIndex in savedIndexes)
        {
            writer.Write(savedIndex.Key);
            writer.Write(savedIndex.Position);
        }

        writer.Write(-1);
        writer.Write(-1);
    }

    private void RemoveEntityIndexInOversize(int key)
    {
        int offset = FindOffsetInOversize(key);
        List<Index> savedIndexes = GetSavedIndexesInOversize(offset);
        if (savedIndexes.Count != 0)
        {
            savedIndexes.RemoveAt(0);  
        } 
        using BinaryWriter writer = new BinaryWriter(File.Open(_pathOversize, FileMode.Open));
        writer.Seek(offset, SeekOrigin.Begin);
        foreach (var savedIndex in savedIndexes)
        {
            writer.Write(savedIndex.Key);
            writer.Write(savedIndex.Position);
        }
        writer.Write(-1);
        writer.Write(-1);
    }
    private List<Index> GetSavedIndexesInOversize(int offset)
    {
        List<Index> savedIndex = new List<Index>();
        using BinaryReader reader = new BinaryReader(File.Open(_pathOversize, FileMode.Open));
        reader.BaseStream.Position = offset;
        Index readIndex = new Index()
        {
            Key = reader.ReadInt32(),
            Position = reader.ReadInt32()
        };
        while (reader.PeekChar() > -1 && readIndex.Key != -1)
        {
            savedIndex.Add(readIndex);
            readIndex = new Index()
            {
                Key = reader.ReadInt32(),
                Position = reader.ReadInt32()
            };
        }

        return savedIndex;
    }

    private int FindOffsetInOversize(int key)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathOversize, FileMode.Open));
        while (reader.PeekChar() > -1)
        {
            Index index = new Index()
            {
                Key = reader.ReadInt32(),
                Position = reader.ReadInt32()
            };
            if (index.Key == key)
            {
                return (int)reader.BaseStream.Position - _indexSize;
            }
        }

        throw new Exception("Key doesn't exist");
    }

    public void GetIndexFile(int numOfStrings = 129)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathIndex, FileMode.Open));
        for (int i = 0; i < numOfStrings; i++)
        {
            Console.WriteLine("{0} {1}", reader.ReadInt32(), reader.ReadInt32());
        }
    }

    public void GetOversizeFile(int numOfStrings = 129)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathOversize, FileMode.Open));
        for (int i = 0; i < numOfStrings && i * _indexSize < reader.BaseStream.Length; i++)
        {
            Console.WriteLine("{0} {1}", reader.ReadInt32(), reader.ReadInt32());
        }
    }

    public void GetMainFile(int numOfStrings = 129)
    {
        using BinaryReader reader = new BinaryReader(File.Open(_pathMain, FileMode.Open));
        while (reader.PeekChar() > -1)
        {
            Console.WriteLine("{0}", reader.ReadString());
        }
    }

    public int GetComparisons()
    {
        return _comparisons;
    }
}