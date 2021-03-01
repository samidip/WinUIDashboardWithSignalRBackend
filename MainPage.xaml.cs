using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.DataVisualization;

namespace WinUIDashboard
{
    public sealed partial class MainPage : Page
    {
        DummyViewModel vm = new DummyViewModel();
        HubConnection hubConnection;

        public MainPage()
        {
            this.InitializeComponent();
            this.DataGrid.ItemsSource = vm.GridValues;
            DoRealTimeSuff();
        }

        async private void DoRealTimeSuff()
        {
            SignalRDashSyncSetup();
            await SignalRConnect();
        }

        private void SignalRDashSyncSetup()
        {
            hubConnection = new HubConnectionBuilder().WithUrl($"Localhost SignalR backend or hosted URL/dashboardHub").Build();

            hubConnection.On<string>("ReceiveDashUpdate", (dashupdate) =>
            {
                var receivedDashUpdate = dashupdate;
                ((ArrowGaugeIndicator)this.RadialGauge.Indicators[0]).Value = Int32.Parse(dashupdate);
                ((LinearBarGaugeIndicator)this.LinearGauge.Indicators[0]).Value = Int32.Parse(dashupdate);
            });

            hubConnection.On<GridData[]>("ReceiveObjUpdate", (gridUpdate) =>
            {
                var receivedChartUpdate = gridUpdate;
                this.DataGrid.ItemsSource = gridUpdate;
            });

        }

        private async Task SignalRConnect()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                // Connection failed.
            }
        }
    }

    

    public class DummyViewModel
    {
        public int GaugeValue { get; set; }
        public List<GridData> GridValues { get; set; }

        public DummyViewModel()
        {
            this.GaugeValue = 0;

            this.GridValues = new List<GridData> {
                                new GridData { Category = "A", Value = 20 },
                                new GridData { Category = "B", Value = 30 }
                                };
        }
    }

    public class GridData
    {
        public string Category { get; set; }
        public int Value { get; set; }
    }
}
