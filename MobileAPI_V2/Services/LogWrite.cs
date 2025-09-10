using System.IO;
using System;

namespace MobileAPI_V2.Services
{
    public class LogWrite
    {
        public void LogException(Exception ex)
        {
            // Define the log file path
            string logFileName = "error_log.txt";
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFileName);

            // Create the log file if it doesn't exist
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Close();
            }

            // Write the exception details to the log file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"[{DateTime.Now}] Exception occurred: {ex.Message}");
                writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                writer.WriteLine(); // Add a blank line for separation
            }

            Console.WriteLine("Exception logged. Please check the log file for details.");
        }

        public void LogRequestException(string ex)
        {
            // Define the log file path
            string logFileName = "Request_Log.txt";
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFileName);

            // Create the log file if it doesn't exist
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Close();
            }

            // Write the exception details to the log file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"[{DateTime.Now}] RequestException occurred: {ex}");
                writer.WriteLine(); // Add a blank line for separation
            }

            Console.WriteLine("Request Exception logged. Please check the log file for details.");
        }
    }

}
