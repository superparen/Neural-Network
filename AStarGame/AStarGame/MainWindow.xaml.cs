using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AStarGame.ThreeModel;

namespace AStarGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        int wayCounter = 0;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (mainWay == null)
                return;

            character.SetValue(Canvas.TopProperty, mainWay[wayCounter].Y);
            character.SetValue(Canvas.LeftProperty, mainWay[wayCounter].X);
            wayCounter++;

            if (wayCounter == mainWay.Count)
            {
                mainWay = null;
                wayCounter = 0;
            }
        }

        Rectangle character;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            character = new Rectangle();
            character.SetValue(Canvas.TopProperty, 0d);
            character.SetValue(Canvas.LeftProperty, 0d);
            character.Fill = Brushes.DeepSkyBlue;
            character.Stroke = Brushes.Gray;
            character.Width = gridWidth;
            character.Height = gridHeight;

            canvas.Children.Add(character);
        }

        double gridWidth = 20,
            gridHeight = 20;

        Vertex<RectangleCoor>[][] vertexes;
        private Three<RectangleCoor> SetThree()
        {
            int xCount = (int)(canvas.ActualWidth / gridWidth) + 1;
            int yCount = (int)(canvas.ActualHeight / gridHeight) + 1;

            vertexes = new Vertex<RectangleCoor>[yCount][];
            for (int i = 0; i < yCount; i++)
            {
                vertexes[i] = new Vertex<RectangleCoor>[xCount];
                for (int j = 0; j < xCount; j++)
                {
                    vertexes[i][j] = new  Vertex<RectangleCoor>(new RectangleCoor(j * gridWidth, i * gridHeight, gridWidth, gridHeight));
                }
            }

            var movable = GridCheck();
            for (int i = 0; i < yCount; i++)
            {
                for (int j = 0; j < xCount; j++)
                {
                    if (i - 1 >= 0 && !movable[i - 1][j])
                        vertexes[i][j].Edges.Add(new Edge<RectangleCoor>(vertexes[i - 1][j]));
                    if(i + 1 < yCount && !movable[i + 1][j])
                        vertexes[i][j].Edges.Add(new Edge<RectangleCoor>(vertexes[i + 1][j]));
                    if (j - 1 >= 0 && !movable[i][j - 1])
                        vertexes[i][j].Edges.Add(new Edge<RectangleCoor>(vertexes[i][j - 1]));
                    if (j + 1 < xCount && !movable[i][j + 1])
                        vertexes[i][j].Edges.Add(new Edge<RectangleCoor>(vertexes[i][j + 1]));
                }
            }

            return new Three<RectangleCoor>(vertexes[0][0]);
        }

        private bool[][] GridCheck()
        {
            int xCount = (int)(canvas.ActualWidth / gridWidth) + 1;
            int yCount = (int)(canvas.ActualHeight / gridHeight) + 1;

            double x = 0, y = 0;
            bool[][] res = new bool[yCount][];
            for(int i = 0; i < yCount; i++)
            {
                res[i] = new bool[xCount];
                for(int j = 0; j < xCount; j++)
                {
                    for(int c = 1; c < canvas.Children.Count; c++)
                    {
                        double bleft = Canvas.GetLeft(canvas.Children[c]),
                            btop = Canvas.GetTop(canvas.Children[c]),
                            bwidth = ((Rectangle)canvas.Children[c]).Width,
                            bheight = ((Rectangle)canvas.Children[c]).Height;
                        if (GetRectanglesIntersectionSquare(x, x + gridWidth, y, y + gridHeight, 
                            bleft, bleft + bwidth, btop, btop + bheight) > 0)
                        {
                            res[i][j] = true;
                            break;
                        }
                    }
                    x += gridWidth;
                }
                y += gridHeight;
                x = 0;
            }

            return res;
        }

        private double GetRectanglesIntersectionSquare(double aleft, double aright, double atop, double abottom,
            double bleft, double bright, double btop, double bbottom)
        {
            double xIntersection = GetSegmentsIntersectionLength(aleft, aright, bleft, bright);
            double yIntersection = GetSegmentsIntersectionLength(atop, abottom, btop, bbottom);

            return xIntersection * yIntersection;
        }

        private double GetSegmentsIntersectionLength(double aLeft, double aRight, double bLeft, double bRight)
        {
            double left = Math.Max(aLeft, bLeft);
            double right = Math.Min(aRight, bRight);

            return Math.Max(right - left, 0);
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            while(canvas.Children.Count > 1)
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
        }

        bool isDrawing;
        Point point;
        Rectangle rectangle;
        List<RectangleCoor> mainWay;
        private void MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            point = e.GetPosition(canvas);

            if ((bool)BuildRadioButton.IsChecked)
            {
                rectangle = new Rectangle();
                rectangle.Fill = Brushes.LightPink;
                rectangle.SetValue(Canvas.LeftProperty, point.X);
                rectangle.SetValue(Canvas.TopProperty, point.Y);
                canvas.Children.Add(rectangle);

                isDrawing = true;
            }
            else if ((bool)GameRadioButton.IsChecked && mainWay == null)
            {
                var three = SetThree();
                Vertex<RectangleCoor> from = vertexes[((int)(double)character.GetValue(Canvas.TopProperty) + 1) / (int)gridHeight][((int)(double)character.GetValue(Canvas.LeftProperty) + 1) / (int)gridWidth],
                    to = vertexes[(int)(point.Y / gridHeight)][(int)(point.X / gridWidth)];
                var way = three.AStar(from, to, (r1, r2) => Math.Abs(r1.X - r2.X) + Math.Abs(r1.Y - r2.Y));

                if (way == null)
                    MessageBox.Show("No way");
                else
                {
                    mainWay = new List<RectangleCoor>();
                    Vertex<RectangleCoor> tmp = to;
                    mainWay.Add(tmp.Source);
                    while(mainWay.Last() != from.Source)
                    {
                        var vert = way.Where(v => v.Key.Edges.Any(ed => ed.Element == tmp)).OrderBy(v => v.Value).First().Key;
                        tmp = vert;
                        mainWay.Add(vert.Source);
                    }

                    mainWay.Reverse();
                }
            }
        }

        private void MouseUpEvent(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }

        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if ((bool)BuildRadioButton.IsChecked)
            {
                if (!isDrawing)
                    return;

                Point newPoint = e.GetPosition(canvas);
                if (newPoint.X < point.X)
                    rectangle.SetValue(Canvas.LeftProperty, newPoint.X);
                if (newPoint.Y < point.Y)
                    rectangle.SetValue(Canvas.TopProperty, newPoint.Y);
                rectangle.Width = Math.Abs(point.X - newPoint.X);
                rectangle.Height = Math.Abs(point.Y - newPoint.Y);
            }
        }
    }
}
