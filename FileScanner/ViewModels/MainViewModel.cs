using FileScanner.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();
        private ObservableCollection<Item> items = new ObservableCollection<Item>();

        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }

        public ObservableCollection<string> FolderItems { 
            get => folderItems;
            set 
            { 
                folderItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Item> Items {
            get => items;
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel()
        {
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolder, CanExecuteScanFolder);
        }

        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }

        private void OpenFolder(string obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolder = fbd.SelectedPath;
                }
            }
        }

        private async void ScanFolder(string dir)
        {
            var watch = Stopwatch.StartNew();
            try
            {
                await Scan(dir);
            }
            catch (System.UnauthorizedAccessException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                
            }
            watch.Stop();
        }
        private async Task Scan(string dir)
        {
            //FolderItems = new ObservableCollection<string>(GetDirs(dir));
            Items = new ObservableCollection<Item>();
            var directory = Directory.EnumerateDirectories(dir, "*");
            var file = Directory.EnumerateFiles(dir, "*");

            foreach (var item in directory)
            {
                //FolderItems.Add(item);
                //Item temp = new Item { Name = item, Image = "Image/temp-folder.jpg" };
                Item temp = await Task.Run(() => ItemAdd(item, "Image/temp-folder.jpg"));
                Items.Add(temp);
            }
            foreach (var item in file)
            {
                //FolderItems.Add(item);
                //WebsiteDataModel ws = await Task.Run(() => DownloadWebsite(site));
                //Item temp = new Item { Name = item, Image = "Image/directory.jpg" };
                Item temp = await Task.Run(() => ItemAdd(item, "Image/directory.jpg"));
                Items.Add(temp);
            }
        }
        private Item ItemAdd(string n, string i)
        {
            Item output = new Item();
            output.Name = n;
            output.Image = i;
            return output;
        }
        IEnumerable<string> GetDirs(string dir)
        {            
            foreach (var d in Directory.EnumerateDirectories(dir, "*"))
            {
                yield return d;

                foreach (var f in Directory.EnumerateFiles(d, "*"))
                {
                    yield return f;
                }
            }
        }
        public class Item
        {
            public string Image { get; set; }
            public string Name { get; set; }
        }

        ///TODO : Tester avec un dossier avec beaucoup de fichier
        ///TODO : Rendre l'application asynchrone
        ///TODO : Ajouter un try/catch pour les dossiers sans permission


    }
}
