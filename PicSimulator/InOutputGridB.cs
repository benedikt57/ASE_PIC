using PicSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CustomControl
{
    public class InOutputGridB : UserControl
    {
        public InOutputGridB()
        {
            var myGrid = new Grid();
            // Definition der Spalten
            myGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto)
            });
            for (int i = 1; i <= 8; i++)
            {
                myGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star)
                });
            }

            // Definition der Zeilen
            for (int i = 0; i <= 3; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Füllen des Grids mit Textblöcken
            for (int i = 0; i < 8; i++)
            {
                var txt = new TextBlock { Text = (7 - i).ToString() };
                txt.Background = System.Windows.Media.Brushes.LightGray;
                txt.TextAlignment = System.Windows.TextAlignment.Center;
                Grid.SetRow(txt, 0);
                Grid.SetColumn(txt, i + 1);
                myGrid.Children.Add(txt);
            }
            var port = new TextBlock { Text = "PortB" };
            port.Background = System.Windows.Media.Brushes.DarkGray;
            port.TextAlignment = System.Windows.TextAlignment.Center;
            Grid.SetRow(port, 0);
            Grid.SetColumn(port, 0);
            myGrid.Children.Add(port);

            var inOut = new TextBlock { Text = "In/Out" };
            inOut.Background = System.Windows.Media.Brushes.LightGray;
            inOut.TextAlignment = System.Windows.TextAlignment.Center;
            Grid.SetRow(inOut, 1);
            Grid.SetColumn(inOut, 0);
            myGrid.Children.Add(inOut);

            var Wert = new TextBlock { Text = "Wert" };
            Wert.Background = System.Windows.Media.Brushes.LightGray;
            Wert.TextAlignment = System.Windows.TextAlignment.Center;
            Grid.SetRow(Wert, 2);
            Grid.SetColumn(Wert, 0);
            myGrid.Children.Add(Wert);

            // In/Out Textblöcke
            for (int i = 0; i < 8; i++)
            {
                var bd = new Border();
                var txt = new TextBlock();
                txt.SetBinding(TextBlock.TextProperty,
                    new System.Windows.Data.Binding("InOutB[" + (7 - i) + "]"));
                txt.TextAlignment = System.Windows.TextAlignment.Center;
                bd.BorderBrush = System.Windows.Media.Brushes.Gray;
                bd.BorderThickness = new System.Windows.Thickness(.1);
                bd.Child = txt;
                Grid.SetRow(bd, 1);
                Grid.SetColumn(bd, i + 1);
                myGrid.Children.Add(bd);
            }

            // Wert Textblöcke
            for (int i = 0; i < 8; i++)
            {
                var bd = new Border();
                var btn = new Button();
                btn.SetBinding(Button.ContentProperty,
                                       new System.Windows.Data.Binding("WertB[" + (7 - i) + "]"));
                btn.SetBinding(Button.CommandProperty,
                                       new System.Windows.Data.Binding("InputCommand"));
                btn.SetBinding(Button.IsEnabledProperty,
                                       new System.Windows.Data.Binding("InOutB[" + (7 - i) + "]")
                                       {
                                           Converter = new InOutToBoolConverter()
                                       });
                btn.CommandParameter = "btnB" + (7 - i);
                btn.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                btn.Background = System.Windows.Media.Brushes.White;
                btn.BorderBrush = System.Windows.Media.Brushes.Transparent;
                bd.BorderBrush = System.Windows.Media.Brushes.Gray;
                bd.BorderThickness = new System.Windows.Thickness(.1);
                bd.Child = btn;
                Grid.SetRow(bd, 2);
                Grid.SetColumn(bd, i + 1);
                myGrid.Children.Add(bd);
            }

            // Setzen des Grids als Content
            Content = myGrid;
        }
    }
}
