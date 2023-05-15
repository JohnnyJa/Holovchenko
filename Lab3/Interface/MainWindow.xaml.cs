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
using Entities;
using MainLogic;

namespace Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Repository rep = new Repository();

        public MainWindow()
        {
            InitializeComponent();
            rep.CreateDb();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Entity entity = new Entity()
            {
                Id = Convert.ToInt32(KeyToAddTextBox.Text),
                Data = DataToAddTextBox.Text
            };
            rep.AddNewEntity(entity);
            MessageBox.Show("Added");
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(KeyToFindTextBox.Text);
            try
            {
                Entity entity = rep.GetEntityByKey(id);
                MessageBox.Show(entity.Data);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        

        private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(KeyToRemoveTextBox.Text);
            try
            {
                rep.RemoveDataByKey(id);
                MessageBox.Show("Removed");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Entity entity = new Entity()
                {
                    Id = Convert.ToInt32(KeyToAddTextBox.Text),
                    Data = DataToAddTextBox.Text
                };
                rep.UpdateDataByKey(entity.Id, entity.Data);
                MessageBox.Show("Updated");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }
    }
}