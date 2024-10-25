﻿using PicSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CustomControl
{
    public class OptionGrid : UserControl
    {
        public OptionGrid()
        {
            var myGrid = new Grid();
            // Definition der Spalten
            for (int i = 1; i <= 8; i++)
            {
                if (i == 2)
                {
                    myGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new System.Windows.GridLength(1.5, System.Windows.GridUnitType.Star)
                    });
                }
                else
                {
                    myGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star)
                    });
                }
            }

            // Definition der Zeilen
            for (int i = 0; i < 2; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Füllen des Grids mit Textblöcken
            myGrid.Children.Add(CreateTextBlock("RBPU", 0, 0));
            myGrid.Children.Add(CreateTextBlock("INTDEG", 0, 1));
            myGrid.Children.Add(CreateTextBlock("T0CS", 0, 2));
            myGrid.Children.Add(CreateTextBlock("T0SE", 0, 3));
            myGrid.Children.Add(CreateTextBlock("PSA", 0, 4));
            myGrid.Children.Add(CreateTextBlock("PS2", 0, 5));
            myGrid.Children.Add(CreateTextBlock("PS1", 0, 6));
            myGrid.Children.Add(CreateTextBlock("PS0", 0, 7));

            myGrid.Children.Add(CreateButton(1, 0));
            myGrid.Children.Add(CreateButton(1, 1));
            myGrid.Children.Add(CreateButton(1, 2));
            myGrid.Children.Add(CreateButton(1, 3));
            myGrid.Children.Add(CreateButton(1, 4));
            myGrid.Children.Add(CreateButton(1, 5));
            myGrid.Children.Add(CreateButton(1, 6));
            myGrid.Children.Add(CreateButton(1, 7));

            Content = myGrid;
        }
        private TextBlock CreateTextBlock(string text, int row, int column)
        {
            var textBlock = new TextBlock { Text = text };
            textBlock.Background = System.Windows.Media.Brushes.LightGray;
            textBlock.TextAlignment = System.Windows.TextAlignment.Center;
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, column);
            return textBlock;
        }

        private Button CreateButton(int row, int column)
        {
            var button = new Button();
            var txt = new TextBlock();
            txt.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Ram[129]")
            {
                ConverterParameter = 7 - column,
                Converter = new SFRConverter(),
            });
            txt.TextAlignment = System.Windows.TextAlignment.Center;
            button.BorderThickness = new System.Windows.Thickness(0);
            button.Background = System.Windows.Media.Brushes.Transparent;
            button.SetBinding(Button.CommandProperty, new System.Windows.Data.Binding("SFRCommand"));
            button.CommandParameter = "O" + (7 - column);
            button.Content = txt;

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            return button;
        }
    }
}
