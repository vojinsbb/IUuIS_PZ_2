using NetworkService;
using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : ClassINotifyPropertyChanged
    {
        #region Initialization

        public ClassICommand<string> NavigationCommand { get; private set; }

        public NetworkDisplayView networkDisplayView;
        public NetworkEntitiesView networkEntitiesView;
        public MeasurementGraphView measurementGraphView;
        public NetworkDisplayViewModel networkDisplayViewModel;
        public NetworkEntitiesViewModel networkEntitiesViewModel;
        public MeasurementGraphViewModel measurementGraphViewModel;


        private ClassINotifyPropertyChanged currentViewModel;
        private ClassINotifyPropertyChanged alwaysOnViewModel;

        public ClassINotifyPropertyChanged CurrentViewModel
        {
            get { return currentViewModel; }
            set { SetProperty(ref currentViewModel, value); }
        }

        public ClassINotifyPropertyChanged AlwaysOnViewModel
        {
            get { return alwaysOnViewModel; }
            set { SetProperty(ref alwaysOnViewModel, value); }
        }

        public void OnNavigation(string destination)
        {
            switch (destination)
            {
                case "1_Entities":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "2_Graph":
                    CurrentViewModel = measurementGraphViewModel;

                    // sigurnosna inicijalizacija pre nego sto se graf prikaze
                    if (measurementGraphViewModel.SelectedEntityObject == null && measurementGraphViewModel.AllEntities.Count > 0)
    {
        measurementGraphViewModel.SelectedEntityObject = measurementGraphViewModel.AllEntities[0];
    }
                    break;
            }
        }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            NavigationCommand = new ClassICommand<string>(OnNavigation);

            networkDisplayViewModel = new NetworkDisplayViewModel(); 
            networkEntitiesViewModel = new NetworkEntitiesViewModel(networkDisplayViewModel);
            measurementGraphViewModel = new MeasurementGraphViewModel(networkEntitiesViewModel);

            networkDisplayView = new NetworkDisplayView { DataContext = networkDisplayViewModel };
            networkEntitiesView = new NetworkEntitiesView();
            measurementGraphView = new MeasurementGraphView(measurementGraphViewModel);


            CurrentViewModel = networkEntitiesViewModel;
            AlwaysOnViewModel = networkDisplayViewModel;

            createListener(); //Povezivanje sa serverskom aplikacijom

            networkEntitiesViewModel.Entities.CollectionChanged += this.OnCollectionChanged;

            networkEntitiesViewModel.Entities.CollectionChanged += OnCollectionChangedMeasurementGraphViewModel;
            //networkDisplayViewModel.Entities.CollectionChanged += this.OnCollectionChangedMeasurementGraphViewModel;
        }

        public MainWindowViewModel()
        {
            NavigationCommand = new ClassICommand<string>(OnNavigation);

            networkDisplayViewModel = new NetworkDisplayViewModel(this);
            networkDisplayView = new NetworkDisplayView(this);
            networkEntitiesViewModel = new NetworkEntitiesViewModel(networkDisplayViewModel);
            measurementGraphViewModel = new MeasurementGraphViewModel(networkEntitiesViewModel);

            CurrentViewModel = networkEntitiesViewModel;
            AlwaysOnViewModel = networkDisplayViewModel;
        }

        #endregion

        #region NotifyCollection

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Entity newEntity in e.NewItems)
                {
                    if (!networkDisplayViewModel.EntitiesInList.Contains(newEntity))
                    {
                        networkDisplayViewModel.EntitiesInList.Add(newEntity);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (Entity oldEntity in e.OldItems)
                {
                    if (networkDisplayViewModel.EntitiesInList.Contains(oldEntity))
                    {
                        networkDisplayViewModel.EntitiesInList.Remove(oldEntity);
                    }
                    else
                    {
                        int canvasIndex = networkDisplayViewModel.GetCanvasIndexForEntityId(oldEntity.Id);
                        networkDisplayViewModel.DeleteEntityFromCanvas(oldEntity);
                    }
                }
            }
        }

        private void OnCollectionChangedMeasurementGraphViewModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Entity newEntity in e.NewItems)
                {
                    if (!measurementGraphViewModel.AllEntities.Contains(newEntity))
                    {
                        measurementGraphViewModel.AllEntities.Add(newEntity);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (Entity oldEntity in e.OldItems)
                {
                    if (measurementGraphViewModel.AllEntities.Contains(oldEntity))
                    {
                        measurementGraphViewModel.AllEntities.Remove(oldEntity);
                    }
                }
            }
            //if (measurementGraphViewModel.AllEntities.Count == 1)
            //{
            //    measurementGraphViewModel.ComboBoxItems.Clear();
            //    foreach (var en in measurementGraphViewModel.AllEntities)
            //        measurementGraphViewModel.ComboBoxItems.Add($"{en.Name}: {en.Id}");

            //    measurementGraphViewModel.SelectedEntity = measurementGraphViewModel.ComboBoxItems[0];
            //}
            if (measurementGraphViewModel.AllEntities.Count == 1)
            {
                measurementGraphViewModel.SelectedEntityObject = measurementGraphViewModel.AllEntities[0];
            }
        }
        #endregion

        #region TCP Server

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(networkEntitiesViewModel.Entities.Count.ToString());
                            stream.Write(data, 0, data.Length);

                            if (File.Exists("Log.txt"))
                            {
                                File.WriteAllText("Log.txt", String.Empty);
                            }
                            else
                            {
                                File.Create("Log.txt");
                            }
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            if (networkEntitiesViewModel.Entities.Count > 0)
                            {
                                var splited = incomming.Split(':');
                                DateTime dt = DateTime.Now;
                                using (StreamWriter sw = File.AppendText("Log.txt"))
                                {
                                    sw.WriteLine(dt + "; " + splited[0] + ", " + splited[1]);
                                }

                                int id = Int32.Parse(splited[0].Split('_')[1]);

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var entity = networkEntitiesViewModel.Entities.FirstOrDefault(e => e.Id == id);
                                    if (entity != null)
                                    {
                                        entity.AddValue(float.Parse(splited[1]));

                                        networkEntitiesViewModel.FilterValues.Clear();
                                        foreach (var e in networkEntitiesViewModel.Entities)
                                        {
                                            if (networkEntitiesViewModel.TempFilter?.FilterEntity(e) ?? true)
                                            {
                                                networkEntitiesViewModel.FilterValues.Add(e);
                                            }
                                        }

                                        networkDisplayViewModel.UpdateEntityOnCanvas(entity);
                                        //measurementGraphViewModel.UpdateGraph();

                                        if (measurementGraphViewModel.SelectedEntityObject != null && measurementGraphViewModel.SelectedEntityObject.Id == entity.Id)
                                        {
                                            measurementGraphViewModel.UpdateGraph(entity);
                                        }
                                    }
                                });
                            }
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

    #endregion

    }
}
