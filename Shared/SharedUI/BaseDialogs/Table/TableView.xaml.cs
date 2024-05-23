﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonControls.Table
{
    /// <summary>
    /// Interaction logic for TableView.xaml
    /// </summary>
    public partial class TableView : UserControl
    {
        public TableView()
        {
            InitializeComponent();
            MouseRightButtonDown += new MouseButtonEventHandler(Window1_MouseRightButtonUp);
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var viewModel = DataContext as TableViewModel;
            var factory = viewModel.Factory;

            var resourceName = factory.GetCellTemplate(e.PropertyName);
            DataTemplate dt = (DataTemplate)Resources[resourceName];
            if (dt != null)
            {
                DataGridTemplateColumn c = new DataGridTemplateColumn()
                {
                    CellTemplate = dt,
                    Header = e.Column.Header,
                    HeaderTemplate = e.Column.HeaderTemplate,
                    HeaderStringFormat = e.Column.HeaderStringFormat,
                    SortMemberPath = e.PropertyName // this is used to index into the DataRowView so it MUST be the property's name (for this implementation anyways)
                };
                e.Column = c;
            }
        }

        void Window1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null) return;

            if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;
                cell.Focus();

                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                DataGridRow row = dep as DataGridRow;
                dataGrid1.SelectedItem = row.DataContext;
            }
        }
    }

    public class DataRowViewConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridCell cell = value as DataGridCell;
            if (cell == null)
                return null;

            System.Data.DataRowView drv = cell.DataContext as System.Data.DataRowView;
            if (drv == null)
                return null;


            return drv.Row[cell.Column.SortMemberPath];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
