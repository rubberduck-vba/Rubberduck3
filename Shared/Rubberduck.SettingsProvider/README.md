## Rubberduck.SettingsProvider

### Adding new settings

Let's keep things organized, but never *ever* be afraid to add new options and configuration settings.

You'll find settings under `Rubberduck.SettingsProvider.Model` namespaces.

#### Pick a namespace, add the new file.

Settings in Rubberduck3 are `record class` types that inherit `TypedRubberduckSetting<TSetting>`, where `TSetting` is a mapped data type:

- Settings with a `bool` value inherit `BooleanRubberduckSetting`
- Settings with a `double` value inherit `NumericRubberduckSetting`
- Settings with a `string` value inherit `StringRubberduckSetting`
- Settings with a `Uri` value inherit `UriRubberduckSetting`
- Settings with any `enum` value inherit `TypedRubberduckSetting<TSetting>` directly, using the enum type for `TSetting`; `SettingDataType` is set to `EnumValueSetting`.

New setting groups can be added by inheriting `TypedSettingGroup` (which inherits `TypedRubberduckSetting<RubberduckSetting[]>`, i.e. it's a setting with a value that consists of an array of settings).

`EnumSettingGroup<TEnum>` is a special type of setting where the value is a `BooleanRubberduckSetting[]`, where each array element is generated and keyed from the `TEnum` enum member names. In order to ensure unique keys, the enum member names are qualified/prefixed with the name of the setting group. See the `TelemetrySettingGroup<TKey>` implementations for examples.

##### Naming
- End the name with the word `Setting`
- For _setting groups_, end the name with the word `Settings`.
- The name of a setting is included in the related resource keys.

### Implementation

Implementing a setting amounts to specifying a default value and some metadata.

- Add a `public static TValue DefaultSettingValue { get; } = /* default value here */;` static property
- Add a default (parameterless) constructor and minimally specify `SettingDataType` and `DefaultValue`; if you inherited a typed base class (e.g. `BooleanRubberduckSetting`), the `SettingDataType` is already known.
- Optionally specify `Tags` to alter how/whether the setting gets rendered in the settings UI:
  - Use `SettingTags.Hidden` for an automated setting that doesn't need a UI (but still de/serialize to/from .json), e.g. first-startup flags.
  - Use `SettingTags.ReadOnlyRecommended` for settings that should probably be left alone, e.g. server executable path startup settings.

### That's it?

Almost. There's a `[JsonPolymorphic]` attribute on the base `RubberduckSetting` record class, and then a bunch of `[JsonDerivedType]` attributes with `typeof` and `nameof` arguments. You guessed it.

And then yeah, once the JSON serialization understands the new setting, that's it indeed - *Rubberduck* understands the new setting now, but it won't be able to tell you until you add the resource keys for the new setting:
- `{Key}.Name` where the value is e.g. the title of the corresponding checkbox in the UI.
- `{Key}.Description` where the value is a short (?) description of the setting that could be used for a tooltip or a description label.
 

### UI?

Unless you added support for a new data type, you're done - **there is no UI work involved for adding a new setting in Rubberduck 3.x**.
Adding a new setting is only a matter of a few minutes, so go ahead and put that hard-coded default value in a new setting instead of leaving it hard-coded in some service!

If you *do* need to add support for a new data type, you need a new XAML control template and a little tweak to the template selector.

The control templates are located under `Rubberduck.UI.Settings.Templates` and while each template is unique, all templates share a number of common traits, like the 1px border with a 4px corner radius that wraps the entire control.
  -	Background: `{DynamicResource ThemeBackgroundDarkColorBrush}`
  - BorderBrush: `{DynamicResource ThemeBackGroundMediumColorBrush}`

The control itself should have `Background="{DynamicResource ThemeBackgroundColorBrush}"`, and then inside the border is a `DockPanel` with a `FlatToggleButton` docked at the right, a `Label` with `Style="{DynamicResource FormTitleLabelStyle}"` and a `TextBlock` content (for wrapping) with the setting's `Name` (localized string).
Then a `ScrollViewer` also docked at the top, with a `<Label Style="{DynamicResource FormLabelStyle}"` label and a `TextBlock` content (for wrapping) binding to the `Description` string.

The rest completely depends on how the data type is to be represented... aaaand that makes a very good case for extracting the common parts into a user control with a content presenter - with which the only thing to worry about would be the input controls and the value binding.