using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Xml;


namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : ClassINotifyPropertyChanged
    {
        #region Initialization

        public static int Count { get; set; } = 0;

        private static int _idSolarPanel = -1;
        private static int _idWindGenerator = 0;
        private static int _id = 4;
        private string _selectedItemAdd;
        private string _selectedItemFilter;
        private string _selectedTypeFilter;
        private Entity _selectedEntityForDelete;
        private string _idTextBox;
        private bool _isLowerChecked;
        private bool _isEqualChecked;
        private bool _isHigherChecked;
        private ObservableCollection<Entity> _showedCollection;
        private ObservableCollection<Entity> _selectedItems;
        private List<string> _filterOptions;
        private List<string> _types;
        private Filter _tempFiler;
        private Entity _selectedEntity;


        public List<string> FilterOptions
        {
            get { return _filterOptions; }
            set
            {
                _filterOptions = value;
                OnPropertyChanged("FilterOptions");
            }
        }

        public List<string> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                OnPropertyChanged("Types");
            }
        }

        public ObservableCollection<Entity> FilterValues { get; set; }

        public Entity SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (_selectedEntity != value)
                {
                    _selectedEntity = value;
                    OnPropertyChanged("SelectedEntity");
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _selectedFilterText;
        public string SelectedFilterText
        {
            get { return _selectedFilterText; }
            set
            {
                if (_selectedFilterText != value)
                {
                    _selectedFilterText = value;
                    OnPropertyChanged("SelectedFilterText");
                }
            }
        }

        public Dictionary<string, Filter> Filters { get; set; }
        public ObservableCollection<string> FilterNames { get; set; }

        public ObservableCollection<Entity> ShowedCollection
        {
            get { return _showedCollection; }
            set
            {
                _showedCollection = value;
                OnPropertyChanged("ShowedCollection");
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<Entity> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
                OnPropertyChanged("SelectedItems");
            }
        }

        public ObservableCollection<Entity> Entities { get; set; }
        public Entity SelectedEntityForDelete
        {
            get { return _selectedEntityForDelete; }
            set
            {
                _selectedEntityForDelete = value;
                OnPropertyChanged("SelectedEntityForDelete");
            }
        }

        public Filter TempFilter
        {
            get { return _tempFiler; }
            set
            {
                _tempFiler = value;
                OnPropertyChanged("TempFilter");
            }
        }

        public string SelectedItemAdd
        {
            get { return _selectedItemAdd; }
            set
            {
                _selectedItemAdd = value;
                OnPropertyChanged("SelectedItemAdd");
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string SelectedItemFilter
        {
            get { return _selectedItemFilter; }
            set
            {
                _selectedItemFilter = value;
                OnPropertyChanged("SelectedItemFilter");
            }
        }

        public string SelectedTypeFilter
        {
            get { return _selectedTypeFilter; }
            set
            {
                _selectedTypeFilter = value;
                OnPropertyChanged("SelectedTypeFilter");
                ApplyCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string IdTextBox
        {
            get { return _idTextBox; }
            set
            {
                _idTextBox = value;
                OnPropertyChanged("IdTextBox");
                ApplyCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLowerChecked
        {
            get { return _isLowerChecked; }
            set
            {
                if (_isLowerChecked != value)
                {
                    _isLowerChecked = value;
                    OnPropertyChanged("IsLowerChecked");
                    ApplyCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsEqualChecked
        {
            get { return _isEqualChecked; }
            set
            {
                if (_isEqualChecked != value)
                {
                    _isEqualChecked = value;
                    OnPropertyChanged("IsEqualChecked");
                    ApplyCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsHigherChecked
        {
            get { return _isHigherChecked; }
            set
            {
                if (_isHigherChecked != value)
                {
                    _isHigherChecked = value;
                    OnPropertyChanged("IsHigherChecked");
                    ApplyCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ClassICommand AddCommand { get; private set; }
        public ClassICommand DeleteCommand { get; private set; }
        public ClassICommand ApplyCommand { get; private set; }
        public ClassICommand SaveCommand { get; private set; }
        public ClassICommand ClearCommand { get; private set; }
        public ClassICommand ComboBoxSelectionChangedCommand { get; private set; }

        private NetworkDisplayView _networkDisplayView;
        private NetworkDisplayViewModel _networkDisplayViewModel;

        public NetworkEntitiesViewModel(NetworkDisplayViewModel networkDisplayViewModel)
        {
            _networkDisplayViewModel = networkDisplayViewModel;

            Entities = new ObservableCollection<Entity>();
            
            FilterOptions = new List<string> { "All", "Solar_Panel", "Wind_Generator" };
            Types = new List<string> { "Solar_Panel", "Wind_Generator" };
            FilterValues = new ObservableCollection<Entity>(Entities);
            Filters = new Dictionary<string, Filter>();
            FilterNames = new ObservableCollection<string>();

            AddCommand = new ClassICommand(AddEntity, CanAdd);
            DeleteCommand = new ClassICommand(DeleteEntity, CanDelete);
            ApplyCommand = new ClassICommand(FilterEntity);
            ClearCommand = new ClassICommand(ResetEntity);
            SaveCommand = new ClassICommand(SaveEntity);
            ComboBoxSelectionChangedCommand = new ClassICommand(OnComboBoxSelectionChanged);

            SelectedTypeFilter = "All";

            AddInitialEntities();
        }

        public NetworkEntitiesViewModel()
        {
            Entities = new ObservableCollection<Entity>();

            FilterOptions = new List<string> { "All", "Solar_Panel", "Wind_Generator" };
            Types = new List<string> { "Solar_Panel", "Wind_Generator" };
            FilterValues = new ObservableCollection<Entity>(Entities);
            Filters = new Dictionary<string, Filter>();
            FilterNames = new ObservableCollection<string>();

            AddCommand = new ClassICommand(AddEntity, CanAdd);
            DeleteCommand = new ClassICommand(DeleteEntity, CanDelete);
            ApplyCommand = new ClassICommand(FilterEntity);
            ClearCommand = new ClassICommand(ResetEntity);
            SaveCommand = new ClassICommand(SaveEntity);
            ComboBoxSelectionChangedCommand = new ClassICommand(OnComboBoxSelectionChanged);

            SelectedTypeFilter = "All";

            AddInitialEntities();
        }

        private void AddInitialEntities()
        {
            Entity solarPanel1 = new Entity(1, "Solar_Panel 1", EntityTypes.Solar_Panel, 0);
            Entities.Add(solarPanel1);
            FilterValues.Add(solarPanel1);
            _networkDisplayViewModel.EntitiesInList.Add(solarPanel1);
            _idSolarPanel++;

            Entity windGen1 = new Entity(2, "Wind_Generator 1", EntityTypes.Wind_Generator, 0);
            Entities.Add(windGen1);
            FilterValues.Add(windGen1);
            _networkDisplayViewModel.EntitiesInList.Add(windGen1);
            _idWindGenerator++;

            Entity solarPanel2 = new Entity(3, "Solar_Panel 2", EntityTypes.Solar_Panel, 0);
            Entities.Add(solarPanel2);
            FilterValues.Add(solarPanel2);
            _networkDisplayViewModel.EntitiesInList.Add(solarPanel2);
            _idSolarPanel++;
        }

        #endregion

        private bool CanAdd()
        {
            return SelectedItemAdd != null;
        }

        private bool CanDelete()
        {
            return SelectedEntity != null;
        }

        private void DeleteEntity()
        {
            MessageBoxResult res = MessageBox.Show("Do you want to delete this Entity?", "Delete Entity", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                List<MyLine> line = new List<MyLine>();
                _networkDisplayViewModel.EntitiesInList.Remove(SelectedEntity);
                Entities.Remove(SelectedEntity);

                foreach (var  linija in _networkDisplayViewModel.LineCollection)
                {
                    if (linija.Destination == GetCanvasIndexForEntityId(SelectedEntity.Id) || linija.Source == GetCanvasIndexForEntityId(SelectedEntity.Id))
                    {
                        if (!line.Contains(linija))
                        {
                            line.Add(linija);
                        }
                    }
                }

                foreach (var lineDelete in line)
                {
                    _networkDisplayViewModel.LineCollection.Remove(lineDelete);
                }

                DeleteEntityFromCanvas(SelectedEntity);
                FilterValues.Remove(SelectedEntity);
            }
        }

        public int GetCanvasIndexForEntityId(int entityId)
        {
            for (int i = 0; i < _networkDisplayViewModel.CanvasCollection.Count; i++)
            {
                Entity entity = (_networkDisplayViewModel.CanvasCollection[i].Resources["data"]) as Entity;

                if ((entity != null) && (entity.Id == entityId))
                {
                    return i;
                }
            }

            return -1;
        }

        public void DeleteEntityFromCanvas(Entity entity)
        {
            int canvasIdx = GetCanvasIndexForEntityId(entity.Id);

            if(canvasIdx != -1)
            {
                _networkDisplayViewModel.CanvasCollection[canvasIdx].Resources.Remove("taken");
                _networkDisplayViewModel.CanvasCollection[canvasIdx].Resources.Remove("data");
                _networkDisplayViewModel.CanvasCollection[canvasIdx].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949BA4"));
                _networkDisplayViewModel.BorderBrushCollection[canvasIdx] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1F22"));
                _networkDisplayViewModel.DescriptionCollection[canvasIdx] = ($" ");
                MessageBox.Show("Entity successfully deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddEntity()
        {
            string type = "";

            try
            {
                type = SelectedItemAdd;
            }
            catch (Exception)
            {

            }

            if (type.Equals("Solar_Panel"))
            {
                string name = $"Solar_Panel {_idSolarPanel}";
                float value = 0;
                Entity newEntity = new Entity(_id, name, EntityTypes.Solar_Panel, value);
                Entities.Add(newEntity);

                if (TempFilter != null)
                {
                    if (TempFilter.FilterEntity(newEntity))
                    {
                        FilterValues.Add(newEntity);
                    }
                }
                else
                {
                    FilterValues.Add(newEntity);
                }
                //_networkDisplayViewModel.EntitiesInList.Add(newEntity);
                Count = Entities.Count;
                _idSolarPanel++;
                _id++;
            }
            else if (type.Equals("Wind_Generator"))
            {
                string name = $"Wind_Generator {_idWindGenerator}";
                float value = 0;
                Entity newEntity = new Entity(_id, name, EntityTypes.Wind_Generator, value);
                Entities.Add(newEntity);

                if (TempFilter != null)
                {
                    if (TempFilter.FilterEntity(newEntity))
                    {
                        FilterValues.Add(newEntity);
                    }
                }
                else
                {
                    FilterValues.Add(newEntity);
                }

                //_networkDisplayViewModel.EntitiesInList.Add(newEntity);
                Count = Entities.Count;
                _idWindGenerator++;
                _id++;
            }
        }

        private void FilterEntity()
        {
            // Validnost: ili ID + radio ili Type
            bool idFilled = !string.IsNullOrWhiteSpace(IdTextBox);
            bool radioChecked = IsEqualChecked || IsLowerChecked || IsHigherChecked;
            bool typeSelected = SelectedTypeFilter != "All";

            if (!((idFilled && radioChecked) || typeSelected))
            {
                MessageBox.Show("You need to select Radio Button and input Id or select Type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ako je ID unet, proveri da li je validan broj
            if (idFilled && !int.TryParse(IdTextBox, out _))
            {
                MessageBox.Show("Id must be a valid number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Filter filter = CollectFilterInfo();
            TempFilter = filter;

            FilterValues.Clear();
            foreach (Entity en in Entities)
            {
                if (filter.FilterEntity(en))
                {
                    FilterValues.Add(en);
                }
            }
        }


        private Filter CollectFilterInfo()
        {
            Filter filter = new Filter();

            // Validacija ID
            if (!string.IsNullOrWhiteSpace(IdTextBox))
            {
                if (int.TryParse(IdTextBox, out int id))
                {
                    filter.Id = id;

                    if (IsHigherChecked) filter.Operation = "Higher";
                    else if (IsLowerChecked) filter.Operation = "Lower";
                    else if (IsEqualChecked) filter.Operation = "Equal";
                    else
                    {
                        // Ako radio nije selektovan, ID filter ne sme biti primenjen
                        filter.Id = 0;
                        filter.Operation = string.Empty;
                    }
                }
                else
                {
                    // Ako unos nije validan broj
                    filter.Id = 0;
                    filter.Operation = string.Empty;
                }
            }

            // Filtriranje po tipu
            if (!string.IsNullOrEmpty(SelectedTypeFilter) && SelectedTypeFilter != "All")
            {
                if (Enum.TryParse<EntityTypes>(SelectedTypeFilter, out var type))
                {
                    filter.TypeEnum = type;
                    filter.Type = SelectedTypeFilter;
                }
            }
            else
            {
                filter.TypeEnum = EntityTypes.All;
                filter.Type = null;
            }

            return filter;
        }

        private void SaveEntity()
        {
            bool valid = false;
            bool isChecked = IsEqualChecked || IsHigherChecked || IsLowerChecked;
            bool idChecked = IdTextBox != null & IdTextBox != String.Empty;
            if (idChecked && isChecked)
            {
                valid = true;
            }

            if (SelectedTypeFilter != "All")
            {
                valid = true;
            }

            if (valid)
            {
                Filter filter = CollectFilterInfo();
                if (!(Filters.ContainsKey(filter.GetName())))
                {
                    Filters[filter.GetName()] = filter;
                    FilterNames.Add(filter.GetName());
                    SelectedFilterText = filter.GetName();
                }
                else
                {
                    MessageBox.Show("This filter already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("You need to select radio button and input id or select type!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetFilterForm()
        {
            IdTextBox = null;
            IsEqualChecked = false;
            IsHigherChecked = false;
            IsLowerChecked = false;
            SelectedFilterText = null;
            SelectedTypeFilter = "All";
            MessageBox.Show("Filters successfully cleared.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetEntity()
        {
            TempFilter = null;// Obrise trenutno aktivan filter
            ResetFilterForm();

            // Napuni FilterValues svim entitetima
            FilterValues.Clear();
            foreach (Entity en in Entities)
            {
                FilterValues.Add(en);
            }
        }

        private void OnComboBoxSelectionChanged()
        {
            if (SelectedFilterText != null)
            {
                Filter filter = Filters[SelectedFilterText];
                IdTextBox = filter.Id.ToString();
                IsHigherChecked = false;
                IsLowerChecked = false;
                IsEqualChecked = false;

                switch (filter.Operation)
                {
                    case "Higher":
                        IsHigherChecked = true;
                        break;
                    case "Equal":
                        IsEqualChecked = true; 
                        break;
                    case "Lower":
                        IsLowerChecked = true;
                        break;
                }

                if (filter.Type == EntityTypes.Solar_Panel.ToString())
                {
                    SelectedTypeFilter = "Solar_Panel";
                }
                else if (filter.Type == EntityTypes.Wind_Generator.ToString())
                {
                    SelectedTypeFilter = "Wind_Generator";
                }
                else
                {
                    SelectedTypeFilter = "All";
                }
            }
        }
    }
}
