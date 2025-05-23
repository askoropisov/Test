using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    public static class NativeMethods
    {
        const string DllName = "calibry_sdk.dll";

        // Структура, соответствующая виртуальному классу Properties
        [StructLayout(LayoutKind.Sequential)]
        public struct Properties { }

        // Структура, соответствующая виртуальному классу CalibrySdkObject
        [StructLayout(LayoutKind.Sequential)]
        public struct CalibrySdkObject { }


        public enum CaptureStatus
        {
            Capture_Status_Error = -1,
            Capture_Status_Offline = 0,
            Capture_Status_Initializing = 1,
            Capture_Status_Ready = 2,
            Capture_Status_Busy = 3
        }; 

        // Возвращает статус
        [DllImport(DllName)]
        public static extern CaptureStatus get_capture_status(IntPtr obj);

        // Читает свойства из файла
        [DllImport(DllName)]
        public static extern bool read_properties(out IntPtr props, string pathBytes);

        // Создает экземпляр CalibrySdkObject
        [DllImport(DllName)]
        public static extern bool create_calibry_sdk_object(out IntPtr obj, IntPtr properties);

        // Устанавливает обработчик логирования
        [DllImport(DllName)]
        public static extern bool set_logger(IntPtr obj, IntPtr log);


        // Инициализирует устройство захвата
        [DllImport(DllName)]
        public static extern bool initialize_capture(IntPtr obj);

        // Запускает захват
        [DllImport(DllName)]
        public static extern bool start_capturing(IntPtr obj);

        // Останавливает захват
        [DllImport(DllName)]
        public static extern bool stop_capturing(IntPtr obj);

        // Обрабатывает полученные данные
        [DllImport(DllName)]
        public static extern bool process_scanned_data(IntPtr obj);

        // Сохраняет результат
        [DllImport(DllName)]
        public static extern bool save_result(IntPtr obj, string path);

        // Удаляет экземпляр CalibrySdkObject
        [DllImport(DllName)]
        public static extern bool destroy_calibry_sdk_object(ref IntPtr obj);
    }

    // Основная логика приложения
    public class Program
    {
        public static int Main(string[] args)
        {
            var settingsPath = "";
            var outputPath = "result.ply";
            IntPtr properties = IntPtr.Zero;

            for (var i = 0; i < args.Length; ++i)
            {
                if (args[i].Equals("-s") || args[i].Equals("--settings"))
                    settingsPath = args[i + 1];

                if (args[i].Equals("-o") || args[i].Equals("--output"))
                    outputPath = args[i + 1];
            }

            if (string.IsNullOrEmpty(settingsPath))
            {
                Console.WriteLine("Error: No settings file provided.");
                return 1;
            }

            // Чтение настроек
            IntPtr propsPointer;
            if (!NativeMethods.read_properties(out propsPointer, settingsPath))
            {
                Console.WriteLine("Error loading settings file.");
                return 1;
            }

            // Создание экземпляра CalibrySdkObject
            IntPtr sdkObjectPointer;
            if (!NativeMethods.create_calibry_sdk_object(out sdkObjectPointer, propsPointer))
            {
                Console.WriteLine("Error initializing Calibry SDK.");
                return 1;
            }


            bool askedForExit = false;
            do
            {
                Console.WriteLine("What shall we do?");
                Console.WriteLine("\ti -- initialize capture device");
                Console.WriteLine("\tc -- start capturing");
                Console.WriteLine("\tf -- finish capturing");
                Console.WriteLine("\tp -- process scanned data");
                Console.WriteLine("\tv -- save result");
                Console.WriteLine("\ts -- check capture status");
                Console.WriteLine("\tx -- exit");

                var inputChar = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (inputChar)
                {
                    case 'i':
                    case 'I':
                        try
                        {
                            NativeMethods.initialize_capture(sdkObjectPointer);
                            Console.WriteLine("Initialize device is success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 'c':
                    case 'C':
                        try
                        {
                            NativeMethods.start_capturing(sdkObjectPointer);
                            Console.WriteLine("Start Scanning");
                        } 
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 'f':
                    case 'F':
                        try
                        {
                            NativeMethods.stop_capturing(sdkObjectPointer);
                            Console.WriteLine("Finish Scanning");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 'p':
                    case 'P':
                        try
                        {
                            Console.WriteLine("Data processing started");
                            NativeMethods.process_scanned_data(sdkObjectPointer);
                            Console.WriteLine("Data processing is completed");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 'v':
                    case 'V':
                        try
                        {
                            NativeMethods.save_result(sdkObjectPointer, outputPath);
                            Console.WriteLine("Result is saved");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 's':
                    case 'S':
                        var status = NativeMethods.get_capture_status(sdkObjectPointer);
                        Console.WriteLine($"Current capture status is: {(int)status}");
                        break;
                    case 'x':
                    case 'X':
                        askedForExit = true;
                        break;
                    default:
                        Console.WriteLine("Unknown command. Please enter a valid option.");
                        break;
                }
            } while (!askedForExit);

            // Уничтожаем экземпляр CalibrySdkObject
            NativeMethods.destroy_calibry_sdk_object(ref sdkObjectPointer);

            return 0;
        }
    }

    // Обратный вызов для передачи сообщений от native-кода в C#
    delegate void LoggerCallback(string message);
}
