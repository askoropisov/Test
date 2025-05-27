using Avalonia.Controls.Shapes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Visual_Matrix.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public class Cell
        {
            public Cell() { }
            public int Cost { get; set; }      // Стоимость клетки
            public int Color { get; set; }    // Признак красной клетки
        }

        private readonly string Input;
        private readonly string Output;
        public MainWindowViewModel(string[] args)
        {
            Input = args[0];
            Output = args[1];
            ReadInputFile(Input);
        }

        public ObservableCollection<ObservableCollection<Cell>> RP { get; set; } = new ObservableCollection<ObservableCollection<Cell>>();

        public int RPSize { get; set; }
        public float PercentRed { get; set; }
        public int CountRedVisit { get; set; }


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
    }
}
