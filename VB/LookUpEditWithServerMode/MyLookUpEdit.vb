' Developer Express Code Central Example:
' How to implement a custom LookUpEdit supporting Server Mode
' 
' The current LookUpEdit version does not allow using Server Mode datasources.
' This example demonstrates how to create a custom editor that allows using them.
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E4560


Imports Microsoft.VisualBasic
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
				Return CObj(GetValue(ItemsSourceProperty))
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
				Return CObj(GetValue(SelectedValueProperty))
			End Get
			Set(ByVal value As Object)
				SetValue(SelectedValueProperty, value)
			End Set
		End Property
		Public Property FocusedRowHandle() As Integer
			Get
				Return CInt(Fix(GetValue(FocusedRowHandleProperty)))
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
			CType(o, MyLookUpEdit).OnEditValueChanged(e)
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

		Protected Overridable Sub OnEditValueChanged(ByVal e As DependencyPropertyChangedEventArgs)
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
			If CInt(Fix(obj)) < 0 Then
				Return
			End If
			Dim selectedValue As Object = GridControl.DataController.GetRow(CInt(Fix(obj)), AddressOf OnRowObjectFound)
			If ValueIsCorrect(selectedValue) AndAlso Not(TypeOf selectedValue Is NotLoadedObject) Then
				Me.SelectedValue = selectedValue
			End If
		End Sub
		Private Function ValueIsCorrect(ByVal value As Object) As Boolean
			If value Is Nothing Then
				Return False
			End If
			Return If(TypeOf value Is ReadonlyThreadSafeProxyForObjectFromAnotherThread, (CType(value, ReadonlyThreadSafeProxyForObjectFromAnotherThread)).OriginalRow IsNot Nothing, True)
		End Function
		Private Sub OnRowObjectFound(ByVal value As Object)
			SelectedValue = value
		End Sub

		Protected Overrides Sub OnEditBoxTextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
			GridSearchString = (CType(EditStrategy, MyEditStrategy)).GetEditTextPublic()
			If GridControl.View IsNot Nothing Then
				GridControl.View.SearchString = GridSearchString
			End If
			Dim controllerRow = Controller.FindRowByValue(DisplayMember, GridSearchString, Function(n) AnonymousMethod1(n, sender, e))
			If controllerRow >= 0 Then
				SelectedValue = Controller.GetRow(controllerRow)
				MyBase.OnEditBoxTextChanged(sender, e)
			End If
		End Sub
		
		Private Function AnonymousMethod1(ByVal n As Object, ByVal sender As Object, ByVal e As TextChangedEventArgs) As Boolean
			If CInt(Fix(n)) > -1 Then
				Controller.EnsureRowLoaded(CInt(Fix(n)), New OperationCompleted(Function(nn) AnonymousMethod2(nn, sender, e)))
			End If
			Return True
		End Function
		
		Private Function AnonymousMethod2(ByVal nn As Object, ByVal sender As Object, ByVal e As TextChangedEventArgs) As Boolean
			SelectedValue = nn
			MyBase.OnEditBoxTextChanged(sender, e)
			Return True
		End Function

		Protected Overrides Function GetPopupContent() As FrameworkElement
			Dim popupChild As New NonLogicalDecorator()
			Dim popupContent As PopupContentControl = Nothing
			If (Not AllowRecreatePopupContent) Then
				popupContent = CType(PopupContentOwner.Child, PopupContentControl)
			End If
			If popupContent Is Nothing Then
				popupContent = New PopupContentControl()
				FocusHelper.SetFocusable(popupContent, False)
			End If
			popupContent.Editor = Me
			popupChild.Child = popupContent
			PopupContentOwner.Child = popupContent
			SetOwnerEdit(popupContent, Me)
			popupContent.Tag = Me
#If SL Then
			popupContent.DataContext = Me
#End If
			'popupContent.Template = PopupContentTemplate;
			popupContent.Content = GridControl
			If ProcessForegroundMethodInfo IsNot Nothing Then
				ProcessForegroundMethodInfo.Invoke(EditStrategy, Nothing)
			End If
			Return popupChild
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
			If (Not String.IsNullOrEmpty(ValueMember)) Then
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
			If LayoutHelper.FindParentObject(Of GridRow)(CType(e.OriginalSource, DependencyObject)) IsNot Nothing Then
				SelectedValue = GridControl.View.FocusedRow
				ClosePopup()
			End If
		End Sub
		Protected Overridable Sub OnGridControlPreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
            If e.Key.Equals(System.Windows.Input.Key.Enter) Then
                SelectedValue = GridControl.View.FocusedRow
                ClosePopup()
            End If
		End Sub
		Protected Overridable Sub OnGridControlLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			GridControl.View.SearchString = GridSearchString
			GridControl.View.SetBinding(TableView.SearchPanelAllowFilterProperty, New Binding("IncrementalFiltering") With {.Source = Me})
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
				originalRow = (CType(originalRow, ReadonlyThreadSafeProxyForObjectFromAnotherThread)).OriginalRow
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
			Dim i As Integer = Controller.FindRowByValue(DisplayMember, displayValue, Function(n) AnonymousMethod3(n))
			Dim selectedValue As Object = GridControl.DataController.GetRow(i, Function(value) AnonymousMethod4(value))
			If String.IsNullOrEmpty(ValueMember) Then
				Return selectedValue
			End If
			Return GetPropertyValue(selectedValue, ValueMember)
		End Function
		
		Private Function AnonymousMethod3(ByVal n As Object) As Boolean
			Return True
		End Function
		
		Private Function AnonymousMethod4(ByVal value As Object) As Boolean
			Return True
		End Function
	End Class
	Public Class MyEditStrategy
		Inherits ButtonEditStrategy
		Private LookUp As MyLookUpEdit
		Public Overrides Sub EditValueChanged(ByVal oldValue As Object, ByVal newValue As Object)
			MyBase.EditValueChanged(oldValue, newValue)
		End Sub
		Public Sub New(ByVal edit As MyLookUpEdit)
			MyBase.New(edit)
			LookUp = edit
		End Sub
		Protected Overrides Function GetEditValueInternal() As Object
			Dim containersValue As String = GetEditText()
			If String.IsNullOrEmpty(LookUp.DisplayMember) AndAlso String.IsNullOrEmpty(LookUp.ValueMember) Then
				Return containersValue
			End If
			Dim a = LookUp.GetEditValueByDisplayValue(containersValue)
			Return a
		End Function
		Public Function GetEditTextPublic() As String
			Return GetEditText()
		End Function
		Protected Overrides Sub SyncWithEditorInternal()
			Dim editText As String = GetEditText()
			Dim editValue As Object = If(String.IsNullOrEmpty(editText) AndAlso ReplaceTextWithNull(editText), Editor.NullValue, editText)
			editValue = ConvertEditTextToEditValueCandidate(editValue)

			If editValue Is LookUp.GetEditValueByDisplayValue(GetEditValueInternal()) Then
				ValueContainer.SetEditValue(GetEditValueInternal(), UpdateEditorSource.TextInput)
				Me.UpdateDisplayText()
			End If
		End Sub
	End Class
End Namespace