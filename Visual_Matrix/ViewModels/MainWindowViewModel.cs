using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Visual_Matrix.Models;

namespace Visual_Matrix.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly string Input;
        private readonly string Output;
        private TimeSpan start;
        private TimeSpan finish;

        public MainWindowViewModel(string[] args)
        {
            Input = args[0];
            Output = args[1];
            ReadInputFile(Input);
            FindOptimalPath();
        }

        public ObservableCollection<ObservableCollection<Cell>> RP { get; set; } = new ObservableCollection<ObservableCollection<Cell>>();

        public int RPSize { get; set; }
        public float PercentRed { get; set; }
        public int CountRedVisit { get; set; }

        private int _res;
        public int Result
        {
            get => _res;
            set => this.RaiseAndSetIfChanged(ref _res, value);
        }

        public void FindOptimalPath()
        {
            var finder = new PathFinder(RP, CountRedVisit);

            start = DateTime.Now.TimeOfDay;
            finder.Solve();
            finish = DateTime.Now.TimeOfDay;
            DrawPath(finder.Path);
            WriteOutputData(finder.Path);
        }

        // Генерация простой случайной матрицы
        public void GenerateMaze()
        {
            RP.Clear();
            for (int i = 0; i < RPSize; i++)
            {
                var row = new ObservableCollection<Cell>();
                for (int j = 0; j < RPSize; j++)
                {
                    Cell cell = new Cell();
                    cell.Cost = Random.Shared.Next(-10, 51);
                    cell.Color = Random.Shared.NextDouble() > (1 - PercentRed / 100) ? 1 : 0;
                    row.Add(cell);
                }
                RP.Add(row);
            }
        }

        //Функция чтения входного файла
        private void ReadInputFile(string file)
        {
            string[] lines = File.ReadAllLines(file);

            string firstLine = lines[0];
            string[] property = firstLine.Split(';');
            RPSize = int.Parse(property[0]);
            PercentRed = int.Parse(property[1]);
            CountRedVisit = int.Parse(property[2]);

            ReadCostMatrix(lines.Skip(1).Take(RPSize).ToList());
            ReadRedMatrix(lines.Skip(RPSize + 1).Take(RPSize).ToArray());
        }

        //Функция чтения стоимости клеток
        private void ReadCostMatrix(IEnumerable<string> lines)
        {
            RP.Clear();
            foreach (string line in lines)
            {
                string[] values = line.Trim().Split(';');
                var row = new ObservableCollection<Cell>();

                foreach (string value in values)
                {
                    Cell cell = new Cell();
                    cell.Cost = (int)double.Parse(value);
                    row.Add(cell);
                }
                RP.Add(row);
            }
        }

        //Функция чтения красных клеток
        private void ReadRedMatrix(string[] lines)
        {
            for (int i = 0; i < RPSize; i++)
            {
                string[] values = lines[i].Trim().Split(' ');
                for (int j = 0; j < values.Length; j++)
                {
                    if (values[j] == "1") RP[i][j].Color = 1;
                }
            }
        }

        //Функция отрисовку пути и расчета его стоимости
        private void DrawPath(List<(int, int)> path)
        {
            if (path.Count == 0)
            {
                Result = 0;
            }

            int res = 0;
            foreach (var item in path)
            {
                RP[item.Item1][item.Item2].Color = 2;
                res += RP[item.Item1][item.Item2].Cost;
            }
            Result = res;
        }

        //Функция записи результатов работы алгоритма в файл
        private void WriteOutputData(List<(int, int)> path)
        {
            string pathPoints = string.Empty;
            foreach (var cellCoordinate in path)
            {
                pathPoints += $"({cellCoordinate.Item1},{cellCoordinate.Item2}); ";
            }

            using StreamWriter file = new StreamWriter("result.txt");

            file.WriteLine($"Размер лабиринта: {RPSize}");
            file.WriteLine($"Процент красных клеток: {PercentRed}");
            file.WriteLine();
            file.WriteLine($"Сколько раз можно зайти в красные клетки: {CountRedVisit}");
            file.WriteLine("Найденный путь:");
            file.WriteLine(pathPoints);
            file.WriteLine($"Стоимость пути: {Result}");
            file.WriteLine($"Время, затраченное на вычисление пути: {(finish-start).TotalMilliseconds} мс");

        }
    }

}
