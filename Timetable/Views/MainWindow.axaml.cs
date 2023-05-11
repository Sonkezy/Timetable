using Avalonia.Controls;
using Avalonia.Input;
using Timetable.Models;
using Timetable.ViewModels;

namespace Timetable.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            viewModel.AddWindow(this);
        }
        public void Released(object? sender, PointerReleasedEventArgs e)
        {
            var src = (Control)(e.Source ?? throw new System.Exception("Error"));
            while (src.Parent != null)
            {
                if (src is ListBoxItem)
                {
                    var item = (TableItem?)src.DataContext;
                    item?.Released();
                    return;
                }
                src = (Control)src.Parent;
            }
        }
    }
}