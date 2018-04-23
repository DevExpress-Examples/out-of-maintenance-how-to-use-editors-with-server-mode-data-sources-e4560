using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using System.Windows;
using DevExpress.Data.Async.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using DevExpress.Data;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Popups;

namespace DXGridSample {
    public class MyLookUpEdit : PopupBaseEdit {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyPropertyManager.Register("ItemsSource", typeof(object), typeof(MyLookUpEdit), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty DisplayMemberProperty =
            DependencyPropertyManager.Register("DisplayMember", typeof(string), typeof(MyLookUpEdit), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyPropertyManager.Register("SelectedValue", typeof(object), typeof(MyLookUpEdit), new FrameworkPropertyMetadata(null, OnSelectedValuePropertyChanged));
        public static readonly DependencyProperty ValueMemberProperty =
            DependencyPropertyManager.Register("ValueMember", typeof(string), typeof(MyLookUpEdit), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty GridControlProperty =
            DependencyPropertyManager.Register("GridControl", typeof(GridControl), typeof(MyLookUpEdit), new FrameworkPropertyMetadata(null, OnGridControlPropertyChanged));
        public static readonly DependencyProperty FocusedRowHandleProperty =
            DependencyProperty.Register("FocusedRowHandle", typeof(int), typeof(MyLookUpEdit), new FrameworkPropertyMetadata(GridControl.InvalidRowHandle, OnFocusedRowHandlePropertyChanged));
        public static readonly DependencyProperty IncrementalFilteringProperty =
            DependencyProperty.Register("IncrementalFiltering", typeof(bool), typeof(MyLookUpEdit), new UIPropertyMetadata(true));
        public static readonly DependencyProperty GridSearchStringProperty =
            DependencyProperty.Register("GridSearchString", typeof(string), typeof(MyLookUpEdit), new UIPropertyMetadata(string.Empty));

        static MyLookUpEdit() {
            AllowRecreatePopupContentProperty.OverrideMetadata(typeof(MyLookUpEdit), new FrameworkPropertyMetadata(false));
            EditValueProperty.OverrideMetadata(typeof(MyLookUpEdit), new FrameworkPropertyMetadata(null, OnEditValuePropertyChanged));
        }

        #region Properties
        public GridControl GridControl {
            get { return (GridControl)GetValue(GridControlProperty); }
            set { SetValue(GridControlProperty, value); }
        }
        public object ItemsSource {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public string DisplayMember {
            get { return (string)GetValue(DisplayMemberProperty); }
            set { SetValue(DisplayMemberProperty, value); }
        }
        public string ValueMember {
            get { return (string)GetValue(ValueMemberProperty); }
            set { SetValue(ValueMemberProperty, value); }
        }
        public object SelectedValue {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }
        public int FocusedRowHandle {
            get { return (int)GetValue(FocusedRowHandleProperty); }
            set { SetValue(FocusedRowHandleProperty, value); }
        }
        public bool IncrementalFiltering {
            get { return (bool)GetValue(IncrementalFilteringProperty); }
            set { SetValue(IncrementalFilteringProperty, value); }
        }
        public string GridSearchString {
            get { return (string)GetValue(GridSearchStringProperty); }
            set { SetValue(GridSearchStringProperty, value); }
        }
        public DataController Controller { get { return GridControl.DataController; } }
        MethodInfo processForegroundMethodInfo;
        MethodInfo ProcessForegroundMethodInfo {
            get {
                if (processForegroundMethodInfo == null)
                    processForegroundMethodInfo = EditStrategy.GetType().GetMethod("ProcessForeground", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return processForegroundMethodInfo;
            }
        }
        #endregion

        #region PropertyChangedCallbacks
        static void OnEditValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((MyLookUpEdit)o).OnEditValueChanged(e);
        }
        static void OnSelectedValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((MyLookUpEdit)o).OnSelectedValueChanged(e);
        }
        static void OnGridControlPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((MyLookUpEdit)o).OnGridControlChanged(e);
        }
        static void OnFocusedRowHandlePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            ((MyLookUpEdit)o).OnFocusedRowHandleChanged(e);
        }
        #endregion

        protected override EditStrategyBase CreateEditStrategy() {
            return new MyEditStrategy(this);
        }

        protected virtual void OnEditValueChanged(DependencyPropertyChangedEventArgs e) {
            if (string.IsNullOrEmpty(ValueMember)) {
                SelectedValue = EditValue;
                return;
            }
            UpdateFocusedRowHandle();
        }
        protected virtual void OnSelectedValueChanged(DependencyPropertyChangedEventArgs e) {
            if (string.IsNullOrEmpty(ValueMember)) {
                EditValue = SelectedValue;
                return;
            }
            EditValue = GetPropertyValue(e.NewValue, ValueMember);
        }
        protected virtual void OnFocusedRowHandleChanged(DependencyPropertyChangedEventArgs e) {
            SelectedValue = GridControl.GetRow(FocusedRowHandle);
        }
        protected virtual void OnGridControlChanged(DependencyPropertyChangedEventArgs e) {
            SubscribeOnEvents();
            GridControl.SetBinding(GridControl.ItemsSourceProperty, new Binding("ItemsSource") { Source = this });
        }
        void OnRowHandleFound(object obj) {
            if ((int)obj < 0)
                return;
            object selectedValue = GridControl.DataController.GetRow((int)obj, OnRowObjectFound);
            if (ValueIsCorrect(selectedValue) && !(selectedValue is NotLoadedObject)) {
                SelectedValue = selectedValue;
            }
        }
        bool ValueIsCorrect(object value) {
            if (value == null)
                return false;
            return value is ReadonlyThreadSafeProxyForObjectFromAnotherThread ? ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)value).OriginalRow != null : true;
        }
        void OnRowObjectFound(object value) {
            SelectedValue = value;
        }

