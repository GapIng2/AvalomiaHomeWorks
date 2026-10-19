using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;


namespace AvaloniaApplication1.Views;


public partial class MainWindow : Window
{
    int vertexCount = 0;
    Dictionary<string, (int x, int y)> vertexPositions = new Dictionary<string, (int x, int y)>();
    List<(string from, string to, int weight)> edges = new List<(string from, string to, int weight)>();
    public MainWindow()
    {
        
        InitializeComponent();
       
    }
    private List<string> FindShortestPath(string start, string finish)
    {
        Dictionary<string, int> distances = new Dictionary<string, int>();
        Dictionary<string, string> previous = new Dictionary<string, string>();
        List<string> unvisited = new List<string>();

        foreach (string vertex in vertexPositions.Keys)
        {
            distances[vertex] = int.MaxValue;
            unvisited.Add(vertex);
        }

        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            string current = null;
            int minDistance = int.MaxValue;

            foreach (string vertex in unvisited)
            {
                if (distances[vertex] < minDistance)
                {
                    minDistance = distances[vertex];
                    current = vertex;
                }
            }

            if (current == null || minDistance == int.MaxValue)
                break;

            if (current == finish)
                break;

            unvisited.Remove(current);

            foreach (var edge in edges)
            {
                string neighbor = null;

                if (edge.from == current)
                    neighbor = edge.to;
                else if (edge.to == current)
                    neighbor = edge.from;

                if (neighbor == null || !unvisited.Contains(neighbor))
                    continue;

                int newDistance = distances[current] + edge.weight;

                if (newDistance < distances[neighbor])
                {
                    distances[neighbor] = newDistance;
                    previous[neighbor] = current;
                }
            }
        }

        if (!distances.ContainsKey(finish) || distances[finish] == int.MaxValue)
            return new List<string>();

        List<string> path = new List<string>();
        string step = finish;

        path.Add(step);

        while (step != start)
        {
            if (!previous.ContainsKey(step))
                return new List<string>();

            step = previous[step];
            path.Add(step);
        }

        path.Reverse();

        return path;
    }

    private void AddEdgeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        string from = ComboBoxOne.SelectedItem.ToString();
        string to = ComboBoxTwo.SelectedItem.ToString();
        
        int weight = int.Parse(WeightTextBox.Text);
        edges.Add((from, to, weight));
        
        var fromPosition = vertexPositions[from];
        var toPosition = vertexPositions[to];
        
        Line line = new Line();

        Random random = new Random();

        line.Stroke = new SolidColorBrush(Color.FromRgb(
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256)
        ));

        line.StrokeThickness = 3;
        
        line.StartPoint = new Avalonia.Point(fromPosition.x + 25, fromPosition.y + 25);
        line.EndPoint = new Avalonia.Point(toPosition.x + 25, toPosition.y + 25);
        
        CanvasGraph.Children.Insert(0, line);
        
        TextBlock weightText = new TextBlock();
        weightText.Text = weight.ToString();

        double middleX = (fromPosition.x + toPosition.x) / 2.0 + 25;
        double middleY = (fromPosition.y + toPosition.y) / 2.0 + 25;

        Canvas.SetLeft(weightText, middleX);
        Canvas.SetTop(weightText, middleY);

        CanvasGraph.Children.Add(weightText);
    }
    private void BtnAdd_OnClick(object? sender, RoutedEventArgs e)
    {
       
        string VertexTextBoxText = VertexTextBox.Text;
        

        if (vertexPositions.ContainsKey(VertexTextBoxText))
        {
            return;
        }
        
        TextBlock vertexName = new TextBlock();
        vertexName.Text = VertexTextBoxText;
        
        Ellipse vertex = new Ellipse();
        vertex.Width = 50;
        vertex.Height = 50;
        vertex.Fill = Brushes.Orange;
        
        
        
        ComboBoxOne.Items.Add(VertexTextBoxText);
        ComboBoxTwo.Items.Add(VertexTextBoxText);
        
        StartVertexComboBox.Items.Add(VertexTextBoxText);
        FinishVertexComboBox.Items.Add(VertexTextBoxText);

       
        vertexCount++;

        int column = (vertexCount - 1) % 5;
        int row = (vertexCount - 1) / 5;

        int left = 100 + column * 80;
        int top = 50 + row * 80;
        
        Canvas.SetLeft(vertexName, left);
        Canvas.SetTop(vertexName, top + 52);

        CanvasGraph.Children.Add(vertexName);
        
        vertexPositions.Add(VertexTextBoxText, (left, top));

        Canvas.SetLeft(vertex, left);
        Canvas.SetTop(vertex, top);

        CanvasGraph.Children.Add(vertex);
    }
    
    private void FindPathButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
        if (StartVertexComboBox.SelectedItem == null ||
            FinishVertexComboBox.SelectedItem == null)
        {
            PathResultText.Text = "Выбери начальную и конечную вершины";
            return;
        }
        
        string start = StartVertexComboBox.SelectedItem.ToString();
        string finish = FinishVertexComboBox.SelectedItem.ToString();

        List<string> path = FindShortestPath(start, finish);

        if (path.Count == 0)
        {
            PathResultText.Text = "Путь не найден";
            return;
        }

        PathResultText.Text = string.Join(" -> ", path);
    }
}