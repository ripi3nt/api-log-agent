using System.Diagnostics;
using System.Text.Json;

namespace AIToolbox.Services;

public class DataAnalysisService
{
   
    public async Task<string> executePython(string code, List<string> data) 
    {
        var tempFile = Path.GetTempFileName().Replace(".tmp", ".py");
        await File.WriteAllTextAsync(tempFile, code);
        
        string dataFile = Path.GetTempFileName().Replace(".tmp", ".json");
        File.WriteAllText(dataFile, JsonSerializer.Serialize(data));

        try
        {
            // Set up process to run Python
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python", // or "python3"
                    //Arguments = $"\"{tempFile}\"",
                    Arguments = $"\"{tempFile}\" \"{dataFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string stdout = await process.StandardOutput.ReadToEndAsync();
            string stderr = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return $"Error:\n{stderr}";
            }

            return stdout;
        }
        catch (Exception ex)
        {
            return $"Exception occurred: {ex.Message}";
        }
        finally
        {
            // Clean up
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}