        protected override void OnEditBoxTextChanged(object sender, TextChangedEventArgs e) {
            GridSearchString = this.EditBox.Text;
            if (GridControl.View != null)
                GridControl.View.SearchString = GridSearchString;
            var controllerRow = Controller.FindRowByValue(DisplayMember, GridSearchString, n => {
                if ((int)n > -1)
                    Controller.EnsureRowLoaded((int)n, new OperationCompleted(nn => { SelectedValue = nn; base.OnEditBoxTextChanged(sender, e); }));
            });
            if (controllerRow >= 0) {
                SelectedValue = Controller.GetRow(controllerRow);
                base.OnEditBoxTextChanged(sender, e);
            }
        }
        protected override PopupSettings CreatePopupSettings() {
            return new MyPopupSettings(this);
        }
        void SetOwnerEdit(DependencyObject element, BaseEdit value) {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(OwnerEditPropertyKey, value);
        }
        protected override string GetDisplayText(object editValue, bool applyFormatting) {
            if (SelectedValue == null)
                return null;
            if (string.IsNullOrEmpty(DisplayMember))
                return SelectedValue.ToString();
            var value = GetPropertyValue(SelectedValue, DisplayMember);
            if (value != null)
                return value.ToString();
            return base.GetDisplayText(editValue, applyFormatting);
        }

        void SubscribeOnEvents() {
            GridControl.PreviewMouseUp -= OnGridControlPreviewMouseDown;
            GridControl.PreviewKeyDown -= OnGridControlPreviewKeyDown;
            GridControl.Loaded -= OnGridControlLoaded;
            GridControl.ItemsSourceChanged -= OnGridControlItemsSourceChanged;
            GridControl.EndSorting -= OnGridControlEndSorting;
            GridControl.AsyncOperationCompleted -= OnGridControlAsyncOperationCompleted;

            GridControl.PreviewMouseUp += OnGridControlPreviewMouseDown;
            GridControl.PreviewKeyDown += OnGridControlPreviewKeyDown;
            GridControl.Loaded += OnGridControlLoaded;
            GridControl.ItemsSourceChanged += OnGridControlItemsSourceChanged;
            GridControl.EndSorting += OnGridControlEndSorting;
            GridControl.AsyncOperationCompleted += OnGridControlAsyncOperationCompleted;
        }

        void UpdateFocusedRowHandle() {
            if (!string.IsNullOrEmpty(ValueMember)) {
                int i = Controller.FindRowByValue(ValueMember, EditValue, OnRowHandleFound);
                UpdateFocusedRowHandleCore(i);
            }
            else {
                int i = Controller.FindRowByRowValue(EditValue);
                UpdateFocusedRowHandleCore(i);
            }
        }
        void UpdateFocusedRowHandleCore(int i) {
            if (i >= 0 && FocusedRowHandle != i) {
                int rowHandle = GridControl.GetRowHandleByVisibleIndex(Controller.GetVisibleIndex(i));
                FocusedRowHandle = rowHandle;
            }
        }

