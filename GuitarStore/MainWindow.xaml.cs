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

        public int FirstResult = 0;
        public int MaxResult = 25;
        public int totalCount = 0;
        private void PopulateDataGrid()
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string>
            {
            "Builder", "Model", "Price", "Id","Profit"
            };
            IList guitarInventory;
            var inventoryCount = nhi.GetInventoryPaging(MaxResult, FirstResult, out guitarInventory);
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, guitarInventory).DefaultView;
            totalCount = inventoryCount;
            labelPaging.Content = FirstResult.ToString() + " - " + (FirstResult + MaxResult).ToString() + " / " +
                                  inventoryCount.ToString();
            SetDatabaseRoundTripImage();
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
                "Builder", "Model", "Price", "Id","Profit"
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

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            buttonPrevious.IsEnabled = true;
            FirstResult = FirstResult + MaxResult;
            PopulateDataGrid();
            if (FirstResult > 0)
            {
                buttonPrevious.IsEnabled = true;
            }
            if (FirstResult + MaxResult >= totalCount)
            {
                buttonNext.IsEnabled = false;
            }
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            buttonNext.IsEnabled = true;
            if (FirstResult > 0)
            {
                FirstResult = FirstResult - MaxResult;
                if (FirstResult < 0) FirstResult = 0;
            }
            else
            {
                buttonPrevious.IsEnabled = false;
            }
            PopulateDataGrid();
            if (FirstResult == 0)
            {
                buttonPrevious.IsEnabled = true;
            }
        }

        private void buttonSUM_Click(object sender, RoutedEventArgs e)
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string> { "Guitar Type", "Total Value" };
            IList GuitarInventory = nhi.ExecuteNamedQuery("GuitarValueByTypeHQL");
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, GuitarInventory).DefaultView;
            SetDatabaseRoundTripImage();
        }

        private void buttonAverage_Click(object sender, RoutedEventArgs e)
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string>
            {
                "Guitar Type", "Average Value"
            };
            var GuitarInventory = nhi.ExecuteNamedQuery("GuitarAVGValueByTypeHQL");
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, GuitarInventory).DefaultView;
            SetDatabaseRoundTripImage();
        }

        private void buttonMinimum_Click(object sender, RoutedEventArgs e)
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string>
            {
                "Guitar Type", "Minimum Value"
            };
            var GuitarInventory = nhi.ExecuteNamedQuery("GuitarMINValueByTypeHQL");
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, GuitarInventory).DefaultView;
            SetDatabaseRoundTripImage();
        }

        private void buttonMaximum_Click(object sender, RoutedEventArgs e)
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string>
            {
                "Guitar Type", "Maximum Value"
            };
            var GuitarInventory = nhi.ExecuteNamedQuery("GuitarMAXValueByTypeHQL");
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, GuitarInventory).DefaultView;
            SetDatabaseRoundTripImage();
        }

        private void buttonCount_Click(object sender, RoutedEventArgs e)
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string>
            {
                "Guitar Type", "Count Value"
            };
            var GuitarInventory = nhi.ExecuteNamedQuery("GuitarCOUNTByTypeHQL");
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, GuitarInventory).DefaultView;
            SetDatabaseRoundTripImage();
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            var nhi = new NHibernateInventory();
            var fields = new List<string>
            {
            "Builder", "Model", "Price", "Id"
            };
            var guitarInventory =
            nhi.ExecuteDetachedQuery("%" + textBoxSearch.Text + "%");
            dataGridInventory.ItemsSource =
            BuildDataTable(fields, guitarInventory).DefaultView;
            SetDatabaseRoundTripImage();
        }
    }
}
