using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class P2PChat
{
    static UdpClient udpClient;
    static TcpListener tcpListener;
    static List<TcpClient> peers = new List<TcpClient>();
    static string userName;   
    static HashSet<string> receivedMessages = new HashSet<string>();
    static List<string> story = new List<string>();
    static int udpPort = 8888; 
    static int tcpPort; 

    static void Main(string[] args)
    {
        Console.Write("Введите ваше имя: ");
        userName = Console.ReadLine();
        tcpPort = new Random().Next(8900, 9000); 

        
        

        StartUdpListener();
        StartTcpListener();
        BroadcastPresence();
       

        Console.WriteLine("Вы подключились");
        story.Add("[Входящие] Вы подключились");
        SetConsoleCtrlHandler(new ConsoleCtrlDelegate(ConsoleClosing), true);

        while (true)
        {
            string message = Console.ReadLine();
            if(message.ToLower() == "история")
            {
                ShowHistory();
                continue;
            }
            string formattedMessage = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {userName}: {message}";
            Console.WriteLine(formattedMessage);
            story.Add("[Исходящие] " + formattedMessage);
            SendMessageToPeers(formattedMessage);
        }
    }

    static void ShowHistory()
    {
        Console.WriteLine("\nИстория сообщений:");
        Console.WriteLine("====================");
        Console.WriteLine("[Исходящие]");
        foreach (var msg in story.Where(m => m.StartsWith("[Исходящие]")))
        {
            Console.WriteLine(msg.Substring(13));
        }
        Console.WriteLine("\n[Входящие]");
        foreach (var msg in story.Where(m => m.StartsWith("[Входящие]")))
        {
            Console.WriteLine(msg.Substring(11));
        }
        Console.WriteLine("====================\n");
    }

    [System.Runtime.InteropServices.DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    private delegate bool ConsoleCtrlDelegate(int sig);

    private static bool ConsoleClosing(int sig)
    {
        Disconnect(); 
        return false; 
    }


    static void Disconnect()
    {
        string exitMessage = $"[EXIT] {userName}";
        
        try
        {
            Console.WriteLine(exitMessage);

            
            foreach (var peer in new List<TcpClient>(peers)) 
            {
                try
                {
                    if (peer.Connected)
                    {
                        NetworkStream stream = peer.GetStream();
                        byte[] exitData = Encoding.UTF8.GetBytes(exitMessage + "\n");
                        

                        stream.Write(exitData, 0, exitData.Length);
                        
                        stream.Flush();
                    }
                }
                catch
                {
                    
                }
                peer.Close();
            }

            
            peers.Clear();
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выходе: {ex.Message}");
        }
        finally
        {
            

            udpClient?.Close();
            tcpListener?.Stop();
        }
    }


    static void StartUdpListener()
    {
        udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, udpPort));

        Thread udpThread = new Thread(() =>
        {
            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, udpPort);
                byte[] data = udpClient.Receive(ref remoteEP);
                string receivedData = Encoding.UTF8.GetString(data);

                string[] parts = receivedData.Split(':');
                if (parts.Length == 2)
                {
                    string receivedName = parts[0];
                    int receivedTcpPort = int.Parse(parts[1]);

                    
                    ConnectToPeer(remoteEP.Address.ToString(), receivedTcpPort);
                }
            }
        });
        udpThread.IsBackground = true;
        udpThread.Start();
    }

    static void BroadcastPresence()
    {
        UdpClient client = new UdpClient();
        client.EnableBroadcast = true;
        string message = $"{userName}:{tcpPort}"; 
        byte[] data = Encoding.UTF8.GetBytes(message);
        client.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, udpPort));
    }

    static void StartTcpListener()
    {
        tcpListener = new TcpListener(IPAddress.Any, tcpPort);
        tcpListener.Start();
        Thread tcpThread = new Thread(() =>
        {
            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                peers.Add(client);
               
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        });
        tcpThread.IsBackground = true;
        tcpThread.Start();
    }

    static void ConnectToPeer(string ipAddress, int peerTcpPort)
    {
        try
        {
            TcpClient client = new TcpClient(ipAddress, peerTcpPort);
            peers.Add(client);
            
            SendMessageToPeers($"Пользователь {userName} подключился");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.IsBackground = true;
            clientThread.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения к {ipAddress}:{peerTcpPort} - {ex.Message}");
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        try
        {
            while (client.Connected)
            {
                string message = reader.ReadLine();

                if (message != null && !message.Contains(userName))
                {
                    if (receivedMessages.Contains(message))
                    {
                        continue;
                    }
                    if (message.StartsWith("Пользователь ") && message.EndsWith(" подключился"))
                    {
                        if (!receivedMessages.Contains(message)) 
                        {
                            receivedMessages.Add(message);
                            story.Add("[Входящие] " + message);
                            Console.WriteLine(message);
                            SendMessageToPeers(message); 
                        }
                    }
                    else if (message.StartsWith("[EXIT]"))
                    {
                        string exitedUser = message.Substring(7);
                        string deleteMessage = receivedMessages.FirstOrDefault(x => x.Contains(exitedUser));
                        receivedMessages.Remove(deleteMessage);
                        string exitMsg = $"Пользователь {exitedUser} вышел";
                        story.Add("[Входящие] " + exitMsg);
                        Console.WriteLine(exitMsg);
                    }
                    else
                    {
                        receivedMessages.Add(message);
                        story.Add("[Входящие] " + message);
                        Console.WriteLine(message);
                        SendMessageToPeers(message);
                    }
                }

            }
        }
        catch
        {
            client.Close();
            peers.Remove(client);
        }
    }

    static void SendMessageToPeers(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");

        
        List<TcpClient> peersCopy = new List<TcpClient>(peers);

        foreach (var peer in peersCopy)
        {
            try
            {
                
                if (((IPEndPoint)peer.Client.RemoteEndPoint).Port != tcpPort)
                {
                    peer.GetStream().Write(data, 0, data.Length);
                }
            }
            catch
            {
                peer.Close();
                peers.Remove(peer); 
            }
        }
    }


}