        #region Grid Event handlers
        protected virtual void OnGridControlPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (((TableView)GridControl.View).GetRowElementByMouseEventArgs(e) != null) {
                SelectedValue = GridControl.View.FocusedRow;
                ClosePopup();
            }
        }
        protected virtual void OnGridControlPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {
                SelectedValue = GridControl.View.FocusedRow;
                ClosePopup();
            }
        }
        protected virtual void OnGridControlLoaded(object sender, RoutedEventArgs e) {
            GridControl.View.SearchString = GridSearchString;
            GridControl.View.SetBinding(TableView.SearchPanelAllowFilterProperty, new Binding("IncrementalFiltering") { Source = this });
            Dispatcher.BeginInvoke((Action)GridControl.InvalidateMeasure);
        }
        protected virtual void OnGridControlItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
            UpdateFocusedRowHandle();
        }
        protected virtual void OnGridControlEndSorting(object sender, RoutedEventArgs e) {
            UpdateFocusedRowHandle();
        }
        void OnGridControlAsyncOperationCompleted(object sender, RoutedEventArgs e) {
            UpdateFocusedRowHandle();
        }
        #endregion

        public virtual object GetPropertyValue(object obj, string property) {
            if (obj == null)
                return null;
            object originalRow = obj;
            if (originalRow is ReadonlyThreadSafeProxyForObjectFromAnotherThread)
                originalRow = ((ReadonlyThreadSafeProxyForObjectFromAnotherThread)originalRow).OriginalRow;
            var descr = originalRow.GetType().GetProperty(property);
            if (descr == null)
                return null;
            return descr.GetValue(originalRow, null);
        }
        public virtual object GetEditValueByDisplayValue(object displayValue) {
            if (string.IsNullOrEmpty(DisplayMember))
                if (string.IsNullOrEmpty(ValueMember))
                    return displayValue;
                else
                    GetPropertyValue(displayValue, ValueMember);
            int i = Controller.FindRowByValue(DisplayMember, displayValue, n => { });
            object selectedValue = GridControl.DataController.GetRow(i, value => { });
            if (string.IsNullOrEmpty(ValueMember))
                return selectedValue;
            return GetPropertyValue(selectedValue, ValueMember);
        }
        public IPopupContentOwner PublicPopupContentOwner { get { return PopupContentOwner; } }
    }
    public class MyEditStrategy : TextEditStrategy {
        MyLookUpEdit LookUp;
        public override void EditValueChanged(object oldValue, object newValue) {
            base.EditValueChanged(oldValue, newValue);
        }
        public MyEditStrategy(MyLookUpEdit edit)
            : base(edit) {
            LookUp = edit;
        }
        protected override object GetEditValueInternal() {
            string containersValue = this.EditBox.Text;
            if (string.IsNullOrEmpty(LookUp.DisplayMember) && string.IsNullOrEmpty(LookUp.ValueMember))
                return containersValue;
            var a = LookUp.GetEditValueByDisplayValue(containersValue);
            return a;
        }
    }
    public class MyPopupSettings : PopupSettings {
        MyLookUpEdit LookUp { get { return (MyLookUpEdit)OwnerEdit; } }
        public MyPopupSettings(MyLookUpEdit editor) : base(editor) { }
        protected override FrameworkElement GetPopupContent() {
            bool allowRecreatePopupContent = OwnerEdit.AllowRecreatePopupContent;
            NonLogicalDecorator popupChild = new NonLogicalDecorator();
            PopupContentControl popupContent = null;
            if (!allowRecreatePopupContent)
                popupContent = (PopupContentControl)LookUp.PublicPopupContentOwner.Child;
            if (popupContent == null) {
                popupContent = new PopupContentControl();
                FocusHelper.SetFocusable(popupContent, false);
            }
            popupContent.Editor = OwnerEdit;
            popupChild.Child = popupContent;
            LookUp.PublicPopupContentOwner.Child = popupContent;
            popupContent.Tag = OwnerEdit;
            popupContent.Content = LookUp.GridControl;
            return popupChild;
        }
    }
}