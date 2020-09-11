using FileScanner.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Security;
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
        private ObservableCollection<string> images = new ObservableCollection<string>();

        private ObservableCollection<FolderView> items = new ObservableCollection<FolderView>();
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

        public ObservableCollection<FolderView> Items
        {
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
            ScanFolderCommand = new DelegateCommand<string>(ScanFolderAsync, CanExecuteScanFolder);
            
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
      

       

        private async void ScanFolderAsync(string dir)
        {

            Items = new ObservableCollection<FolderView>();

            try
            {
               
                var FilesItems = await Task.Run(() => GetFileAsync(dir));
                var FolderItems = await Task.Run(() => GetFolderAsync(dir)); ;
                foreach(var file in FilesItems)
                {
                    Items.Add(new FolderView() { TheItems = file, Image = "C:/Users/pierluc/Desktop/session/session_5/Développement sujet spéciaux/Labo1/Labo1/FileScanner/Images/file.png" });
                }
                foreach (var folder in FolderItems)
                {
                    Items.Add(new FolderView() { TheItems = folder, Image = "C:/Users/pierluc/Desktop/session/session_5/Développement sujet spéciaux/Labo1/Labo1/FileScanner/Images/folder.png" });
                }

            }
            catch(SecurityException)
            {
                System.Windows.MessageBox.Show("You dont have the permission to acces this folder!: " + dir);
            }
           
            
            


        }


        public List<string> GetFileAsync(string dir)
        {
            var Files = new List<string>();

            foreach (var file in Directory.EnumerateFiles(dir, "*"))
            {
                 Files.Add(file);
            }

            return Files;
        }

        public List<string> GetFolderAsync(string dir)
        {
            var Folders = new List<string>();

            foreach (var folder in  Directory.EnumerateDirectories(dir, "*"))
            {                
                Folders.Add(folder);
            }

            return Folders;
        }



        public class FolderView
        {
            public string TheItems { get; set; }

            public string Image { get; set; }
        }

        ///TODO : Tester avec un dossier avec beaucoup de fichier
        ///TODO : Rendre l'application asynchrone
        ///TODO : Ajouter un try/catch pour les dossiers sans permission


    }
}
