namespace _2BPRG.ViewModels
{
    using _2BPRG.Commands;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    internal class MainViewModel : BaseViewModel
    {
        internal string[] propertiesThatRaisNeedToUpdateModel = new string[] {
            nameof(NumberOfElements),
            nameof(rectangleHeightMaxValue),
            nameof(rectangleHeightMinValue),
            nameof(rectanglesCountMinValue),
            nameof(rectanglesCountMaxValue),
            nameof(rectangleWidthMaxValue),
            nameof(rectangleWidthMinValue) };

        private Grid currentGrid;

        private Brush fillButtonBackGroundBrush = Brushes.Gray;

        private Command fillCommand;

        private bool flag = false;

        private int numberOfElements = 100;

        private int rectangleHeightMaxValue = 500;

        private int rectangleHeightMinValue = 50;

        private int rectanglesCountMaxValue = 1000;

        private int rectanglesCountMinValue = 100;

        private List<Rectangle> rectanglesToPutOnTheSheets = new List<Rectangle>();

        private int rectangleWidthMaxValue = 250;

        private int rectangleWidthMinValue = 50;

        private ObservableCollection<FrameworkElement> sheets = new ObservableCollection<FrameworkElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            PropertyChanged += NeedToUpdate_PropertyChanged;
        }

        public Grid CurrentGrid { get => currentGrid; set => SetProperty(ref currentGrid, value); }

        public Brush FillButtonBackGroundBrush { get => fillButtonBackGroundBrush; set => SetProperty(ref fillButtonBackGroundBrush, value); }

        public Command FillCommand
        {
            get => fillCommand ?? (fillCommand = new Command((paramter) =>
            {
                if (flag) return;
                flag = true;
                ResetSheets();
                FillButtonBackGroundBrush = Brushes.Gray; DistributeRectangles();
                flag = false;
            })); set => SetProperty(ref fillCommand, value);
        }

        public int NumberOfElements { get => numberOfElements; set => SetProperty(ref numberOfElements, value); }

        public int RectangleHeightMaxValue { get => rectangleHeightMaxValue; set => SetProperty(ref rectangleHeightMaxValue, value); }

        public int RectangleHeightMinValue { get => rectangleHeightMinValue; set => SetProperty(ref rectangleHeightMinValue, value); }

        public int RectanglesCountMaxValue { get => rectanglesCountMaxValue; set => SetProperty(ref rectanglesCountMaxValue, value); }

        public int RectanglesCountMinValue { get => rectanglesCountMinValue; set => SetProperty(ref rectanglesCountMinValue, value); }

        public List<Rectangle> RectanglesToPutOnTheSheets { get => rectanglesToPutOnTheSheets; set => SetProperty(ref rectanglesToPutOnTheSheets, value); }

        public int RectangleWidthMaxValue { get => rectangleWidthMaxValue; set => SetProperty(ref rectangleWidthMaxValue, value); }

        public int RectangleWidthMinValue { get => rectangleWidthMinValue; set => SetProperty(ref rectangleWidthMinValue, value); }

        public ObservableCollection<FrameworkElement> Sheets { get => sheets; set => SetProperty(ref sheets, value); }

        private List<StackPanel> stackPanels;

        private double sumOfHeight = 0;

        private double sumOfWidth = 0;

        private double currentColumnWidht = 0;

        internal void DistributeRectangles()
        {




            Sheets.Add(AddNextSheet());

            //Fill list with radnoms
            RectanglesToPutOnTheSheets = GetRandomRectangles(NumberOfElements);

            //rotate rectangles if its width is smaller than its height
            for (int i = 0; i < RectanglesToPutOnTheSheets.Count; i++) RotateRectangleToMaximumWidth(RectanglesToPutOnTheSheets[i]);

            //Sorting
            RectanglesToPutOnTheSheets.Sort(CompareRectangles);



            bool firstIteration = true;
            stackPanels = new List<StackPanel>() { new StackPanel() };
            sumOfHeight = 0;
            sumOfWidth = 0;

            //adding to stackpanels
            for (int i = RectanglesToPutOnTheSheets.Count - 1; i >= 0; i--)
            {
                Rectangle item = RectanglesToPutOnTheSheets[i];
                if (firstIteration)
                {
                    firstIteration = false;
                    UpdateWidth(item);
                }



                if (item.Height + sumOfHeight > 900)
                {
                    i = CheckIfOthersFit(i);

                    UpdateWidth(item);
                    if (sumOfWidth > 3000)
                    {
                        AddingStackPanrelsToGrid(stackPanels);
                        stackPanels.Clear();
                        Sheets.Add(AddNextSheet());
                        stackPanels.Add(new StackPanel());
                        sumOfWidth = 0;
                        UpdateWidth(item);
                    }


                    stackPanels.Add(new StackPanel());
                    sumOfHeight = 0;
                }
                i -= InsertRectangleOnSheet(item);
            }
            AddingStackPanrelsToGrid(stackPanels);
        }

