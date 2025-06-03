using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Visual_Matrix.Models;
using Visual_Matrix.Views;

namespace Visual_Matrix.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly string Input;
        private readonly string Output;
        private TimeSpan start;
        private TimeSpan finish;
        private MainWindow _mv;

        public MainWindowViewModel(string[] args)
        {
            Input = args[0];
            Output = args[1];
            (RPSize, PercentRed, CountRedVisit, RP) = FileHelper.ReadInputData(Input);

            FindOptimalPath();
        }

        public ObservableCollection<ObservableCollection<Cell>> RP { get; set; } = new ObservableCollection<ObservableCollection<Cell>>();


        [Required(ErrorMessage = "Поле не должно быть пустым")]
        [Range(10, 30, ErrorMessage = "Некорректное значение. Укажите число от 10 до 30")]
        public int RPSize { get; set; }

        [Required(ErrorMessage = "Поле не должно быть пустым")]
        [Range(0, 100, ErrorMessage = "Некорректное значение. Укажите число от 0 до 100")]
        public float PercentRed { get; set; }

        [Required(ErrorMessage = "Поле не должно быть пустым")]
        [Range(3, 5, ErrorMessage = "Некорректное значение. Укажите число от 3 до 5")]
        public int CountRedVisit { get; set; }

        private int _res;
        public int Result
        {
            get => _res;
            set => this.RaiseAndSetIfChanged(ref _res, value);
        }

        public void FindOptimalPath()
        {
            if (RPSize <= 0 || PercentRed < 0 || CountRedVisit < 0) return;

            Progress.Show();
            var finder = new PathFinder(RP, CountRedVisit);
            start = DateTime.Now.TimeOfDay;
            finder.Solve();
            finish = DateTime.Now.TimeOfDay;
            Progress.Hide();

            DrawPath(finder.Path);
            FileHelper.WriteOutputData(finder.Path, RPSize, PercentRed, CountRedVisit, Result, (finish - start).TotalMilliseconds, Output);
        }

        // Генерация простой случайной матрицы
        public async void GenerateMaze()
        {
            RP.Clear();
            Progress.Show();

            await Task.Run(async () =>
            {
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
            });

            Progress.Hide();
        }

        //Функция отрисовки пути и расчета его стоимости
        private void DrawPath(List<(int, int)> path)
        {
            if (path.Count == 0) Result = 0;

            int res = 0;
            foreach (var item in path)
            {
                RP[item.Item1][item.Item2].Color = 2;
                res += RP[item.Item1][item.Item2].Cost;
            }
            Result = res;
        }

        private Progres Progress { get; set; } = new Progres();
    }
}
