﻿using HCI_Manifestations.Dialogs;
using HCI_Manifestations.Help;
using HCI_Manifestations.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Xceed.Wpf.Toolkit;

namespace HCI_Manifestations.dialogs
{

    public partial class AddManifestation : Window, INotifyPropertyChanged
    {
        #region Attributes
        private Manifestation manifestation;
        public Manifestation Manifestation
        {
            get { return manifestation; }
            set
            {
                if (value != manifestation)
                {
                    manifestation = value;
                    OnPropertyChanged("Manifestation");
                }
            }
        }

        private bool idError;
        private bool nameError;
        private bool descriptionError;
        private bool publicError;
        #endregion

        #region Constructors
        public AddManifestation()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            Manifestation = new Manifestation();
            Manifestation.Date = DateTime.Now.Date;
            Manifestation.X = -1;
            Manifestation.Y = -1;
            
            autoCompleteBoxTypes.DataContext = Database.getInstance();
            autoCompleteBoxTags.DataContext = Database.getInstance();

            DataContext = Manifestation;
            
            idError = false;
            nameError = false;
            descriptionError = false;
            publicError = false;
        }
        #endregion

        #region Event handlers
        private void loadIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg,*.ico)|*.ico;*.png;*.jpg";
            if (dialog.ShowDialog() == true)
            {
                textBoxIconPath.Text = dialog.FileName;
            }
        }

        private void buttonAddNewType_Click(object sender, RoutedEventArgs e)
        {
            if (Database.GetManifestation(autoCompleteBoxTypes.Text) == null)
            {
                AddType addType = new AddType(autoCompleteBoxTypes.Text);
                addType.ShowDialog();
            }
            else
            {
                AddType addType = new AddType();
                addType.ShowDialog();
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            // Force revalidation
            idError = false; nameError = false;  descriptionError = false;  publicError = false;
            textBoxId.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textBoxName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textBoxDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textBoxPublic.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // Type validation
            if (string.IsNullOrWhiteSpace(autoCompleteBoxTypes.Text)
                || Database.GetType(autoCompleteBoxTypes.Text) == null)
            {
                textBoxTypeError.Visibility = System.Windows.Visibility.Visible;
                autoCompleteBoxTypes.Text = "";
                autoCompleteBoxTypes.Focus();
                return;
            }

            if (idError == false &&
                nameError == false &&
                descriptionError == false &&
                publicError == false)
            {
                manifestation.Type = Database.GetType(autoCompleteBoxTypes.Text);

                // Set the default type icon
                if (string.IsNullOrEmpty(textBoxIconPath.Text) || string.IsNullOrWhiteSpace(textBoxIconPath.Text))
                {
                    manifestation.IconPath = manifestation.Type.IconPath;
                }

                Database.AddManifestation(manifestation);
                Close();
            }

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void buttonAddNewTag_Click(object sender, RoutedEventArgs e)
        {
            // if it hasn't been found in database, open dialog to add it
            if (Database.GetTag(autoCompleteBoxTags.Text) == null)
            {
                AddTag dialog;
                if (string.IsNullOrWhiteSpace(autoCompleteBoxTags.Text))
                {
                    dialog = new AddTag();
                }
                else
                {
                    dialog = new AddTag(autoCompleteBoxTags.Text);
                }
                dialog.ShowDialog();

                // If it has successfully added a new tag
                if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                {
                    ManifestationTag tag = new ManifestationTag(Database.getInstance().Tags.Last());
                    Manifestation.Tags.Add(tag);
                    comboBoxTags.SelectedItems.Add(tag);
                }

                // solve xceed library bug
                if (comboBoxTags.SelectedItems.Count == 1)
                {
                    comboBoxTags.Text = Manifestation.Tags[0].Id;
                }
                if (comboBoxTags.SelectedItems.Count == 0)
                {
                    comboBoxTags.Text = "";
                }
            }
            // if it has been found in database
            else
            {
                ManifestationTag tag = new ManifestationTag(Database.GetTag(autoCompleteBoxTags.Text));

                // make sure tag is't already added
                bool found = false;
                foreach (var item in comboBoxTags.SelectedItems)
                {
                    if (((ManifestationTag)item).Id.Equals(tag.Id))
                        found = true;
                }
                if (!found)
                {
                    Manifestation.Tags.Add(tag);
                    comboBoxTags.SelectedItems.Add(tag);
                }

                // solve xceed library bug
                if (comboBoxTags.SelectedItems.Count == 1)
                {
                    comboBoxTags.Text = Manifestation.Tags[0].Id;
                }
                if (comboBoxTags.SelectedItems.Count == 0)
                {
                    comboBoxTags.Text = "";
                }
            }

            // reset field
            autoCompleteBoxTags.SelectedItem = null;
            autoCompleteBoxTags.Text = string.Empty;
        }

        private void comboBoxTags_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            var selectedTags = comboBoxTags.SelectedItems;
            Manifestation.Tags.Clear();

            foreach (var selectedTag in selectedTags)
            {
                Manifestation.Tags.Add(new ManifestationTag((ManifestationTag)selectedTag));
            }

            if (comboBoxTags.SelectedItems.Count == 1)
            {
                comboBoxTags.Text = Manifestation.Tags[0].Id;
            }
            if (comboBoxTags.SelectedItems.Count == 0)
            {
                comboBoxTags.Text = "";
            }
        }

        private void comboBoxTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manifestation.Type = new ManifestationType((ManifestationType)autoCompleteBoxTypes.SelectedItem);
        }

        private void textBoxId_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                idError = true;
        }

        private void textBoxName_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                nameError = true;
        }

        private void textBoxDescription_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                descriptionError = true;
        }

        private void textBoxPublic_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                publicError = true;
        }

        private void textBoxId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxId.Text.Length > 0)
                buttonSave.IsEnabled = true;
            else
                buttonSave.IsEnabled = false;
        }

        private void autoCompleteBoxType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Database.GetType(autoCompleteBoxTypes.Text) == null)
                {
                    buttonAddNewType_Click(null, null);
                }
            }
        }

        private void autoCompleteBoxTag_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonAddNewTag_Click(null, null);
            }
        }

        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HelpProvider.ShowHelp("Manifestation", this);
        }

        private void textBoxTypeError_LostFocus(object sender, RoutedEventArgs e)
        {
            textBoxTypeError.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion

        #region PropertyChangedNotifier
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
