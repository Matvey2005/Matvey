using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

class SimpleHttpProxy
{
    private static HashSet<string> blackList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    static void Main()
    {
        LoadBlackList();
        int port = 8888;
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Proxy server started on port {port}");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Task.Run(() => HandleClient(client));
        }
    }

    static void LoadBlackList()
    {
        string filePath = "blacklist.txt";
        if (File.Exists(filePath))
        {
            blackList = new HashSet<string>(File.ReadAllLines(filePath), StringComparer.OrdinalIgnoreCase);
            Console.WriteLine($"Loaded {blackList.Count} blacklisted sites.");
        }
        else
        {
            Console.WriteLine("Blacklist file not found. Creating an empty list.");
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            using (client)
            using (NetworkStream clientStream = client.GetStream())
            using (StreamReader reader = new StreamReader(clientStream, Encoding.ASCII))
            using (StreamWriter writer = new StreamWriter(clientStream, Encoding.ASCII) { AutoFlush = true })
            {
                string requestLine = reader.ReadLine();
                if (string.IsNullOrEmpty(requestLine)) return;

                string[] tokens = requestLine.Split(' ');
                if (tokens.Length < 3 || tokens[0] != "GET") return;

                string fullUrl = tokens[1];
                if (!Uri.TryCreate(fullUrl, UriKind.Absolute, out Uri uri)) return;

                string host = uri.Host;
                int port = uri.Port;
                string path = uri.PathAndQuery;

                Console.WriteLine($"Request: {fullUrl}");

                if (IsBlocked(host, fullUrl))
                {
                    Console.WriteLine($"Blocked: {fullUrl}");
                    SendBlockedResponse(clientStream, host);
                    return;
                }

                using (TcpClient server = new TcpClient(host, port))
                using (NetworkStream serverStream = server.GetStream())
                using (StreamWriter serverWriter = new StreamWriter(serverStream, Encoding.ASCII) { AutoFlush = true })
                {
                    
                    serverWriter.WriteLine($"GET {path} HTTP/1.1");
                    serverWriter.WriteLine($"Host: {host}");
                    serverWriter.WriteLine("User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36");
                    serverWriter.WriteLine("Connection: close");
                    serverWriter.WriteLine();

                    
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    string statusLine = "";
                    bool firstLine = true;
                    while ((bytesRead = serverStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (firstLine)
                        {
                            statusLine = Encoding.ASCII.GetString(buffer, 0, bytesRead).Split('\n')[0].Trim();
                            firstLine = false;
                        }
                        clientStream.Write(buffer, 0, bytesRead);
                    }
                    Console.WriteLine($"Answer: {fullUrl} - {statusLine}");
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


    }

    static bool IsBlocked(string host, string url)
    {
        return blackList.Contains(host) || blackList.Contains(url) || blackList.Contains("http://" + host) || blackList.Contains("https://" + host); ;
    }

    static void SendBlockedResponse(NetworkStream stream, string blockedHost)
    {
        string blockedPage = $@"
<html>
    <head><title>Access Denied</title></head>
<body>
    <h1>Access Denied</h1>
    <p>The site <b>{blockedHost}</b> is blocked by the proxy server.</p>
</body>
</html>";

        string response = $@"
HTTP/1.1 403 Forbidden
Content-Type: text/html
Content-Length: {Encoding.UTF8.GetByteCount(blockedPage)}
Connection: close

{blockedPage}";

        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        stream.Write(responseBytes, 0, responseBytes.Length);
        stream.Flush();
    }

}
