namespace _2BPRG.ViewModels.Samples
{
    using System;

    internal class MainViewModelSample : MainViewModel
    {

        public MainViewModelSample()
        {
            //Intizialization of the list

            RectanglesToPutOnTheSheets = new System.Collections.Generic.List<System.Windows.Shapes.Rectangle>();


            //Filling the list with rectangles

            //Random random = new Random();
            //var size = random.Next(RectanglesCountMinValue, RectanglesCountMaxValue);
            //for (int i = 0; i < size; i++)
            //{
            //    RectanglesToPutOnTheSheets.Add(new System.Windows.Shapes.Rectangle() { Width = random.Next(RectangleWidthMinValue, RectangleWidthMaxValue), Height = random.Next(RectangleHeightMinValue, RectangleHeightMaxValue) });
            //}

            DistributeRectangles();
        }


    }
}
