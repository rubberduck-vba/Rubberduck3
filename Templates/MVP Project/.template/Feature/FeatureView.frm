VERSION 5.00
Begin {C62A69F0-16DC-11CE-9E98-00AA00574A4F} FeatureView 
   Caption         =   "View"
   ClientHeight    =   8100
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   10680
   OleObjectBlob   =   "OrderForm.frx":0000
   StartUpPosition =   1  'CenterOwner
End
Attribute VB_Name = "FeatureView"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'@ModuleDescription "A simple View with Accept and Cancel buttons for a FeatureModel."
Option Explicit
Implements IView

Private Type TView
    Model As FeatureModel
End Type
Private This As TView

Public Event BeforeCancel(ByRef Cancel As Boolean)
Public Event Cancelled()
Public Event Accepted(ByVal Model As FeatureModel)

Public Function Create(ByVal Model As FeatureModel) As IView
    Dim View As FeatureView
    Set View = New FeatureView
    Set View.Model = Model
    Set Create = View
End Function

'@Description "Gets/sets the model that holds the state for this view."
Public Property Get Model() As FeatureModel
    Set Model = This.Model
End Property

Public Property Set Model(ByVal Value As FeatureModel)
    Set This.Model = Value
    OnSetModel
End Property

Private Sub OnSetModel()
    'TODO add TitleBox and DescriptionBox TextBox controls in the form designer.
    Me.TitleBox.Text = This.Model.Name
    Me.DescriptionBox.Text = This.Model.Description
End Sub

Private Sub CancelDialog()
    Dim Abort As Boolean
    RaiseEvent BeforeCancel(Abort)
    If Abort Then Exit Sub

    Dim Cancellable As IsCancellable
    If This.Model Is ICancellable Then
        Set Cancellable = This.Model
        Cancellable.Cancel
    End If

    RaiseEvent Cancelled
    Me.Hide
End Sub

Private Sub Validate()
    Me.AcceptButton.Enabled = This.Model.IsValid
End Sub

'TODO add "AcceptButton" CommandButton control in the form designer.
Private Sub AcceptButton_Click()
    Me.Hide
End Sub

'TODO add "CancelButton" CommandButton control in the form designer.
Private Sub CancelButton_Click()
    CancelDialog
End Sub

Private Sub TitleBox_Exit(ByVal Cancel As MSForms.ReturnBoolean)
    This.Model.Title = TitleBox.Text
    Validate
End Sub

Private Sub DescriptionBox_Exit(ByVal Cancel As MSForms.ReturnBoolean)
    This.Model.Description = DescriptionBox.Text
    Validate
End Sub

Private Sub UserForm_QueryClose(ByRef Cancel As Integer, ByRef CloseMode As Integer)
    If CloseMode = VbQueryClose.vbFormControlMenu Then
        Cancel = True
        CancelDialog
    End If
End Sub

'@Region IView
Private Property Get IView_Model() As Object
    Set IView_Model = Me.Model
End Property

Private Sub IView_ShowModal()
    Me.Show vbModal
End Sub

Private Sub IView_ShowModeless()
    Me.Show vbModeless
End Sub
'@EndRegion
