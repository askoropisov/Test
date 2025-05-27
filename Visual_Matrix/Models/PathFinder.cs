using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Visual_Matrix.Models;
using System.Linq;
using System.Diagnostics;

public class PathFinder
{
    private readonly int _maxRedVisits;                                             // Максимальное количество посещений красных клеток
    private readonly ObservableCollection<ObservableCollection<Cell>> _cellsMatrix; // Матрица клеток
    private int?[,,] _dpMemo;                                                       // Таблица мемоизации
    private int _rows;                                                              // Число строк в матрице
    private int _columns;                                                           // Число столбцов в матрице

    public PathFinder(ObservableCollection<ObservableCollection<Cell>> matrix, int maxRedVisits)
    {
        _cellsMatrix = matrix;
        _maxRedVisits = maxRedVisits;
        Path = new List<(int Row, int Col)>();
        _rows = _columns = matrix.Count;
    }

    public List<(int, int)> Path = new List<(int, int)> ();

    /// <summary>
    /// Запуск метода поиска оптимального пути
    /// </summary>
    public void Solve()
    {
        _dpMemo = new int?[_rows, _columns, _maxRedVisits + 1];

        // Поиск оптимального пути
        FindMaxPathCost(_rows - 1, 0, _maxRedVisits);

        if (_dpMemo[_rows - 1, 0, _maxRedVisits].HasValue)
        {
            Debug.WriteLine($"Максимальная стоимость пути: {_dpMemo[_rows - 1, 0, _maxRedVisits]}");
            ReconstructPath(); // Строим путь
        }
        else
        {
            Debug.WriteLine("Оптимального пути не найдено.");
        }
    }

    /// <summary>
    /// Поиск максимальной стоимости пути (рекурсивный метод)
    /// </summary>
    private int? FindMaxPathCost(int row, int column, int remainingRedVisits)
    {
        // Проверка выхода за границы поля
        if (row < 0 || column >= _columns)
            return null;

        // Недостаточно ресурсов на посещение красных клеток
        if (remainingRedVisits < 0)
            return null;

        // Мемоизированное значение
        if (_dpMemo[row, column, remainingRedVisits].HasValue)
            return _dpMemo[row, column, remainingRedVisits];

        // Базовый случай: достижение целевой клетки
        if (row == 0 && column == _columns - 1)
            return _cellsMatrix[0][_columns - 1].Cost;

        // Подсчет возможных направлений: вверх и вправо
        int fromTop = FindMaxPathCost(row - 1, column, remainingRedVisits) ?? int.MinValue;
        int fromRight = FindMaxPathCost(row, column + 1, remainingRedVisits) ?? int.MinValue;

        // Если клетка красная, пробуем уменьшать ресурсы
        if (_cellsMatrix[row][column].Color == 1)
        {
            fromTop = Math.Max(fromTop, FindMaxPathCost(row - 1, column, remainingRedVisits - 1) ?? int.MinValue);
            fromRight = Math.Max(fromRight, FindMaxPathCost(row, column + 1, remainingRedVisits - 1) ?? int.MinValue);
        }

        // Максимизируем результат
        int maxPreviousCost = Math.Max(fromTop, fromRight);
        if (maxPreviousCost != int.MinValue)
        {
            _dpMemo[row, column, remainingRedVisits] = maxPreviousCost + _cellsMatrix[row][column].Cost;
            return maxPreviousCost + _cellsMatrix[row][column].Cost;
        }

        return null;
    }

    /// <summary>
    /// Восстановление оптимального пути
    /// </summary>
    private void ReconstructPath()
    {
        int row = _rows - 1;
        int column = 0;
        int remainingRedVisits = _maxRedVisits;

        Path.Clear();
        Path.Add((Row: row, Column: column)); // Начинаем с начальной клетки

        try
        {
            while (!(row == 0 && column == _columns - 1))
            {
                int currentCost = _dpMemo[row, column, remainingRedVisits].GetValueOrDefault();
                int topCost = row > 0 ? _dpMemo[row - 1, column, remainingRedVisits].GetValueOrDefault(Int32.MinValue) : Int32.MinValue;
                int rightCost = column < _columns - 1 ? _dpMemo[row, column + 1, remainingRedVisits].GetValueOrDefault(Int32.MinValue) : Int32.MinValue;

                //// Дополнительно проверяем переходы через красные клетки
                //if (_cellsMatrix[row][column].Color == 1)
                //{
                //    topCost = Math.Max(topCost, row > 0 ? _dpMemo[row - 1, column, remainingRedVisits - 1].GetValueOrDefault(Int32.MinValue) : Int32.MinValue);
                //    rightCost = Math.Max(rightCost, column < _columns - 1 ? _dpMemo[row, column + 1, remainingRedVisits - 1].GetValueOrDefault(Int32.MinValue) : Int32.MinValue);
                //}

                // Выбираем следующий шаг
                if (topCost == currentCost - _cellsMatrix[row][column].Cost)
                {
                    row--;
                    if (_cellsMatrix[row + 1][column].Color == 1) remainingRedVisits--;
                }
                else if (rightCost == currentCost - _cellsMatrix[row][column].Cost)
                {
                    column++;
                    if (_cellsMatrix[row][column - 1].Color == 1) remainingRedVisits--;
                }
                else if (Path.Count == (_rows + _columns - 2))
                    {
                        row = 0;
                        column = _columns - 1;
                    }
                else
                {
                    throw new Exception("Невозможно восстановить путь");
                }

                Path.Add((Row: row, Column: column));
            }


            Path.Reverse();
            foreach ((int r, int c) in Path)
            {
                Debug.WriteLine($"Клетка ({r},{c}), Стоимость: {_cellsMatrix[r][c].Cost}");
            }
        }
        catch (Exception e)
        {
            Path = new List<(int, int)>();
        }
    }
}