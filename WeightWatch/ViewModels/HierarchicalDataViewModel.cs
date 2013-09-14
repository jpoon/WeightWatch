
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace WeightWatch.ViewModels
{
    public class HierarchicalDataViewModel : ViewModelBase
    {
        private ObservableCollection<CollectionDataItemViewModel> items;

        /// <summary>
        /// Initializes the items.
        /// </summary>
        private void InitializeItems()
        {
            this.items = new ObservableCollection<CollectionDataItemViewModel>();
            for (int i = 1; i <= 4; i++)
            {
                CollectionDataItemViewModel newItem = new CollectionDataItemViewModel(string.Format("Collection {0}", i),
                    string.Format("Detailed information for collection {0}", i));
                this.items.Add(newItem);
            }
        }

        /// <summary>
        /// A collection for <see cref="DataItemViewModel"/> objects.
        /// </summary>
        public ObservableCollection<CollectionDataItemViewModel> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.InitializeItems();
                }
                return this.items;
            }
            private set
            {
                this.items = value;
            }
        }
    }

    public class CollectionDataItemViewModel : ViewModelBase
    {
        private ObservableCollection<DataItemViewModel> items;
        private string title;
        private string information;

        public CollectionDataItemViewModel(string title, string information = null)
        {
            this.title = title;
            this.information = information;
            this.InitializeItems();
        }

        /// <summary>
        /// Gets or sets the title of the collection.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }
            private set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.OnPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// Gets or sets the information for the collection.
        /// </summary>
        public string Information
        {
            get
            {
                return this.information;
            }
            private set
            {
                if (this.information != value)
                {
                    this.information = value;
                    this.OnPropertyChanged("Information");
                }
            }
        }

        /// <summary>
        /// A collection for <see cref="DataItemViewModel"/> objects.
        /// </summary>
        public ObservableCollection<DataItemViewModel> Items
        {
            get
            {
                return this.items;
            }
            private set
            {
                this.items = value;
            }
        }

        /// <summary>
        /// Initializes the items.
        /// </summary>
        private void InitializeItems()
        {
            this.items = new ObservableCollection<DataItemViewModel>();
            for (int i = 1; i <= 3; i++)
            {
                this.items.Add(new DataItemViewModel()
                {
                    ImageSource = new Uri("Images/Frame.png", UriKind.RelativeOrAbsolute),
                    ImageThumbnailSource = new Uri("Images/FrameThumbnail.png", UriKind.RelativeOrAbsolute),
                    Title = "Title " + i,
                    Information = "Information " + i,
                    Group = (i % 2 == 0) ? "EVEN" : "ODD"
                });
            }
        }
    }
}
