using System;

public class Engine
{
    public enum Direction
    {
        Any = 0,
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4
    }

    public class Cell
    {
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Connected { get; set; } = false;
        public bool Locked { get; set; } = false;
    }

    public int Rows { get; }
    public int Columns { get; }
    public Cell[,] Map { get; private set; }
    public (int i, int j) StartCell { get; private set; }

    public Engine(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Map = new Cell[Rows, Columns];
        StartCell = (0, 0);
    }

    public void InitMap(bool randomize)
    {
        Map = new Cell[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Map[i, j] = randomize ? CreateRandomCell() : CreateEmptyCell();
            }
        }
    }

    private Cell CreateEmptyCell(bool connected = false)
    {
        return new Cell
        {
            Up = false,
            Down = false,
            Left = false,
            Right = false,
            Connected = connected
        };
    }

    private Cell CreateRandomCell(bool connected = false)
    {
        Random rand = new Random();
        return new Cell
        {
            Up = rand.Next(2) == 1,
            Down = rand.Next(2) == 1,
            Left = rand.Next(2) == 1,
            Right = rand.Next(2) == 1,
            Connected = connected
        };
    }

    // The CreateGameMatrix method goes here
    private int[,] CreateGameMatrix(int rows, int columns)
    {
        int[,] matrix = new int[rows, columns];
        Random random = new Random();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Randomly assign a type of cell (0 to 5 based on your cellTypes array)
                matrix[i, j] = random.Next(0, cellTypes.Length);
            }
        }

        return matrix;
    }

    public void RotateCellCCW(int i, int j)
    {
        Cell cell = Map[i, j];
        bool temp = cell.Left;
        cell.Left = cell.Up;
        cell.Up = cell.Right;
        cell.Right = cell.Down;
        cell.Down = temp;
    }

    public void RotateCellCW(int i, int j)
    {
        Cell cell = Map[i, j];
        bool temp = cell.Left;
        cell.Left = cell.Down;
        cell.Down = cell.Right;
        cell.Right = cell.Up;
        cell.Up = temp;
    }

    public void ResetConnections()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Map[i, j].Connected = false;
            }
        }
        Connect(StartCell.i, StartCell.j, Direction.Any);
    }

    private void Connect(int i, int j, Direction direction)
    {
        Cell cell = Map[i, j];
        if (cell != null && !cell.Connected && ResolveDirection(i, j, direction))
        {
            cell.Connected = true;
            if (cell.Left && i > 0)
            {
                Connect(i - 1, j, Direction.Right);
            }
            if (cell.Right && i < Columns - 1)
            {
                Connect(i + 1, j, Direction.Left);
            }
            if (cell.Up && j > 0)
            {
                Connect(i, j - 1, Direction.Down);
            }
            if (cell.Down && j < Rows - 1)
            {
                Connect(i, j + 1, Direction.Up);
            }
        }
    }

    private bool ResolveDirection(int i, int j, Direction direction)
    {
        Cell cell = Map[i, j];
        return direction switch
        {
            Direction.Any => true,
            Direction.Left => cell.Left,
            Direction.Right => cell.Right,
            Direction.Up => cell.Up,
            Direction.Down => cell.Down,
            _ => false,
        };
    }

    public void Restart()
    {
        Random rand = new Random();
        int i = rand.Next(0, Columns);
        int j = rand.Next(0, Rows);
        StartCell = (i, j);

        InitMap(randomize: true);
        GenerateMap(CreateGameMatrix(Rows, Columns));
        ResetConnections();
    }

    private void GenerateMap(int[,] mapData)
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                var cellType = cellTypes[mapData[i, j]];
                foreach (var direction in cellType)
                {
                    switch (direction)
                    {
                        case "up": Map[j, i].Up = true; break;
                        case "down": Map[j, i].Down = true; break;
                        case "left": Map[j, i].Left = true; break;
                        case "right": Map[j, i].Right = true; break;
                    }
                }
                int rand = new Random().Next(4);
                while (rand-- > 0)
                {
                    RotateCellCCW(j, i);
                }
            }
        }
    }

    public bool CheckSolution()
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (!Map[i, j].Connected) return false;
            }
        }
        return true;
    }

    public void ToggleLock(int i, int j)
    {
        Map[i, j].Locked = !Map[i, j].Locked;
    }

    // Define the cellTypes array as in your original code.
    private static readonly string[][] cellTypes = new[]
    {
        new[] {"up"},
        new[] {"up", "right"},
        new[] {"up", "down"},
        new[] {"up", "down", "right"},
        new[] {"up", "down", "left", "right"},
        Array.Empty<string>()
    };
}