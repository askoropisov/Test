using RepkaLoadTest;
using System.Diagnostics;


//For Linux

Logger _logger = new Logger(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Amplituda", "RepkaLoadTest.txt"));
Timer _timer = new Timer(GetCPUParams, null, 0, 1000);

Console.WriteLine("Starting CPU stress test for Linux...");

Task.Run(CpuStress);
Task.Run(CpuStress);
Task.Run(CpuStress);
//Task.Run(CpuStress);

Console.ReadKey();

void GetCPUParams(object? state)
{
    double cpu = EvaluateCpuUsageOnLinux();

    Console.WriteLine($"CPU Total: {cpu} %, {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    _logger.Log($"CPU Total: {cpu} %");
}

double EvaluateCpuUsageOnLinux()
{
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        FileName = "/bin/bash",
        Arguments = "-c \"top -bn1 | grep 'Cpu(s)' | awk '{print $2 + $4}'\"",
        RedirectStandardOutput = true,
        UseShellExecute = false
    };

    using (Process process = Process.Start(startInfo))
    {
        string output = process.StandardOutput.ReadToEnd().Trim();
        float cpuUsage = float.Parse(output.Replace("%us", ""));
        return Math.Round(cpuUsage, 2);
    }
}


//For Windows

//PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
//Logger _logger = new Logger(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Amplituda", "RepkaLoadTest.txt"));
//Timer _timer = new Timer(GetCPUParams, null, 0, 1000);

//Console.WriteLine("Starting CPU stress test for Windows...");

//Task.Run(CpuStress);
//Task.Run(CpuStress);
//Task.Run(CpuStress);
//Task.Run(CpuStress);

//Console.ReadKey();

//void GetCPUParams(object? state)
//{
//    double cpu = EvaluateCpuUsageOnWindows();

//    Console.WriteLine($"CPU Total: {cpu} %, {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
//    _logger.Log($"CPU Total: {cpu} %");
//}

//double EvaluateCpuUsageOnWindows()
//{
//    float cpuUsage = cpuCounter.NextValue();
//    return Math.Round(cpuUsage, 2);
//}


//Public function
void CpuStress()
{
    Random rnd = new Random();

    while (true)
    {
        var number = rnd.Next(100, 10000);
        long factorialResult = CalculateFactorial(number);
    }
}

long CalculateFactorial(long n)
{
    return n == 0 ? 1 : CalculateFactorial(n - 1);
}

