<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128645038/16.1.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E4560)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [MainWindow.xaml](./CS/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/MainWindow.xaml.vb))
* [Reference.cs](./CS/Service%20References/ServiceReference1/Reference.cs) (VB: [Reference.vb](./VB/Service%20References/ServiceReference1/Reference.vb))
<!-- default file list end -->
# How to use editors with Server Mode data sources


<p>This example demonstrates how to use editors from the <strong>DXEditors</strong> suite with <a href="https://documentation.devexpress.com/#WPF/CustomDocument6279">Server Mode</a> data sources. The implementation will depend on the version you are using:<br><br>- Starting with version <strong>16.1</strong>, <a href="https://documentation.devexpress.com/#WPF/clsDevExpressXpfEditorsComboBoxEdittopic">ComboBoxEdit</a>, <a href="https://documentation.devexpress.com/#WPF/CustomDocument8862">LookUpEdit</a> and <a href="https://documentation.devexpress.com/#WPF/clsDevExpressXpfEditorsListBoxEdittopic">ListBoxEdit</a> officially support synchronous as well as asynchronous (Instant Feedback) data sources. I.e., it will be sufficient to set the editor's <strong>ItemsSource</strong> property to the required Server Mode data source object.<br><br>- In version <strong>15.2</strong>, these editors support synchronous data sources.<br><br>- In versions where these sources are not supported, it is possible to implement a custom <a href="https://documentation.devexpress.com/#WPF/clsDevExpressXpfEditorsPopupBaseEdittopic">PopupBaseEdit</a> descendant implemented in this example. Please take special note that the custom editor from this example uses non-documented methods intended for internal use. Thus, the implementation for different DevExpress versions may be different. This example contains projects for multiple major releases - please make sure that you have chosen the correct version before downloading the project.</p>

<br/>


