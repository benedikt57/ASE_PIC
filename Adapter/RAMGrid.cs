using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using PicSimulator;

namespace CustomControl
{
    public class RAMGrid : UserControl
    {
        public RAMGrid()
        {
            // Erstellen des Grids
            var myGrid = new Grid();

            // Definition der Spalten
            for (int i = 0; i <= 8; i++)
            {
                myGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Definition der Zeilen
            for (int i = 0; i <= 32; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Füllen des Grids mit Textblöcken
            for (int i = 0; i < 8; i++)
            {
                var txt = new TextBlock { Text = i.ToString().PadLeft(2, '0') };
                txt.Background = System.Windows.Media.Brushes.LightGray;
                txt.TextAlignment = System.Windows.TextAlignment.Center;
                Grid.SetRow(txt, 0);
                Grid.SetColumn(txt, i + 1);
                myGrid.Children.Add(txt);
            }
            for (int i = 0; i < 32; i++)
            {
                var txt = new TextBlock { Text = (i * 8).ToString("X").PadLeft(2, '0') };
                txt.Background = System.Windows.Media.Brushes.LightGray;
                txt.TextAlignment = System.Windows.TextAlignment.Center;
                Grid.SetRow(txt, i + 1);
                Grid.SetColumn(txt, 0);
                myGrid.Children.Add(txt);
            }

            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 32; j++)
                {
                    var bd = new Border();
                    var btn = new Button();
                    var txt = new TextBlock();
                    txt.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Ram[" + ((j - 1) * 8 + i - 1) + "]")
                    {
                        StringFormat = "X2"
                    });
                    txt.TextAlignment = System.Windows.TextAlignment.Center;
                    btn.BorderThickness = new System.Windows.Thickness(0);
                    btn.Background = System.Windows.Media.Brushes.Transparent;
                    btn.SetBinding(Button.CommandProperty, new System.Windows.Data.Binding("RamEditCommand"));
                    btn.CommandParameter = "Ram" + ((j - 1) * 8 + i - 1);
                    btn.Content = txt;
                    bd.BorderBrush = System.Windows.Media.Brushes.Gray;
                    bd.BorderThickness = new System.Windows.Thickness(.1);
                    bd.Child = btn;
                    Grid.SetRow(bd, j);
                    Grid.SetColumn(bd, i);
                    myGrid.Children.Add(bd);
                }
            }


            // Setzen Sie das benutzerdefinierte Steuerelement als Content
            Content = myGrid;
        }
    }
}
