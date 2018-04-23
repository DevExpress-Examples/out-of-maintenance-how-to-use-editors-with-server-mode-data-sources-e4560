// Developer Express Code Central Example:
// How to implement a custom LookUpEdit supporting Server Mode
// 
// The current LookUpEdit version does not allow using Server Mode datasources.
// This example demonstrates how to create a custom editor that allows using them.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4560

// Developer Express Code Central Example:
// How to implement a custom LookUpEdit supporting Server Mode
// 
// The current LookUpEdit version does not allow using Server Mode datasources.
// This example demonstrates how to create a custom editor that allows using them.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4560

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Xpf.Grid.LookUp;
using DevExpress.Xpf.Core.ServerMode;
using System.Collections;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.LookUp.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Core.Native;
using System.Text;
using DevExpress.Xpf.Editors.Helpers;
using System.ComponentModel;
using DevExpress.Data.Linq;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Diagnostics;
using System.Windows.Controls;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using DevExpress.Data.Async.Helpers;
using DevExpress.Xpf.Data;
using System.Reflection;
using System.Threading;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace DXGridSample {
	public partial class MainWindow: Window {
		public MainWindow() {
			XpoDefault.DataLayer = XpoDefault.GetDataLayer(AccessConnectionProvider.GetConnectionString(@"..\..\Database.mdb"), AutoCreateOption.DatabaseAndSchema);
			InitializeComponent();

			XPInstantFeedbackSource XPInstantFeedbackSource = new XPInstantFeedbackSource(typeof(Items1));
			XPInstantFeedbackSource.DisplayableProperties = "Id;Name";

			DatabaseEntities entities = new DatabaseEntities();
			EntityInstantFeedbackDataSource dataSource = new EntityInstantFeedbackDataSource { KeyExpression = "Id", QueryableSource = entities.Items };
			//EntityServerModeDataSource dataSource = new EntityServerModeDataSource { KeyExpression = "Id", QueryableSource = entities.Items };
			
			edit.DisplayMember = "Name";
			edit.ValueMember = "Id";
			//edit.SelectedValue = entities.Items.ToList()[100];
			edit.EditValue = "19";

			edit.ItemsSource = XPInstantFeedbackSource;
			//edit.ItemsSource = dataSource.Data;
		}
	}
	public class Items1: XPObject {
		public Items1(Session session) : base(session) { }
		public Items1() { }
		string fId;
		public string Id {
			get { return fId; }
			set { SetPropertyValue("Id", ref fId, value); }
		}
		string fName;
		public string Name {
			get { return fName; }
			set { SetPropertyValue("Name", ref fName, value); }
		}
	}
}