Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Windows.Data
Imports System.Windows.Markup
Imports DevExpress.Xpf.Grid.LookUp
Imports DevExpress.Xpf.Core.ServerMode
Imports System.Collections
Imports DevExpress.Xpf.Editors.Popups
Imports DevExpress.Xpf.Grid
Imports DevExpress.Xpf.Grid.LookUp.Native
Imports DevExpress.Xpf.Editors
Imports DevExpress.Xpf.Editors.Native
Imports DevExpress.Xpf.Core.DataSources
Imports DevExpress.Xpf.Editors.Validation.Native
Imports DevExpress.Xpf.Core.Native
Imports System.Text
Imports DevExpress.Xpf.Editors.Helpers
Imports System.ComponentModel
Imports DevExpress.Data.Linq
Imports System.Windows.Threading
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Utils
Imports System.Diagnostics
Imports System.Windows.Controls
Imports DevExpress.Data.Helpers
Imports DevExpress.Data
Imports DevExpress.Data.Async.Helpers
Imports DevExpress.Xpf.Data
Imports System.Reflection
Imports System.Threading
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB

Namespace DXGridSample
    Partial Public Class MainWindow
        Inherits Window

        Public Sub New()
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(AccessConnectionProvider.GetConnectionString("..\..\Database.mdb"), AutoCreateOption.DatabaseAndSchema)
            InitializeComponent()

            Dim XPInstantFeedbackSource As New XPInstantFeedbackSource(GetType(Items1))
            XPInstantFeedbackSource.DisplayableProperties = "Id;Name"

            Dim entities As New DatabaseEntities()
            Dim dataSource As EntityInstantFeedbackDataSource = New EntityInstantFeedbackDataSource With {.KeyExpression = "Id", .QueryableSource = entities.Items}
            'EntityServerModeDataSource dataSource = new EntityServerModeDataSource { KeyExpression = "Id", QueryableSource = entities.Items };

            edit.DisplayMember = "Name"
            edit.ValueMember = "Id"
            'edit.SelectedValue = entities.Items.ToList()[100];
            edit.EditValue = "19"

            edit.ItemsSource = XPInstantFeedbackSource
            'edit.ItemsSource = dataSource.Data;
        End Sub
    End Class
    Public Class Items1
        Inherits XPObject

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub
        Public Sub New()
        End Sub
        Private fId As String
        Public Property Id() As String
            Get
                Return fId
            End Get
            Set(ByVal value As String)
                SetPropertyValue("Id", fId, value)
            End Set
        End Property
        Private fName As String
        Public Property Name() As String
            Get
                Return fName
            End Get
            Set(ByVal value As String)
                SetPropertyValue("Name", fName, value)
            End Set
        End Property
    End Class
End Namespace