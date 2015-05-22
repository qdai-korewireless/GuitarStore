using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using NHibernate.Util;

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
            PopulateDataGrid();
            PopulateComboBox();
            SetDatabaseRoundTripImage();
        }

        private void PopulateDataGrid()
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string>
            {
            "Builder", "Model", "Price", "Id"
            };
            IList guitarInventory = nhi.GetDynamicInventory();
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, guitarInventory).DefaultView;
         

        }

        private void PopulateComboBox()
        {
            if(comboBoxGuitarTypes.Items.Any())
                comboBoxGuitarTypes.Items.Clear();
            var nhb = new NHibernateBase();
            IList<Guitar> GuitarTypes = nhb.ExecuteICriteria<Guitar>();
            foreach (var item in GuitarTypes)
            {
                var guitar = new Guitar(){Id=item.Id,Type=item.Type};
                comboBoxGuitarTypes.DisplayMemberPath = "Type";
                comboBoxGuitarTypes.SelectedValuePath = "Id";
                comboBoxGuitarTypes.Items.Add(guitar);
            }
        }

        private void comboBoxGuitarTypes_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            try
            {
                dataGridInventory.ItemsSource = null;
                var guitar = (Guitar)comboBoxGuitarTypes.SelectedItem;
                var guitarType = new Guid(guitar.Id.ToString());
                var nhi = new NHibernateInventory();
                //var list = (List<Inventory>)nhi.ExecuteICriteria(guitarType);
                var listResult = nhi.GetDynamicInventory(guitarType);
                var fields = new List<string>
                {
                "Builder", "Model", "Price", "Id"
                };
                var list = BuildDataTable(fields, listResult);
                dataGridInventory.ItemsSource = list.DefaultView;
               
              
            }
            catch (Exception ex)
            {
                labelMessage.Content = ex.Message;
            }
        }

        private void buttonViewSQL_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Utils.FormatSQL(), "Most recent NHibernate generated SQL",
            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SetDatabaseRoundTripImage()
        {
            if (Utils.QueryCounter < 0)
            {
                labelTraffic.Content = "Red";
            }
            else if (Utils.QueryCounter == 0)
            {
            labelTraffic.Content = "Green";
            }
            else if (Utils.QueryCounter == 1)
            {
            labelTraffic.Content = "Green";
            }
            else if (Utils.QueryCounter == 2)
            {
                labelTraffic.Content = "Yellow";
            }
            else if (Utils.QueryCounter > 2)
            {
            labelTraffic.Content = "Red";
            }
            //reset the value each time this method is called.
            Utils.QueryCounter = 0;
          }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var inventoryItem = (Inventory)dataGridInventory.SelectedItem;
            var item = new Guid(inventoryItem.Id.ToString());
            var nhi = new NHibernateInventory();
            if (nhi.DeleteInventoryItem(item))
            {
                dataGridInventory.ItemsSource = null;
                PopulateDataGrid();
                labelMessage.Content = "Item deleted.";
            }
            else
            {
                labelMessage.Content = "Item deletion failed.";
            }
        }

        public DataTable BuildDataTable(List<string> columns, IList results)
        {
            var dataTable = new DataTable();
            foreach (string column in columns)
            {
                dataTable.Columns.Add(column, typeof(string));
            }
            if (columns.Count > 1)
            {
                foreach (object[] row in results)
                {
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }
    }
}
