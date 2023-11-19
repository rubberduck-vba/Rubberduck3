VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "IView"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
'@Interface IView
'@ModuleDescription "Represents a UI component."
Option Explicit

'@Description "Gets a reference to the Model."
Public Property Get Model() As Object
End Property

'@Description "Displays the view as a modal dialog."
Public Sub ShowModal()
End Sub

'@Description "Displays the view as a non-modal dialog."
Public Sub ShowModeless()
End Sub
