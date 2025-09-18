using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : ClassINotifyPropertyChanged
    {

        private readonly NetworkEntitiesViewModel _networkEntitiesViewModel;

        public ObservableCollection<string> ComboBoxItems { get; set; } = new ObservableCollection<string>();

        public MeasurementGraphViewModel(NetworkEntitiesViewModel networkEntitiesViewModel)
        {
            AllEntities = new ObservableCollection<Entity>();
            _networkEntitiesViewModel = networkEntitiesViewModel;
            AllEntities = _networkEntitiesViewModel.Entities;
            AllEntities.CollectionChanged += AllEntities_CollectionChanged;

            ComboBoxItems = new ObservableCollection<string>(AllEntities.Select(e => $"{e.Name}: {e.Id}"));

            if (AllEntities.Count > 0)
            {
                SelectedEntityObject = AllEntities[0];
            }

            LastFiveValues = new ObservableCollection<float> { 0, 0, 0, 0, 0 };
            LastFiveDateTime = new ObservableCollection<string> { "", "", "", "", "" };

            Height1 = 0;
            Height2 = 0;
            Height3 = 0;
            Height4 = 0;
            Height5 = 0;
        }

        private void AllEntities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Entity newEntity in e.NewItems)
                {
                    ComboBoxItems.Add($"{newEntity.Name}: {newEntity.Id}");
                }
            }

            if (e.OldItems != null)
            {
                foreach (Entity oldEntity in e.OldItems)
                {
                    var itemToRemove = ComboBoxItems.FirstOrDefault(c => c.StartsWith(oldEntity.Name + ":"));
                    if (itemToRemove != null)
                        ComboBoxItems.Remove(itemToRemove);
                }
            }

            // Ako nema izabranog entiteta, izaberi prvi
            if (string.IsNullOrEmpty(SelectedEntity) && ComboBoxItems.Count > 0)
            {
                SelectedEntity = ComboBoxItems[0];
                UpdateGraph();
            }
        }

        public ObservableCollection<Entity> AllEntities { get; set; }

        private string _timeLabel1;
        private string _timeLabel2;
        private string _timeLabel3;
        private string _timeLabel4;
        private string _timeLabel5;

        private ObservableCollection<float> LastFiveValues { get; set; } = new ObservableCollection<float>();
        public  ObservableCollection<string> LastFiveDateTime { get; set; } = new ObservableCollection<string>();

        public string TimeLabel1
        {
            get { return _timeLabel1; }
            set
            {
                _timeLabel1 = value;
                OnPropertyChanged("TimeLabel1");
            }
        }
        public string TimeLabel2
        {
            get { return _timeLabel2; }
            set
            {
                _timeLabel2 = value;
                OnPropertyChanged("TimeLabel2");
            }
        }
        public string TimeLabel3
        {
            get { return _timeLabel3; }
            set
            {
                _timeLabel3 = value;
                OnPropertyChanged("TimeLabel3");
            }
        }
        public string TimeLabel4
        {
            get { return _timeLabel4; }
            set
            {
                _timeLabel4 = value;
                OnPropertyChanged("TimeLabel4");
            }
        }
        public string TimeLabel5
        {
            get { return _timeLabel5; }
            set
            {
                _timeLabel5 = value;
                OnPropertyChanged("TimeLabel5");
            }
        }

        private SolidColorBrush _blockColor1h = Brushes.DarkGreen;
        public SolidColorBrush BlockColor1h
        {
            get { return _blockColor1h; }
            set
            {
                _blockColor1h = value;
                OnPropertyChanged("BlockColor1h");
            }
        }
        private SolidColorBrush _blockColor2h = Brushes.DarkGreen;
        public SolidColorBrush BlockColor2h
        {
            get { return _blockColor2h; }
            set
            {
                _blockColor2h = value;
                OnPropertyChanged("BlockColor2h");
            }
        }
        private SolidColorBrush _blockColor3h = Brushes.DarkGreen;
        public SolidColorBrush BlockColor3h
        {
            get { return _blockColor3h; }
            set
            {
                _blockColor3h = value;
                OnPropertyChanged("BlockColor3h");
            }
        }
        private SolidColorBrush _blockColor4h = Brushes.DarkGreen;
        public SolidColorBrush BlockColor4h
        {
            get { return _blockColor4h; }
            set
            {
                _blockColor4h = value;
                OnPropertyChanged("BlockColor4h");
            }
        }
        private SolidColorBrush _blockColor5h = Brushes.DarkGreen;
        public SolidColorBrush BlockColor5h
        {
            get { return _blockColor5h; }
            set
            {
                _blockColor5h = value;
                OnPropertyChanged("BlockColor5h");
            }
        }

        private string selectedEntity;

        public string SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (selectedEntity != value)
                {
                    selectedEntity = value;
                    OnPropertyChanged("SelectedEntity");

                    if (!string.IsNullOrEmpty(selectedEntity) && AllEntities != null && AllEntities.Count > 0)
                        UpdateGraph();
                }
            }
        }

        private Entity _selectedEntityObject;
        public Entity SelectedEntityObject
        {
            get { return _selectedEntityObject; }
            set
            {
                if (_selectedEntityObject != value)
                {
                    _selectedEntityObject = value;
                    OnPropertyChanged("SelectedEntityObject");
                    if (_selectedEntityObject != null)
                        UpdateGraph(_selectedEntityObject);
                }
            }
        }

        public void UpdateGraph()
        {
            if (SelectedEntity == null) return;

            string[] parts = SelectedEntity.Split(':');
            if (parts.Length < 2) return;

            string idStr = parts[1].Trim().Split(' ')[0];
            if (!int.TryParse(idStr, out int id)) return;

            var en = AllEntities.FirstOrDefault(e => e.Id == id);
            if (en == null) return;

            UpdateGraph(en);
        }

        public void UpdateGraph(Entity en)
        {
            if (en == null) return;

            UpdateValues(en.Id);

            if (LastFiveValues.Count < 5 || LastFiveDateTime.Count < 5)
                return;

            double maxCanvasHeight = 500;
            double maxMW = 9;

            Height1 = (LastFiveValues[0] / maxMW) * maxCanvasHeight;
            Height2 = (LastFiveValues[1] / maxMW) * maxCanvasHeight;
            Height3 = (LastFiveValues[2] / maxMW) * maxCanvasHeight;
            Height4 = (LastFiveValues[3] / maxMW) * maxCanvasHeight;
            Height5 = (LastFiveValues[4] / maxMW) * maxCanvasHeight;

            BlockColor1h = (LastFiveValues[0] < 1 || LastFiveValues[0] > 5) ? Brushes.Red : Brushes.DarkGreen;
            BlockColor2h = (LastFiveValues[1] < 1 || LastFiveValues[1] > 5) ? Brushes.Red : Brushes.DarkGreen;
            BlockColor3h = (LastFiveValues[2] < 1 || LastFiveValues[2] > 5) ? Brushes.Red : Brushes.DarkGreen;
            BlockColor4h = (LastFiveValues[3] < 1 || LastFiveValues[3] > 5) ? Brushes.Red : Brushes.DarkGreen;
            BlockColor5h = (LastFiveValues[4] < 1 || LastFiveValues[4] > 5) ? Brushes.Red : Brushes.DarkGreen;

            TimeLabel1 = LastFiveDateTime[0] != DateTime.MinValue.ToString() ? LastFiveDateTime[0] : "";
            TimeLabel2 = LastFiveDateTime[1] != DateTime.MinValue.ToString() ? LastFiveDateTime[1] : "";
            TimeLabel3 = LastFiveDateTime[2] != DateTime.MinValue.ToString() ? LastFiveDateTime[2] : "";
            TimeLabel4 = LastFiveDateTime[3] != DateTime.MinValue.ToString() ? LastFiveDateTime[3] : "";
            TimeLabel5 = LastFiveDateTime[4] != DateTime.MinValue.ToString() ? LastFiveDateTime[4] : "";
        }


        public void UpdateValues(int id)
        {
            var entity = AllEntities.FirstOrDefault(e => e.Id == id);
            if (entity == null) return;

            if (entity.ValueHistory == null) 
                entity.ValueHistory = new List<float>();
            if (entity.TimelineValues == null) 
                entity.TimelineValues = new List<string>();

            LastFiveValues.Clear();
            LastFiveDateTime.Clear();

            for (int i = 0; i < 5; i++)
            {
                int valueIndex = Math.Max(0, entity.ValueHistory.Count - 5 + i);
                LastFiveValues.Add(entity.ValueHistory.Count > 0 ? entity.ValueHistory[valueIndex] : 0);

                int timeIndex = Math.Max(0, entity.TimelineValues.Count - 5 + i);
                LastFiveDateTime.Add(entity.TimelineValues.Count > 0 ? entity.TimelineValues[timeIndex] : DateTime.MinValue.ToString());
            }
        }

        private double height1;
        private double height2;
        private double height3;
        private double height4;
        private double height5;

        public double Height1
        {
            get { return height1; }
            set
            {
                if (value != height1)
                {
                    height1 = value;
                    OnPropertyChanged("Height1");
                }
            }
        }
        public double Height2
        {
            get { return height2; }
            set
            {
                if (value != height2)
                {
                    height2 = value;
                    OnPropertyChanged("Height2");
                }
            }
        }
        public double Height3
        {
            get { return height3; }
            set
            {
                if (value != height3)
                {
                    height3 = value;
                    OnPropertyChanged("Height3");
                }
            }
        }
        public double Height4
        {
            get { return height4; }
            set
            {
                if (value != height4)
                {
                    height4 = value;
                    OnPropertyChanged("Height4");
                }
            }
        }
        public double Height5
        {
            get { return height5; }
            set
            {
                if (value != height5)
                {
                    height5 = value;
                    OnPropertyChanged("Height5");
                }
            }
        }
    }
}
