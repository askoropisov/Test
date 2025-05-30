using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Visual_Matrix.Models
{
    public class FileHelper
    {
        //Функция чтения входного файла
        public static (int, int, int, ObservableCollection<ObservableCollection<Cell>>) ReadInputData(string file)
        {
            string[] lines = File.ReadAllLines(file);

            string firstLine = lines[0];
            string[] property = firstLine.Split(';');
            int RPSize = int.Parse(property[0]);
            int PercentRed = int.Parse(property[1]);
            int CountRedVisit = int.Parse(property[2]);

            ObservableCollection<ObservableCollection<Cell>> RP = new ObservableCollection<ObservableCollection<Cell>>();

            ReadCostMatrix(lines.Skip(1).Take(RPSize).ToList(), RP);
            ReadRedMatrix(lines.Skip(RPSize + 1).Take(RPSize).ToArray(), RPSize, RP);

            return (RPSize, PercentRed, CountRedVisit, RP);
        }

        //Функция чтения стоимости клеток
        private static void ReadCostMatrix(IEnumerable<string> lines, ObservableCollection<ObservableCollection<Cell>> RP)
        {
            foreach (string line in lines)
            {
                string[] values = line.Trim().Split(';');
                var row = new ObservableCollection<Cell>();

                foreach (string value in values)
                {
                    row.Add(new Cell { Cost = int.Parse(value) });
                }
                RP.Add(row);
            }
        }

        //Функция чтения красных клеток
        private static void ReadRedMatrix(string[] lines, int RPSize, ObservableCollection<ObservableCollection<Cell>> RP)
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

        public static void WriteOutputData(List<(int, int)> path, int RPSize, float PercentRed,
                                           int CountRedVisit, int Result, double duration, string output)
        {
            string pathPoints = string.Join("; ", path.Select(p => $"({p.Item1},{p.Item2})"));

            using StreamWriter file = new StreamWriter("result.txt");

            file.WriteLine($"Размер лабиринта: {RPSize}");
            file.WriteLine($"Процент красных клеток: {PercentRed}");
            file.WriteLine();
            file.WriteLine($"Сколько раз можно зайти в красные клетки: {CountRedVisit}");
            file.WriteLine("Найденный путь:");
            file.WriteLine(pathPoints);
            file.WriteLine($"Стоимость пути: {Result}");
            file.WriteLine($"Время, затраченное на вычисление пути: {duration} мс");


            using StreamWriter outFile = new StreamWriter(output);
            outFile.WriteLine(Result);
        }
    }
}
