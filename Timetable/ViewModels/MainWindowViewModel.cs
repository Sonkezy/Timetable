using Avalonia.Media;
using Avalonia.Controls;
using ReactiveUI;
using Avalonia;
using Avalonia.Collections;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using System.Reactive;
using System;
using Timetable.Models;

namespace Timetable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        bool switcher = true;
        int day = 1;
        Button? button_departure, button_arrival, button_yesterday, button_today, button_tomorrow;
        TableItem[] items = Array.Empty<TableItem>();
        BaseReader br = new();
        public MainWindowViewModel() {
           
            SelectDeparture = ReactiveCommand.Create(() => { Switcher(true); });
            SelectArrival = ReactiveCommand.Create(() => { Switcher(false); });
            SelectYesterday = ReactiveCommand.Create(() => { DaySwitcher(0); });
            SelectToday = ReactiveCommand.Create(() => { DaySwitcher(1); });
            SelectTomorrow = ReactiveCommand.Create(() => { DaySwitcher(2); });
            UpdateItems();
        }
        public TableItem[] Items { get => items; set { this.RaiseAndSetIfChanged(ref items, value); } }

        private void UpdateItems()
        {
            Items = br.GetItems(switcher, day);
        }

        void Switcher(bool switcher)
        {
            if (this.switcher == switcher) return;
            this.switcher = switcher;
            TT_Switcher(0, switcher);
            TT_Switcher(1, !switcher);
            UpdateItems();
        }
        void DaySwitcher(int day)
        {
            if (this.day == day) return;
            this.day = day;
            TT_day_Switcher(0, 0 == day);
            TT_day_Switcher(1, 1 == day);
            TT_day_Switcher(2, 2 == day);
            UpdateItems();
        }

        public void AddWindow(Window mw)
        {
            button_departure = mw.Find<Button>("Button_Departure");
            button_arrival = mw.Find<Button>("Button_Arrival");
            button_yesterday = mw.Find<Button>("Button_Yesterday");
            button_today = mw.Find<Button>("Button_Today");
            button_tomorrow = mw.Find<Button>("Button_Tomorrow");
            TT_Switcher(0, switcher);
            TT_Switcher(1, !switcher);
            TT_day_Switcher(1, switcher);
        }
        public void TT_Switcher(int num, bool state)
        {
            var button = num == 0 ? button_departure : button_arrival;
            if (button == null) return;
            button.Classes = new Classes(state ? "tt-switcher-active" : "tt-switcher");
            //button.Background = new SolidColorBrush(Color.Parse(state ? "#EB7501" : "#323B44"));
            
            var canvas = (Canvas)((AvaloniaList<ILogical>)button.GetLogicalChildren())[0];
            var app = Application.Current;
            var res = app?.Resources;
            var img2 = (Image?)res?[num == 0 ? (state ? "selected_departure" : "unselected_departure") : (state ? "selected_arrival" : "unselected_arrival")];
            var img = (Image)canvas.Children[0];
            img.Source = img2?.Source;

            //var tb = (TextBlock)canvas.Children[1];
            //tb.Foreground = new SolidColorBrush(Color.Parse(state ? "#1C242B" : "#6F788B"));
        }
        public void TT_day_Switcher(int num, bool state)
        {
            var button = num == 0 ? button_yesterday : num == 1 ? button_today : button_tomorrow;
            if (button == null) return;
            button.Background = new SolidColorBrush(Color.Parse(state ? "#8892a5" : "#475562"));
            button.Foreground = new SolidColorBrush(Color.Parse(state ? "#fff" : "#8892a5"));
        }
        public ReactiveCommand<Unit, Unit> SelectDeparture { get; }
        public ReactiveCommand<Unit, Unit> SelectArrival { get; }
        public ReactiveCommand<Unit, Unit> SelectYesterday { get; }
        public ReactiveCommand<Unit, Unit> SelectToday { get; }
        public ReactiveCommand<Unit, Unit> SelectTomorrow { get; }
    }
}