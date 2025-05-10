namespace RepkaLoadTest
{
    public class Logger
    {
        private readonly string _logFilePath;

        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            //using (StreamWriter writer = File.AppendText(_logFilePath))
            //{
            //    try
            //    {
            //        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            //    }
            //    catch (Exception ex) { }
            //}

            try
            {
                if (!File.Exists(_logFilePath))
                {
                    File.Create(_logFilePath).Dispose();
                }

                // Открываем файл для добавления текста, с возможностью совместного доступа для чтения и записи
                using (var fs = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var writer = new StreamWriter(fs))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (IOException ioEx)
            {
                // Обрабатываем исключения ввода-вывода
                Console.WriteLine($"Ошибка ввода-вывода при записи в журнал: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                // Обрабатываем общие исключения
                Console.WriteLine($"Неожиданная ошибка при записи в журнал: {ex.Message}");
            }
        }
    }
}
