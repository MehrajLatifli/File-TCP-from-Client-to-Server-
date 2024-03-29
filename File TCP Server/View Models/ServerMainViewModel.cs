﻿using File_TCP_Server.Commands;
using File_TCP_Server.Views;
using FileHelper_ClassLibrary;
using Files_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace File_TCP_Server.View_Models
{
    public class ServerMainViewModel : BaseViewModel
    {
        public ServerMainWindow ServerMainWindows { get; set; }



        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }

        public RelayCommand OpenFileCommand { get; set; }


        public ObservableCollection<Socket> _localsockets { get; set; }

        public ObservableCollection<Socket> Localsockets { get { return _localsockets; } set { _localsockets = value; OnPropertyChanged(); } }

        public ObservableCollection<Socket> _clientsockets { get; set; }

        public ObservableCollection<Socket> Clientsockets { get { return _clientsockets; } set { _clientsockets = value; OnPropertyChanged(); } }

        public ObservableCollection<Files> _fileList { get; set; }

        public ObservableCollection<Files> FileList { get { return _fileList; } set { _fileList = value; OnPropertyChanged(); } }

        public ObservableCollection<Files> _fileList2 { get; set; }

        public ObservableCollection<Files> FileList2 { get { return _fileList2; } set { _fileList2 = value; OnPropertyChanged(); } }


        public ObservableCollection<string> _Texts { get; set; }

        public ObservableCollection<string> Texts { get { return _Texts; } set { _Texts = value; OnPropertyChanged(); } }


        Socket _localsocket { get; set; }

        public Socket Localsocket { get { return _localsocket; } set { _localsocket = value; OnPropertyChanged(); } }


        string ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();

        IPAddress ipaddress = null;


        int threadcount = 1001;

        bool tbutton = false;
        object tbuttoncontent = new object();

        string historylistboxdata = string.Empty;
        string historylistboxdata2 = string.Empty;
        string historylistboxdata3 = string.Empty;
        string hs = string.Empty;
        string hc = string.Empty;
        string hc2 = string.Empty;
        string h = string.Empty;
        bool check = false;
        bool checker = false;
        bool check2 = false;
        bool checker2 = false;
        bool s = false;

        DispatcherTimer timer = new DispatcherTimer();

        DispatcherTimer timer2 = new DispatcherTimer();


        public ServerMainViewModel()
        {
            Texts = new ObservableCollection<string>();

            Thread[] threads = new Thread[threadcount];

            Semaphore semaphore = new Semaphore(1, 2, "Semaphore");

            ipaddress = IPAddress.Parse(ip);

            FileList = new ObservableCollection<Files>();
            FileList2 = new ObservableCollection<Files>();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            timer.Tick += Timer_Tick;

            timer.Start();



            timer2.Interval = new TimeSpan(0, 0, 0, 10);

            timer2.Tick += Timer2_Tick;

            timer2.Start();






            for (int i = 0; i < threadcount; i++)
            {
                threads[i] = new Thread(() =>
                {


                    Connect(semaphore);


                });
            }

            LoadedCommand = new RelayCommand((sender) =>
            {
                ServerMainWindows.Listbox1.ItemsSource = null;

                ServerMainWindows.Listbox1.Items.Clear();

                timer.Start();
                timer2.Start();
            });

            ConnectCommand = new RelayCommand((sender) =>
            {

                threadcount--;


                timer.Start();


                if (tbutton == false)
                {

                    ServerMainWindows.ConnectToggleButton.Content = "Disconnect";

                    s = true;

                    threads.ElementAt(threadcount).Start();
                    Thread.Sleep(100);
                    MessageBox.Show($"Added to history");
                    timer2.Start();


                    //ServerMainWindows.HistoryListbox.Items.Add($"\n {Texts}");





                    Texts.Add(hs);


                    ServerMainWindows.Title += $"{hs}";

                    Localsockets.Add(Localsocket);


                }
                else
                {
                    check = false;
                    check2 = false;

                    ServerMainWindows.ConnectToggleButton.Content = "Connect";

                    for (int i = 0; i < Localsockets.Count; i++)
                    {

                        foreach (var item in Localsockets)
                        {
                            var ep = Localsockets.ElementAt(i);

                            if (ep == item)
                            {
                                Localsockets.ElementAt(i).Close();

                                //  ServerMainWindows.HistoryListbox.Items.Remove(item);
                            }

                        }


                        Localsockets.Remove(Localsocket);


                    }

                    if (!string.IsNullOrEmpty(hs))
                    {

                        Texts.Add(hs + " Stoped");

                    }
                    if (!string.IsNullOrEmpty(hc))
                    {

                        Texts.Add(hc + " Stoped");

                    }


                    timer2.Stop();
                }



            });



            OpenFileCommand = new RelayCommand((sender) =>
            {



                ServerMainWindows.Listbox1.SelectionChanged += Listbox1_SelectionChanged; 

            });

        }

        private void Listbox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            if (ServerMainWindows.Listbox1.SelectedIndex != -1)
            {
                var j = ServerMainWindows.Listbox1.SelectedIndex;

                if (j != -1)
                {


                    if (!Directory.Exists("../../DownloadFolder"))
                    {
                        Directory.CreateDirectory("../../DownloadFolder");

                    }

                    Guid guid = Guid.NewGuid();

                    if (Directory.Exists("../../DownloadFolder"))
                    {
                        File.Copy($"{FileList.ElementAt(j).FilePath}", $"../../DownloadFolder/{guid} {Path.GetExtension(FileList.ElementAt(j).FilePath)}", true);


                        MessageBox.Show($"{FileList.ElementAt(j).FilePath} downloaded");

                       // Process.Start("msedge.exe", $"{FileList.ElementAt(j).FilePath}");
                    }
                }
               

                    
                
            }
        }

        private void Listbox1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
       
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (System.IO.File.Exists("../../../Files/files.bin"))
            {
                var deserializedList = FileHelper.BinaryDeSerialize();
                FileList = (ObservableCollection<Files>)deserializedList;

                ServerMainWindows.Listbox1.ItemsSource = null;

                ServerMainWindows.Listbox1.Items.Clear();


                foreach (var item in FileList)
                {
           
                    ServerMainWindows.Listbox1.Items.Add(item);

                }





            }


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ServerMainWindows.Dispatcher.BeginInvoke(new Action(() =>
            {

                tbutton = (bool)ServerMainWindows.ConnectToggleButton.IsChecked;


            }));





            if (check == true || check2 == true)
            {
                if (!string.IsNullOrEmpty(hs))
                {




                }

                if (!string.IsNullOrEmpty(hc))
                {


                    Texts.Add(hc);
                    Texts.Add(hc2);


                    timer.Stop();


                }

            }




        }




        private void Connect(object state)
        {
            Task.Run(() =>
            {

                //string ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();

                //var ipaddress = IPAddress.Parse(ip);

                var s = state as Semaphore;

                bool st = false;

                while (!st)
                {
                    if (s.WaitOne(500))
                    {

                        try
                        {


                            Localsockets = new ObservableCollection<Socket>();

                            Clientsockets = new ObservableCollection<Socket>();

                            try
                            {

                                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                                {
                                    var Localendpoint = new IPEndPoint(ipaddress, 5001);
                                    socket.Bind(Localendpoint);

                                    socket.Listen(10);

                                    checker = true;
                                    check = checker;


                                    MessageBox.Show($"Listening on {socket.LocalEndPoint}");

                                    Localsockets.Add(socket);

                                    historylistboxdata += socket.LocalEndPoint.ToString();

                                    hs = historylistboxdata + $"\t {DateTime.Now} \t";

                                    MessageBox.Show($"{hs}");

                                    while (true)
                                    {

                                        var clientsocket = socket.Accept();

                                        checker2 = true;
                                        check2 = checker2;

                                        Task.Run(() =>
                                        {

                                            MessageBox.Show($" {clientsocket.RemoteEndPoint} connected ...");

                                            var length = 0;

                                            var length2 = 0;
                                            var bytes = new byte[1024];

                                            do
                                            {
                                                try
                                                {


                                                    if (clientsocket.Connected == true)
                                                    {
                                                        length = clientsocket.Receive(bytes);

                                                        var message = Encoding.UTF8.GetString(bytes, 0, length);


                                                        string listtext = $"Client ({clientsocket.RemoteEndPoint}) : {message}";

                                                        historylistboxdata2 += $"Client ({clientsocket.RemoteEndPoint}) : {message}";

                                                        hc = historylistboxdata2;


                                                        if (!string.IsNullOrEmpty(listtext))
                                                        {

                                                            byte[] clientData = new byte[1024];
                                                            length2 = clientsocket.Receive(clientData);
                                                            var message2 = Encoding.UTF8.GetString(clientData, 0, length2);





                                                            //  addList(message2);

                                                            historylistboxdata3 += $"Client send: ( { message2})";

                                                            hc2 = historylistboxdata3;

                                                            MessageBox.Show($"{h} haha");


                                                        }


                                                        if (message == "ok")
                                                        {
                                                            clientsocket.Shutdown(SocketShutdown.Both);
                                                            clientsocket.Dispose();
                                                            break;
                                                        }




                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                    MessageBox.Show($"Shutdown => Client ({ex.Message})");

                                                }


                                            } while (true);

                                        });



                                        Clientsockets.Add(clientsocket);




                                    }




                                }




                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"{ex.Message}");

                            }

                        }
                        catch (Exception)
                        {


                        }
                        finally
                        {

                            st = true;
                        }
                    }

                    else
                    {
                        try
                        {
                            s.Release();
                        }
                        catch (Exception)
                        {


                        }
                    }
                }

            });

        }



        public void addList(string location)
        {
            ServerMainWindows.Listbox1.ItemsSource = null;

            ServerMainWindows.Listbox1.Items.Clear();



            DirectoryInfo d = new DirectoryInfo(location);

            ServerMainWindows.Listbox1.ItemsSource = null;

            ServerMainWindows.Listbox1.Items.Clear();


            if (location.Contains(".jpg") || location.Contains(".png") || location.Contains(".jpeg") || location.Contains(".gif"))
            {

                FileList.Add(new Files()
                {
                    FileShortName = $"{System.IO.Path.GetFileName(location)}",
                    FileName = $"{System.IO.Path.GetFileName(location)}",
                    FileAddDateTime = $" Add Time: {DateTime.Now.ToLocalTime()}",
                    FilePath = $"{ location}",
                    FolderofFile = $" Folder of File: {d}",
                    FileİmagePath = location,

                });

                MessageBox.Show($"{location}");
            }

            else
            {
                //Font font = new Font(ClientMainWindows.SendTextbox.FontFamily.FamilyNames,);

                //FileHelper.DrawText(ClientMainWindows.SendTextbox.Text, font,)
            }




        }
    }
}
