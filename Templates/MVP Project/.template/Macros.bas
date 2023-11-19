Option Explicit
Private ModelessPresenter As FeaturePresenter
Private ModelessModel As FeatureModel

Public Sub ModalExample()
    Dim Presenter As FeaturePresenter
    Set Presenter = New FeaturePresenter

    Dim Model As FeatureModel
    Set Model = Presenter.ShowDialog

    If Not Model Is Nothing Then
        MsgBox Model.Description, vbOkOnly Or vbInformation, Model.Title
    End If
End Sub

Public Sub ModelessExample()
    If ModelessPresenter Is Nothing Then 
        Set ModelessPresenter = New FeaturePresenter
    End If
    Set ModelessModel = ModelessPresenter.ShowModeless
End Sub