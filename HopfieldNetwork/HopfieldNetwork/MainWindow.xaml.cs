using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using HopfieldNetwork.Models;

namespace HopfieldNetwork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Rectangle[,] Rectangles;
        private void LoadRectangles()
        {
            int xCount = 10, yCount = 10;
            Rectangles = new Rectangle[yCount, xCount];

            for (int i = 0; i < xCount; i++)
                PaintGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < yCount; i++)
                PaintGrid.RowDefinitions.Add(new RowDefinition());

            for(int i = 0; i < yCount; i++)
            {
                for(int j = 0; j < xCount; j++)
                {
                    Rectangles[i, j] = new Rectangle() { Fill = Brushes.LightPink };
                    Grid.SetColumn(Rectangles[i, j], j);
                    Grid.SetRow(Rectangles[i, j], i);
                    PaintGrid.Children.Add(Rectangles[i, j]);
                }
            }
        }

        HopfieldNetwork.Models.HopfieldNetwork network;
        private void LoadNetwork()
        {
            network = new Models.HopfieldNetwork();
            network.AddSet(JsonConvert.DeserializeObject<TrainingSet>(System.IO.File.ReadAllText("Acomplete.cfg")));
            network.AddSet(JsonConvert.DeserializeObject<TrainingSet>(System.IO.File.ReadAllText("Bcomplete.cfg")));

            network.LearnNetwork();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Rectangles.GetLength(0); i++)
                for (int j = 0; j < Rectangles.GetLength(1); j++)
                    Rectangles[i, j].Fill = Brushes.LightPink;
        }

        private void PaintGrid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRectangles();
            LoadNetwork();
        }

        private void PaintGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Rectangle)e.Source).Fill = Brushes.PowderBlue;
        }

        private double[] GetInputs()
        {
            List<double> inputs = new List<double>();
            foreach (var rect in Rectangles)
                inputs.Add(rect.Fill == Brushes.PowderBlue ? 1 : -1);
            return inputs.ToArray();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            if(saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName,
                    Newtonsoft.Json.JsonConvert.SerializeObject(new TrainingSet() { Inputs = GetInputs(), Description = "A" }));
            }
        }

        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            var res = network.GetOutput(GetInputs());
            if (res != null)
                MessageBox.Show(res.Description);
            else
                MessageBox.Show("sraka");
        }
    }
}
