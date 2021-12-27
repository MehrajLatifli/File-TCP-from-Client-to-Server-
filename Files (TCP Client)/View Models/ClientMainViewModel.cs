using FileHelper_ClassLibrary;
using Files__TCP_Client_.Commands;
using Files_ClassLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Files__TCP_Client_.View_Models
{
    public class ClientMainViewModel : BaseViewModel
    {
        public ClientMainWindow ClientMainWindows { get; set; }

        public RelayCommand LoadedCommand { get; set; }


        public RelayCommand SendImageCommand { get; set; }


        public ObservableCollection<Socket> _sockets { get; set; }

        public ObservableCollection<Socket> Sockets { get { return _sockets; } set { _sockets = value; OnPropertyChanged(); } }

        public ObservableCollection<Files> _fileList { get; set; }

        public ObservableCollection<Files> FileList { get { return _fileList; } set { _fileList = value; OnPropertyChanged(); } }

        public ObservableCollection<Files> _fileList2 { get; set; }

        public ObservableCollection<Files> FileList2 { get { return _fileList2; } set { _fileList2 = value; OnPropertyChanged(); } }


        string location = string.Empty;

        public ObservableCollection<string> _Texts { get; set; }

        public ObservableCollection<string> Texts { get { return _Texts; } set { _Texts = value; OnPropertyChanged(); } }


        DispatcherTimer timer = new DispatcherTimer();

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        bool check = false;

        bool addcheck = false;


        int threadcount = 1001;

        public static object obj = new object();
        public ClientMainViewModel()
        {

            string ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();

            var ipaddress = IPAddress.Parse(ip);

            var endpoint = new IPEndPoint(ipaddress, 5001);

            FileList = new ObservableCollection<Files>();

          

            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);

            timer.Tick += Timer_Tick;

            timer.Start();


            Thread[] threads = new Thread[threadcount];


            for (int i = 0; i < threadcount; i++)
            {
                threads[i] = new Thread(() =>
                {


                    addList(location);


                });
            }

            LoadedCommand = new RelayCommand((sender) =>
            {
                try
                {

                    socket.Connect(endpoint);

                    if (socket.Connected)
                    {
                        MessageBox.Show("Connected to the server ...");

                        var message = $"Connected to the server ...";
                        var bytes = Encoding.UTF8.GetBytes(message);
                        socket.Send(bytes);
                    }
                }
                catch (Exception)
                {


                }

            });

            SendImageCommand = new RelayCommand((sender) =>
            {
                check = true;




                //OpenFileDialog openFileDialog = new OpenFileDialog();
                //openFileDialog.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";

                //if (openFileDialog.ShowDialog() == true)
                //{

                //    location = openFileDialog.FileName;



                //    MessageBox.Show($"{location}");



                //   

                //}

                threadcount--;

                ClientMainWindows.ImageSendGrid.Drop += ImageSendBorder_Drop;

                ClientMainWindows.ImageSendGrid.DragEnter += ImageSendBorder_DragEnter;

                if (addcheck == true)
                {

                    threads.ElementAt(threadcount).Start();
                }



            });

        }



        private void ImageSendBorder_DragEnter(object sender, DragEventArgs e)
        {
            string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string d in drags)
            {
                if (!System.IO.File.Exists(d))
                {
                    return;
                }
            }

            e.Effects = DragDropEffects.All;
        }

        private void ImageSendBorder_Drop(object sender, DragEventArgs e)
        {
                addcheck = true;

           if (!Directory.Exists("../../../Files"))
            {
                Directory.CreateDirectory("../../../Files");
            }

            if (Directory.Exists("../../../Files"))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


                if (files == null) return;
                if (files.Length == 0) return;


                // location = Path.GetFullPath( files.ElementAt(0));

                for (int i = 0; i < files.Length; i++)
                {
                    addcheck = true;
                    location = Path.GetFullPath(files.ElementAt(i));
                }
            }

       

     


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {

                    ClientMainWindows.Listbox1.ItemsSource = null;

                    ClientMainWindows.Listbox1.Items.Clear();

                    foreach (var item in FileList)
                    {

                        ClientMainWindows.Listbox1.Items.Add(item);

                    }

                    ClientMainWindows.Listbox1.ItemsSource = FileList;







                }
                catch (Exception)
                {


                }


         

            }));

        }

        public  void addList(string location)
        {
            lock (obj)
            {

                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ClientMainWindows.Listbox1.ItemsSource = null;

                    ClientMainWindows.Listbox1.Items.Clear();



                    DirectoryInfo d = new DirectoryInfo(location);

                    ClientMainWindows.Listbox1.ItemsSource = null;

                    ClientMainWindows.Listbox1.Items.Clear();


                    try
                    {



                        if (location.Contains(".jpg"))
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


                            FileHelper.BinarySerialize(FileList);

                            MessageBox.Show($"{location}");
                        }

       

                        if (location.Contains(".pdf"))
                        {

                            FileList.Add(new Files()
                            {
                                FileShortName = $"{System.IO.Path.GetFileName(location)}",
                                FileName = $"{System.IO.Path.GetFileName(location)}",
                                FileAddDateTime = $" Add Time: {DateTime.Now.ToLocalTime()}",
                                FilePath = $"{ location}",
                                FolderofFile = $" Folder of File: {d}",
                                FileİmagePath = new BitmapImage(new Uri($"../../Images/Book.png", UriKind.RelativeOrAbsolute)).ToString()

                            });


                            FileHelper.BinarySerialize(FileList);

                            MessageBox.Show($"{location}");
                        }

                        if (location.Contains(".mp3"))
                        {

                            FileList.Add(new Files()
                            {
                                FileShortName = $"{System.IO.Path.GetFileName(location)}",
                                FileName = $"{System.IO.Path.GetFileName(location)}",
                                FileAddDateTime = $" Add Time: {DateTime.Now.ToLocalTime()}",
                                FilePath = $"{ location}",
                                FolderofFile = $" Folder of File: {d}",
                                FileİmagePath = new BitmapImage(new Uri($"../../Images/Music.png", UriKind.RelativeOrAbsolute)).ToString()

                            });


                            FileHelper.BinarySerialize(FileList);

                            MessageBox.Show($"{location}");
                        }

                        if (location.Contains(".mp4"))
                        {

                            FileList.Add(new Files()
                            {
                                FileShortName = $"{System.IO.Path.GetFileName(location)}",
                                FileName = $"{System.IO.Path.GetFileName(location)}",
                                FileAddDateTime = $" Add Time: {DateTime.Now.ToLocalTime()}",
                                FilePath = $"{ location}",
                                FolderofFile = $" Folder of File: {d}",
                                FileİmagePath = new BitmapImage(new Uri($"../../Images/Video.png", UriKind.RelativeOrAbsolute)).ToString()

                            });


                            FileHelper.BinarySerialize(FileList);

                            MessageBox.Show($"{location}");
                        }

                        if (location.Contains(".rar"))
                        {

                            FileList.Add(new Files()
                            {
                                FileShortName = $"{System.IO.Path.GetFileName(location)}",
                                FileName = $"{System.IO.Path.GetFileName(location)}",
                                FileAddDateTime = $" Add Time: {DateTime.Now.ToLocalTime()}",
                                FilePath = $"{ location}",
                                FolderofFile = $" Folder of File: {d}",
                                FileİmagePath = new BitmapImage(new Uri($"../../Images/Archive.png", UriKind.RelativeOrAbsolute)).ToString()

                            });


                            FileHelper.BinarySerialize(FileList);

                            MessageBox.Show($"{location}");
                        }

                        if (!location.Contains(".rar") && location.Contains(".rar") && !location.Contains(".mp4") && !location.Contains(".mp3"))
                        {
                            

                            FileList.Add(new Files()
                            {
                                FileShortName = $"{System.IO.Path.GetFileName(location)}",
                                FileName = $"{System.IO.Path.GetFileName(location)}",
                                FileAddDateTime = $" Add Time: {DateTime.Now.ToLocalTime()}",
                                FilePath = $"{ location}",
                                FolderofFile = $" Folder of File: {d}",
                                FileİmagePath = new BitmapImage(new Uri($"../../../Images/Other.png", UriKind.RelativeOrAbsolute)).ToString()

                            });


                            FileHelper.BinarySerialize(FileList);

                            MessageBox.Show($"{location}");
                        }

                    }
                    catch (Exception)
                    {

                  

                    }

                }));
            }
        }


    }
}
