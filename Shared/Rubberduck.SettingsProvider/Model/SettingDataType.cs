namespace Rubberduck.SettingsProvider.Model
{
    public enum SettingDataType
    {
        NotSet,
        /// <summary>
        /// Represents a group of configurable values.
        /// </summary>
        SettingGroup,
        /// <summary>
        /// A configuration setting that represents a string value.
        /// </summary>
        /// <remarks>
        /// Templated as a <c>TextBox</c> input field in the settings UI.
        /// </remarks>
        TextSetting,
        /// <summary>
        /// A configuration setting that represents a valid URI value.
        /// </summary>
        /// <remarks>
        /// Templated as a <c>PathBox</c> input field in the settings UI.
        /// </remarks>
        UriSetting,
        /// <summary>
        /// A configuration setting that represents a Boolean value.
        /// </summary>
        /// <remarks>
        /// Templated as a <c>CheckBox</c> input field in the settings UI.
        /// </remarks>
        BooleanSetting,
        /// <summary>
        /// A configuration setting that represents an enum constant value.
        /// </summary>
        EnumSettingGroup,
        /// <summary>
        /// A configuration setting that represents an enum constant value.
        /// </summary>
        EnumValueSetting,
        /// <summary>
        /// A configuration setting that represents a numeric value.
        /// </summary>
        /// <remarks>
        /// Templated as a constrained input field in the settings UI.
        /// </remarks>
        NumericSetting,
        /// <summary>
        /// A configuration setting that represents an elapsed amount of time.
        /// </summary>
        /// <remarks>
        /// Templated as an input-masked <c>TextBox</c> field in the settings UI.
        /// </remarks>
        TimeSpanSetting,
        /// <summary>
        /// A configuration setting that represents a list of (string) values.
        /// </summary>
        /// <remarks>
        /// Templated as a <c>ListView</c> in the settings UI.
        /// </remarks>
        ListSetting,
    }
}
