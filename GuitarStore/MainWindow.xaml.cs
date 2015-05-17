﻿using System;
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
using NHibernate.GuitarStore.Common;
using NHibernate.GuitarStore.DataAccess;

namespace GuitarStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var nhb = new NHibernateBase();
            nhb.Initialize("NHibernate.GuitarStore");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var nhb = new NHibernateInventory();
            var list = nhb.ExecuteICriteriaOrderBy("Builder");
            dataGridInventory.ItemsSource = list;
            if (list != null)
            {
                dataGridInventory.Columns[0].Visibility =
                System.Windows.Visibility.Hidden;
                dataGridInventory.Columns[1].Visibility =
                System.Windows.Visibility.Hidden;
                dataGridInventory.Columns[8].Visibility =
                System.Windows.Visibility.Hidden;
            }
        }
    }
}
