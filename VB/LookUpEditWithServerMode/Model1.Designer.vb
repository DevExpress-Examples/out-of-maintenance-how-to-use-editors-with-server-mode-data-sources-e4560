﻿'------------------------------------------------------------------------------
' <auto-generated>
'    This code was generated from a template.
'
'    Manual changes to this file may cause unexpected behavior in your application.
'    Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.ComponentModel
Imports System.Data.EntityClient
Imports System.Data.Objects
Imports System.Data.Objects.DataClasses
Imports System.Linq
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

<Assembly: EdmSchemaAttribute()>
Namespace DXGridSample
    #Region "Contexts"

    ''' <summary>
    ''' No Metadata Documentation available.
    ''' </summary>
    Partial Public Class DatabaseEntities
        Inherits ObjectContext

        #Region "Constructors"

        ''' <summary>
        ''' Initializes a new DatabaseEntities object using the connection string found in the 'DatabaseEntities' section of the application configuration file.
        ''' </summary>
        Public Sub New()
            MyBase.New("name=DatabaseEntities", "DatabaseEntities")
            Me.ContextOptions.LazyLoadingEnabled = True
            OnContextCreated()
        End Sub

        ''' <summary>
        ''' Initialize a new DatabaseEntities object.
        ''' </summary>
        Public Sub New(ByVal connectionString As String)
            MyBase.New(connectionString, "DatabaseEntities")
            Me.ContextOptions.LazyLoadingEnabled = True
            OnContextCreated()
        End Sub

        ''' <summary>
        ''' Initialize a new DatabaseEntities object.
        ''' </summary>
        Public Sub New(ByVal connection As EntityConnection)
            MyBase.New(connection, "DatabaseEntities")
            Me.ContextOptions.LazyLoadingEnabled = True
            OnContextCreated()
        End Sub

        #End Region

        #Region "Partial Methods"

        Partial Private Sub OnContextCreated()
        End Sub

        #End Region

        #Region "ObjectSet Properties"

        ''' <summary>
        ''' No Metadata Documentation available.
        ''' </summary>
        Public ReadOnly Property Items() As ObjectSet(Of Items)
            Get
                If (_Items Is Nothing) Then
                    _Items = MyBase.CreateObjectSet(Of Items)("Items")
                End If
                Return _Items
            End Get
        End Property
        Private _Items As ObjectSet(Of Items)

        #End Region

        #Region "AddTo Methods"

        ''' <summary>
        ''' Deprecated Method for adding a new object to the Items EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        ''' </summary>

        Public Sub AddToItems(ByVal items_Renamed As Items)
            MyBase.AddObject("Items", items_Renamed)
        End Sub

        #End Region

    End Class

    #End Region

    #Region "Entities"

    ''' <summary>
    ''' No Metadata Documentation available.
    ''' </summary>
    <EdmEntityTypeAttribute(NamespaceName:="DatabaseModel", Name:="Items"), Serializable(), DataContractAttribute(IsReference:=True)> _
    Partial Public Class Items
        Inherits EntityObject

        #Region "Factory Method"

        ''' <summary>
        ''' Create a new Items object.
        ''' </summary>
        ''' <param name="id">Initial value of the Id property.</param>
        Public Shared Function CreateItems(ByVal id As Global.System.String) As Items

            Dim items_Renamed As New Items()
            items_Renamed.Id = id
            Return items_Renamed
        End Function

        #End Region

        #Region "Primitive Properties"

        ''' <summary>
        ''' No Metadata Documentation available.
        ''' </summary>
        <EdmScalarPropertyAttribute(EntityKeyProperty:=True, IsNullable:=False), DataMemberAttribute()> _
        Public Property Id() As Global.System.String
            Get
                Return _Id
            End Get
            Set(ByVal value As System.String)
                If _Id <> value Then
                    OnIdChanging(value)
                    ReportPropertyChanging("Id")
                    _Id = StructuralObject.SetValidValue(value, False)
                    ReportPropertyChanged("Id")
                    OnIdChanged()
                End If
            End Set
        End Property
        Private _Id As Global.System.String
        Partial Private Sub OnIdChanging(ByVal value As Global.System.String)
        End Sub
        Partial Private Sub OnIdChanged()
        End Sub

        ''' <summary>
        ''' No Metadata Documentation available.
        ''' </summary>
        <EdmScalarPropertyAttribute(EntityKeyProperty:=False, IsNullable:=True), DataMemberAttribute()> _
        Public Property Name() As Global.System.String
            Get
                Return _Name
            End Get
            Set(ByVal value As System.String)
                OnNameChanging(value)
                ReportPropertyChanging("Name")
                _Name = StructuralObject.SetValidValue(value, True)
                ReportPropertyChanged("Name")
                OnNameChanged()
            End Set
        End Property
        Private _Name As Global.System.String
        Partial Private Sub OnNameChanging(ByVal value As Global.System.String)
        End Sub
        Partial Private Sub OnNameChanged()
        End Sub

        #End Region


    End Class

    #End Region


End Namespace
