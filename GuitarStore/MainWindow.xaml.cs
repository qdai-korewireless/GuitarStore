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
            PopulateDataGrid();
            PopulateComboBox();
            SetDatabaseRoundTripImage();
        }

        private void PopulateDataGrid()
        {
            var nhb = new NHibernateInventory();
            var list = nhb.ExecuteICriteriaOrderBy("Builder");
            dataGridInventory.ItemsSource = list;
            if (list != null)
            {
                dataGridInventory.Columns[0].Visibility =
                Visibility.Hidden;
                dataGridInventory.Columns[1].Visibility =
                Visibility.Hidden;
                //dataGridInventory.Columns[8].Visibility =
               // Visibility.Hidden;
            }
        }

        private void PopulateComboBox()
        {
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
                var list = (List<Inventory>)nhi.ExecuteICriteria(guitarType);
                dataGridInventory.ItemsSource = list;
                if (list != null)
                {
                    dataGridInventory.Columns[0].Visibility = System.Windows.Visibility.Hidden;
                    dataGridInventory.Columns[1].Visibility = System.Windows.Visibility.Hidden;
                    //dataGridInventory.Columns[8].Visibility = System.Windows.Visibility.Hidden;
                }
                PopulateComboBox();
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
        
    }
}
