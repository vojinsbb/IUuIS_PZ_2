using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : ClassINotifyPropertyChanged
    {
        #region Initialization

        public BindingList<Entity> EntitiesInList { get; set; }
        public ObservableCollection<Brush> BorderBrushCollection { get; set; }
        public ObservableCollection<Canvas> CanvasCollection { get; set; }
        public ObservableCollection<MyLine> LineCollection { get; set; }
        public ObservableCollection<string> DescriptionCollection { get; set; }

        private Entity selectedEntity;
        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                OnPropertyChanged("SelectedEntity");
            }
        }

        private Entity draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;

        public ClassICommand<object> DropEntityOnCanvas { get; set; }
        public ClassICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
        public ClassICommand MouseLeftButtonUp { get; set; }
        public ClassICommand<object> SelectionChanged { get; set; }
        public ClassICommand<object> FreeCanvas { get; set; }
        public ClassICommand<object> RightMouseButtonDownOnCanvas { get; set; }
        public ClassICommand OrganizeAllCommand { get; set; }

        private bool isLineSourceSelected = false;
        private int sourceCanvasIndex = -1;
        private int destinationCanvasIndex = -1;
        private MyLine currentLine = new MyLine();
        private Point linePoint1 = new Point();
        private Point linePoint2 = new Point();

        public NetworkDisplayViewModel()
        {
            EntitiesInList = new BindingList<Entity>();
            LineCollection = new ObservableCollection<MyLine>();
            CanvasCollection = new ObservableCollection<Canvas>();
            BorderBrushCollection = new ObservableCollection<Brush>();
            DescriptionCollection = new ObservableCollection<string>();

            for (int i=0; i<12; i++)
            {
                BorderBrushCollection.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1F22")));

                CanvasCollection.Add(new Canvas()
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949BA4")),
                    AllowDrop = true
                });

                DescriptionCollection.Add(" ");
            }

            DropEntityOnCanvas = new ClassICommand<object>(OnDrop);
            LeftMouseButtonDownOnCanvas = new ClassICommand<object>(OnLeftMouseButtonDown);
            MouseLeftButtonUp = new ClassICommand(OnMouseLeftButtonUp);
            SelectionChanged = new ClassICommand<object>(OnSelectionChanged);
            FreeCanvas = new ClassICommand<object>(OnFreeCanvas);
            RightMouseButtonDownOnCanvas = new ClassICommand<object>(OnRightMouseButtonDown);
            OrganizeAllCommand = new ClassICommand(OnOrganize);
        }
        #endregion

        #region GetCanvasIndexForEntity

        public int GetCanvasIndexForEntityId(int entityId)
        {
            for (int i = 0; i < CanvasCollection.Count; i++)
            {
                Entity entity = (CanvasCollection[i].Resources["data"]) as Entity;

                if ((entity != null) && (entity.Id == entityId))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Organization

        private void OnOrganize()
        {
            List<Entity> addedEntities = new List<Entity>();
            try
            {
                int idx = 0;
                foreach (var item in EntitiesInList)
                {
                    while (idx < CanvasCollection.Count)
                    {
                        if (CanvasCollection[idx].Resources != null && CanvasCollection[idx].Resources["taken"] == null)
                        {
                            BitmapImage logo = new BitmapImage();
                            logo.BeginInit();
                            logo.UriSource = new Uri(item.Type.ImageSource, UriKind.RelativeOrAbsolute);
                            logo.EndInit();

                            CanvasCollection[idx].Background = new ImageBrush(logo);
                            CanvasCollection[idx].Resources.Add("taken", true);
                            CanvasCollection[idx].Resources.Add("data", item);
                            BorderBrushCollection[idx] = (item.isValueValid()) ? Brushes.Green : Brushes.Red;
                            DescriptionCollection[idx] = ($"ID: {item.Id} Value: {item.Value}");

                            addedEntities.Add(item);

                            break;
                        }
                        idx++;
                    }
                    idx = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            
            foreach (var entity in addedEntities)
            {
                EntitiesInList.Remove(entity);
            }
        }

        #endregion

        #region OnDrop

        private  void OnDrop(object parameter)
        {
            if (draggedItem != null)
            {
                int idx = Convert.ToInt32(parameter);

                if (CanvasCollection[idx].Resources["taken"] == null)
                {
                    BitmapImage logo  = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri(draggedItem.Type.ImageSource, UriKind.RelativeOrAbsolute);
                    logo.EndInit();

                    CanvasCollection[idx].Background = new ImageBrush(logo);
                    CanvasCollection[idx].Resources.Add("taken", true);
                    CanvasCollection[idx].Resources.Add("data", draggedItem);
                    BorderBrushCollection[idx] = (draggedItem.isValueValid()) ? Brushes.Green : Brushes.Red;
                    DescriptionCollection[idx] = ($"ID: {draggedItem.Id} Value: {draggedItem.Value}");

                    //Prevlacenje iz drugog canvasa
                    if (draggingSourceIndex != -1)
                    {
                        CanvasCollection[draggingSourceIndex].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949BA4"));

                        CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
                        CanvasCollection[draggingSourceIndex].Resources.Remove("data");
                        BorderBrushCollection[draggingSourceIndex] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1F22"));
                        DescriptionCollection[draggingSourceIndex] = (" ");

                        UpdateLinesForCanvas(draggingSourceIndex, idx);

                        //Crtanje linije se prekida ako je izmedju postavljanja tacaka entitet pomeren na drugi canvas
                        if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            currentLine = new MyLine();
                        }

                        draggingSourceIndex = -1;
                    }

                    //Prevlacenje iz liste
                    if (EntitiesInList.Contains(draggedItem))
                    {
                        EntitiesInList.Remove(draggedItem);
                    }
                }
            }
        }

        #endregion

        #region UpdateEntityOnCanvas

        public void UpdateEntityOnCanvas(Entity entity)
        {
            int canvasIdx = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIdx != -1)
            {
                DescriptionCollection[canvasIdx] = ($"ID: {entity.Id} Value: {entity.Value}");
                if (entity.isValueValid())
                {
                    BorderBrushCollection[canvasIdx] = Brushes.Green;
                }
                else
                {
                    BorderBrushCollection[canvasIdx] = Brushes.Red;
                }
            }
        }

        #endregion

        #region Delete

        //ovo je vezano za MainWindowViewModel
        public void DeleteEntityFromCanvas(Entity entity)
        {
            int canvasIdx = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIdx != -1)
            {
                CanvasCollection[canvasIdx].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949BA4"));
                CanvasCollection[canvasIdx].Resources.Remove("taken");
                CanvasCollection[canvasIdx].Resources.Remove("data");
                BorderBrushCollection[canvasIdx] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1F22"));
                DescriptionCollection[canvasIdx] = ($" ");

                DeleteLinesForCanvas(canvasIdx);
            }
        }

        private void DeleteLinesForCanvas(int canvasIdx)
        {
            List<MyLine> linesToDelete = new List<MyLine>();

            for(int i = 0; i < LineCollection.Count; i++)
            {
                if ((LineCollection[i].Source == canvasIdx) || (LineCollection[i].Destination == canvasIdx))
                {
                    linesToDelete.Add(LineCollection[i]);
                }
            }

            foreach (MyLine line in linesToDelete)
            {
                LineCollection.Remove(line);
            }
        }

        private void OnFreeCanvas(object parameter)
        {
            int idx = Convert.ToInt32(parameter);

            if (CanvasCollection[idx].Resources["taken"] != null)
            {
                //Crtanje linije se prekida ako je izmedju postavljanja tacaka entitet uklonjen sa canvasa
                if (sourceCanvasIndex != -1)
                {
                    isLineSourceSelected = false;
                    sourceCanvasIndex = -1;
                    linePoint1 = new Point();
                    linePoint2 = new Point();
                    currentLine = new MyLine();
                }

                DeleteLinesForCanvas(idx);

                EntitiesInList.Add((Entity)CanvasCollection[idx].Resources["data"]);
                CanvasCollection[idx].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949BA4"));
                CanvasCollection[idx].Resources.Remove("taken");
                CanvasCollection[idx].Resources.Remove("data");
                BorderBrushCollection[idx] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1F22"));
                DescriptionCollection[idx] = ($" ");
            }
        }

        #endregion

        #region SelectionChanged

        private void OnSelectionChanged(object parameter)
        {
            if (!dragging)
            {
                dragging = true;
                draggedItem = SelectedEntity;
                DragDrop.DoDragDrop((ListView)parameter, draggedItem, DragDropEffects.Move);
            }
        }

        #endregion

        #region LeftMouseButton

        private void OnLeftMouseButtonDown(object parameter)
        {
            if (!dragging)
            {
                int idx = Convert.ToInt32(parameter);

                if (CanvasCollection[idx].Resources["taken"] != null)
                {
                    dragging = true;
                    draggedItem = (Entity)CanvasCollection[idx].Resources["data"];
                    draggingSourceIndex = idx;
                    DragDrop.DoDragDrop(CanvasCollection[idx], draggedItem, DragDropEffects.Move);
                }
            }
        }

        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            SelectedEntity = null;
            dragging = false;
            draggingSourceIndex = -1;
        }

        #endregion

        #region RightMouseButton

        private void OnRightMouseButtonDown(object parameter)
        {
            int idx = Convert.ToInt32(parameter);

            if (CanvasCollection[idx].Resources["taken"] != null)
            {
                if (!isLineSourceSelected)
                {
                    sourceCanvasIndex = idx;
                    linePoint1 = GetPointForCanvasIndex(sourceCanvasIndex);

                    currentLine.X1 = linePoint1.X;
                    currentLine.Y1 = linePoint1.Y;
                    currentLine.Source = sourceCanvasIndex;

                    isLineSourceSelected = true;
                }
                else
                {
                    destinationCanvasIndex = idx;

                    if ((sourceCanvasIndex != destinationCanvasIndex) && !DoesLineAlreadyExist(sourceCanvasIndex, destinationCanvasIndex))
                    {
                        linePoint2 = GetPointForCanvasIndex(destinationCanvasIndex);

                        currentLine.X2 = linePoint2.X;
                        currentLine.Y2 = linePoint2.Y;
                        currentLine.Destination = destinationCanvasIndex;

                        LineCollection.Add(new MyLine
                        {
                            X1 = currentLine.X1,
                            Y1 = currentLine.Y1,
                            X2 = currentLine.X2,
                            Y2 = currentLine.Y2,
                            Source = currentLine.Source,
                            Destination = currentLine.Destination
                        });

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new MyLine();
                    }
                    else
                    {
                        //Pocetak i kraj linije  su u istom canvasu
                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new MyLine();
                    }
                }
            }
            else
            {
                //Canvas na koji se postavlja tacka nije zauzet
                isLineSourceSelected = false;

                linePoint1 = new Point();
                linePoint2 = new Point();
                currentLine = new MyLine();
            }
        }

        #endregion

        #region LineHelpers

        private void UpdateLinesForCanvas(int sourceCanvas, int destinationCanvas)
        {
            for (int i = 0; i < LineCollection.Count; i++)
            {
                if (LineCollection[i].Source == sourceCanvas)
                {
                    Point newSourcePoint = GetPointForCanvasIndex(destinationCanvas);
                    LineCollection[i].X1 = newSourcePoint.X;
                    LineCollection[i].Y1 = newSourcePoint.Y;
                    LineCollection[i].Source = destinationCanvas;
                }
                else if (LineCollection[i].Destination == sourceCanvas)
                {
                    Point newDestinationPoint = GetPointForCanvasIndex(destinationCanvas);
                    LineCollection[i].X2 = newDestinationPoint.X;
                    LineCollection[i].Y2 = newDestinationPoint.Y;
                    LineCollection[i].Destination = destinationCanvas;
                }
            }
        }

        private bool DoesLineAlreadyExist(int sourceCanvas, int destinationCanvas)
        {
            foreach (MyLine line in LineCollection)
            {
                if ((line.Source == sourceCanvas) && (line.Destination == destinationCanvas))
                {
                    return true;
                }
                
                if ((line.Source == destinationCanvas) && (line.Destination == sourceCanvas))
                {
                    return true;
                }
            }

            return false;
        }

        //Centralna tacka na Canvas kontroli
        private Point GetPointForCanvasIndex(int canvasIdx)
        {
            double x = 0;
            double y = 0;

            for (int row = 0; row <= 3; row++)
            {
                for (int col = 0; col <= 2; col++)
                {
                    int currentIdx = row * 3 + col;

                    if (canvasIdx == currentIdx)
                    {
                        x = 50 + (col * 135);
                        y = 50 + (row * 135);

                        break;
                    }
                }
            }

            return new Point(x, y);
        }

        #endregion
    }
}
