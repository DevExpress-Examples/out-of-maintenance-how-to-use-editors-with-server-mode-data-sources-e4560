Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.Xpf.Core.Native
Imports DevExpress.Xpf.Grid
Imports System.Windows
Imports DevExpress.Data.Async.Helpers
Imports DevExpress.Xpf.Editors
Imports DevExpress.Xpf.Utils
Imports DevExpress.Data
Imports System.Reflection
Imports System.Windows.Data
Imports System.Windows.Controls
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Editors.Native
Imports DevExpress.Xpf.Editors.Validation.Native
Imports DevExpress.Xpf.Editors.EditStrategy
Imports DevExpress.Xpf.Editors.Popups

Namespace DXGridSample
    Public Class MyLookUpEdit
        Inherits PopupBaseEdit

        Public Shared ReadOnly ItemsSourceProperty As DependencyProperty = DependencyPropertyManager.Register("ItemsSource", GetType(Object), GetType(MyLookUpEdit), New FrameworkPropertyMetadata(Nothing))
        Public Shared ReadOnly DisplayMemberProperty As DependencyProperty = DependencyPropertyManager.Register("DisplayMember", GetType(String), GetType(MyLookUpEdit), New FrameworkPropertyMetadata(String.Empty))
        Public Shared ReadOnly SelectedValueProperty As DependencyProperty = DependencyPropertyManager.Register("SelectedValue", GetType(Object), GetType(MyLookUpEdit), New FrameworkPropertyMetadata(Nothing, AddressOf OnSelectedValuePropertyChanged))
        Public Shared ReadOnly ValueMemberProperty As DependencyProperty = DependencyPropertyManager.Register("ValueMember", GetType(String), GetType(MyLookUpEdit), New FrameworkPropertyMetadata(Nothing))
        Public Shared ReadOnly GridControlProperty As DependencyProperty = DependencyPropertyManager.Register("GridControl", GetType(GridControl), GetType(MyLookUpEdit), New FrameworkPropertyMetadata(Nothing, AddressOf OnGridControlPropertyChanged))
        Public Shared ReadOnly FocusedRowHandleProperty As DependencyProperty = DependencyProperty.Register("FocusedRowHandle", GetType(Integer), GetType(MyLookUpEdit), New FrameworkPropertyMetadata(GridControl.InvalidRowHandle, AddressOf OnFocusedRowHandlePropertyChanged))
        Public Shared ReadOnly IncrementalFilteringProperty As DependencyProperty = DependencyProperty.Register("IncrementalFiltering", GetType(Boolean), GetType(MyLookUpEdit), New UIPropertyMetadata(True))
        Public Shared ReadOnly GridSearchStringProperty As DependencyProperty = DependencyProperty.Register("GridSearchString", GetType(String), GetType(MyLookUpEdit), New UIPropertyMetadata(String.Empty))

        Shared Sub New()
            AllowRecreatePopupContentProperty.OverrideMetadata(GetType(MyLookUpEdit), New FrameworkPropertyMetadata(False))
            EditValueProperty.OverrideMetadata(GetType(MyLookUpEdit), New FrameworkPropertyMetadata(Nothing, AddressOf OnEditValuePropertyChanged))
        End Sub

        #Region "Properties"
        Public Property GridControl() As GridControl
            Get
                Return CType(GetValue(GridControlProperty), GridControl)
            End Get
            Set(ByVal value As GridControl)
                SetValue(GridControlProperty, value)
            End Set
        End Property
        Public Property ItemsSource() As Object
            Get
                Return DirectCast(GetValue(ItemsSourceProperty), Object)
            End Get
            Set(ByVal value As Object)
                SetValue(ItemsSourceProperty, value)
            End Set
        End Property
        Public Property DisplayMember() As String
            Get
                Return CStr(GetValue(DisplayMemberProperty))
            End Get
            Set(ByVal value As String)
                SetValue(DisplayMemberProperty, value)
            End Set
        End Property
        Public Property ValueMember() As String
            Get
                Return CStr(GetValue(ValueMemberProperty))
            End Get
            Set(ByVal value As String)
                SetValue(ValueMemberProperty, value)
            End Set
        End Property
        Public Property SelectedValue() As Object
            Get
                Return DirectCast(GetValue(SelectedValueProperty), Object)
            End Get
            Set(ByVal value As Object)
                SetValue(SelectedValueProperty, value)
            End Set
        End Property
        Public Property FocusedRowHandle() As Integer
            Get
                Return CInt((GetValue(FocusedRowHandleProperty)))
            End Get
            Set(ByVal value As Integer)
                SetValue(FocusedRowHandleProperty, value)
            End Set
        End Property
        Public Property IncrementalFiltering() As Boolean
            Get
                Return CBool(GetValue(IncrementalFilteringProperty))
            End Get
            Set(ByVal value As Boolean)
                SetValue(IncrementalFilteringProperty, value)
            End Set
        End Property
        Public Property GridSearchString() As String
            Get
                Return CStr(GetValue(GridSearchStringProperty))
            End Get
            Set(ByVal value As String)
                SetValue(GridSearchStringProperty, value)
            End Set
        End Property
        Public ReadOnly Property Controller() As DataController
            Get
                Return GridControl.DataController
            End Get
        End Property

        Private processForegroundMethodInfo_Renamed As MethodInfo
        Private ReadOnly Property ProcessForegroundMethodInfo() As MethodInfo
            Get
                If processForegroundMethodInfo_Renamed Is Nothing Then
                    processForegroundMethodInfo_Renamed = EditStrategy.GetType().GetMethod("ProcessForeground", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
                End If
                Return processForegroundMethodInfo_Renamed
            End Get
        End Property
        #End Region

        #Region "PropertyChangedCallbacks"
        Private Shared Sub OnEditValuePropertyChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            CType(o, MyLookUpEdit).OnMyEditValueChanged(e)
        End Sub
        Private Shared Sub OnSelectedValuePropertyChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            CType(o, MyLookUpEdit).OnSelectedValueChanged(e)
        End Sub
        Private Shared Sub OnGridControlPropertyChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            CType(o, MyLookUpEdit).OnGridControlChanged(e)
        End Sub
        Private Shared Sub OnFocusedRowHandlePropertyChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            CType(o, MyLookUpEdit).OnFocusedRowHandleChanged(e)
        End Sub
#End Region

        Protected Overrides Function CreateEditStrategy() As EditStrategyBase
            Return New MyEditStrategy(Me)
        End Function

        Protected Overridable Sub OnMyEditValueChanged(ByVal e As DependencyPropertyChangedEventArgs)
            If String.IsNullOrEmpty(ValueMember) Then
                SelectedValue = EditValue
                Return
            End If
            UpdateFocusedRowHandle()
        End Sub
        Protected Overridable Sub OnSelectedValueChanged(ByVal e As DependencyPropertyChangedEventArgs)
            If String.IsNullOrEmpty(ValueMember) Then
                EditValue = SelectedValue
                Return
            End If
            EditValue = GetPropertyValue(e.NewValue, ValueMember)
        End Sub
        Protected Overridable Sub OnFocusedRowHandleChanged(ByVal e As DependencyPropertyChangedEventArgs)
            SelectedValue = GridControl.GetRow(FocusedRowHandle)
        End Sub
        Protected Overridable Sub OnGridControlChanged(ByVal e As DependencyPropertyChangedEventArgs)
            SubscribeOnEvents()
            GridControl.SetBinding(GridControl.ItemsSourceProperty, New Binding("ItemsSource") With {.Source = Me})
        End Sub
        Private Sub OnRowHandleFound(ByVal obj As Object)
            If DirectCast(obj, Integer) < 0 Then
                Return
            End If

            Dim selectedValue_Renamed As Object = GridControl.DataController.GetRow(DirectCast(obj, Integer), AddressOf OnRowObjectFound)
            If ValueIsCorrect(selectedValue_Renamed) AndAlso Not(TypeOf selectedValue_Renamed Is NotLoadedObject) Then
                SelectedValue = selectedValue_Renamed
            End If
        End Sub
        Private Function ValueIsCorrect(ByVal value As Object) As Boolean
            If value Is Nothing Then
                Return False
            End If
            Return If(TypeOf value Is ReadonlyThreadSafeProxyForObjectFromAnotherThread, DirectCast(value, ReadonlyThreadSafeProxyForObjectFromAnotherThread).OriginalRow IsNot Nothing, True)
        End Function
        Private Sub OnRowObjectFound(ByVal value As Object)
            SelectedValue = value
        End Sub

        Protected Overrides Sub OnEditBoxTextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
            GridSearchString = Me.EditBox.Text
            If GridControl.View IsNot Nothing Then
                GridControl.View.SearchString = GridSearchString
            End If
            Dim controllerRow = Controller.FindRowByValue(DisplayMember, GridSearchString, Sub(n)
                If CInt((n)) > -1 Then
                    Controller.EnsureRowLoaded(CInt((n)), New OperationCompleted(Sub(nn)
                    SelectedValue = nn
                    MyBase.OnEditBoxTextChanged(sender, e)
                    End Sub))
                End If
            End Sub)
            If controllerRow >= 0 Then
                SelectedValue = Controller.GetRow(controllerRow)
                MyBase.OnEditBoxTextChanged(sender, e)
            End If
        End Sub
        Protected Overrides Function CreatePopupSettings() As PopupSettings
            Return New MyPopupSettings(Me)
        End Function
        Private Sub SetOwnerEdit(ByVal element As DependencyObject, ByVal value As BaseEdit)
            If element Is Nothing Then
                Throw New ArgumentNullException("element")
            End If
            element.SetValue(OwnerEditPropertyKey, value)
        End Sub
        Protected Overrides Function GetDisplayText(ByVal editValue As Object, ByVal applyFormatting As Boolean) As String
            If SelectedValue Is Nothing Then
                Return Nothing
            End If
            If String.IsNullOrEmpty(DisplayMember) Then
                Return SelectedValue.ToString()
            End If
            Dim value = GetPropertyValue(SelectedValue, DisplayMember)
            If value IsNot Nothing Then
                Return value.ToString()
            End If
            Return MyBase.GetDisplayText(editValue, applyFormatting)
        End Function

        Private Sub SubscribeOnEvents()
            RemoveHandler GridControl.PreviewMouseUp, AddressOf OnGridControlPreviewMouseDown
            RemoveHandler GridControl.PreviewKeyDown, AddressOf OnGridControlPreviewKeyDown
            RemoveHandler GridControl.Loaded, AddressOf OnGridControlLoaded
            RemoveHandler GridControl.ItemsSourceChanged, AddressOf OnGridControlItemsSourceChanged
            RemoveHandler GridControl.EndSorting, AddressOf OnGridControlEndSorting
            RemoveHandler GridControl.AsyncOperationCompleted, AddressOf OnGridControlAsyncOperationCompleted

            AddHandler GridControl.PreviewMouseUp, AddressOf OnGridControlPreviewMouseDown
            AddHandler GridControl.PreviewKeyDown, AddressOf OnGridControlPreviewKeyDown
            AddHandler GridControl.Loaded, AddressOf OnGridControlLoaded
            AddHandler GridControl.ItemsSourceChanged, AddressOf OnGridControlItemsSourceChanged
            AddHandler GridControl.EndSorting, AddressOf OnGridControlEndSorting
            AddHandler GridControl.AsyncOperationCompleted, AddressOf OnGridControlAsyncOperationCompleted
        End Sub

        Private Sub UpdateFocusedRowHandle()
            If Not String.IsNullOrEmpty(ValueMember) Then
                Dim i As Integer = Controller.FindRowByValue(ValueMember, EditValue, AddressOf OnRowHandleFound)
                UpdateFocusedRowHandleCore(i)
            Else
                Dim i As Integer = Controller.FindRowByRowValue(EditValue)
                UpdateFocusedRowHandleCore(i)
            End If
        End Sub
        Private Sub UpdateFocusedRowHandleCore(ByVal i As Integer)
            If i >= 0 AndAlso FocusedRowHandle <> i Then
                Dim rowHandle As Integer = GridControl.GetRowHandleByVisibleIndex(Controller.GetVisibleIndex(i))
                FocusedRowHandle = rowHandle
            End If
        End Sub

        #Region "Grid Event handlers"
        Protected Overridable Sub OnGridControlPreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
            If CType(GridControl.View, TableView).GetRowElementByMouseEventArgs(e) IsNot Nothing Then
                SelectedValue = GridControl.View.FocusedRow
                ClosePopup()
            End If
        End Sub
        Protected Overridable Sub OnGridControlPreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
            If e.Key = System.Windows.Input.Key.Enter Then
                SelectedValue = GridControl.View.FocusedRow
                ClosePopup()
            End If
        End Sub
        Protected Overridable Sub OnGridControlLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            GridControl.View.SearchString = GridSearchString
            GridControl.View.SetBinding(TableView.SearchPanelAllowFilterProperty, New Binding("IncrementalFiltering") With {.Source = Me})
            Dispatcher.BeginInvoke(New Action(AddressOf GridControl.InvalidateMeasure))
        End Sub
        Protected Overridable Sub OnGridControlItemsSourceChanged(ByVal sender As Object, ByVal e As ItemsSourceChangedEventArgs)
            UpdateFocusedRowHandle()
        End Sub
        Protected Overridable Sub OnGridControlEndSorting(ByVal sender As Object, ByVal e As RoutedEventArgs)
            UpdateFocusedRowHandle()
        End Sub
        Private Sub OnGridControlAsyncOperationCompleted(ByVal sender As Object, ByVal e As RoutedEventArgs)
            UpdateFocusedRowHandle()
        End Sub
        #End Region

        Public Overridable Function GetPropertyValue(ByVal obj As Object, ByVal [property] As String) As Object
            If obj Is Nothing Then
                Return Nothing
            End If
            Dim originalRow As Object = obj
            If TypeOf originalRow Is ReadonlyThreadSafeProxyForObjectFromAnotherThread Then
                originalRow = DirectCast(originalRow, ReadonlyThreadSafeProxyForObjectFromAnotherThread).OriginalRow
            End If
            Dim descr = originalRow.GetType().GetProperty([property])
            If descr Is Nothing Then
                Return Nothing
            End If
            Return descr.GetValue(originalRow, Nothing)
        End Function
        Public Overridable Function GetEditValueByDisplayValue(ByVal displayValue As Object) As Object
            If String.IsNullOrEmpty(DisplayMember) Then
                If String.IsNullOrEmpty(ValueMember) Then
                    Return displayValue
                Else
                    GetPropertyValue(displayValue, ValueMember)
                End If
            End If
            Dim i As Integer = Controller.FindRowByValue(DisplayMember, displayValue, Sub(n)
            End Sub)

            Dim selectedValue_Renamed As Object = GridControl.DataController.GetRow(i, Sub(value)
            End Sub)
            If String.IsNullOrEmpty(ValueMember) Then
                Return selectedValue_Renamed
            End If
            Return GetPropertyValue(selectedValue_Renamed, ValueMember)
        End Function
        Public ReadOnly Property PublicPopupContentOwner() As IPopupContentOwner
            Get
                Return PopupContentOwner
            End Get
        End Property
    End Class
    Public Class MyEditStrategy
        Inherits TextEditStrategy

        Private LookUp As MyLookUpEdit
        Public Overrides Sub EditValueChanged(ByVal oldValue As Object, ByVal newValue As Object)
            MyBase.EditValueChanged(oldValue, newValue)
        End Sub
        Public Sub New(ByVal edit As MyLookUpEdit)
            MyBase.New(edit)
            LookUp = edit
        End Sub
        Protected Overrides Function GetEditValueInternal() As Object
            Dim containersValue As String = Me.EditBox.Text
            If String.IsNullOrEmpty(LookUp.DisplayMember) AndAlso String.IsNullOrEmpty(LookUp.ValueMember) Then
                Return containersValue
            End If
            Dim a = LookUp.GetEditValueByDisplayValue(containersValue)
            Return a
        End Function
    End Class
    Public Class MyPopupSettings
        Inherits PopupSettings

        Private ReadOnly Property LookUp() As MyLookUpEdit
            Get
                Return CType(OwnerEdit, MyLookUpEdit)
            End Get
        End Property
        Public Sub New(ByVal editor As MyLookUpEdit)
            MyBase.New(editor)
        End Sub
        Protected Overrides Function GetPopupContent() As FrameworkElement
            Dim allowRecreatePopupContent As Boolean = OwnerEdit.AllowRecreatePopupContent
            Dim popupChild As New NonLogicalDecorator()
            Dim popupContent As PopupContentControl = Nothing
            If Not allowRecreatePopupContent Then
                popupContent = CType(LookUp.PublicPopupContentOwner.Child, PopupContentControl)
            End If
            If popupContent Is Nothing Then
                popupContent = New PopupContentControl()
                FocusHelper.SetFocusable(popupContent, False)
            End If
            popupContent.Editor = OwnerEdit
            popupChild.Child = popupContent
            LookUp.PublicPopupContentOwner.Child = popupContent
            popupContent.Tag = OwnerEdit
            popupContent.Content = LookUp.GridControl
            Return popupChild
        End Function
    End Class
End Namespace