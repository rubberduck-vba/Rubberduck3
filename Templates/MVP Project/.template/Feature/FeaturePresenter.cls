VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "FeaturePresenter"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit
Private WithEvents ModelessView As FeatureView
Private Service As New SomeService

Public Function ShowDialog() As FeatureModel
    Dim Model As FeatureModel
    Set Model = New FeatureModel

    With FeatureView.Create(Model)
        .ShowModal
    End With

    If Not Model.IsCancelled Then
        DoSomething Model
        Set ShowDialog = Model
    End If
End Sub

Public Function ShowModeless() As FeatureModel
    Dim Model As FeatureModel
    Set Model = New FeatureModel
    Set ModelessView = FeatureView.Create(Model)

    Dim View As IView
    Set View = ModelessView
    View.ShowModeless

    Set ShowModeless = Model
End Sub

Private Sub DoSomething(ByVal Model As FeatureModel)
    Service.DoSomething Model
End Sub

Private Sub ModelessView_BeforeCancel(ByRef Cancel As Boolean)
    Cancel = MsgBox("Confirm cancellation?", vbYesNo Or vbExclamation, "Confirmation") = vbNo
End Sub

Private Sub ModelessView_Cancelled()
    MsgBox "Operation was cancelled.", vbOkOnly Or vbInformation, "Feature"
    Set ModelessView = Nothing
End Sub

Private Sub ModelessView_Accepted(ByVal Model As FeatureModel)
    DoSomething Model
End Sub
