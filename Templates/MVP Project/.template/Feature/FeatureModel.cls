VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "FeatureModel"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
'@ModuleDescription "The model holding the FeaturePresenter state."
Option Explicit
Implements ICancellable

Private Type TModel
    'ICancellable state:
    IsCancelled As Boolean
    'model state:
    Title As String
    Description As String
End Type
Private This As TModel

'@Description "Gets/sets the title text."
Public Property Get Title() As String
    Title = This.Title
End Property

Public Property Let Title(ByVal Value As String)
    If This.Title <> Value Then
        This.Title = Value
    End If
End Property

'@Description "Gets/sets the description text."
Public Property Get Description() As String
    Description = This.Description
End Property

Public Property Let Description(ByVal Value As String)
    If This.Description <> Value Then
        This.Description = Value
    End If
End Property

'@Description "Gets a value indicating whether the held values are valid, or the user cancelled the input."
Public Property Get IsCancelled() As Boolean
    IsCancelled = This.IsCancelled
End Property

Public Function IsValid() As Boolean
    IsValid = Len(This.Title) > 0 And Len(This.Description) > 0
End Function

'@Region ICancellable
Private Property Get ICancellable_IsCancelled() As Boolean
    ICancellable_IsCancelled = This.IsCancelled
End Property

Private Sub ICancellable_Cancel()
    This.IsCancelled = True
End Sub
'@EndRegion
