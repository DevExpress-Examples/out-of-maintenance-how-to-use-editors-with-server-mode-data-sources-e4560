Imports System
Imports System.Windows
Imports DXGridSample.ServiceReference1
Imports DevExpress.Xpf.Core.ServerMode

Namespace DXGridSample
    Partial Public Class MainWindow
        Inherits Window

        Public Property Source() As WcfInstantFeedbackDataSource
        Public Sub New()
            Source = New WcfInstantFeedbackDataSource()
            AddHandler Source.GetSource, (Sub(d, e)
                e.KeyExpression = "Oid"
                e.Query = (New SCEntities(New Uri("http://demos.devexpress.com/Services/WcfLinqSC/WcfSCService.svc"))).SCIssuesDemo
                e.Handled = True
            End Sub)
            DataContext = Me
            InitializeComponent()
        End Sub
    End Class
End Namespace