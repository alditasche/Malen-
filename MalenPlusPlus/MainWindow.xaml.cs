// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaktionslogik für MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MalenPlusPlus
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using Microsoft.Win32;

    using Color = System.Windows.Media.Color;
    using Point = System.Windows.Point;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int strokewidth = 10;

        private Point lastpPoint;

        private List<Line> momentLine = new List<Line>();
        private List<int> undo = new List<int>();

        private int undoIndex = 0;
        private bool check = true;
        private List<Polyline> linien = new List<Polyline>();

        public MainWindow()
        {
            InitializeComponent();
            this.ComboBox.Items.Add("1");
            this.ComboBox.Items.Add("5");
            this.ComboBox.Items.Add("10");
            this.ComboBox.Items.Add("15");
            this.ComboBox.Items.Add("20");
            this.ComboBox.Items.Add("25");
            this.ComboBox.Items.Add("30");
            this.ComboBox.Items.Add("50");
            this.ComboBox.SelectedIndex = 2;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.FarbeVorschau != null)
            {
                this.FarbeVorschau.Fill = new SolidColorBrush(
                    Color.FromArgb(
                        (byte)this.Alpha.Value,
                        (byte)this.Rot.Value,
                        (byte)this.Grün.Value,
                        (byte)this.Blau.Value));
            }
        }

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.strokewidth = int.Parse(sender.GetType().Name);
        }
        private void Canvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this.Canvas);
            this.lastpPoint = p;

            if (this.check)
            {
                this.undo.Add(new int());
                this.check = false;
                this.linien.Add(new Polyline());
            }

        }

        private void Canvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (Line line in this.momentLine)
            {
                this.Canvas.Children.Remove(line);
            }

            this.momentLine.Clear();
            this.Canvas.Children.Add(this.linien.Last());
            this.undo[this.undoIndex]++;
            this.undoIndex++;
            this.check = true;
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(this.Canvas);
                if ((Math.Pow(this.lastpPoint.X - p.X, 2) + Math.Pow(this.lastpPoint.Y - p.Y, 2) > 5))
                {
                    Line line = new Line();
                    line.X1 = this.lastpPoint.X;
                    line.X2 = p.X;
                    line.Y1 = this.lastpPoint.Y;
                    line.Y2 = p.Y;
                    line.Stroke = new SolidColorBrush(Color.FromArgb((byte)this.Alpha.Value, (byte)this.Rot.Value, (byte)this.Grün.Value, (byte)this.Blau.Value));
                    line.StrokeThickness = this.strokewidth;
                    line.StrokeStartLineCap = PenLineCap.Round;
                    line.StrokeEndLineCap = PenLineCap.Round;
                    this.Canvas.Children.Add(line);
                    this.momentLine.Add(line);

                    this.linien.Last().Points.Add(new Point(this.lastpPoint.X, this.lastpPoint.Y));
                    this.linien.Last().Stroke = new SolidColorBrush(Color.FromArgb((byte)this.Alpha.Value, (byte)this.Rot.Value, (byte)this.Grün.Value, (byte)this.Blau.Value));
                    this.linien.Last().StrokeStartLineCap = PenLineCap.Round;
                    this.linien.Last().StrokeEndLineCap = PenLineCap.Round;
                    this.linien.Last().StrokeThickness = this.strokewidth;
                }

                this.lastpPoint = p;

            }
        }


        /// <summary>
        /// The save button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "picture (*.png)|*.png|jpg (*.jpg)|*.jpg";
            Files bild = new Files();
            bildDataDataContext context = new bildDataDataContext();
            if (saveFileDialog.ShowDialog() == true)
            {
                Rect bounds = VisualTreeHelper.GetDescendantBounds(this.Canvas);
                double dpi = 96d;


                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)bounds.Width,
                    (int)bounds.Height,
                    dpi,
                    dpi,
                    PixelFormats.Default);


                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(this.Canvas);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }

                rtb.Render(dv);
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                try
                {
                    MemoryStream ms = new System.IO.MemoryStream();

                    pngEncoder.Save(ms);
                    ms.Close();

                    File.WriteAllBytes(saveFileDialog.FileName, ms.ToArray());
                    Binary bildData = ms.ToArray();
                    bild.Image = bildData;
                    bild.Id = context.Files.ToArray().Last().Id + 1;
                    context.Files.InsertOnSubmit(bild);
                    context.SubmitChanges();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }





        }

        /// <summary>
        /// The clearr button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ClearrButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Canvas.Children.Clear();
            this.undoIndex = 0;
            this.undo.Clear();
        }

        /// <summary>
        /// The undo button_ onclick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UndoButton_Onclick(object sender, RoutedEventArgs e)
        {
            if (this.undo.Count > 0)
            {
                this.Canvas.Children.RemoveRange(
                    this.Canvas.Children.Count - this.undo[this.undoIndex - 1],
                    this.undo[this.undoIndex - 1]);
                if (this.undoIndex > 0)
                {
                    this.undo.RemoveAt(this.undoIndex - 1);
                    this.undoIndex--;
                }
            }
        }

        /// <summary>
        /// The button base_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            bildDataDataContext context = new bildDataDataContext();

            Console.WriteLine("vorher");

            var query = from b in context.Files select b;

            foreach (var v in query)
            {
                Console.WriteLine(v);
            }
            Console.WriteLine("nachher");



        }

        /// <summary>
        /// The load button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            ImageBrush brush = new ImageBrush();

            bildDataDataContext context = new bildDataDataContext();
            MemoryStream ms = new MemoryStream(context.Files.ToArray().Last().Image.ToArray());

            BitmapImage gemalde = new BitmapImage();

            ms.Position = 0;
            gemalde.BeginInit();
            gemalde.StreamSource = ms;
            gemalde.CacheOption = BitmapCacheOption.OnLoad;
            gemalde.EndInit();
            brush.ImageSource = gemalde;
            this.Canvas.Background = brush;

        }

        /// <summary>
        /// The clear all button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ClearAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Canvas.Children.Clear();
            this.undoIndex = 0;
            this.undo.Clear();
            this.Canvas.Background = new BitmapCacheBrush();
        }
    }
}
