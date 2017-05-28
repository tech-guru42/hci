﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace HCI_Manifestations.Dialogs
{
    public partial class ShowHelp : Window
    {
        public ShowHelp(string key, Window originator)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            string path = string.Format("{0}/Resources/Documentation/{1}.html", Directory.GetCurrentDirectory(), key);

            if (!File.Exists(path))
                key = "Error";
            
            Uri url = new Uri(string.Format("file:///{0}/Resources/Documentation/{1}.html", Directory.GetCurrentDirectory(), key));

            HelpBrowser.Navigate(url);
        }
    }
}
