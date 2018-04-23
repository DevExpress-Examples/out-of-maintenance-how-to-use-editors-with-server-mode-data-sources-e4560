using System;
using System.Windows;
using DXGridSample.ServiceReference1;
using DevExpress.Xpf.Core.ServerMode;

namespace DXGridSample {
    public partial class MainWindow : Window {
        public WcfInstantFeedbackDataSource Source { get; set; }
        public MainWindow() {
            Source = new WcfInstantFeedbackDataSource();
            Source.GetSource += ((d, e) => {
                e.KeyExpression = "Oid";
                e.Query = new SCEntities(new Uri(@"http://demos.devexpress.com/Services/WcfLinqSC/WcfSCService.svc")).SCIssuesDemo;
                e.Handled = true;
            });
            DataContext = this;
            InitializeComponent();
        }
    }
}