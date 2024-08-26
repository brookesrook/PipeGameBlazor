using System;
using System.Collections.Generic;

public class GameCreator
{
    public const int WHITE = 0;
    public const int GRAY = 1;
    public const int BLACK = 2;

    public class Cell
    {
        public Cell? Up { get; set; }
        public Cell? Down { get; set; }
        public Cell? Left { get; set; }
        public Cell? Right { get; set; }
        public string? Direction { get; set; }
        public Cell? NeighbourCell { get; set; }
        public int Color { get; set; } = WHITE;
        public List<Neighbour> Neighbours { get; } = new List<Neighbour>();
    }

    public class Neighbour
    {
        public string Direction { get; set; } = string.Empty;
        public Cell NeighbourCell { get; set; } = default!;
    }

    public Cell[,] CreateGame(int rows, int columns)
    {
        var matrix = EmptyMatrix(rows, columns);

        var queue = new Queue<Cell>();
        var start = matrix[Random(0, rows - 1), Random(0, columns - 1)];
        start.Color = GRAY;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            item.Color = BLACK;
            var availableConnections = item.Neighbours.FindAll(n => n.NeighbourCell.Color == WHITE);
            var connectionsToGenerate = Random(0, Math.Min(item.HasConnections() ? 2 : 3, availableConnections.Count));

            for (int i = 0; i < connectionsToGenerate; i++)
            {
                var itemToPop = Random(0, availableConnections.Count - 1);
                var connection = availableConnections[itemToPop];
                availableConnections.RemoveAt(itemToPop);

                item.SetConnection(connection);
                connection.NeighbourCell.SetConnection(connection.GetOppositeDirection(), item);
                connection.NeighbourCell.Color = GRAY;
                queue.Enqueue(connection.NeighbourCell);
            }

            if (queue.Count == 0)
            {
                var islandItem = PickFromIslands(matrix);
                if (islandItem != null)
                {
                    var blackNeighbours = islandItem.Neighbours.FindAll(n => n.NeighbourCell.Color == BLACK && n.NeighbourCell.ConnectionCount() < 3);
                    if (blackNeighbours.Count > 0)
                    {
                        var connection = blackNeighbours[Random(0, blackNeighbours.Count - 1)];
                        islandItem.SetConnection(connection);
                        connection.NeighbourCell.SetConnection(connection.GetOppositeDirection(), islandItem);
                        queue.Enqueue(islandItem);
                    }
                }
            }
        }

        return ConvertMatrix(matrix);
    }

    private Cell[,] EmptyMatrix(int rows, int columns)
    {
        var matrix = new Cell[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = new Cell();
                if (j > 0)
                {
                    var up = matrix[i, j - 1];
                    matrix[i, j].Neighbours.Add(new Neighbour { Direction = "up", NeighbourCell = up });
                    up.Neighbours.Add(new Neighbour { Direction = "down", NeighbourCell = matrix[i, j] });
                }
                if (i > 0)
                {
                    var left = matrix[i - 1, j];
                    matrix[i, j].Neighbours.Add(new Neighbour { Direction = "left", NeighbourCell = left });
                    left.Neighbours.Add(new Neighbour { Direction = "right", NeighbourCell = matrix[i, j] });
                }
            }
        }
        return matrix;
    }

    private int Random(int min, int max)
    {
        return new Random().Next(min, max + 1);
    }

    private Cell? PickFromIslands(Cell[,] matrix)
    {
        var collection = new List<Cell>();
        foreach (var cell in matrix)
        {
            if (cell.Color == WHITE && cell.HasBlackNeighbour())
            {
                collection.Add(cell);
            }
        }
        return collection.Count > 0 ? collection[Random(0, collection.Count - 1)] : null;
    }

    private Cell[,] ConvertMatrix(Cell[,] matrix)
    {
        var result = new Cell[matrix.GetLength(0), matrix.GetLength(1)];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                result[i, j] = matrix[i, j];
            }
        }
        return result;
    }
}

public static class CellExtensions
{
    public static bool HasConnections(this GameCreator.Cell cell)
    {
        return cell.Up != null || cell.Down != null || cell.Left != null || cell.Right != null;
    }

    public static void SetConnection(this GameCreator.Cell cell, GameCreator.Neighbour neighbour)
    {
        switch (neighbour.Direction)
        {
            case "up":
                cell.Up = neighbour.NeighbourCell;
                break;
            case "down":
                cell.Down = neighbour.NeighbourCell;
                break;
            case "left":
                cell.Left = neighbour.NeighbourCell;
                break;
            case "right":
                cell.Right = neighbour.NeighbourCell;
                break;
        }
    }

    public static void SetConnection(this GameCreator.Cell cell, string direction, GameCreator.Cell neighbourCell)
    {
        switch (direction)
        {
            case "up":
                cell.Up = neighbourCell;
                break;
            case "down":
                cell.Down = neighbourCell;
                break;
            case "left":
                cell.Left = neighbourCell;
                break;
            case "right":
                cell.Right = neighbourCell;
                break;
        }
    }

    public static string GetOppositeDirection(this GameCreator.Neighbour neighbour)
    {
        return neighbour.Direction switch
        {
            "up" => "down",
            "down" => "up",
            "left" => "right",
            "right" => "left",
            _ => throw new ArgumentException("Invalid direction")
        };
    }

    public static int ConnectionCount(this GameCreator.Cell cell)
    {
        int count = 0;
        if (cell.Up != null) count++;
        if (cell.Down != null) count++;
        if (cell.Left != null) count++;
        if (cell.Right != null) count++;
        return count;
    }

    public static bool HasBlackNeighbour(this GameCreator.Cell cell)
    {
        return cell.Neighbours.Exists(n => n.NeighbourCell.Color == GameCreator.BLACK);
    }
}