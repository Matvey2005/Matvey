using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

async Task GetListFiles(HttpRequest request, HttpResponse response, string path)
{
    var files = Directory.EnumerateFiles(path).Select(Path.GetFileName).ToArray();
    
    await response.WriteAsJsonAsync(/*new { Files = files }*/files);
    //await response.WriteAsync($"DEBUG: {string.Join(", ", files)}");
    Console.WriteLine(files);
}

async Task GetContentFile(HttpResponse response, string path)
{
    await OpenFile(path);
}

async Task OpenFile(string filePath)
{
    
    if (!File.Exists(filePath))
    {
        Console.WriteLine("Файл не существует: " + filePath);
        return;
    }

    try
    {
        
        Process.Start(new ProcessStartInfo
        {
            FileName = filePath, 
            UseShellExecute = true 
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Произошла ошибка при открытии файла: {ex.Message}");
    }
}

static string GetMimeType(string filePath)
{
    string extension = Path.GetExtension(filePath).ToLower();
    return extension switch
    {
        ".txt" => "text/plain",
        ".html" => "text/html",
        ".jpg" => "image/jpeg",
        ".png" => "image/png",
        ".pdf" => "application/pdf",
        ".zip" => "application/zip",
        _ => "application/octet-stream"
    };
}

async Task GetFileInfo(HttpResponse response, string filePath)
{
    if (File.Exists(filePath))
    {
        var fileInfo = new FileInfo(filePath);
        response.Headers.Append("Content-Length", fileInfo.Length.ToString());
        response.Headers.Append("Last-Modified", fileInfo.LastWriteTime.ToString("R")); 

        response.StatusCode = 200;
    }
    else
    {
        response.StatusCode = 404;
    }
}



var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "api", "file");
Directory.CreateDirectory(uploadPath); 

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    string path = request.Path;
    var regex = new Regex(@"^[a-zA-Z0-9_\-]+\.txt$");

    if (path == "/api/file" && request.Method == "GET")
    {
        await GetListFiles(request, response, uploadPath);
    }
    else if (path.Split('/').Length > 2 && request.Method == "GET")
    {
        string fileName = path.Split('/')[3];
        string encodeName = Uri.UnescapeDataString(fileName);
        var filePath = Path.Combine(uploadPath, encodeName);

        if (File.Exists(filePath))
        {
            response.StatusCode = 200;
            response.ContentType = GetMimeType(filePath);
            var encodedFileName = Uri.EscapeDataString(encodeName);
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{encodedFileName}\"");

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(response.Body);
            }
        }
        else
        {
            response.StatusCode = 404;
        }
    }
    else if (path.Split('/').Length > 2 && request.Method == "PUT")
    {
        string copyPath = request.Headers["X-Copy-From"];
        if (!string.IsNullOrEmpty(copyPath))
        {
            string fileName = path.Split('/')[3];
            string encodeName = Uri.UnescapeDataString(fileName);
            string copyPathDirectory = Path.Combine(Directory.GetCurrentDirectory(), "api", "copy");
            if (!Directory.Exists(copyPathDirectory)) Directory.CreateDirectory(copyPathDirectory);
            string newPath = Path.Combine(copyPathDirectory, encodeName);
            string oldPath = Path.Combine(uploadPath, encodeName);
            if (File.Exists(oldPath))
            {
                File.Copy(oldPath, newPath, true);
                response.StatusCode = (int)HttpStatusCode.OK;
            }
        }
        else
        {
            string fileName = path.Split('/')[3];
            string encodeName = Uri.UnescapeDataString(fileName);
            var filePath = Path.Combine(uploadPath, encodeName);
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await request.Body.CopyToAsync(fileStream);
            }
            response.StatusCode = (int)HttpStatusCode.Created;
            await response.WriteAsync("Файл успешно загружен.");
        }
        
    }
    else if (path.Split('/').Length > 2 && request.Method == "DELETE")
    {
        string fileName = path.Split('/')[3];
        string encodeName = Uri.UnescapeDataString(fileName);
        var filePath = Path.Combine(uploadPath, encodeName);
        var file = new FileInfo(filePath);
        if (file.Exists)
        {
            file.Delete();
            response.StatusCode = (int)HttpStatusCode.NoContent;
            await response.WriteAsync("Файл успешно удален.");
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            await response.WriteAsync("Файл не найден.");
        }
    }
    else if (path.Split('/').Length > 2 && request.Method == "HEAD")
    {
        string fileName = path.Split('/')[3];
        string encodeName = Uri.UnescapeDataString(fileName);
        var filePath = Path.Combine(uploadPath, encodeName);
        //var file = new FileInfo(filePath);
        await GetFileInfo(response, filePath);
    }
    else
    {
        await response.SendFileAsync("html/indexTest.html"); 
    }
});

app.Run();