        private void UpdateWidth(Rectangle item)
        {
            sumOfWidth += item.Width;
            currentColumnWidht = item.Width;
        }

        private int CheckIfOthersFit(int i)
        {
            double placeLeft = 900 - sumOfHeight;
            int differenceInCounter = InsertFirstFit(placeLeft);
            i -= differenceInCounter;

            while (differenceInCounter > 0)
            {
                placeLeft = 900 - sumOfHeight;
                differenceInCounter = InsertFirstFit(placeLeft);
                i -= differenceInCounter;
            }

            return i;
        }

        private int InsertRectangleOnSheet(Rectangle item)
        {
            int differenceInCounter = 0;

            RectanglesToPutOnTheSheets.Remove(item);



            StackPanel stackpanel = new StackPanel() { Orientation = Orientation.Horizontal };
            stackpanel.Children.Add(item);
            var szerokosc = item.Width;
            var wysokosc = item.Height;
            var pozostaleMiejsca = currentColumnWidht - szerokosc;
            var rect = InsertAdditional(wysokosc, pozostaleMiejsca);

            if (rect != null)
            {
                RectanglesToPutOnTheSheets.Remove(rect);
                stackpanel.Children.Add(rect);
                differenceInCounter = 1;
            }

            stackPanels.Last().Children.Add(stackpanel);
            sumOfHeight += item.Height;

            return differenceInCounter;
        }

        private Rectangle InsertAdditional(double maxHeight, double maxWidth)
        {
            for (int i = RectanglesToPutOnTheSheets.Count - 1; i >= 0; i--)
            {
                Rectangle item = RectanglesToPutOnTheSheets[i];
                if (item.Width<=maxWidth && item.Height <=maxHeight)
                {
                    return item;
                }
                RotateRectangle(item);
                if (item.Width <= maxWidth && item.Height <= maxHeight)
                {
                    return item;
                }
                RotateRectangle(item);
            }
            return null;
        }

        private int InsertFirstFit(double placeLeft)
        {
            int additional = 0;
            bool elementFound = false;

            for (int i = RectanglesToPutOnTheSheets.Count - 1; i >= 0; i--)
            {
                Rectangle item = RectanglesToPutOnTheSheets[i];
                if (item.Height <= placeLeft)
                {
                    additional = InsertRectangleOnSheet(item);
                    elementFound = true;
                    break;
                }
            }
            return (elementFound ? 1 : 0)+ additional;
        }

        private static void RotateRectangleToMaximumWidth(Rectangle item)
        {
            if (item.Width < item.Height)
            {
                RotateRectangle(item);
            }
        }

        private static void RotateRectangle(Rectangle item)
        {
            var height = item.Height;
            var width = item.Width;
            item.Width = height;
            item.Height = width;
        }

        private void AddingStackPanrelsToGrid(List<StackPanel> StackPanels)
        {
            if (CurrentGrid == null) return;

            for (int i = 0; i < StackPanels.Count; i++)
            {
                CurrentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(i, GridUnitType.Auto) });
                StackPanels[i].SetValue(Grid.ColumnProperty, i);
                CurrentGrid.Children.Add(StackPanels[i]);
            }
        }

        private FrameworkElement AddNextSheet()
        {
            Grid grid = new Grid() { Height = 900, Width = 3000 };
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.Margin = new Thickness(50);
            border.Child = grid;
            CurrentGrid = grid;
            return border;
        }

        private int CompareRectangles(Rectangle a, Rectangle b)
        {
            if (a == null)
            {
                if (b == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            if (b == null)
            {
                return 1;
            }

            if (a.Width > b.Width) return 1;
            else if (a.Width < b.Width) return -1;

            return a.Height.CompareTo(b.Height);
        }

        private List<Rectangle> GetRandomRectangles(int numberOfRectangles = 500)
        {
            List<Rectangle> listRectangle = new List<Rectangle>();
            //Create random rectangles
            Random random = new Random();
            for (int i = 0; i < numberOfRectangles; i++)
            {
                Rectangle rectangle = new Rectangle() { HorizontalAlignment = HorizontalAlignment.Left };
                rectangle.Width = random.Next(RectangleWidthMinValue, RectangleWidthMaxValue);
                rectangle.Height = random.Next(RectangleHeightMinValue, RectangleHeightMaxValue);
                rectangle.Fill = PickRandomBrush(random);
                rectangle.Stroke = Brushes.Black;
                listRectangle.Add(rectangle);
            }
            return listRectangle;
        }

        private void NeedToUpdate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (propertiesThatRaisNeedToUpdateModel.Contains(e.PropertyName))
            {
                FillButtonBackGroundBrush = Brushes.Red;
            }
        }

        private Brush PickRandomBrush(Random rnd)
        {
            Brush result = Brushes.Transparent;
            Type brushesType = typeof(Brushes);
            PropertyInfo[] properties = brushesType.GetProperties();
            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);
            return result;
        }

        private void ResetSheets()
        {
            Sheets.Clear();
            RectanglesToPutOnTheSheets.Clear();
        }
    }
}
